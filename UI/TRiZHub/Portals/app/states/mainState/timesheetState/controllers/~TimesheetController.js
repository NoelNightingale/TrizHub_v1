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
        _this.disableDateSelection = false;
        _this.checkDateDisable = function () {
            _this.disableDateSelection = _this.viewModel.billingCycle.id == 0 ? false : true;
            if (_this.disableDateSelection) {
                _this.filterModel.startDate = new Date(_this.viewModel.billingCycle.startdate);
                _this.filterModel.endDate = new Date(_this.viewModel.billingCycle.enddate);
            }
        };
        _this.startOfWeek = function (date) {
            // If no date object supplied, use current date
            // Copy date so don't modify supplied date
            var now = date ? new Date(date) : new Date();
            // set time to some convenient value
            now.setHours(0, 0, 0, 0);
            // Get the previous Monday
            var monday = new Date(now.toDateString());
            monday.setDate(monday.getDate() - monday.getDay() + 1);
            //return monday
            return _this.convertToUTCDate(monday);
        };
        _this.endOfWeek = function (date) {
            // If no date object supplied, use current date
            // Copy date so don't modify supplied date
            var now = date ? new Date(date) : new Date();
            // set time to some convenient value
            now.setHours(0, 0, 0, 0);
            // Get next Sunday
            var sunday = new Date(now.toDateString());
            sunday.setDate(sunday.getDate() - sunday.getDay() + 7);
            //  return sunday
            return _this.convertToUTCDate(sunday);
        };
        _this.convertToUTCDate = function (date) {
            return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0));
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
            _this.disableDateSelection = false;
            _this.viewModel.billingCycle = _this.filterOptions.billingCycles[0];
            _this.viewModel.billingOption = _this.filterOptions.billingOptions[0];
            _this.filterModel.startDate = _this.startOfWeek(null);
            _this.filterModel.endDate = _this.endOfWeek(null);
            _this.filterModel.projectId = "";
            _this.filterModel.projectDescription = "";
            _this.reloadGrid();
        };
        _this.validateRecordValues = function (record) {
            //if (!record.id) return null;
            var me = _this;
            if (me.$scope["RecordForm"]["projectGridId".concat(record.id)].$invalid)
                return "Project is not valid";
            if (me.$scope["RecordForm"]["teamId".concat(record.id)].$invalid)
                return "Team is not valid";
            if (me.$scope["RecordForm"]["dateEntry".concat(record.id)].$invalid)
                return "Date is not valid";
            if (me.$scope["RecordForm"]["activityId".concat(record.id)].$invalid)
                return "Activity is not valid";
            if (me.$scope["RecordForm"]["comments".concat(record.id)].$invalid)
                return "Comments is not valid";
            if (me.$scope["RecordForm"]["hours".concat(record.id)].$invalid)
                return "Hours is not valid";
            return null;
        };
        //#endregion
        _this.validateAddRecord = function () {
            //if (!record.id) return null;
            var me = _this;
            if (me.$scope["AddForm"]['projectId'].$invalid)
                return "Project is not valid";
            if (me.$scope["AddForm"]['teamId'].$invalid)
                return "Team is not valid";
            if (me.$scope["AddForm"]['dateEntry'].$invalid)
                return "Date is not valid";
            if (me.$scope["AddForm"]['activityId'].$invalid)
                return "Activity is not valid";
            if (me.$scope["AddForm"]['comments'].$invalid)
                return "Comments is not valid";
            if (me.$scope["AddForm"]['hours'].$invalid)
                return "Hours is not valid";
            return null;
        };
        _this.submitAddRow = function () {
            var me = _this;
            var validation = me.validateAddRecord();
            if (validation) {
                me.$scope.$broadcast("show-errors-check-validity");
                if (me.$scope["AddForm"].$invalid)
                    return;
                me.handleError(validation);
                return;
            }
            if (!me.filterModel.userId) {
                me.handleError("Please select a user in the filter!");
                return;
            }
            var newRecord = {
                userAccountId: me.filterModel.userId,
                projectGridId: me.addRowRecord.projectGridId,
                clientEntityName: me.addRowRecord.clientEntityName,
                billable: me.addRowRecord.billable,
                projectId: me.addRowRecord.projectId,
                projectDescription: me.addRowRecord.projectDescription,
                subProjectId: me.addRowRecord.subProjectId,
                teamId: me.addRowRecord.teamId,
                activityId: me.addRowRecord.activityId,
                comments: me.addRowRecord.comments,
                hours: me.addRowRecord.hours,
                dateEntry: me.addRowRecord.dateEntry,
                id: new Date().getTime(),
                new: true,
                valid: {
                    'projectGridId': false, 'dateEntry': false, 'teamId': false, 'activityId': false, 'comments': false, 'hours': false
                }
            };
            me.gridModel.data.push(newRecord);
        };
        _this.submitForm = function () {
            var me = _this;
            if (me.gridModel.data == me.gridModel.originalData) {
                return;
            }
            for (var i = 0; i < me.gridModel.data.length; i++) {
                var validation = me.validateRecordValues(me.gridModel.data[i]);
                if (validation) {
                    me.$scope.$broadcast("show-errors-check-validity");
                    if (me.$scope["RecordForm"].$invalid)
                        return;
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
            me.$scope.$broadcast("show-errors-check-validity");
            if (me.$scope["RecordForm"].$invalid)
                return;
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
                        me.gridModel.data.splice(index, 1);
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
            me.Popups.timeSheetRecordDailog(me.$scope, "Add Records")
                .then(function (action) {
                if (action) {
                    var project = action.project;
                    var startDate = new Date(action.startDate);
                    var endDate = new Date(action.endDate);
                    var team = action.team;
                    var activity = action.activity;
                    if (endDate <= startDate) {
                        me.handleError("Selected End Date must be after Start Date");
                    }
                    else {
                        //let count = endDate.getDate() - startDate.getDate();
                        endDate.setHours(0, 0, 0, 0);
                        startDate.setHours(0, 0, 0, 0);
                        var count = Math.round(Math.abs((endDate.getTime() - startDate.getTime()) / (24 * 60 * 60 * 1000)));
                        for (var i = 0; i <= (count); i++) {
                            var date = new Date(startDate.getTime());
                            date.setDate(date.getDate() + i);
                            var newRecord = {
                                userAccountId: me.filterModel.userId,
                                projectDescription: project.projectDescription,
                                projectGridId: project.projectId,
                                projectId: project.projectId,
                                clientEntityName: me.addRowRecord.clientEntityName,
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
                            // Mark row as invalid
                            newRecord.valid["dateEntry"] = false;
                            newRecord.valid["teamId"] = false;
                            newRecord.valid["activityId"] = false;
                            newRecord.valid["comments"] = false;
                            newRecord.valid["hours"] = false;
                        }
                    }
                }
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.submitNewRecord = function () {
            var me = _this;
            if (!me.filterModel.userId) {
                me.handleError("Please select a user in the filter!");
                return;
            }
            var newRecord = {
                userAccountId: me.filterModel.userId,
                projectGridId: null,
                projectId: null,
                projectDescription: null,
                subProjectId: null,
                teamId: null,
                activityId: null,
                comments: null,
                hours: null,
                dateEntry: null,
                id: new Date().getTime(),
                new: true,
            };
            me.gridModel.data.push(newRecord);
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
            //if (object.hours > 24) object.hours = 24;
            if (object.hours < 0)
                object.hours = 0;
            var number = Math.floor(object.hours);
            var fraction = object.hours % 1;
            var number = Math.floor(object.hours);
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
        };
        _this.validateOriginal = function (propertyName, object) {
            var me = _this;
            var originalObject = _this.getOriginalRecord(object);
            if (!object.valid) {
                object.valid = {};
            }
            if (originalObject != null) {
                object.valid[propertyName] = originalObject[propertyName] === object[propertyName];
                return;
            }
            else {
                object.valid[propertyName] = false;
                return false;
            }
            ;
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
                    var date = me.gridModel.data[i].dateEntry.split("T")[0];
                    var existing = me.$filter("filter")(me.summary.days, { date: date })[0];
                    if (existing) {
                        existing["hours"] += me.gridModel.data[i].hours;
                        if (me.gridModel.data[i].billable)
                            existing["billhours"] += me.gridModel.data[i].hours;
                    }
                    else {
                        var billhours = 0;
                        if (me.gridModel.data[i].billable)
                            billhours += me.gridModel.data[i].hours;
                        me.summary.days.push({ date: date, hours: me.gridModel.data[i].hours, billhours: billhours });
                    }
                    me.summary.totalHours += me.gridModel.data[i].hours;
                    if (me.gridModel.data[i].billable)
                        me.summary.totalBillableHours += me.gridModel.data[i].hours;
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
                me.validateOriginal('projectGridId', item);
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
            billingOptions: [
                { val: 0, description: "All" },
                { val: 1, description: "Yes" },
                { val: 2, description: "No" },
            ],
            userTeams: []
        };
        me.addRowRecord = {
            userAccountId: null,
            projectGridId: null,
            projectId: null,
            projectDescription: null,
            subProjectId: null,
            teamId: null,
            activityId: null,
            comments: null,
            hours: null,
            dateEntry: null,
            id: new Date().getTime(),
            new: true,
        };
        // Set default Billable
        me.filterModel.billingOption = me.filterOptions.billingOptions[0];
        //        ProjectService.projectAndSubProjectDropdownList()
        //            .then(
        //                result => {
        //                    me.filterOptions.userProjects = result;
        //                },
        //               error => {
        //                    me.handleError(error);
        //                });
        BillingCycleService.billingCycleDropdownList()
            .then(function (results) {
            me.filterOptions.billingCycles = results;
            me.filterOptions.billingCycles.splice(0, 0, {
                id: 0,
                description: "Manual Date"
            });
            me.viewModel.billingCycle = me.filterOptions.billingCycles[0];
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
            model.startDate = me.filterModel.startDate;
            model.endDate = me.filterModel.endDate;
            model.projectId = me.filterModel.projectId;
            model.billingOption = me.filterModel.billingOption.val;
        }, null, $state);
        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;
        //if (me.filterModel.userId === null)
        //this.$state.go("mainState.home");
        // Populate user's projects
        _this.getUserProjects();
        me.filterModel.startDate = me.startOfWeek(null);
        me.filterModel.endDate = me.endOfWeek(null);
        me.filterModel.projectId = me.filterModel.projectId;
        me.filterModel.projectId = me.filterModel.projectId;
        me.reloadGrid();
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
    TimesheetController.prototype.userSelectChange = function () {
        this.reloadGrid();
        this.getUserProjects();
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