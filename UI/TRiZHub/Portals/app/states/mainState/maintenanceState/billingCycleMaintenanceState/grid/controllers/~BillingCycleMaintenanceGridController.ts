class BillingCycleMaintenanceGridController extends CHControllerBase {

    //#region Members

    year: number;
    currentYear = new Date().getFullYear();
    periods: any[] = [];
    loading = false;
    summary: any = { total: 0, open: 0, closed: 0, exemptions: 0 };

    /** Id of the period whose Close/Reopen request is in flight. */
    statusBusyId: string = null;

    /** Id of the period just saved or toggled; drives a brief highlight. */
    highlightId: string = null;
    private highlightTimer: any = null;

    /** Users that can be exempted, plus any inactive users already exempt on the open period. */
    candidates: any[] = [];
    candidatesLoaded = false;

    drawer: any = { open: false, loading: false, saving: false, submitted: false, isNew: false, error: null, record: null };
    exemptFilterText = "";
    exemptShowSelectedOnly = false;
    editForm: any;
    private drawerSnapshot: string = null;

    private monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        me.year = me.currentYear;
        me.loadPeriods();

        const onKeydown = (e: any) => {
            if (e.keyCode !== 27 || !me.drawer.open) {
                return;
            }
            me.$scope.$applyAsync(() => {
                me.closeDrawer();
            });
        };
        angular.element(document).on("keydown.bpDrawer", onKeydown);
        me.$scope.$on("$destroy", () => {
            angular.element(document).off("keydown.bpDrawer", onKeydown);
        });
    }

    //#endregion

    //#region Period list

    loadPeriods = (): void => {
        const me = this;
        const year = me.year;
        me.loading = true;
        me.BillingCycleService.billingCycleGrid({
            searchFor: String(year),
            sortKey: "startdate",
            sortOrder: "ASC",
            currentPage: 1,
            recordsPerPage: 100
        } as any).then(
            result => {
                if (me.year !== year) {
                    return;
                }
                me.periods = (result && result.results) || [];
                me.refreshSummary();
                me.loading = false;
            },
            error => {
                me.loading = false;
                me.handleError(error);
            });
    };

    incYear = (): void => {
        this.year = this.year + 1;
        this.loadPeriods();
    };

    decYear = (): void => {
        this.year = this.year - 1;
        this.loadPeriods();
    };

    goToCurrentYear = (): void => {
        this.year = this.currentYear;
        this.loadPeriods();
    };

    private refreshSummary = (): void => {
        const s = { total: this.periods.length, open: 0, closed: 0, exemptions: 0 };
        for (let i = 0; i < this.periods.length; i++) {
            if (this.periods[i].isClosed) {
                s.closed++;
            } else {
                s.open++;
            }
            s.exemptions += this.periods[i].exemptUserCount || 0;
        }
        this.summary = s;
    };

    isCurrent = (period: any): boolean => {
        const today = this.dayKey(new Date());
        const start = this.dayKey(this.parseDay(period.startDate));
        const end = this.dayKey(this.parseDay(period.endDate));
        return !!start && !!end && today >= start && today <= end;
    };

    rangeLabel = (period: any): string => {
        const start = this.parseDay(period.startDate);
        const end = this.parseDay(period.endDate);
        if (!start || !end) {
            return "";
        }
        return this.shortDate(start) + " – " + this.shortDate(end);
    };

    exemptTooltip = (period: any): string => {
        const names = period.exemptUserNames || [];
        return names.length ? "Exempt: " + names.join(", ") : "";
    };

    toggleClosed = (period: any, $event?: any): void => {
        const me = this;
        if ($event) {
            $event.stopPropagation();
        }
        if (me.statusBusyId) {
            return;
        }
        const closing = !period.isClosed;
        const exemptNames: string[] = period.exemptUserNames || [];
        const dialog: any = closing
            ? {
                title: "Close billing period " + period.cycle + "?",
                message: "Timesheets for " + me.rangeLabel(period) + " become read-only.",
                confirmText: "Close period",
                cancelText: "Keep open",
                tone: "warning",
                icon: "fa-lock",
                detailLabel: exemptNames.length ? "Can still capture time" : "",
                details: exemptNames,
                note: exemptNames.length ? "" : "No exempt users. Nobody will be able to capture time in this period."
            }
            : {
                title: "Reopen billing period " + period.cycle + "?",
                message: "Everyone will be able to capture and edit time for " + me.rangeLabel(period) + " again.",
                confirmText: "Reopen period",
                cancelText: "Keep closed",
                tone: "success",
                icon: "fa-unlock"
            };

        me.Popups.actionDialog(me.$scope, dialog)
            .then(
                action => {
                    if (!action) {
                        return;
                    }
                    me.statusBusyId = period.id;
                    me.BillingCycleService.billingCycleSetStatus(period.id, closing)
                        .then(
                            result => {
                                me.statusBusyId = null;
                                period.isClosed = result ? result.isClosed : closing;
                                if (me.drawer.open && me.drawer.record && me.drawer.record.id === period.id) {
                                    me.drawer.record.isClosed = period.isClosed;
                                }
                                me.refreshSummary();
                                me.flash(period.id);
                            },
                            error => {
                                me.statusBusyId = null;
                                me.handleError(error);
                                me.Popups.showError(me.$scope, error);
                            });
                });
    };

    private flash = (id: string): void => {
        const me = this;
        if (me.highlightTimer) {
            me.$timeout.cancel(me.highlightTimer);
        }
        me.highlightId = id;
        me.highlightTimer = me.$timeout(() => {
            me.highlightId = null;
            me.highlightTimer = null;
        }, 2500);
    };

    //#endregion

    //#region Drawer

    openNew = (): void => {
        const me = this;
        let nextCycle = 1;
        let nextStart: Date = null;
        for (let i = 0; i < me.periods.length; i++) {
            nextCycle = Math.max(nextCycle, (me.periods[i].cycle || 0) + 1);
            const end = me.parseDay(me.periods[i].endDate);
            if (end && (!nextStart || end.getTime() >= nextStart.getTime())) {
                nextStart = new Date(end.getFullYear(), end.getMonth(), end.getDate() + 1);
            }
        }

        me.prepareDrawer(true);
        me.drawer.record = {
            id: null,
            cycle: nextCycle,
            year: me.year,
            startDate: nextStart,
            endDate: null,
            weekdays: 0,
            publicHolidays: 0,
            workDays: 0,
            isClosed: false,
            exemptUserAccountIds: []
        };
        me.drawerSnapshot = me.snapshot();
        me.ensureCandidates();
    };

    openEdit = (period: any): void => {
        const me = this;
        me.prepareDrawer(false);
        me.drawer.loading = true;
        me.drawer.requestedId = period.id;
        me.ensureCandidates();
        me.BillingCycleService.billingCycleGet(period.id)
            .then(
                result => {
                    if (!me.drawer.open || me.drawer.requestedId !== period.id) {
                        return;
                    }
                    me.drawer.record = {
                        id: result.id,
                        cycle: result.cycle,
                        year: result.year,
                        startDate: me.parseDay(result.startDate),
                        endDate: me.parseDay(result.endDate),
                        weekdays: result.weekdays,
                        publicHolidays: result.publicHolidays,
                        workDays: result.workDays,
                        isClosed: result.isClosed,
                        exemptUserAccountIds: (result.exemptUserAccountIds || []).slice()
                    };
                    me.mergeCandidates(result.exemptUsers || []);
                    me.drawerSnapshot = me.snapshot();
                    me.drawer.loading = false;
                },
                error => {
                    me.drawer.loading = false;
                    me.drawer.error = error;
                });
    };

    private prepareDrawer = (isNew: boolean): void => {
        this.drawer = {
            open: true,
            loading: false,
            saving: false,
            submitted: false,
            isNew: isNew,
            error: null,
            record: null,
            requestedId: null
        };
        this.exemptFilterText = "";
        this.exemptShowSelectedOnly = false;
        this.drawerSnapshot = null;
    };

    private snapshot = (): string => {
        const r = this.drawer.record;
        if (!r) {
            return null;
        }
        return angular.toJson({
            cycle: r.cycle,
            year: r.year,
            startDate: this.dayKey(r.startDate),
            endDate: this.dayKey(r.endDate),
            weekdays: r.weekdays,
            publicHolidays: r.publicHolidays,
            workDays: r.workDays,
            isClosed: r.isClosed,
            exempt: (r.exemptUserAccountIds || []).slice().sort()
        });
    };

    isDrawerDirty = (): boolean => {
        return !!this.drawer.record && this.snapshot() !== this.drawerSnapshot;
    };

    closeDrawer = (): void => {
        const me = this;
        if (!me.drawer.open || me.drawer.saving) {
            return;
        }
        if (!me.isDrawerDirty()) {
            me.drawer.open = false;
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Discard changes?",
            message: "You have unsaved changes to this billing period.",
            confirmText: "Discard",
            cancelText: "Keep editing",
            tone: "danger",
            icon: "fa-undo"
        }).then(action => {
            if (action) {
                me.drawer.open = false;
            }
        });
    };

    setDrawerClosed = (isClosed: boolean): void => {
        if (this.drawer.record) {
            this.drawer.record.isClosed = isClosed;
        }
    };

    /** Weekdays (Mon–Fri) in the period; work days follow unless public holidays exceed them. */
    onDatesChanged = (): void => {
        const r = this.drawer.record;
        if (!r) {
            return;
        }
        const start = this.parseDay(r.startDate);
        const end = this.parseDay(r.endDate);
        if (!start || !end || end.getTime() < start.getTime()) {
            return;
        }
        let weekdays = 0;
        const cursor = new Date(start.getTime());
        while (cursor.getTime() <= end.getTime()) {
            const day = cursor.getDay();
            if (day > 0 && day < 6) {
                weekdays++;
            }
            cursor.setDate(cursor.getDate() + 1);
        }
        r.weekdays = weekdays;
        if (!r.year) {
            r.year = start.getFullYear();
        }
        this.recalcWorkDays();
    };

    recalcWorkDays = (): void => {
        const r = this.drawer.record;
        if (!r) {
            return;
        }
        const weekdays = Number(r.weekdays) || 0;
        const holidays = Number(r.publicHolidays) || 0;
        r.workDays = Math.max(0, weekdays - holidays);
    };

    datesInvalid = (): boolean => {
        const r = this.drawer.record;
        if (!r) {
            return false;
        }
        const start = this.parseDay(r.startDate);
        const end = this.parseDay(r.endDate);
        return !!start && !!end && end.getTime() < start.getTime();
    };

    saveDrawer = (): void => {
        const me = this;
        const r = me.drawer.record;
        if (!r || me.drawer.saving) {
            return;
        }
        me.drawer.submitted = true;
        me.drawer.error = null;
        if ((me.editForm && me.editForm.$invalid) || !r.startDate || !r.endDate) {
            me.drawer.error = "Please complete the required fields.";
            return;
        }
        if (me.datesInvalid()) {
            me.drawer.error = "End date must be on or after the start date.";
            return;
        }

        const payload = {
            id: r.id || null,
            cycle: r.cycle,
            year: r.year,
            startDate: me.toApiDate(r.startDate),
            endDate: me.toApiDate(r.endDate),
            weekdays: Number(r.weekdays) || 0,
            publicHolidays: Number(r.publicHolidays) || 0,
            workDays: Number(r.workDays) || 0,
            isClosed: !!r.isClosed,
            exemptUserAccountIds: (r.exemptUserAccountIds || []).slice()
        };

        me.drawer.saving = true;
        me.BillingCycleService.billingCycleSave(payload)
            .then(
                result => {
                    me.drawer.saving = false;
                    me.drawer.open = false;
                    const savedStart = me.parseDay(result && result.startDate);
                    if (savedStart && savedStart.getFullYear() !== me.year) {
                        me.year = savedStart.getFullYear();
                    }
                    me.loadPeriods();
                    if (result && result.id) {
                        me.flash(result.id);
                    }
                },
                error => {
                    me.drawer.saving = false;
                    me.drawer.error = error;
                });
    };

    //#endregion

    //#region Exempt users

    private ensureCandidates = (): void => {
        const me = this;
        if (me.candidatesLoaded) {
            return;
        }
        me.candidatesLoaded = true;
        me.BillingCycleService.billingCycleExemptionCandidates()
            .then(
                result => {
                    me.mergeCandidates(result || []);
                },
                error => {
                    me.candidatesLoaded = false;
                    me.handleError(error);
                });
    };

    private mergeCandidates = (users: any[]): void => {
        const known: any = {};
        for (let i = 0; i < this.candidates.length; i++) {
            known[this.candidates[i].id] = this.candidates[i];
        }
        for (let j = 0; j < users.length; j++) {
            const existing = known[users[j].id];
            if (existing) {
                existing.active = existing.active || users[j].active;
                continue;
            }
            this.candidates.push(users[j]);
            known[users[j].id] = users[j];
        }
        this.candidates.sort((a, b) => (a.name || "").localeCompare(b.name || ""));
    };

    filteredCandidates = (): any[] => {
        const q = (this.exemptFilterText || "").toLowerCase().trim();
        const selectedOnly = this.exemptShowSelectedOnly;
        return this.candidates.filter(u =>
            (!q || (u.name || "").toLowerCase().indexOf(q) >= 0)
            && (!selectedOnly || this.isExempt(u.id)));
    };

    isExempt = (userId: string): boolean => {
        const r = this.drawer.record;
        return !!r && (r.exemptUserAccountIds || []).indexOf(userId) >= 0;
    };

    toggleExempt = (userId: string): void => {
        const r = this.drawer.record;
        if (!r) {
            return;
        }
        const idx = r.exemptUserAccountIds.indexOf(userId);
        if (idx >= 0) {
            r.exemptUserAccountIds.splice(idx, 1);
        } else {
            r.exemptUserAccountIds.push(userId);
        }
    };

    removeExempt = (userId: string): void => {
        const r = this.drawer.record;
        if (!r) {
            return;
        }
        const idx = r.exemptUserAccountIds.indexOf(userId);
        if (idx >= 0) {
            r.exemptUserAccountIds.splice(idx, 1);
        }
    };

    clearExempt = (): void => {
        if (this.drawer.record) {
            this.drawer.record.exemptUserAccountIds = [];
        }
    };

    areAllVisibleExempt = (): boolean => {
        const visible = this.filteredCandidates();
        return visible.length > 0 && visible.every(u => this.isExempt(u.id));
    };

    toggleAllVisibleExempt = (): void => {
        const r = this.drawer.record;
        if (!r) {
            return;
        }
        const visible = this.filteredCandidates();
        if (this.areAllVisibleExempt()) {
            visible.forEach(u => this.removeExempt(u.id));
        } else {
            visible.forEach(u => {
                if (r.exemptUserAccountIds.indexOf(u.id) < 0) {
                    r.exemptUserAccountIds.push(u.id);
                }
            });
        }
    };

    candidateFor = (userId: string): any => {
        for (let i = 0; i < this.candidates.length; i++) {
            if (this.candidates[i].id === userId) {
                return this.candidates[i];
            }
        }
        return null;
    };

    exemptName = (userId: string): string => {
        const user = this.candidateFor(userId);
        return user ? user.name : "Unknown user";
    };

    initials = (name: string): string => {
        const parts = (name || "").trim().split(/\s+/);
        const first = parts[0] ? parts[0].charAt(0) : "";
        const last = parts.length > 1 ? parts[parts.length - 1].charAt(0) : "";
        return (first + last).toUpperCase() || "?";
    };

    //#endregion

    //#region Dates

    /** Parse an API date or Date as a local calendar day, so a UTC suffix cannot shift it. */
    parseDay = (raw: any): Date => {
        if (raw == null || raw === "") {
            return null;
        }
        if (raw instanceof Date) {
            return isNaN(raw.getTime()) ? null : new Date(raw.getFullYear(), raw.getMonth(), raw.getDate());
        }
        const iso = /^(\d{4})-(\d{1,2})-(\d{1,2})/.exec(String(raw));
        if (iso) {
            return new Date(parseInt(iso[1], 10), parseInt(iso[2], 10) - 1, parseInt(iso[3], 10));
        }
        const d = new Date(raw);
        return isNaN(d.getTime()) ? null : new Date(d.getFullYear(), d.getMonth(), d.getDate());
    };

    private pad2 = (n: number): string => {
        return (n < 10 ? "0" : "") + n;
    };

    private dayKey = (date: Date): string => {
        if (!date || isNaN(date.getTime())) {
            return "";
        }
        return date.getFullYear() + "-" + this.pad2(date.getMonth() + 1) + "-" + this.pad2(date.getDate());
    };

    private toApiDate = (raw: any): string => {
        return this.dayKey(this.parseDay(raw)) + "T00:00:00";
    };

    private shortDate = (date: Date): string => {
        return date.getDate() + " " + this.monthNames[date.getMonth()] + " " + date.getFullYear();
    };

    //#endregion
}


angular.module("AngularApp")
    .controller("BillingCycleMaintenanceGridController",
        [
            "$scope",
            "$state",
            "$timeout",
            "BillingCycleService",
            "Popups",
            BillingCycleMaintenanceGridController
        ]);
