
class ClientTimeSheetReportController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
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
        private EnumService: EnumServiceModule.EnumService,
        private UserService: UserServiceModule.UserService,
        private ReportService: ReportServiceModule.ReportService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private AccountService: AccountServiceModule.AccountService,
        private Popups: any) {
        super($scope, Popups, $state);

        if (!SecurityService.userHasPrivileges) {
            AccountService.getCurrentUser()
                .then(
                function (result) {
                    if (!SecurityService.isAllowed("CustomerReportAccess"))
                        $state.go("mainState.home");
                },
                function (e) {
                    this.$state.go("root.login");
                });
        }
        else {
            if (!SecurityService.isAllowed("CustomerReportAccess"))
                $state.go("mainState.home");
        }

        const self = this;

        self.viewModel = {
            showRates: true,
            showPhases: false,
            clients : "All",
            projects: "All",
            projectWildCardSearch: "",
            employees: "All",
            startEarliest: false,
            endLatest: false,
            includeInactive: true,
            includeInactiveClients: true,
            includeInactiveProjects: false,
            reportType: "BillableReportExcel"
        };

        self.filterOptions = {
            evaluators: [],
            employees: [],
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

        ClientService.clientReporterDropdownList()
            .then(
            result => {
                self.filterOptions.billableClients = result;
                self.filterOptions.allClients = result;
            },
            error => {
                self.handleError(error);
            });

        ProjectService.projectDropdownListForClientReporter()
            .then(
            result => {
                self.filterOptions.projects = result;
            },
            error => {
                self.handleError(error);
            });

        ProjectService.allProjectDropdownListForClientReporter()
            .then(
            result => {
                self.filterOptions.allProjects = result;
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
        //self.filterOptions.allProjects = [];
        //for (var i = 0; i < self.filterOptions.projects.length; i++) {
        //    if (self.filterOptions.projects[i].isActive || self.viewModel.includeInactiveProjects)
        //        self.filterOptions.allProjects.push(self.filterOptions.projects[i]);
        //}
        //console.log(self.filterOptions.allProjects.length);
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

    clearDate = (type): void => {
        const self = this;
        if (type == 'start') {
            self.viewModel.startDate = null;
        }
        else if (type == 'end') {
            self.viewModel.endDate = null;
        }
    };

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
            for (let j = 0; j < self.filterOptions.projects.length; j++) {
                if (self.filterOptions.projects[j].selected) {
                    projects += comma + self.filterOptions.projects[j].id;
                    comma = ",";
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

        self.$window.open(self.ReportService.reportApi() +
            reportType + "Client" +
            "?startDate=" + startDate +
            "&endDate=" + endDate +
            "&userAccountId=" + employees +
            "&clients=" + billableClients +
            "&projects=" + projects +
            "&projectWildCardSearch=" + projectWildCardSearch +
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
    .controller("ClientTimeSheetReportController",
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
        "EnumService",
        "UserService",
        "ReportService",
        "SecurityService",
        "AccountService",
        "Popups",
        ClientTimeSheetReportController
    ]);


