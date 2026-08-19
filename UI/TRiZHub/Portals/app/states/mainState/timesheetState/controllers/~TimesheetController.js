var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var TimesheetController = /** @class */ (function (_super) {
    __extends(TimesheetController, _super);
    //#endregion
    //#region Ctor
    function TimesheetController($stateParams, $timeout, $window, $state, $scope, $uibModal, $log, $filter, ActivityService, TeamService, UserService, ProjectService, BillingCycleService, ClientService, TimesheetService, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.$scope = $scope;
        _this.$uibModal = $uibModal;
        _this.$log = $log;
        _this.$filter = $filter;
        _this.ActivityService = ActivityService;
        _this.TeamService = TeamService;
        _this.UserService = UserService;
        _this.ProjectService = ProjectService;
        _this.BillingCycleService = BillingCycleService;
        _this.ClientService = ClientService;
        _this.TimesheetService = TimesheetService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.show = String();
        _this.rowId = 1;
        /** Week tabs derived from the selected billing period (Mon–Sun, clamped to period). */
        _this.weeks = [];
        _this.selectedWeekIndex = 0;
        _this.selectedWeek = null;
        _this.dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        _this.monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        _this.weekDayHeaders = [
            { index: 0, short: "M", full: "Monday" },
            { index: 1, short: "T", full: "Tuesday" },
            { index: 2, short: "W", full: "Wednesday" },
            { index: 3, short: "T", full: "Thursday" },
            { index: 4, short: "F", full: "Friday" },
            { index: 5, short: "S", full: "Saturday" },
            { index: 6, short: "S", full: "Sunday" }
        ];
        _this.userSelectChange = function () {
            _this.getUserProjects();
            _this.reloadGrid();
        };
        /** Day for heatmap column 0=Mon … 6=Sun, or null if that weekday is outside the period. */
        _this.dayAt = function (week, col) {
            if (!week) {
                return null;
            }
            if (week.daysByCol && week.daysByCol.length === 7) {
                return week.daysByCol[col] || null;
            }
            var days = week.days || [];
            for (var i = 0; i < days.length; i++) {
                if (_this.weekdayIndexOf(days[i]) === col) {
                    return days[i];
                }
            }
            return null;
        };
        _this.heatDateNum = function (week, col) {
            var day = _this.dayAt(week, col);
            if (!day) {
                return "";
            }
            if (day.dayOfMonth != null && day.dayOfMonth !== "") {
                return day.dayOfMonth;
            }
            return _this.dayOfMonthOf(day) || "";
        };
        _this.heatHours = function (week, col) {
            var day = _this.dayAt(week, col);
            if (!day) {
                return "";
            }
            var hours = Number(day.hours);
            return isNaN(hours) ? 0 : hours;
        };
        _this.heatClassFor = function (week, col) {
            var day = _this.dayAt(week, col);
            if (!day) {
                return "out-of-period";
            }
            var hours = Number(day.hours);
            var h = isNaN(hours) ? 0 : hours;
            var weekend = col >= 5;
            if (h <= 0) {
                return weekend ? "in-period is-weekend hrs-empty" : "in-period hrs-gap";
            }
            if (h < 8) {
                return weekend ? "in-period is-weekend hrs-low" : "in-period hrs-low";
            }
            return weekend ? "in-period is-weekend hrs-ok" : "in-period hrs-ok";
        };
        /** Heatmap cell click: switch week tab and open that day. */
        _this.jumpToCol = function (week, col) {
            var day = _this.dayAt(week, col);
            if (!week || !day) {
                return;
            }
            _this.selectWeek(week.index);
            day.expanded = true;
        };
        _this.clearProjectFilter = function ($event) {
            if ($event) {
                $event.stopPropagation();
                $event.preventDefault();
            }
            _this.filterModel.projectId = "";
            _this.filterModel.projectDescription = "";
            _this.filterModel.subProjectId = null;
            _this.reloadGrid();
        };
        /**
         * Prefer the billing cycle that contains today; otherwise the first cycle in the list
         * (API returns newest-first by Startdate).
         */
        _this.pickDefaultBillingCycle = function (cycles) {
            var todayKey = _this.dateToKey(_this.stripTime(new Date()));
            for (var i = 0; i < cycles.length; i++) {
                var start = _this.readCycleDate(cycles[i], "start");
                var end = _this.readCycleDate(cycles[i], "end");
                if (!start || !end) {
                    continue;
                }
                var startKey = _this.dateToKey(start);
                var endKey = _this.dateToKey(end);
                if (todayKey >= startKey && todayKey <= endKey) {
                    return cycles[i];
                }
            }
            return cycles[0];
        };
        _this.getCycleId = function (cycle) {
            if (!cycle) {
                return null;
            }
            return cycle.id != null ? cycle.id : cycle.Id;
        };
        /** Resolve selected period from list by id (same approach as timesheet reports). */
        _this.getSelectedBillingCycle = function () {
            var me = _this;
            var id = me.filterModel.billingCycleId;
            if (id == null || id === "" || id === 0) {
                return null;
            }
            var cycles = me.filterOptions.billingCycles || [];
            for (var i = 0; i < cycles.length; i++) {
                var cid = me.getCycleId(cycles[i]);
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
        _this.readCycleDate = function (cycle, which) {
            if (!cycle) {
                return null;
            }
            // Prefer the proven field names from report controllers, then fallbacks
            var raw;
            if (which === "start") {
                raw = cycle.startdate;
                if (raw == null)
                    raw = cycle.startDate;
                if (raw == null)
                    raw = cycle.Startdate;
                if (raw == null)
                    raw = cycle.StartDate;
            }
            else {
                raw = cycle.enddate;
                if (raw == null)
                    raw = cycle.endDate;
                if (raw == null)
                    raw = cycle.Enddate;
                if (raw == null)
                    raw = cycle.EndDate;
            }
            return _this.parseApiDate(raw);
        };
        /**
         * Parse API DateTime as a local calendar day (avoids UTC midnight shifting the day).
         */
        _this.parseApiDate = function (raw) {
            if (raw == null || raw === "") {
                return null;
            }
            if (raw instanceof Date || (raw && typeof raw.getTime === "function" && typeof raw.getFullYear === "function")) {
                if (isNaN(raw.getTime())) {
                    return null;
                }
                return _this.stripTime(raw);
            }
            if (typeof raw === "number" && !isNaN(raw)) {
                return _this.stripTime(new Date(raw));
            }
            var s = String(raw).trim();
            // Microsoft JSON date: /Date(1723248000000)/ or /Date(1723248000000+0200)/
            var msMatch = /\/Date\((-?\d+)(?:[+-]\d+)?\)\//.exec(s);
            if (msMatch) {
                return _this.stripTime(new Date(parseInt(msMatch[1], 10)));
            }
            // yyyy-MM-dd[THH:mm:ss...] — use calendar parts so "Z" does not shift the day backward
            var iso = /^(\d{4})-(\d{1,2})-(\d{1,2})/.exec(s);
            if (iso) {
                return new Date(parseInt(iso[1], 10), parseInt(iso[2], 10) - 1, parseInt(iso[3], 10), 0, 0, 0, 0);
            }
            // Same as original timesheet: new Date(startdate)
            var d = new Date(s);
            if (isNaN(d.getTime())) {
                return null;
            }
            return _this.stripTime(d);
        };
        _this.decorateBillingCycleOptions = function (cycles) {
            var list = cycles || [];
            for (var i = 0; i < list.length; i++) {
                list[i].optionLabel = _this.billingCycleOptionLabel(list[i]);
            }
            return list;
        };
        _this.billingCycleOptionLabel = function (cycle) {
            var desc = (cycle && cycle.description) ? String(cycle.description) : "";
            var start = _this.readCycleDate(cycle, "start");
            var end = _this.readCycleDate(cycle, "end");
            if (!start || !end) {
                return desc;
            }
            return desc + "  (" + _this.dateToKey(start).replace(/-/g, "/") + " – " + _this.dateToKey(end).replace(/-/g, "/") + ")";
        };
        /**
         * Bind grid date range and week tabs to the selected billing period's defined StartDate/EndDate.
         * Called on load (default period), whenever the period dropdown changes, and on Reset.
         */
        _this.applyBillingPeriod = function () {
            var me = _this;
            var cycle = me.getSelectedBillingCycle();
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
            var periodStart = me.readCycleDate(cycle, "start");
            var periodEnd = me.readCycleDate(cycle, "end");
            if (!periodStart || !periodEnd) {
                me.weeks = [];
                me.selectedWeek = null;
                me.selectedWeekIndex = 0;
                me.displayOptions.show = false;
                me.filterModel.startDate = null;
                me.filterModel.endDate = null;
                me.handleError("Billing period \"" + (cycle.description || "") + "\" has no start/end dates on the server.");
                return;
            }
            if (periodEnd.getTime() < periodStart.getTime()) {
                var tmp = periodStart;
                periodStart = periodEnd;
                periodEnd = tmp;
            }
            me.filterModel.startDate = periodStart;
            me.filterModel.endDate = periodEnd;
            me.buildWeeksFromPeriod(periodStart, periodEnd);
            if (!me.weeks.length) {
                me.handleError("Could not build weeks for billing period "
                    + me.dateToKey(periodStart) + " – " + me.dateToKey(periodEnd) + ".");
                me.displayOptions.show = true;
                return;
            }
            me.selectWeekContainingToday();
            me.reloadGrid();
        };
        /** Alias for templates still using ng-change name history. */
        _this.onBillingCycleChange = function () {
            _this.applyBillingPeriod();
        };
        /** Monday-based start of the calendar week containing `date`. */
        _this.startOfWeek = function (date) {
            var now = date ? new Date(date) : new Date();
            now.setHours(0, 0, 0, 0);
            var monday = _this.getMondayOnOrBefore(now);
            return _this.convertToUTCDate(monday);
        };
        _this.endOfWeek = function (date) {
            var now = date ? new Date(date) : new Date();
            now.setHours(0, 0, 0, 0);
            var monday = _this.getMondayOnOrBefore(now);
            var sunday = new Date(monday.getTime());
            sunday.setDate(monday.getDate() + 6);
            return _this.convertToUTCDate(sunday);
        };
        _this.convertToUTCDate = function (date) {
            return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0));
        };
        _this.stripTime = function (date) {
            if (!date || typeof date.getTime !== "function" || isNaN(date.getTime())) {
                return null;
            }
            return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);
        };
        _this.getMondayOnOrBefore = function (date) {
            var d = _this.stripTime(date);
            var day = d.getDay(); // 0=Sun … 6=Sat
            var diff = day === 0 ? -6 : 1 - day;
            d.setDate(d.getDate() + diff);
            return d;
        };
        _this.pad2 = function (n) {
            return (n < 10 ? "0" : "") + n;
        };
        _this.dateToKey = function (date) {
            if (!date || typeof date.getTime !== "function" || isNaN(date.getTime())) {
                return "";
            }
            return date.getFullYear() + "-" + _this.pad2(date.getMonth() + 1) + "-" + _this.pad2(date.getDate());
        };
        /**
         * Normalize a row's dateEntry (Date or various string forms) to yyyy-MM-dd.
         */
        _this.parseDateKey = function (dateEntry) {
            if (dateEntry == null || dateEntry === "") {
                return "";
            }
            if (dateEntry instanceof Date || (dateEntry && typeof dateEntry.getTime === "function" && typeof dateEntry.getFullYear === "function")) {
                return _this.dateToKey(dateEntry);
            }
            var s = String(dateEntry);
            // Drop timezone offsets (+02:00 / -0500) and time portions
            s = s.split("+")[0];
            if (s.indexOf("-") > 0 && /[+-]\d{2}:?\d{2}$/.test(s) === false) {
                // already stripped +
            }
            // Also strip trailing -HH:MM if timezone used minus (rare after + split)
            var tIdx = s.indexOf("T");
            if (tIdx >= 0) {
                s = s.substring(0, tIdx);
            }
            else {
                var sp = s.indexOf(" ");
                if (sp >= 0) {
                    s = s.substring(0, sp);
                }
            }
            var parts = s.split("-");
            if (parts.length === 3) {
                return parts[0] + "-" + _this.pad2(parseInt(parts[1], 10)) + "-" + _this.pad2(parseInt(parts[2], 10));
            }
            return s;
        };
        _this.weekdayIndexOf = function (day) {
            if (!day) {
                return -1;
            }
            if (typeof day.weekdayIndex === "number" && day.weekdayIndex >= 0 && day.weekdayIndex <= 6) {
                return day.weekdayIndex;
            }
            if (day.date && typeof day.date.getDay === "function" && !isNaN(day.date.getTime())) {
                return (day.date.getDay() + 6) % 7;
            }
            if (day.dateKey) {
                var parts = String(day.dateKey).split("-");
                if (parts.length === 3) {
                    var dt = new Date(parseInt(parts[0], 10), parseInt(parts[1], 10) - 1, parseInt(parts[2], 10), 0, 0, 0, 0);
                    if (!isNaN(dt.getTime())) {
                        return (dt.getDay() + 6) % 7;
                    }
                }
            }
            return -1;
        };
        _this.dayOfMonthOf = function (day) {
            if (!day) {
                return 0;
            }
            if (day.date && typeof day.date.getDate === "function" && !isNaN(day.date.getTime())) {
                return day.date.getDate();
            }
            if (day.dateKey) {
                var parts = String(day.dateKey).split("-");
                if (parts.length === 3) {
                    return parseInt(parts[2], 10) || 0;
                }
            }
            return 0;
        };
        _this.indexDaysByCol = function (week) {
            var slots = [null, null, null, null, null, null, null];
            if (!week) {
                return;
            }
            var days = week.days || [];
            for (var i = 0; i < days.length; i++) {
                var day = days[i];
                var col = _this.weekdayIndexOf(day);
                if (col < 0) {
                    continue;
                }
                day.weekdayIndex = col;
                day.dayOfMonth = _this.dayOfMonthOf(day);
                slots[col] = day;
            }
            week.daysByCol = slots;
        };
        _this.formatDayLabel = function (date) {
            return _this.dayNames[date.getDay()] + " " + date.getDate() + " " + _this.monthNames[date.getMonth()];
        };
        _this.formatShortDate = function (date) {
            return date.getDate() + " " + _this.monthNames[date.getMonth()];
        };
        /**
         * Split the billing period into Mon–Sun week tabs; each day is one expandable group.
         * Days outside the period are omitted (partial first/last weeks).
         * Uses yyyy-MM-dd keys for range membership so Date object / UTC quirks cannot empty the list.
         */
        _this.buildWeeksFromPeriod = function (periodStart, periodEnd) {
            var me = _this;
            var start = me.stripTime(periodStart);
            var end = me.stripTime(periodEnd);
            if (!start || !end) {
                me.weeks = [];
                me.selectedWeekIndex = 0;
                me.selectedWeek = null;
                return;
            }
            var startKey = me.dateToKey(start);
            var endKey = me.dateToKey(end);
            var weeks = [];
            var cursor = me.getMondayOnOrBefore(start);
            var weekNum = 0;
            var guard = 0;
            while (guard < 60) {
                guard++;
                var mondayKey = me.dateToKey(cursor);
                if (mondayKey > endKey) {
                    // Entire week is after the period
                    break;
                }
                var days = [];
                for (var i = 0; i < 7; i++) {
                    var dayDate = new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate() + i, 0, 0, 0, 0);
                    var dayKey = me.dateToKey(dayDate);
                    if (dayKey < startKey || dayKey > endKey) {
                        continue;
                    }
                    days.push({
                        date: dayDate,
                        dateKey: dayKey,
                        label: me.formatDayLabel(dayDate),
                        weekdayIndex: (dayDate.getDay() + 6) % 7,
                        dayOfMonth: dayDate.getDate(),
                        hours: 0,
                        billhours: 0,
                        expanded: false,
                        records: []
                    });
                }
                if (days.length) {
                    weekNum++;
                    var week = {
                        index: weeks.length,
                        weekNum: weekNum,
                        start: days[0].date,
                        end: days[days.length - 1].date,
                        label: "Week " + weekNum + " · " + me.formatShortDate(days[0].date) + "–" + me.formatShortDate(days[days.length - 1].date),
                        totalHours: 0,
                        days: days,
                        daysByCol: null
                    };
                    me.indexDaysByCol(week);
                    weeks.push(week);
                }
                // Advance to next Monday
                cursor = new Date(cursor.getFullYear(), cursor.getMonth(), cursor.getDate() + 7, 0, 0, 0, 0);
            }
            me.weeks = weeks;
            if (me.weeks.length) {
                me.selectedWeekIndex = 0;
                me.selectedWeek = me.weeks[0];
            }
            else {
                me.selectedWeekIndex = 0;
                me.selectedWeek = null;
            }
        };
        _this.selectWeekContainingToday = function () {
            var me = _this;
            if (!me.weeks.length) {
                return;
            }
            var todayKey = me.dateToKey(me.stripTime(new Date()));
            for (var w = 0; w < me.weeks.length; w++) {
                for (var d = 0; d < me.weeks[w].days.length; d++) {
                    if (me.weeks[w].days[d].dateKey === todayKey) {
                        me.selectWeek(w);
                        return;
                    }
                }
            }
            me.selectWeek(0);
        };
        _this.selectWeek = function (index) {
            if (index < 0 || index >= _this.weeks.length) {
                return;
            }
            _this.selectedWeekIndex = index;
            _this.selectedWeek = _this.weeks[index];
            _this.applyDefaultDayExpand();
        };
        /** Switch to the week/day that owns a record (used when save validation fails off-tab). */
        _this.selectWeekForRecord = function (record) {
            var me = _this;
            var key = me.parseDateKey(record.dateEntry);
            if (!key || !me.weeks) {
                return;
            }
            for (var w = 0; w < me.weeks.length; w++) {
                for (var d = 0; d < me.weeks[w].days.length; d++) {
                    if (me.weeks[w].days[d].dateKey === key) {
                        me.selectWeek(w);
                        me.weeks[w].days[d].expanded = true;
                        return;
                    }
                }
            }
        };
        _this.toggleDay = function (day) {
            day.expanded = !day.expanded;
        };
        _this.applyDefaultDayExpand = function () {
            var me = _this;
            if (!me.selectedWeek) {
                return;
            }
            var todayKey = me.dateToKey(me.stripTime(new Date()));
            var expandedOne = false;
            for (var i = 0; i < me.selectedWeek.days.length; i++) {
                var day = me.selectedWeek.days[i];
                var isToday = day.dateKey === todayKey;
                var hasHours = day.hours > 0 || (day.records && day.records.length);
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
        _this.rebuildWeekRecords = function () {
            var me = _this;
            if (!me.weeks || !me.weeks.length) {
                return;
            }
            var byKey = {};
            if (me.gridModel && me.gridModel.data) {
                for (var i = 0; i < me.gridModel.data.length; i++) {
                    var row = me.gridModel.data[i];
                    var key = me.parseDateKey(row.dateEntry);
                    if (!key) {
                        continue;
                    }
                    if (!byKey[key]) {
                        byKey[key] = [];
                    }
                    byKey[key].push(row);
                }
            }
            for (var w = 0; w < me.weeks.length; w++) {
                var weekHours = 0;
                for (var d = 0; d < me.weeks[w].days.length; d++) {
                    var day = me.weeks[w].days[d];
                    day.records = byKey[day.dateKey] || [];
                    var hours = 0;
                    var billhours = 0;
                    for (var r = 0; r < day.records.length; r++) {
                        var h = day.records[r].hours || 0;
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
                me.indexDaysByCol(me.weeks[w]);
            }
            if (me.weeks[me.selectedWeekIndex]) {
                me.selectedWeek = me.weeks[me.selectedWeekIndex];
            }
        };
        _this.getProjectId = function (id) {
            return _this.$filter("filter")(_this.filterOptions.projects, { id: id })[0];
        };
        _this.getOriginalRecord = function (record) {
            if (record)
                if (record.id)
                    return _this.$filter("filter")(_this.gridModel.originalData, { id: record.id })[0];
            return null;
        };
        _this.resetFilters = function () {
            if (_this.filterOptions.billingCycles && _this.filterOptions.billingCycles.length) {
                var defaultCycle = _this.pickDefaultBillingCycle(_this.filterOptions.billingCycles);
                _this.filterModel.billingCycleId = _this.getCycleId(defaultCycle);
                _this.viewModel.billingCycle = defaultCycle;
            }
            else {
                _this.filterModel.billingCycleId = null;
                _this.viewModel.billingCycle = null;
            }
            _this.filterModel.billingOption = _this.filterOptions.billingOptions[0];
            _this.filterModel.projectId = "";
            _this.filterModel.projectDescription = "";
            _this.applyBillingPeriod();
        };
        /**
         * Validate a row from the model (and form controls when present).
         * Form controls only exist for the selected week's day rows; other weeks must still validate on save.
         */
        _this.validateRecordValues = function (record) {
            var me = _this;
            var form = me.$scope["RecordForm"];
            var ctrlInvalid = function (name) {
                if (!form || !form[name]) {
                    return false;
                }
                return !!form[name].$invalid;
            };
            if (ctrlInvalid("projectGridId".concat(record.id)) || !(record.projectGridId || record.projectId)) {
                return "Project is not valid";
            }
            if (ctrlInvalid("teamId".concat(record.id)) || record.teamId == null || record.teamId === "") {
                return "Team is not valid";
            }
            if (!me.parseDateKey(record.dateEntry)) {
                return "Date is not valid";
            }
            if (ctrlInvalid("activityId".concat(record.id)) || record.activityId == null || record.activityId === "") {
                return "Activity is not valid";
            }
            if (ctrlInvalid("comments".concat(record.id)) || record.comments == null || String(record.comments).trim() === "") {
                return "Comments is not valid";
            }
            if (ctrlInvalid("hours".concat(record.id)) || record.hours == null || record.hours === "" || isNaN(record.hours)) {
                return "Hours is not valid";
            }
            return null;
        };
        //#endregion
        /**
         * Add an empty capture line under a specific day — date is implied, no picker.
         */
        _this.addRowForDay = function (day) {
            var me = _this;
            if (!me.filterModel.userId) {
                me.handleError("Please select a user in the filter!");
                return;
            }
            if (!me.filterModel.billingCycleId) {
                me.handleError("Please select a billing period.");
                return;
            }
            if (!me.gridModel || !me.gridModel.data) {
                me.gridModel = { data: [], originalData: [], totalItems: 0 };
            }
            var entryDate = new Date(day.date.getFullYear(), day.date.getMonth(), day.date.getDate(), 0, 0, 0, 0);
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
        _this.submitForm = function () {
            var me = _this;
            if (!me.gridModel || !me.gridModel.data) {
                return;
            }
            if (me.gridModel.data == me.gridModel.originalData) {
                return;
            }
            for (var i = 0; i < me.gridModel.data.length; i++) {
                var validation = me.validateRecordValues(me.gridModel.data[i]);
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
                    var s = me.gridModel.data[i].dateEntry.split('+')[0];
                    me.gridModel.data[i].dateEntry = s;
                }
                if (me.gridModel.data[i].id <= dateTimeInt) {
                    me.gridModel.data[i].id = null;
                }
            }
            me.TimesheetService.timesheetListSave(me.gridModel.data)
                .then(function (result) {
                me.saveSuccess = true;
                me.$timeout(function () {
                    me.saveSuccess = false;
                    me.reloadGrid();
                }, 1000);
            }, function (error) {
                for (var i = 0; i < me.gridModel.data.length; i++) {
                    if (me.gridModel.data[i].id == null) {
                        me.gridModel.data[i].id = new Date().getTime() + 1;
                    }
                }
                me.handleError(error);
                me.Popups.showError(me.$scope, error);
            });
        };
        _this.deleteRecord = function (record) {
            var me = _this;
            me.Popups.confirmationDialog(me.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (action)
                    if (!record.new) {
                        me.TimesheetService.timesheetDelete(record)
                            .then(function (result) {
                            me.saveSuccess = true;
                            me.$timeout(function () {
                                me.saveSuccess = false;
                                me.reloadGrid();
                            }, 1000);
                        }, function (error) {
                            me.handleError(error);
                            me.Popups.showError(me.$scope, error);
                        });
                    }
                    else {
                        var index = me.gridModel.data.indexOf(record);
                        if (index >= 0) {
                            me.gridModel.data.splice(index, 1);
                        }
                        me.rebuildWeekRecords();
                        me.summaryList();
                    }
            }, function (error) {
                me.handleError(error);
                me.Popups.showError(me.$scope, error);
            });
        };
        _this.submitnewRecords = function () {
            var me = _this;
            if (!me.filterModel.userId) {
                me.handleError("Please select a user in the filter!");
                return;
            }
            if (!me.weeks || !me.weeks.length) {
                me.handleError("Please select a billing period with valid weeks first.");
                return;
            }
            me.Popups.timeSheetRecordDailog(me.$scope, "Add Records", null, null, me.weeks)
                .then(function (action) {
                if (action && action.result) {
                    var project = action.project;
                    var team = action.team;
                    var activity = action.activity;
                    var bulkHours = action.hours != null ? action.hours : null;
                    var bulkComments = action.comments || null;
                    var selectedDates = action.selectedDates || [];
                    if (!project || !project.projectId) {
                        me.handleError("Please select a project for the template lines.");
                        return;
                    }
                    if (team == null || team === "") {
                        me.handleError("Please select a team for the template lines.");
                        return;
                    }
                    if (activity == null || activity === "") {
                        me.handleError("Please select an activity for the template lines.");
                        return;
                    }
                    if (!selectedDates.length) {
                        me.handleError("Please select at least one day.");
                        return;
                    }
                    if (!me.gridModel || !me.gridModel.data) {
                        me.gridModel = { data: [], originalData: [], totalItems: 0 };
                    }
                    for (var i = 0; i < selectedDates.length; i++) {
                        var date = selectedDates[i];
                        if (!(date instanceof Date)) {
                            date = new Date(date);
                        }
                        date = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);
                        var newRecord = {
                            userAccountId: me.filterModel.userId,
                            projectDescription: project.projectDescription,
                            projectGridId: project.projectId,
                            projectId: project.projectId,
                            clientEntityName: project.clientEntityName || project.clientName || '',
                            billable: project.billable != null ? project.billable : project.isBillable,
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
                            comments: bulkComments,
                            hours: bulkHours,
                            dateEntry: date,
                            id: new Date().getTime() + i,
                            new: true,
                            valid: {}
                        };
                        me.gridModel.data.push(newRecord);
                        newRecord.valid["dateEntry"] = true;
                        newRecord.valid["teamId"] = false;
                        newRecord.valid["activityId"] = false;
                        newRecord.valid["projectGridId"] = false;
                        newRecord.valid["comments"] = false;
                        newRecord.valid["hours"] = false;
                    }
                    me.rebuildWeekRecords();
                    me.applyDefaultDayExpand();
                    me.summaryList();
                }
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.cancelForm = function () {
            _this.$state.transitionTo("mainState.home");
        };
        _this.reloadGrid = function () {
            var me = _this;
            if (!me.filterModel.userId) {
                me.handleError("Please select a user in the filter!");
                return;
            }
            if (!me.filterModel.billingCycleId) {
                me.handleError("Please select a billing period.");
                return;
            }
            // Ensure date range always matches the selected billing period before loading
            var cycle = me.getSelectedBillingCycle();
            if (cycle) {
                var start = me.readCycleDate(cycle, "start");
                var end = me.readCycleDate(cycle, "end");
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
        _this.projectSelected = function (project) {
            var me = _this;
            me.filterModel.projectId = project.id;
            me.filterModel.projectDescription = project.description;
            me.filterModel.subProjectId = project.subProjectId;
        };
        _this.projectRowSelected = function (rowRecord) {
            var me = _this;
            rowRecord.projectDescription = rowRecord.project.description;
            rowRecord.projectGridId = rowRecord.project.id;
            rowRecord.projectId = rowRecord.project.id;
            rowRecord.subProjectId = rowRecord.project.subProjectId;
        };
        _this.projectTemplateSelected = function (templateRecord) {
            var me = _this;
            templateRecord.project.projectDescription = templateRecord.project.description;
            templateRecord.project.projectGridId = templateRecord.project.id;
            templateRecord.project.projectId = templateRecord.project.id;
            templateRecord.project.subProjectId = templateRecord.project.subProjectId;
        };
        _this.validateHours = function (propertyName, object) {
            if (object.hours < 0)
                object.hours = 0;
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
            _this.refreshTotalsForRecord(object);
        };
        _this.validateOriginal = function (propertyName, object) {
            var me = _this;
            var originalObject = _this.getOriginalRecord(object);
            if (!object.valid) {
                object.valid = {};
            }
            if (originalObject != null) {
                object.valid[propertyName] = originalObject[propertyName] === object[propertyName];
            }
            else {
                object.valid[propertyName] = false;
            }
            if (propertyName === "hours") {
                me.refreshTotalsForRecord(object);
            }
        };
        _this.refreshTotalsForRecord = function (object) {
            var me = _this;
            var key = me.parseDateKey(object.dateEntry);
            if (me.selectedWeek) {
                for (var i = 0; i < me.selectedWeek.days.length; i++) {
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
        _this.refreshDayTotals = function (day) {
            var hours = 0;
            var billhours = 0;
            for (var r = 0; r < day.records.length; r++) {
                var h = day.records[r].hours || 0;
                hours += h;
                if (day.records[r].billable) {
                    billhours += h;
                }
            }
            day.hours = hours;
            day.billhours = billhours;
        };
        _this.resetRecord = function (object) {
            var me = _this;
            me.Popups.confirmationDialog(me.$scope, "Load original values?", "You are about to reset the values back to the orignal...")
                .then(function (action) {
                if (action) {
                    var originalObject = me.getOriginalRecord(object);
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
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.summaryList = function () {
            var me = _this;
            me.summary = {};
            me.summary.totalHours = 0;
            me.summary.totalBillableHours = 0;
            me.summary.days = [];
            if (me.gridModel && me.gridModel.data) {
                for (var i = 0; i < me.gridModel.data.length; i++) {
                    var date = me.parseDateKey(me.gridModel.data[i].dateEntry);
                    if (!date) {
                        continue;
                    }
                    var hours = me.gridModel.data[i].hours || 0;
                    var existing = me.$filter("filter")(me.summary.days, { date: date }, true)[0];
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
        _this.openProjectListModal = function (item) {
            var me = _this;
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
                if (item === me.filterModel) {
                    me.reloadGrid();
                }
                else {
                    me.validateOriginal('projectGridId', item);
                }
            });
        };
        var me = _this;
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
        me.gridModel = { data: [], originalData: [], totalItems: 0 };
        me.summary = { days: [], totalHours: 0, totalBillableHours: 0 };
        // Set default Billable — Manual Date is not offered; period always comes from a real billing cycle
        me.filterModel.billingOption = me.filterOptions.billingOptions[0];
        me.filterModel.billingCycleId = null;
        me.filterModel.startDate = null;
        me.filterModel.endDate = null;
        BillingCycleService.billingCycleDropdownList()
            .then(function (results) {
            // Real cycles only — no "Manual Date" synthetic row
            me.filterOptions.billingCycles = me.decorateBillingCycleOptions(results || []);
            if (me.filterOptions.billingCycles.length) {
                var defaultCycle = me.pickDefaultBillingCycle(me.filterOptions.billingCycles);
                me.filterModel.billingCycleId = me.getCycleId(defaultCycle);
                me.viewModel.billingCycle = defaultCycle;
                me.applyBillingPeriod();
            }
        }, function (error) {
            me.handleError(error);
        });
        UserService.userTimesheetFilterDropdown()
            .then(function (result) {
            me.filterOptions.users = result;
        }, function (error) {
            me.handleError(error);
        });
        ActivityService.activityDropdownList()
            .then(function (result) {
            me.filterOptions.activities = result;
        }, function (error) {
            me.handleError(error);
        });
        TeamService.teamDropdownList()
            .then(function (result) {
            me.filterOptions.teams = result;
        }, function (error) {
            me.handleError(error);
        });
        ClientService.clientDropdownList()
            .then(function (result) {
            me.filterOptions.clients = result;
        }, function (error) {
            me.handleError(error);
        });
        me.pageGrid = new TcrGridServiceModule.TcrGridService("date", _this.TimesheetService.timesheetGrid, _this.onDataLoaded, function (model) {
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
        }, null, $state);
        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;
        // Populate user's projects
        _this.getUserProjects();
        return _this;
    }
    TimesheetController.prototype.getUserProjects = function () {
        var _this = this;
        this.ProjectService.getUserAllocatedProjects(this.filterModel.userId, false)
            .then(function (result) {
            _this.filterOptions.userProjects = result;
        }, function (error) {
            _this.handleError(error);
        });
    };
    TimesheetController.prototype.onLoadEvent = function (event) {
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
    };
    return TimesheetController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("TimesheetController", [
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
//# sourceMappingURL=~TimesheetController.js.map