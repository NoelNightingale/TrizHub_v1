class TimeSheetReportController extends CHControllerBase {
    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel = {
        showRates: false,
        showPhases: false,
        showUnassigned: true,
        clients: "All",
        projects: "All",
        projectWildCardSearch: "",
        employers: "All",
        employees: "All",
        startEarliest: false,
        endLatest: false,
        includeInactive: true,
        includeInactiveClients: true,
        includeInactiveProjects: false,
        includeInactiveEmployers: false,
        reportType: "BillableReportExcel",
        billingCycleId: 0,
        startDate: null,
        endDate: null,
        projectId: null
    };
    scorecardPeriodId: string;
    clientDropdown: any;
    userDropdown: any;
    filterOptions: any;
    filterModel: any;

    disableFilter = false;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $filter: ng.IFilterService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private ClientService: ClientServiceModule.ClientService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private EmployerService: EmployerServiceModule.EmployerService,
        private EnumService: EnumServiceModule.EnumService,
        private UserService: UserServiceModule.UserService,
        private ReportService: ReportServiceModule.ReportService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private AccountService: AccountServiceModule.AccountService,
        private Popups: any) {
        super($scope, Popups, $state);

        const self = this;

        if (!SecurityService.userHasPrivileges) {
            AccountService.getCurrentUser()
                .then(
                    function (result) {
                        if (!SecurityService.isAllowed("ReportGenerationTimesheet"))
                            $state.go("mainState.home");
                    },
                    function (e) {
                        this.$state.go("root.login");
                    });
        }
        else {
            if (!SecurityService.isAllowed("ReportGenerationTimesheet"))
                $state.go("mainState.home");
        }

        var currentUser = this.SecurityService.getCurrentUserDetails();

        //self.viewModel = {
        //    showRates: false,
        //    showPhases: false,
        //    showUnassigned: true,
        //    clients: "All",
        //    projects: "All",
        //    projectWildCardSearch: "",
        //    employers: "All",
        //    employees: "All",
        //    startEarliest: false,
        //    endLatest: false,
        //    includeInactive: true,
        //    includeInactiveClients: true,
        //    includeInactiveProjects: false,
        //    includeInactiveEmployers: false,
        //    reportType: "BillableReportExcel"
        //};

        self.filterOptions = {
            evaluators: [],
            employees: [],
            allEmployers: [],
            employers: [],
        };

        UserService.allUserDropdownList()
            .then(
                result => {
                    self.filterOptions.allemployees = result;
                    this.updateActiveUsers();
                },
                error => {
                    self.handleError(error);
                });

        ClientService.clientDropdownList()
            .then(
                result => {
                    self.filterOptions.billableClients = result;
                    self.filterOptions.allClients = result;
                },
                error => {
                    self.handleError(error);
                });

        ProjectService.projectDropdownList()
            .then(
                result => {
                    self.filterOptions.projects = result;
                },
                error => {
                    self.handleError(error);
                });

        ProjectService.allProjectDropdownList()
            .then(
                result => {
                    self.filterOptions.allProjects = result;
                },
                error => {
                    self.handleError(error);
                });

        EmployerService.allEmployerDropdownList()
            .then(
                result => {
                    self.filterOptions.allEmployers = result;
                    this.updateActiveEmployers();
                },
                error => {
                    self.handleError(error);
                });

        BillingCycleService.billingCycleDropdownList()
            .then(
                results => {
                    self.filterOptions.billingCycles = results;
                    self.filterOptions.billingCycles.splice(0, 0, {
                        id: 0,
                        description: "Manual Date"
                    });
                    self.viewModel.billingCycleId = 0;
                },
                error => {
                    self.handleError(error);
                });
    }

    updateActiveUsers = (): void => {
        const self = this;
        self.filterOptions.employees = [];
        for (var i = 0; i < self.filterOptions.allemployees.length; i++) {
            if (self.filterOptions.allemployees[i].accountName == "Yes" || self.viewModel.includeInactive)
                self.filterOptions.employees.push(self.filterOptions.allemployees[i]);
        }
    }

    updateActiveClients = (): void => {
        const self = this;
        self.filterOptions.allClients = [];
        for (var i = 0; i < self.filterOptions.billableClients.length; i++) {
            if (self.filterOptions.billableClients[i].isActive || self.viewModel.includeInactiveClients)
                self.filterOptions.allClients.push(self.filterOptions.billableClients[i]);
        }
    }

    updateActiveProjects = (): void => {
        const self = this;
        console.log(self.viewModel.includeInactiveProjects);
        //self.filterOptions.allProjects = [];
        //for (var i = 0; i < self.filterOptions.projects.length; i++) {
        //    if (self.filterOptions.projects[i].isActive || self.viewModel.includeInactiveProjects)
        //        self.filterOptions.allProjects.push(self.filterOptions.projects[i]);
        //}
        //console.log(self.filterOptions.allProjects.length);
    }

    updateActiveEmployers = (): void => {
        const self = this;
        self.filterOptions.employers = [];
        for (var i = 0; i < self.filterOptions.allEmployers.length; i++) {
            if (self.filterOptions.allEmployers[i].isActive || self.viewModel.includeInactiveEmployers)
                self.filterOptions.employers.push(self.filterOptions.allEmployers[i]);
        }
    }

    updateFilter = (popupModel): void => {
        const self = this;
        self.disableFilter = false;
        if (self.viewModel.billingCycleId == 0) return;
        for (let j = 0; j < self.filterOptions.billingCycles.length; j++) {
            if (self.filterOptions.billingCycles[j].id === self.viewModel.billingCycleId) {
                self.viewModel.startDate = self.filterOptions.billingCycles[j].startdate;
                self.viewModel.endDate = self.filterOptions.billingCycles[j].enddate;
                self.disableFilter = true;
                self.viewModel.startEarliest = false;
                self.viewModel.endLatest = false;
                break;
            }
        }
    };

    showEmployerFilter = (): boolean => {
        if (this.viewModel.reportType == "TimesheetDetailExcel" || this.viewModel.reportType == "TimesheetSummaryExcel") {
            return true;
        }

        return false;
    }

    clearDate = (type): void => {
        const self = this;
        if (type == 'start') {
            self.viewModel.startDate = null;
        }
        else if (type == 'end') {
            self.viewModel.endDate = null;
        }
    };

    //submitFormOld = (reportType): any => {
    //    const self = this;
    //    self.$scope.$broadcast("show-errors-check-validity");
    //    if (self.$scope["EditForm"].$invalid)
    //        return;

    //    var startDate = null;
    //    var endDate = null;
    //    try {
    //        startDate = self.viewModel.startDate.toJSON();
    //    }
    //    catch (e) {
    //        startDate = self.viewModel.startDate;
    //    }
    //    try {
    //        endDate = self.viewModel.endDate.toJSON();
    //    }
    //    catch (e) {
    //        endDate = self.viewModel.endDate;
    //    }
    //    if (!startDate && !self.viewModel.startEarliest) {
    //        self.handleError("Start Date should be supplied...");
    //        return;
    //    }
    //    if (!endDate && !self.viewModel.endLatest) {
    //        self.handleError("End Date should be supplied...");
    //        return;
    //    }

    //    var billableClients = "";
    //    if (self.viewModel.clients == "All")
    //        billableClients = "All";
    //    else {
    //        var comma = "";
    //        for (let j = 0; j < self.filterOptions.billableClients.length; j++) {
    //            if (self.filterOptions.billableClients[j].selected) {
    //                billableClients += comma + self.filterOptions.billableClients[j].id;
    //                comma = ",";
    //            }
    //        }
    //    }
    //    var projects = "";
    //    if (self.viewModel.projects == "All")
    //        projects = "All";
    //    else {
    //        var comma = "";
    //        if (self.viewModel.includeInactiveProjects) {
    //            for (let j = 0; j < self.filterOptions.allProjects.length; j++) {
    //                if (self.filterOptions.allProjects[j].selected) {
    //                    projects += comma + self.filterOptions.allProjects[j].id;
    //                    comma = ",";
    //                }
    //            }
    //        }
    //        else {
    //            for (let j = 0; j < self.filterOptions.projects.length; j++) {
    //                if (self.filterOptions.projects[j].selected) {
    //                    projects += comma + self.filterOptions.projects[j].id;
    //                    comma = ",";
    //                }
    //            }
    //        }
    //    }

    //    var projectWildCardSearch = "*";
    //    if (self.viewModel.projectWildCardSearch != "") {
    //        projectWildCardSearch = self.viewModel.projectWildCardSearch;
    //    }

    //    var employees = "";
    //    if (self.viewModel.employees == "All")
    //        employees = "All";
    //    else {
    //        var comma = "";
    //        for (let j = 0; j < self.filterOptions.employees.length; j++) {
    //            if (self.filterOptions.employees[j].selected) {
    //                employees += comma + self.filterOptions.employees[j].id;
    //                comma = ",";
    //            }
    //        }
    //    }

    //    var employers = "";
    //    if (self.viewModel.employers == "All")
    //        employers = "All";
    //    else {
    //        var comma = "";
    //        for (let j = 0; j < self.filterOptions.employers.length; j++) {
    //            if (self.filterOptions.employers[j].selected) {
    //                employers += comma + self.filterOptions.employers[j].id;
    //                comma = ",";
    //            }
    //        }
    //    }

    //    self.$window.open(self.ReportService.reportApi() +
    //        reportType +
    //        "?startDate=" + startDate +
    //        "&endDate=" + endDate +
    //        "&projectId=" + self.viewModel.projectId +
    //        "&userAccountId=" + employees +
    //        "&clients=" + billableClients +
    //        "&projects=" + projects +
    //        "&projectWildCardSearch=" + projectWildCardSearch +
    //        "&employers=" + employers +
    //        "&showRates=" + self.viewModel.showRates +
    //        "&showPhases=" + self.viewModel.showPhases,
    //        "_blank");
    //};

    submitForm = (reportType): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        var startDate = null;
        var endDate = null;
        try {
            startDate = self.viewModel.startDate.toJSON();
        }
        catch (e) {
            startDate = self.viewModel.startDate;
        }
        try {
            endDate = self.viewModel.endDate.toJSON();
        }
        catch (e) {
            endDate = self.viewModel.endDate;
        }
        if (!startDate && !self.viewModel.startEarliest) {
            self.handleError("Start Date should be supplied...");
            return;
        }
        if (!endDate && !self.viewModel.endLatest) {
            self.handleError("End Date should be supplied...");
            return;
        }

        var billableClients = "";
        if (self.viewModel.clients == "All")
            billableClients = "All";
        else {
            var comma = "";
            for (let j = 0; j < self.filterOptions.billableClients.length; j++) {
                if (self.filterOptions.billableClients[j].selected) {
                    billableClients += comma + self.filterOptions.billableClients[j].id;
                    comma = ",";
                }
            }
        }
        var projects = "";
        if (self.viewModel.projects == "All")
            projects = "All";
        else {
            var comma = "";
            if (self.viewModel.includeInactiveProjects) {
                for (let j = 0; j < self.filterOptions.allProjects.length; j++) {
                    if (self.filterOptions.allProjects[j].selected) {
                        projects += comma + self.filterOptions.allProjects[j].id;
                        comma = ",";
                    }
                }
            }
            else {
                for (let j = 0; j < self.filterOptions.projects.length; j++) {
                    if (self.filterOptions.projects[j].selected) {
                        projects += comma + self.filterOptions.projects[j].id;
                        comma = ",";
                    }
                }
            }
        }

        var projectWildCardSearch = "*";
        if (self.viewModel.projectWildCardSearch != "") {
            projectWildCardSearch = self.viewModel.projectWildCardSearch;
        }

        var employees = "";
        if (self.viewModel.employees == "All")
            employees = "All";
        else {
            var comma = "";
            for (let j = 0; j < self.filterOptions.employees.length; j++) {
                if (self.filterOptions.employees[j].selected) {
                    employees += comma + self.filterOptions.employees[j].id;
                    comma = ",";
                }
            }
        }

        var employers = "";
        if (self.viewModel.employers == "All")
            employers = "All";
        else {
            var comma = "";
            for (let j = 0; j < self.filterOptions.employers.length; j++) {
                if (self.filterOptions.employers[j].selected) {
                    employers += comma + self.filterOptions.employers[j].id;
                    comma = ",";
                }
            }
        }

        self.$window.open(self.ReportService.reportApi() +
            reportType +
            "?startDate=" + startDate +
            "&endDate=" + endDate +
            "&projectId=" + self.viewModel.projectId +
            "&userAccountId=" + employees +
            "&clients=" + billableClients +
            "&projects=" + projects +
            "&projectWildCardSearch=" + projectWildCardSearch +
            "&employers=" + employers +
            "&showUnassigned=" + self.viewModel.showUnassigned +
            "&showRates=" + self.viewModel.showRates +
            "&showPhases=" + self.viewModel.showPhases,
            "_blank");
    };

    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
    open(verb, url, data, target) {
        var form = document.createElement("form");
        form.action = url;
        form.method = verb;
        form.target = target || "_self";

        if (data) {
            for (var key in data) {
                var input = document.createElement("textarea");
                input.name = key;
                input.value = typeof data[key] === "object" ? JSON.stringify(data[key]) : data[key];
                form.appendChild(input);
            }
        }
        form.style.display = "none";
        document.body.appendChild(form);
        form.submit();
    }

    cancelForm = (): void => {
        this.$state.transitionTo("mainState.scorecard.grid");
    };
}

angular.module("AngularApp")
    .controller("TimeSheetReportController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "$timeout",
            "$window",
            "$filter",
            "BillingCycleService",
            "ClientService",
            "ProjectService",
            "EmployerService",
            "EnumService",
            "UserService",
            "ReportService",
            "SecurityService",
            "AccountService",
            "Popups",
            TimeSheetReportController
        ]);