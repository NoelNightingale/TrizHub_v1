class UserProjectSummaryController extends CHControllerBase {
    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel = {
        employees: "All",
        showInactiveUsers: false,
        onlyActiveUsers: false,
        onlyActiveClients: false,
        onlyActiveProjects: false,
        onlyActiveSubProjects: false,
    };
    
    filterOptions = {
        allEmployees:[], 
        allClients:[], 
        allProjects:[], 
        employees: [],
        clients: [],
        projects: [],
    };

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $filter: ng.IFilterService,
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
                        if (!SecurityService.isAllowed("ReportGenerationUserProjects"))
                            $state.go("mainState.home");
                    },
                    function (e) {
                        this.$state.go("root.login");
                    });
        }
        else {
            if (!SecurityService.isAllowed("ReportGenerationUserProjects"))
                $state.go("mainState.home");
        }

        UserService.allUserDropdownList()
            .then(
                result => {
                    self.filterOptions.allEmployees = result;
                    self.updateActiveUsers();
                },
                error => {
                    self.handleError(error);
                });
    }

    updateActiveUsers = (): void => {
        const self = this;
        self.filterOptions.employees = [];
        for (var i = 0; i < self.filterOptions.allEmployees.length; i++) {
            if (self.filterOptions.allEmployees[i].accountName == "Yes" || self.viewModel.showInactiveUsers)
                self.filterOptions.employees.push(self.filterOptions.allEmployees[i]);
        }
    }

    submitForm = (): any => {

        const self = this;

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

        self.$window.open(self.ReportService.reportApi() + '/ProjectAllocation' +
            "?userAccounts=" + employees
            + "&onlyActiveUsers=" + self.viewModel.onlyActiveUsers
            + "&onlyActiveClients=" + self.viewModel.onlyActiveClients
            + "&onlyActiveProjects=" + self.viewModel.onlyActiveProjects
            + "&onlyActiveSubProjects=" + self.viewModel.onlyActiveSubProjects,
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

    //cancelForm = (): void => {
    //    this.$state.transitionTo("mainState.scorecard.grid");
    //};
}

angular.module("AngularApp")
    .controller("UserProjectSummaryController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "$timeout",
            "$window",
            "$filter",
            "UserService",
            "ReportService",
            "SecurityService",
            "AccountService",
            "Popups",
            UserProjectSummaryController
        ]);