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

    addRowRecord: any;
    disableDateSelection: boolean = false;

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
            .then(
                results => {
                    me.filterOptions.billingCycles = results;
                    me.filterOptions.billingCycles.splice(0, 0, {
                        id: 0,
                        description: "Manual Date"
                    });
                    me.viewModel.billingCycle = me.filterOptions.billingCycles[0];
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
                model.startDate = me.filterModel.startDate;
                model.endDate = me.filterModel.endDate;
                model.projectId = me.filterModel.projectId;
                model.billingOption = me.filterModel.billingOption.val;
            },
            null,
            $state);

        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;

        //if (me.filterModel.userId === null)
        //this.$state.go("mainState.home");

        // Populate user's projects
        this.getUserProjects();

        me.filterModel.startDate = me.startOfWeek(null);
        me.filterModel.endDate = me.endOfWeek(null);
        me.filterModel.projectId = me.filterModel.projectId;
        me.filterModel.projectId = me.filterModel.projectId;
        me.reloadGrid();
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

    checkDateDisable = (): void => {
        this.disableDateSelection = this.viewModel.billingCycle.id == 0 ? false : true;

        if (this.disableDateSelection) {
            this.filterModel.startDate = new Date(this.viewModel.billingCycle.startdate);
            this.filterModel.endDate = new Date(this.viewModel.billingCycle.enddate);
        }
    };

    startOfWeek = (date: any): any => {

        // If no date object supplied, use current date
        // Copy date so don't modify supplied date
        var now = date ? new Date(date) : new Date();

        // set time to some convenient value
        now.setHours(0, 0, 0, 0);

        // Get the previous Monday
        var monday = new Date(now.toDateString());
        monday.setDate(monday.getDate() - monday.getDay() + 1);

        //return monday


        return this.convertToUTCDate(monday);
    };
    endOfWeek = (date: any): any => {

        // If no date object supplied, use current date
        // Copy date so don't modify supplied date
        var now = date ? new Date(date) : new Date();

        // set time to some convenient value
        now.setHours(0, 0, 0, 0);

        // Get next Sunday
        var sunday = new Date(now.toDateString());
        sunday.setDate(sunday.getDate() - sunday.getDay() + 7);

        //  return sunday
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
        this.disableDateSelection = false;
        this.viewModel.billingCycle = this.filterOptions.billingCycles[0];
        this.viewModel.billingOption = this.filterOptions.billingOptions[0];
        this.filterModel.startDate = this.startOfWeek(null);
        this.filterModel.endDate = this.endOfWeek(null);
        this.filterModel.projectId = "";
        this.filterModel.projectDescription = "";
        this.reloadGrid();
    }

    validateRecordValues = (record): string => {
        //if (!record.id) return null;
        const me = this;

        if (me.$scope["RecordForm"][`projectGridId${record.id}`].$invalid) return "Project is not valid";
        if (me.$scope["RecordForm"][`teamId${record.id}`].$invalid) return "Team is not valid";
        if (me.$scope["RecordForm"][`dateEntry${record.id}`].$invalid) return "Date is not valid";
        if (me.$scope["RecordForm"][`activityId${record.id}`].$invalid) return "Activity is not valid";
        if (me.$scope["RecordForm"][`comments${record.id}`].$invalid) return "Comments is not valid";
        if (me.$scope["RecordForm"][`hours${record.id}`].$invalid) return "Hours is not valid";
        return null;
    }

    //#endregion

    validateAddRecord = (): string => {
        //if (!record.id) return null;
        const me = this;

        if (me.$scope["AddForm"]['projectId'].$invalid) return "Project is not valid";
        if (me.$scope["AddForm"]['teamId'].$invalid) return "Team is not valid";
        if (me.$scope["AddForm"]['dateEntry'].$invalid) return "Date is not valid";
        if (me.$scope["AddForm"]['activityId'].$invalid) return "Activity is not valid";
        if (me.$scope["AddForm"]['comments'].$invalid) return "Comments is not valid";
        if (me.$scope["AddForm"]['hours'].$invalid) return "Hours is not valid";
        return null;
    }

    submitAddRow = () => {
        const me = this;
        const validation = me.validateAddRecord();
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


    }

    submitForm = () => {

        const me = this;
        if (me.gridModel.data == me.gridModel.originalData) {
            return;
        }

        for (var i = 0; i < me.gridModel.data.length; i++) {

            const validation = me.validateRecordValues(me.gridModel.data[i]);
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
                let s = me.gridModel.data[i].dateEntry.split('+')[0];
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
                            me.gridModel.data.splice(index, 1);
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

                            //let count = endDate.getDate() - startDate.getDate();
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
                },
                error => {
                    me.handleError(error);
                });
    };

    submitNewRecord = () => {
        const me = this;

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

    cancelForm = (): void => {
        this.$state.transitionTo("mainState.home");
    };

    reloadGrid = () => {
        const me = this;
        if (!me.filterModel.userId) {
            me.handleError("Please select a user in the filter!");
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
        //if (object.hours > 24) object.hours = 24;
        if (object.hours < 0) object.hours = 0;

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
    }

    validateOriginal = (propertyName, object) => {
        const me = this;
        var originalObject = this.getOriginalRecord(object);
        if (!object.valid) {
            object.valid = {};
        }

        if (originalObject != null) {
            object.valid[propertyName] = originalObject[propertyName] === object[propertyName];
            return;
        } else {
            object.valid[propertyName] = false;
            return false;
        };
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
                const date = me.gridModel.data[i].dateEntry.split("T")[0];
                const existing = me.$filter("filter")(me.summary.days, { date: date })[0];
                if (existing) {
                    existing["hours"] += me.gridModel.data[i].hours;
                    if (me.gridModel.data[i].billable)
                        existing["billhours"] += me.gridModel.data[i].hours
                }
                else {
                    var billhours = 0;
                    if (me.gridModel.data[i].billable)
                        billhours += me.gridModel.data[i].hours
                    me.summary.days.push({ date: date, hours: me.gridModel.data[i].hours, billhours: billhours });
                }

                me.summary.totalHours += me.gridModel.data[i].hours;
                if (me.gridModel.data[i].billable)
                    me.summary.totalBillableHours += me.gridModel.data[i].hours;
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