class ScorecardTemplatePeriodMaintenanceGridController extends CHControllerBase {

    //#region Members

    scorecardTemplateId: string;
    template: any = null;
    currentYear = new Date().getFullYear();

    periods: any[] = [];
    groups: any[] = [];
    loading = false;
    loaded = false;
    summary: any = { total: 0, fixed: 0, variable: 0, scorecards: 0 };

    /** Id of the period whose delete request is in flight. */
    deleteBusyId: string = null;

    /** Id of the period just saved; drives a brief highlight. */
    highlightId: string = null;
    private highlightTimer: any = null;

    drawer: any = { open: false, loading: false, saving: false, submitted: false, isNew: false, error: null, record: null };
    editForm: any;
    private drawerSnapshot: string = null;
    private reviewYearTouched = false;

    private monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        me.scorecardTemplateId = me.$stateParams["scorecardTemplateId"];
        me.loadTemplate();
        me.loadPeriods();

        const onKeydown = (e: any) => {
            if (e.keyCode !== 27 || !me.drawer.open) {
                return;
            }
            me.$scope.$applyAsync(() => {
                me.closeDrawer();
            });
        };
        angular.element(document).on("keydown.stPeriodDrawer", onKeydown);
        me.$scope.$on("$destroy", () => {
            angular.element(document).off("keydown.stPeriodDrawer", onKeydown);
            if (me.highlightTimer) {
                me.$timeout.cancel(me.highlightTimer);
            }
        });
    }

    //#endregion

    //#region Period list

    private loadTemplate = (): void => {
        const me = this;
        me.ScorecardTemplateService.scorecardTemplateGet(me.scorecardTemplateId)
            .then(
                result => {
                    me.template = result;
                },
                error => {
                    me.handleError(error);
                });
    };

    loadPeriods = (): void => {
        const me = this;
        me.loading = true;
        me.ScorecardTemplateService.scorecardTemplatePeriodGrid({
            id: me.scorecardTemplateId,
            searchFor: "",
            sortKey: "startdate",
            sortOrder: "ASC",
            currentPage: 1,
            recordsPerPage: 1000
        } as any).then(
            result => {
                me.periods = (result && result.results) || [];
                me.refresh();
                me.loading = false;
                me.loaded = true;
            },
            error => {
                me.loading = false;
                me.handleError(error);
            });
    };

    /** Fixed periods grouped by review year (newest first), then variable periods in their own group. */
    private refresh = (): void => {
        const s = { total: this.periods.length, fixed: 0, variable: 0, scorecards: 0 };
        const byYear: any = {};
        const years: number[] = [];
        const variable: any[] = [];

        for (let i = 0; i < this.periods.length; i++) {
            const p = this.periods[i];
            s.scorecards += p.scorecardCount || 0;
            if (p.isVariable) {
                s.variable++;
                variable.push(p);
                continue;
            }
            s.fixed++;
            if (!byYear[p.reviewYear]) {
                byYear[p.reviewYear] = [];
                years.push(p.reviewYear);
            }
            byYear[p.reviewYear].push(p);
        }

        const bySortThenStart = (a: any, b: any): number => {
            const d = (a.reportSortOrder || 0) - (b.reportSortOrder || 0);
            if (d !== 0) {
                return d;
            }
            const sa = this.parseDay(a.startDate);
            const sb = this.parseDay(b.startDate);
            return (sa ? sa.getTime() : 0) - (sb ? sb.getTime() : 0);
        };

        years.sort((a, b) => b - a);
        const groups: any[] = years.map(y => ({
            key: "year-" + y,
            label: String(y),
            hint: y === this.currentYear ? "Current review year" : "",
            periods: byYear[y].sort(bySortThenStart)
        }));
        if (variable.length) {
            groups.push({
                key: "variable",
                label: "Variable periods",
                hint: "Each scorecard sets its own dates",
                periods: variable.sort(bySortThenStart)
            });
        }

        this.groups = groups;
        this.summary = s;
    };

    isCurrent = (period: any): boolean => {
        if (period.isVariable) {
            return false;
        }
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

    deletePeriod = (period: any, $event?: any): void => {
        const me = this;
        if ($event) {
            $event.stopPropagation();
        }
        if (me.deleteBusyId || period.scorecardCount > 0) {
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Delete \u201c" + period.description + "\u201d?",
            message: period.isVariable
                ? "This variable period will be removed from the template. This cannot be undone."
                : "The period " + me.rangeLabel(period) + " will be removed from the template. This cannot be undone.",
            confirmText: "Delete period",
            cancelText: "Keep",
            tone: "danger",
            icon: "fa-trash"
        }).then(action => {
            if (!action) {
                return;
            }
            me.deleteBusyId = period.id;
            me.ScorecardTemplateService.scorecardTemplatePeriodDelete({ id: period.id })
                .then(
                    () => {
                        me.deleteBusyId = null;
                        const idx = me.periods.indexOf(period);
                        if (idx >= 0) {
                            me.periods.splice(idx, 1);
                        }
                        if (me.drawer.open && me.drawer.record && me.drawer.record.id === period.id) {
                            me.drawer.open = false;
                        }
                        me.refresh();
                    },
                    error => {
                        me.deleteBusyId = null;
                        me.handleError(error);
                    });
        });
    };

    deleteFromDrawer = (): void => {
        const r = this.drawer.record;
        if (!r || !r.id || this.drawer.saving) {
            return;
        }
        for (let i = 0; i < this.periods.length; i++) {
            if (this.periods[i].id === r.id) {
                this.deletePeriod(this.periods[i]);
                return;
            }
        }
    };

    //#endregion

    //#region Drawer

    openNew = (): void => {
        const me = this;
        let nextSort = 1;
        let nextStart: Date = null;
        for (let i = 0; i < me.periods.length; i++) {
            const p = me.periods[i];
            nextSort = Math.max(nextSort, (p.reportSortOrder || 0) + 1);
            if (p.isVariable) {
                continue;
            }
            const end = me.parseDay(p.endDate);
            if (end && (!nextStart || end.getTime() >= nextStart.getTime())) {
                nextStart = new Date(end.getFullYear(), end.getMonth(), end.getDate() + 1);
            }
        }

        me.prepareDrawer(true);
        me.drawer.record = {
            id: null,
            description: "",
            isVariable: false,
            startDate: nextStart,
            endDate: null,
            reviewYear: nextStart ? nextStart.getFullYear() : me.currentYear,
            reportSortOrder: nextSort,
            scorecardCount: 0
        };
        me.drawerSnapshot = me.snapshot();
    };

    openEdit = (period: any): void => {
        const me = this;
        me.prepareDrawer(false);
        me.drawer.loading = true;
        me.drawer.requestedId = period.id;
        me.reviewYearTouched = true;
        me.ScorecardTemplateService.scorecardTemplatePeriodGet(period.id)
            .then(
                result => {
                    if (!me.drawer.open || me.drawer.requestedId !== period.id) {
                        return;
                    }
                    me.drawer.record = {
                        id: result.id,
                        description: result.description,
                        isVariable: !!result.isVariable,
                        startDate: me.parseDay(result.startDate),
                        endDate: me.parseDay(result.endDate),
                        reviewYear: result.reviewYear,
                        reportSortOrder: result.reportSortOrder,
                        scorecardCount: period.scorecardCount || 0
                    };
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
        this.drawerSnapshot = null;
        this.reviewYearTouched = false;
    };

    private snapshot = (): string => {
        const r = this.drawer.record;
        if (!r) {
            return null;
        }
        return angular.toJson({
            description: (r.description || "").trim(),
            isVariable: !!r.isVariable,
            startDate: r.isVariable ? "" : this.dayKey(this.parseDay(r.startDate)),
            endDate: r.isVariable ? "" : this.dayKey(this.parseDay(r.endDate)),
            reviewYear: r.isVariable ? null : Number(r.reviewYear) || null,
            reportSortOrder: Number(r.reportSortOrder) || 0
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
            message: "You have unsaved changes to this review period.",
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

    setVariable = (isVariable: boolean): void => {
        if (this.drawer.record) {
            this.drawer.record.isVariable = isVariable;
        }
    };

    onStartDateChanged = (): void => {
        const r = this.drawer.record;
        const start = r ? this.parseDay(r.startDate) : null;
        if (start && this.drawer.isNew && !this.reviewYearTouched) {
            r.reviewYear = start.getFullYear();
        }
    };

    onReviewYearChanged = (): void => {
        this.reviewYearTouched = true;
    };

    datesInvalid = (): boolean => {
        const r = this.drawer.record;
        if (!r || r.isVariable) {
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

        const missing: string[] = [];
        if (!(r.description || "").trim()) {
            missing.push("Description");
        }
        if (r.reportSortOrder === null || r.reportSortOrder === undefined || r.reportSortOrder === "") {
            missing.push("Report sort order");
        }
        if (!r.isVariable) {
            if (!me.parseDay(r.startDate)) {
                missing.push("Start date");
            }
            if (!me.parseDay(r.endDate)) {
                missing.push("End date");
            }
            if (!r.reviewYear) {
                missing.push("Review year");
            }
        }
        if (missing.length) {
            me.drawer.error = "Please complete the required fields: " + missing.join(", ") + ".";
            return;
        }
        if (me.datesInvalid()) {
            me.drawer.error = "End date must be on or after the start date.";
            return;
        }

        const today = me.parseDay(new Date());
        const start = r.isVariable ? (me.parseDay(r.startDate) || today) : me.parseDay(r.startDate);
        const end = r.isVariable ? (me.parseDay(r.endDate) || today) : me.parseDay(r.endDate);
        const payload: any = {
            scorecardTemplateId: me.scorecardTemplateId,
            description: r.description.trim(),
            isVariable: !!r.isVariable,
            startDate: start,
            endDate: end,
            reviewYear: Number(r.reviewYear) || me.currentYear,
            reportSortOrder: Number(r.reportSortOrder) || 0
        };
        if (r.id) {
            payload.id = r.id;
        }

        me.drawer.saving = true;
        me.ScorecardTemplateService.scorecardTemplatePeriodSave(payload)
            .then(
                result => {
                    me.drawer.saving = false;
                    me.drawer.open = false;
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

    private shortDate = (date: Date): string => {
        return date.getDate() + " " + this.monthNames[date.getMonth()] + " " + date.getFullYear();
    };

    //#endregion
}

angular.module("AngularApp")
    .controller("ScorecardTemplatePeriodMaintenanceGridController",
        [
            "$scope",
            "$state",
            "$stateParams",
            "$timeout",
            "ScorecardTemplateService",
            "Popups",
            ScorecardTemplatePeriodMaintenanceGridController
        ]);
