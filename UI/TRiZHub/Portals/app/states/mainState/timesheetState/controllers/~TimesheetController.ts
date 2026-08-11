class TimesheetController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    gridModelOriginal: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    displayOptions: any;
    filterOptions: any;
    filterModel: any;
    choosenProjectId: any;
    choosenProject: any;
    viewModel: any;
    summary: any;
    templateProject: any;
    show = String();
    rowId = 1;

    /** Week tabs derived from the selected billing period (Mon–Sun, clamped to period). */
    weeks: any[] = [];
    selectedWeekIndex = 0;
    selectedWeek: any = null;

    private dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    private monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private $scope: ng.IScope,
        private $uibModal: any,
        private $log: ng.ILogService,
        private $filter: ng.IFilterService,
        private ActivityService: ActivityServiceModule.ActivityService,
        private TeamService: TeamServiceModule.TeamService,
        private UserService: UserServiceModule.UserService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private ClientService: ClientServiceModule.ClientService,
        private TimesheetService: TimesheetServiceModule.TimesheetService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        me.displayOptions = { show: false };
        me.viewModel = {};
        me.filterModel = {};
        me.templateProject = {};
        me.filterOptions = {
            projects: [],
            users: [],
            teams: [],
            activities: [],
            clients: [],
            billingCycles: [],
            billingOptions: [
                { val: 0, description: "All" },
                { val: 1, description: "Yes" },
                { val: 2, description: "No" },
            ],
            userTeams: []
        };

        me.gridModel = { data: [], originalData: [], totalItems: 0 } as any;
        me.summary = { days: [], totalHours: 0, totalBillableHours: 0 };

        // Set default Billable — Manual Date is not offered; period always comes from a real billing cycle
        me.filterModel.billingOption = me.filterOptions.billingOptions[0];
        me.filterModel.billingCycleId = null;
        me.filterModel.startDate = null;
        me.filterModel.endDate = null;

        BillingCycleService.billingCycleDropdownList()
            .then(
                results => {
                    // Real cycles only — no "Manual Date" synthetic row
                    me.filterOptions.billingCycles = results || [];
                    if (me.filterOptions.billingCycles.length) {
                        const defaultCycle = me.pickDefaultBillingCycle(me.filterOptions.billingCycles);
                        me.filterModel.billingCycleId = me.getCycleId(defaultCycle);
                        me.viewModel.billingCycle = defaultCycle;
                        me.applyBillingPeriod();
                    }
                },
                error => {
                    me.handleError(error);
                });

        UserService.userTimesheetFilterDropdown()
            .then(
                result => {
                    me.filterOptions.users = result;
                },
                error => {
                    me.handleError(error);
                });

        ActivityService.activityDropdownList()
            .then(
                result => {
                    me.filterOptions.activities = result;
                },
                error => {
                    me.handleError(error);
                });

        TeamService.teamDropdownList()
            .then(function (result) {
                me.filterOptions.teams = result;
            }, function (error) {
                me.handleError(error);
            });

        ClientService.clientDropdownList()
            .then(
                result => {
                    me.filterOptions.clients = result;
                },
                error => {
                    me.handleError(error);
                });

        me.pageGrid = new TcrGridServiceModule.TcrGridService("date",
            this.TimesheetService.timesheetGrid,
            this.onDataLoaded,
            model => {
                model.userId = me.filterModel.userId;
                // Clone so TimesheetService UTC conversion cannot mutate the period filter dates
                model.startDate = me.filterModel.startDate
                    ? new Date(me.filterModel.startDate.getFullYear(), me.filterModel.startDate.getMonth(), me.filterModel.startDate.getDate(), 0, 0, 0, 0)
                    : null;
                model.endDate = me.filterModel.endDate
                    ? new Date(me.filterModel.endDate.getFullYear(), me.filterModel.endDate.getMonth(), me.filterModel.endDate.getDate(), 0, 0, 0, 0)
                    : null;
                model.projectId = me.filterModel.projectId;
                model.billingOption = me.filterModel.billingOption.val;
            },
            null,
            $state);

        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;

        // Populate user's projects
        this.getUserProjects();
    }

    getUserProjects() {
        this.ProjectService.getUserAllocatedProjects(this.filterModel.userId, false)
            .then(
                result => {
                    this.filterOptions.userProjects = result;
                },
                error => {
                    this.handleError(error);
                });
    }

    userSelectChange() {
        this.reloadGrid();
        this.getUserProjects();
    }

    /**
     * Prefer the billing cycle that contains today; otherwise the first cycle in the list
     * (API returns newest-first by Startdate).
     */
    pickDefaultBillingCycle = (cycles: any[]): any => {
        const todayKey = this.dateToKey(this.stripTime(new Date()));
        for (let i = 0; i < cycles.length; i++) {
            const start = this.readCycleDate(cycles[i], "start");
            const end = this.readCycleDate(cycles[i], "end");
            if (!start || !end) {
                continue;
            }
            const startKey = this.dateToKey(start);
            const endKey = this.dateToKey(end);
            if (todayKey >= startKey && todayKey <= endKey) {
                return cycles[i];
            }
        }
        return cycles[0];
    };

    getCycleId = (cycle: any): any => {
        if (!cycle) {
            return null;
        }
        return cycle.id != null ? cycle.id : cycle.Id;
    };

    /** Resolve selected period from list by id (same approach as timesheet reports). */
    getSelectedBillingCycle = (): any => {
        const me = this;
        const id = me.filterModel.billingCycleId;
        if (id == null || id === "" || id === 0) {
            return null;
        }
        const cycles = me.filterOptions.billingCycles || [];
        for (let i = 0; i < cycles.length; i++) {
            const cid = me.getCycleId(cycles[i]);
            if (cid === id || String(cid) === String(id)) {
                return cycles[i];
            }
        }
        return null;
    };

    /**
     * BillingCycleDropdownModel: Startdate/Enddate → JSON camelCase startdate/enddate.
     * Same property names used by working report screens.
     */
    readCycleDate = (cycle: any, which: "start" | "end"): Date => {
        if (!cycle) {
            return null;
        }
        // Prefer the proven field names from report controllers, then fallbacks
        let raw;
        if (which === "start") {
            raw = cycle.startdate;
            if (raw == null) raw = cycle.startDate;
            if (raw == null) raw = cycle.Startdate;
            if (raw == null) raw = cycle.StartDate;
        } else {
            raw = cycle.enddate;
            if (raw == null) raw = cycle.endDate;
            if (raw == null) raw = cycle.Enddate;
            if (raw == null) raw = cycle.EndDate;
        }
        return this.parseApiDate(raw);
    };

    /**
     * Parse API DateTime as a local calendar day (avoids UTC midnight shifting the day).
     */
    parseApiDate = (raw: any): Date => {
        if (raw == null || raw === "") {
            return null;
        }
        if (raw instanceof Date || (raw && typeof raw.getTime === "function" && typeof raw.getFullYear === "function")) {
            if (isNaN(raw.getTime())) {
                return null;
            }
            return this.stripTime(raw);
        }
        if (typeof raw === "number" && !isNaN(raw)) {
            return this.stripTime(new Date(raw));
        }
        const s = String(raw).trim();
        // Microsoft JSON date: /Date(1723248000000)/ or /Date(1723248000000+0200)/
        const msMatch = /\/Date\((-?\d+)(?:[+-]\d+)?\)\//.exec(s);
        if (msMatch) {
            return this.stripTime(new Date(parseInt(msMatch[1], 10)));
        }
        // yyyy-MM-dd[THH:mm:ss...] — use calendar parts so "Z" does not shift the day backward
        const iso = /^(\d{4})-(\d{1,2})-(\d{1,2})/.exec(s);
        if (iso) {
            return new Date(parseInt(iso[1], 10), parseInt(iso[2], 10) - 1, parseInt(iso[3], 10), 0, 0, 0, 0);
        }
        // Same as original timesheet: new Date(startdate)
        const d = new Date(s);
        if (isNaN(d.getTime())) {
            return null;
        }
        return this.stripTime(d);
    };

    /**
     * Bind grid date range and week tabs to the selected billing period's defined StartDate/EndDate.
     * Called on load (default period), whenever the period dropdown changes, and on Reset.
     */
    applyBillingPeriod = (): void => {
        const me = this;
        const cycle = me.getSelectedBillingCycle();
        me.viewModel.billingCycle = cycle;

        if (!cycle) {
            me.weeks = [];
            me.selectedWeek = null;
            me.selectedWeekIndex = 0;
            me.displayOptions.show = false;
            me.filterModel.startDate = null;
            me.filterModel.endDate = null;
            return;
        }

        // Date range is solely the billing period’s configured dates
        let periodStart = me.readCycleDate(cycle, "start");
        let periodEnd = me.readCycleDate(cycle, "end");

        if (!periodStart || !periodEnd) {
            me.weeks = [];
            me.selectedWeek = null;
            me.selectedWeekIndex = 0;
            me.displayOptions.show = false;
            me.filterModel.startDate = null;
            me.filterModel.endDate = null;
            me.handleError(
                "Billing period \"" + (cycle.description || "") + "\" has no start/end dates on the server.");
            return;
        }

        if (periodEnd.getTime() < periodStart.getTime()) {
            const tmp = periodStart;
            periodStart = periodEnd;
            periodEnd = tmp;
        }

        me.filterModel.startDate = periodStart;
        me.filterModel.endDate = periodEnd;
        me.buildWeeksFromPeriod(periodStart, periodEnd);

        if (!me.weeks.length) {
            me.handleError(
                "Could not build weeks for billing period "
                + me.dateToKey(periodStart) + " – " + me.dateToKey(periodEnd) + ".");
            me.displayOptions.show = true;
            return;
        }

        me.selectWeekContainingToday();
        me.reloadGrid();
    };

    /** Alias for templates still using ng-change name history. */
    onBillingCycleChange = (): void => {
        this.applyBillingPeriod();
    };
    /** Monday-based start of the calendar week containing `date`. */
    startOfWeek = (date: any): any => {
        var now = date ? new Date(date as any) : new Date();
        now.setHours(0, 0, 0, 0);
        var monday = this.getMondayOnOrBefore(now);
        return this.convertToUTCDate(monday);
    };

    endOfWeek = (date: any): any => {
        var now = date ? new Date(date as any) : new Date();
        now.setHours(0, 0, 0, 0);
        var monday = this.getMondayOnOrBefore(now);
        var sunday = new Date(monday.getTime());
        sunday.setDate(monday.getDate() + 6);
        return this.convertToUTCDate(sunday);
    };

    convertToUTCDate = (date: Date): Date => {
        return new Date(Date.UTC(date.getFullYear(),
            date.getMonth(),
            date.getDate(),
            0,
            0,
            0));
    };

    stripTime = (date: Date): Date => {
        if (!date || typeof date.getTime !== "function" || isNaN(date.getTime())) {
            return null;
        }
        return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);
    };

    getMondayOnOrBefore = (date: Date): Date => {
        const d = this.stripTime(date);
        const day = d.getDay(); // 0=Sun … 6=Sat
        const diff = day === 0 ? -6 : 1 - day;
        d.setDate(d.getDate() + diff);
        return d;
    };

    pad2 = (n: number): string => {
        return (n < 10 ? "0" : "") + n;
    };

    dateToKey = (date: Date): string => {
        if (!date || typeof date.getTime !== "function" || isNaN(date.getTime())) {
            return "";
        }
        return date.getFullYear() + "-" + this.pad2(date.getMonth() + 1) + "-" + this.pad2(date.getDate());
    };

    /**
     * Normalize a row's dateEntry (Date or various string forms) to yyyy-MM-dd.
     */
    parseDateKey = (dateEntry: any): string => {
        if (dateEntry == null || dateEntry === "") {
            return "";
        }
        if (dateEntry instanceof Date || (dateEntry && typeof dateEntry.getTime === "function" && typeof dateEntry.getFullYear === "function")) {
            return this.dateToKey(dateEntry);
        }
        let s = String(dateEntry);
        // Drop timezone offsets (+02:00 / -0500) and time portions
        s = s.split("+")[0];
        if (s.indexOf("-") > 0 && /[+-]\d{2}:?\d{2}$/.test(s) === false) {
            // already stripped +
        }
        // Also strip trailing -HH:MM if timezone used minus (rare after + split)
        const tIdx = s.indexOf("T");
        if (tIdx >= 0) {
            s = s.substring(0, tIdx);
        } else {
            const sp = s.indexOf(" ");
            if (sp >= 0) {
                s = s.substring(0, sp);
            }
        }
        const parts = s.split("-");
        if (parts.length === 3) {
            return parts[0] + "-" + this.pad2(parseInt(parts[1], 10)) + "-" + this.pad2(parseInt(parts[2], 10));
        }
        return s;
    };

    formatDayLabel = (date: Date): string => {
        return this.dayNames[date.getDay()] + " " + date.getDate() + " " + this.monthNames[date.getMonth()];
    };

    formatShortDate = (date: Date): string => {
        return date.getDate() + " " + this.monthNames[date.getMonth()];
    };

    /**
     * Split the billing period into Mon–Sun week tabs; each day is one expandable group.
     * Days outside the period are omitted (partial first/last weeks).
     * Uses yyyy-MM-dd keys for range membership so Date object / UTC quirks cannot empty the list.
     */
    buildWeeksFromPeriod = (periodStart: Date, periodEnd: Date): void => {
        const me = this;
        const start = me.stripTime(periodStart);
        const end = me.stripTime(periodEnd);
        if (!start || !end) {
            me.weeks = [];
            me.selectedWeekIndex = 0;
            me.selectedWeek = null;
            return;
        }

        const startKey = me.dateToKey(start);
        const endKey = me.dateToKey(end);
        const weeks = [];
        let cursor = me.getMondayOnOrBefore(start);
        let weekNum = 0;
        let guard = 0;

        while (guard < 60) {
            guard++;
            const mondayKey = me.dateToKey(cursor);
            if (mondayKey > endKey) {
                // Entire week is after the period
                break;
            }

            const days = [];
            for (let i = 0; i < 7; i++) {
                const dayDate = new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate() + i, 0, 0, 0, 0);
                const dayKey = me.dateToKey(dayDate);
                if (dayKey < startKey || dayKey > endKey) {
                    continue;
                }
                days.push({
                    date: dayDate,
                    dateKey: dayKey,
                    label: me.formatDayLabel(dayDate),
                    hours: 0,
                    billhours: 0,
                    expanded: false,
                    records: []
                });
            }

            if (days.length) {
                weekNum++;
                weeks.push({
                    index: weeks.length,
                    weekNum: weekNum,
                    start: days[0].date,
                    end: days[days.length - 1].date,
                    label: "Week " + weekNum + " · " + me.formatShortDate(days[0].date) + "–" + me.formatShortDate(days[days.length - 1].date),
                    totalHours: 0,
                    days: days
                });
            }

            // Advance to next Monday
            cursor = new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate() + 7, 0, 0, 0, 0);
        }

        me.weeks = weeks;
        if (me.weeks.length) {
            me.selectedWeekIndex = 0;
            me.selectedWeek = me.weeks[0];
        } else {
            me.selectedWeekIndex = 0;
            me.selectedWeek = null;
        }
    };

    selectWeekContainingToday = (): void => {
        const me = this;
        if (!me.weeks.length) {
            return;
        }
        const todayKey = me.dateToKey(me.stripTime(new Date()));
        for (let w = 0; w < me.weeks.length; w++) {
            for (let d = 0; d < me.weeks[w].days.length; d++) {
                if (me.weeks[w].days[d].dateKey === todayKey) {
                    me.selectWeek(w);
                    return;
                }
            }
        }
        me.selectWeek(0);
    };

    selectWeek = (index: number): void => {
        if (index < 0 || index >= this.weeks.length) {
            return;
        }
        this.selectedWeekIndex = index;
        this.selectedWeek = this.weeks[index];
        this.applyDefaultDayExpand();
    };

    /** Switch to the week/day that owns a record (used when save validation fails off-tab). */
    selectWeekForRecord = (record: any): void => {
        const me = this;
        const key = me.parseDateKey(record.dateEntry);
        if (!key || !me.weeks) {
            return;
        }
        for (let w = 0; w < me.weeks.length; w++) {
            for (let d = 0; d < me.weeks[w].days.length; d++) {
                if (me.weeks[w].days[d].dateKey === key) {
                    me.selectWeek(w);
                    me.weeks[w].days[d].expanded = true;
                    return;
                }
            }
        }
    };

    toggleDay = (day: any): void => {
        day.expanded = !day.expanded;
    };

    applyDefaultDayExpand = (): void => {
        const me = this;
        if (!me.selectedWeek) {
            return;
        }
        const todayKey = me.dateToKey(me.stripTime(new Date()));
        let expandedOne = false;
        for (let i = 0; i < me.selectedWeek.days.length; i++) {
            const day = me.selectedWeek.days[i];
            const isToday = day.dateKey === todayKey;
            const hasHours = day.hours > 0 || (day.records && day.records.length);
            day.expanded = isToday || hasHours;
            if (day.expanded) {
                expandedOne = true;
            }
        }
        if (!expandedOne && me.selectedWeek.days.length) {
            me.selectedWeek.days[0].expanded = true;
        }
    };

    /**
     * Project flat grid rows onto the week/day structure (same object references for editing).
     */
    rebuildWeekRecords = (): void => {
        const me = this;
        if (!me.weeks || !me.weeks.length) {
            return;
        }

        const byKey: any = {};
        if (me.gridModel && me.gridModel.data) {
            for (let i = 0; i < me.gridModel.data.length; i++) {
                const row = me.gridModel.data[i];
                const key = me.parseDateKey(row.dateEntry);
                if (!key) {
                    continue;
                }
                if (!byKey[key]) {
                    byKey[key] = [];
                }
                byKey[key].push(row);
            }
        }

        for (let w = 0; w < me.weeks.length; w++) {
            let weekHours = 0;
            for (let d = 0; d < me.weeks[w].days.length; d++) {
                const day = me.weeks[w].days[d];
                day.records = byKey[day.dateKey] || [];
                let hours = 0;
                let billhours = 0;
                for (let r = 0; r < day.records.length; r++) {
                    const h = day.records[r].hours || 0;
                    hours += h;
                    if (day.records[r].billable) {
                        billhours += h;
                    }
                }
                day.hours = hours;
                day.billhours = billhours;
                weekHours += hours;
            }
            me.weeks[w].totalHours = weekHours;
        }

        if (me.weeks[me.selectedWeekIndex]) {
            me.selectedWeek = me.weeks[me.selectedWeekIndex];
        }
    };

    private onLoadEvent(event: TcrGridModel): void {
        // Get local timezone
        // getTimezoneOffset returns difference between UTC and Local Timezone so have to swap minus value
        // +2:00 timezone will be returned as -120
        var timezone = (new Date().getTimezoneOffset() / 60) * -1;
        var timezoneString = ':00';
        if (Math.abs(timezone % 1) == 0.5) {
            var timezoneString = ':30';
        }
        timezone = Math.floor(timezone);


        // Check if single or double digit
        if (Math.abs(timezone) > 9) {
            if (timezone < 0)
                timezoneString = timezone.toString() + timezoneString;
            else
                timezoneString = '+' + timezone.toString() + timezoneString;
        }
        else {
            if (timezone < 0)
                timezoneString = '-0' + timezone.toString()[1] + timezoneString;
            else
                timezoneString = '+0' + timezone.toString() + timezoneString;
        }

        for (var tmIdx = 0; tmIdx < event.data.length; tmIdx++) {
            event.data[tmIdx].dateEntry = event.data[tmIdx].dateEntry + timezoneString;
        }

        this.gridModel = event;
        this.rebuildWeekRecords();
        this.applyDefaultDayExpand();
        this.summaryList();
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    getProjectId = (id): any => {
        return this.$filter("filter")(this.filterOptions.projects, { id: id })[0];
    };

    getOriginalRecord = (record): any => {
        if (record)
            if (record.id)
                return this.$filter("filter")(this.gridModel.originalData, { id: record.id })[0];
        return null;
    };

    resetFilters = (): void => {
        if (this.filterOptions.billingCycles && this.filterOptions.billingCycles.length) {
            const defaultCycle = this.pickDefaultBillingCycle(this.filterOptions.billingCycles);
            this.filterModel.billingCycleId = this.getCycleId(defaultCycle);
            this.viewModel.billingCycle = defaultCycle;
        } else {
            this.filterModel.billingCycleId = null;
            this.viewModel.billingCycle = null;
        }
        this.filterModel.billingOption = this.filterOptions.billingOptions[0];
        this.filterModel.projectId = "";
        this.filterModel.projectDescription = "";
        this.applyBillingPeriod();
    }

    /**
     * Validate a row from the model (and form controls when present).
     * Form controls only exist for the selected week's day rows; other weeks must still validate on save.
     */
    validateRecordValues = (record): string => {
        const me = this;
        const form = me.$scope["RecordForm"];

        const ctrlInvalid = (name: string): boolean => {
            if (!form || !form[name]) {
                return false;
            }
            return !!form[name].$invalid;
        };

        if (ctrlInvalid(`projectGridId${record.id}`) || !(record.projectGridId || record.projectId)) {
            return "Project is not valid";
        }
        if (ctrlInvalid(`teamId${record.id}`) || record.teamId == null || record.teamId === "") {
            return "Team is not valid";
        }
        if (!me.parseDateKey(record.dateEntry)) {
            return "Date is not valid";
        }
        if (ctrlInvalid(`activityId${record.id}`) || record.activityId == null || record.activityId === "") {
            return "Activity is not valid";
        }
        if (ctrlInvalid(`comments${record.id}`) || record.comments == null || String(record.comments).trim() === "") {
            return "Comments is not valid";
        }
        if (ctrlInvalid(`hours${record.id}`) || record.hours == null || record.hours === "" || isNaN(record.hours)) {
            return "Hours is not valid";
        }
        return null;
    }

    //#endregion

    /**
     * Add an empty capture line under a specific day — date is implied, no picker.
     */
    addRowForDay = (day: any) => {
        const me = this;

        if (!me.filterModel.userId) {
            me.handleError("Please select a user in the filter!");
            return;
        }

        if (!me.filterModel.billingCycleId) {
            me.handleError("Please select a billing period.");
            return;
        }

        if (!me.gridModel || !me.gridModel.data) {
            me.gridModel = { data: [], originalData: [], totalItems: 0 } as any;
        }

        const entryDate = new Date(day.date.getFullYear(), day.date.getMonth(), day.date.getDate(), 0, 0, 0, 0);

        var newRecord = {
            userAccountId: me.filterModel.userId,
            projectGridId: null,
            projectId: null,
            projectDescription: null,
            clientEntityName: null,
            billable: null,
            subProjectId: null,
            teamId: null,
            activityId: null,
            comments: null,
            hours: null,
            dateEntry: entryDate,
            id: new Date().getTime(),
            new: true,
            valid: {
                'projectGridId': false, 'dateEntry': true, 'teamId': false, 'activityId': false, 'comments': false, 'hours': false
            }
        };

        me.gridModel.data.push(newRecord);
        day.records.push(newRecord);
        day.expanded = true;
        me.refreshDayTotals(day);
    };

    submitForm = () => {

        const me = this;
        if (!me.gridModel || !me.gridModel.data) {
            return;
        }
        if (me.gridModel.data == me.gridModel.originalData) {
            return;
        }

        for (var i = 0; i < me.gridModel.data.length; i++) {
            const validation = me.validateRecordValues(me.gridModel.data[i]);
            if (validation) {
                me.$scope.$broadcast("show-errors-check-validity");
                // Jump to the week that holds the failing row so the user can fix it
                me.selectWeekForRecord(me.gridModel.data[i]);
                me.handleError(validation);
                return;
            }
        }
        var dateTimeInt = new Date().getTime();
        for (var i = 0; i < me.gridModel.data.length; i++) {
            if (me.gridModel.data[i].dateEntry.getTime) {
                me.gridModel.data[i].dateEntry = me.gridModel.data[i].dateEntry.getFullYear() + "-" + (me.gridModel.data[i].dateEntry.getMonth() + 1) + "-" + me.gridModel.data[i].dateEntry.getDate() + " 00:00:00";
            }
            else {
                // Adding logic to remove timezone
                let s = me.gridModel.data[i].dateEntry.split('+')[0];
                me.gridModel.data[i].dateEntry = s;
            }

            if (me.gridModel.data[i].id <= dateTimeInt) {
                me.gridModel.data[i].id = null;
            }
        }


        me.TimesheetService.timesheetListSave(me.gridModel.data)
            .then(
                result => {
                    me.saveSuccess = true;
                    me.$timeout(function () {

                        me.saveSuccess = false;
                        me.reloadGrid();
                    },
                        1000);

                },
                error => {
                    for (var i = 0; i < me.gridModel.data.length; i++) {
                        if (me.gridModel.data[i].id == null) {
                            me.gridModel.data[i].id = new Date().getTime() + 1;
                        }
                    }

                    me.handleError(error);
                    me.Popups.showError(me.$scope, error);
                });
    };

    deleteRecord = (record) => {
        const me = this;
        me.Popups.confirmationDialog(me.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this record...")
            .then(
                action => {
                    if (action)
                        if (!record.new) {
                            me.TimesheetService.timesheetDelete(record)
                                .then(
                                    result => {
                                        me.saveSuccess = true;
                                        me.$timeout(function () {

                                            me.saveSuccess = false;
                                            me.reloadGrid();
                                        },
                                            1000);
                                    },
                                    error => {
                                        me.handleError(error);
                                        me.Popups.showError(me.$scope, error);
                                    });
                        } else {
                            const index = me.gridModel.data.indexOf(record);
                            if (index >= 0) {
                                me.gridModel.data.splice(index, 1);
                            }
                            me.rebuildWeekRecords();
                            me.summaryList();
                        }

                },
                error => {
                    me.handleError(error);
                    me.Popups.showError(me.$scope, error);
                });
    };

    submitnewRecords = () => {

        const me = this;

        if (!me.filterModel.userId) {
            me.handleError("Please select a user in the filter!");
            return;
        }

        me.Popups.timeSheetRecordDailog(me.$scope,
            "Add Records")
            .then(
                action => {
                    if (action) {

                        let project = action.project;
                        let startDate = new Date(action.startDate);
                        let endDate = new Date(action.endDate);
                        let team = action.team;
                        let activity = action.activity;

                        if (endDate <= startDate) {
                            me.handleError("Selected End Date must be after Start Date");
                        } else {

                            endDate.setHours(0, 0, 0, 0);
                            startDate.setHours(0, 0, 0, 0);
                            let count = Math.round(Math.abs((endDate.getTime() - startDate.getTime()) / (24 * 60 * 60 * 1000)));
                            for (let i = 0; i <= (count); i++) {

                                let date = new Date(startDate.getTime());
                                date.setDate(date.getDate() + i);

                                var newRecord = {
                                    userAccountId: me.filterModel.userId,
                                    projectDescription: project.projectDescription,
                                    projectGridId: project.projectId,
                                    projectId: project.projectId,
                                    clientEntityName: project.clientName || '',
                                    billable: project.isBillable,
                                    subProjectId: project.subProjectId,
                                    project: {
                                        description: project.projectDescription,
                                        id: project.projectId,
                                        projectId: project.projectId,
                                        projectName: project.projectDescription,
                                        subProjectId: project.subProjectId,
                                        subProjectName: ""
                                    },
                                    teamId: team,
                                    activityId: activity,
                                    comments: null,
                                    hours: null,
                                    dateEntry: date,
                                    id: new Date().getTime() + i,
                                    new: true,
                                    valid: {}
                                };

                                me.gridModel.data.push(newRecord);

                                newRecord.valid["dateEntry"] = true;
                                newRecord.valid["teamId"] = false;
                                newRecord.valid["activityId"] = false;
                                newRecord.valid["comments"] = false;
                                newRecord.valid["hours"] = false;
                            }
                            me.rebuildWeekRecords();
                            me.applyDefaultDayExpand();
                            me.summaryList();
                        }
                    }
                },
                error => {
                    me.handleError(error);
                });
    };

    cancelForm = (): void => {
        this.$state.transitionTo("mainState.home");
    };

    reloadGrid = () => {
        const me = this;
        if (!me.filterModel.userId) {
            me.handleError("Please select a user in the filter!");
            return;
        }
        if (!me.filterModel.billingCycleId) {
            me.handleError("Please select a billing period.");
            return;
        }
        // Ensure date range always matches the selected billing period before loading
        const cycle = me.getSelectedBillingCycle();
        if (cycle) {
            const start = me.readCycleDate(cycle, "start");
            const end = me.readCycleDate(cycle, "end");
            if (start && end) {
                me.filterModel.startDate = start.getTime() <= end.getTime() ? start : end;
                me.filterModel.endDate = start.getTime() <= end.getTime() ? end : start;
                if (!me.weeks || !me.weeks.length) {
                    me.buildWeeksFromPeriod(me.filterModel.startDate, me.filterModel.endDate);
                    me.selectWeekContainingToday();
                }
            }
        }
        if (!me.filterModel.startDate || !me.filterModel.endDate) {
            me.handleError("Selected billing period has no start/end dates.");
            return;
        }
        me.displayOptions.show = true;
        me.pageGrid.loadGrid();
        me.summaryList();
    };

    projectSelected = (project) => {
        const me = this;
        me.filterModel.projectId = project.id;
        me.filterModel.projectDescription = project.description;
        me.filterModel.subProjectId = project.subProjectId;
    };

    projectRowSelected = (rowRecord) => {
        const me = this;
        rowRecord.projectDescription = rowRecord.project.description;
        rowRecord.projectGridId = rowRecord.project.id;
        rowRecord.projectId = rowRecord.project.id;
        rowRecord.subProjectId = rowRecord.project.subProjectId;
    };

    projectTemplateSelected = (templateRecord) => {
        const me = this;
        templateRecord.project.projectDescription = templateRecord.project.description;
        templateRecord.project.projectGridId = templateRecord.project.id;
        templateRecord.project.projectId = templateRecord.project.id;
        templateRecord.project.subProjectId = templateRecord.project.subProjectId;
    };

    validateHours = (propertyName, object) => {
        if (object.hours < 0) object.hours = 0;

        var number = Math.floor(object.hours);
        var fraction = object.hours % 1;
        if (fraction < 0.12)
            object.hours = number;
        if (fraction >= 0.12 && fraction < 0.37)
            object.hours = number + 0.25;
        if (fraction >= 0.37 && fraction < 0.62)
            object.hours = number + 0.5;
        if (fraction >= 0.62 && fraction < 0.87)
            object.hours = number + 0.75;
        if (fraction >= 0.87)
            object.hours = number + 1;

        this.refreshTotalsForRecord(object);
    }

    validateOriginal = (propertyName, object) => {
        const me = this;
        var originalObject = this.getOriginalRecord(object);
        if (!object.valid) {
            object.valid = {};
        }

        if (originalObject != null) {
            object.valid[propertyName] = originalObject[propertyName] === object[propertyName];
        } else {
            object.valid[propertyName] = false;
        }

        if (propertyName === "hours") {
            me.refreshTotalsForRecord(object);
        }
    };

    refreshTotalsForRecord = (object: any): void => {
        const me = this;
        const key = me.parseDateKey(object.dateEntry);
        if (me.selectedWeek) {
            for (let i = 0; i < me.selectedWeek.days.length; i++) {
                if (me.selectedWeek.days[i].dateKey === key) {
                    me.refreshDayTotals(me.selectedWeek.days[i]);
                    break;
                }
            }
        }
        // Keep week tab totals fresh across all weeks
        me.rebuildWeekRecords();
        me.summaryList();
    };

    refreshDayTotals = (day: any): void => {
        let hours = 0;
        let billhours = 0;
        for (let r = 0; r < day.records.length; r++) {
            const h = day.records[r].hours || 0;
            hours += h;
            if (day.records[r].billable) {
                billhours += h;
            }
        }
        day.hours = hours;
        day.billhours = billhours;
    };

    resetRecord = (object) => {
        const me = this;
        me.Popups.confirmationDialog(me.$scope,
            "Load original values?",
            "You are about to reset the values back to the orignal...")
            .then(
                action => {
                    if (action) {
                        const originalObject = me.getOriginalRecord(object);
                        if (originalObject != null) {
                            object.projectGridId = originalObject.projectGridId;
                            object.projectId = originalObject.projectId;
                            object.projectDescription = originalObject.projectDescription;
                            object.subProjectId = originalObject.subProjectId;
                            object.valid["projectGridId"] = true;
                            object.teamId = originalObject.teamId;
                            object.valid["teamId"] = true;
                            object.dateEntry = originalObject.dateEntry;
                            object.valid["dateEntry"] = true;
                            object.activityId = originalObject.activityId;
                            object.valid["activityId"] = true;
                            object.comments = originalObject.comments;
                            object.valid["comments"] = true;
                            object.hours = originalObject.hours;
                            object.valid["hours"] = true;
                            object.clientEntityName = originalObject.clientEntityName;
                            object.billable = originalObject.billable;
                            me.refreshTotalsForRecord(object);
                        }
                    }
                },
                error => {
                    me.handleError(error);
                });
    };

    summaryList = () => {
        const me = this;
        me.summary = {};
        me.summary.totalHours = 0;
        me.summary.totalBillableHours = 0;
        me.summary.days = [];
        if (me.gridModel && me.gridModel.data) {
            for (let i = 0; i < me.gridModel.data.length; i++) {
                const date = me.parseDateKey(me.gridModel.data[i].dateEntry);
                if (!date) {
                    continue;
                }
                const hours = me.gridModel.data[i].hours || 0;
                const existing = me.$filter("filter")(me.summary.days, { date: date }, true)[0];
                if (existing) {
                    existing["hours"] += hours;
                    if (me.gridModel.data[i].billable)
                        existing["billhours"] += hours;
                }
                else {
                    var billhours = 0;
                    if (me.gridModel.data[i].billable)
                        billhours += hours;
                    me.summary.days.push({ date: date, hours: hours, billhours: billhours });
                }

                me.summary.totalHours += hours;
                if (me.gridModel.data[i].billable)
                    me.summary.totalBillableHours += hours;
            }
        }
    };

    openProjectListModal = (item) => {
        const me = this;
        me.Popups.filteredDropDownModal(me.$scope, me.filterOptions.userProjects)
            .then(function (project) {

                if (project === undefined) {
                    if (item === undefined)
                        item = {};

                    item.projectDescription = null;
                    item.projectId = null;
                    item.projectGridId = null;
                    item.subProjectId = null;
                    item.clientEntityName = '';
                    item.billable = false;
                }
                else {
                    if (item === undefined)
                        item = {};

                    item.projectDescription = project.description;
                    item.projectId = project.projectId;
                    item.projectGridId = project.projectId;
                    item.subProjectId = project.subProjectId;
                    item.clientEntityName = project.clientName;
                    item.billable = project.isBillable;
                }
                me.validateOriginal('projectGridId', item)
            });

    };
}

angular.module("AngularApp")
    .controller("TimesheetController",
        [
            "$stateParams",
            "$timeout",
            "$window",
            "$state",
            "$scope",
            "$uibModal",
            "$log",
            "$filter",
            "ActivityService",
            "TeamService",
            "UserService",
            "ProjectService",
            "BillingCycleService",
            "ClientService",
            "TimesheetService",
            "SecurityService",
            "Popups",
            TimesheetController
        ]);
