var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var UserProjectSummaryController = (function (_super) {
    __extends(UserProjectSummaryController, _super);
    //#endregion
    //#region Ctor
    function UserProjectSummaryController($stateParams, $scope, $state, $timeout, $window, $filter, UserService, ReportService, SecurityService, AccountService, Popups) {
        var _this = this;
        _super.call(this, $scope, Popups, $state);
        this.$stateParams = $stateParams;
        this.$scope = $scope;
        this.$state = $state;
        this.$timeout = $timeout;
        this.$window = $window;
        this.$filter = $filter;
        this.UserService = UserService;
        this.ReportService = ReportService;
        this.SecurityService = SecurityService;
        this.AccountService = AccountService;
        this.Popups = Popups;
        //#region members
        this.successMessage = "Saved Successfully";
        this.saveSuccess = false;
        this.viewModel = {
            includeInactiveUsers: false
        };
        this.disableFilter = false;
        this.updateActiveUsers = function () {
            var self = _this;
            self.filterOptions.employees = [];
            for (var i = 0; i < self.filterOptions.allemployees.length; i++) {
                if (self.filterOptions.allemployees[i].accountName == "Yes" || self.viewModel.includeInactiveUsers)
                    self.filterOptions.employees.push(self.filterOptions.allemployees[i]);
            }
        };
        this.submitForm = function (reportType) {
            //const self = this;
            //self.$scope.$broadcast("show-errors-check-validity");
            //if (self.$scope["EditForm"].$invalid)
            //    return;
            //var startDate = null;
            //var endDate = null;
            //try {
            //    startDate = self.viewModel.startDate.toJSON();
            //}
            //catch (e) {
            //    startDate = self.viewModel.startDate;
            //}
            //try {
            //    endDate = self.viewModel.endDate.toJSON();
            //}
            //catch (e) {
            //    endDate = self.viewModel.endDate;
            //}
            //if (!startDate && !self.viewModel.startEarliest) {
            //    self.handleError("Start Date should be supplied...");
            //    return;
            //}
            //if (!endDate && !self.viewModel.endLatest) {
            //    self.handleError("End Date should be supplied...");
            //    return;
            //}
            //var billableClients = "";
            //if (self.viewModel.clients == "All")
            //    billableClients = "All";
            //else {
            //    var comma = "";
            //    for (let j = 0; j < self.filterOptions.billableClients.length; j++) {
            //        if (self.filterOptions.billableClients[j].selected) {
            //            billableClients += comma + self.filterOptions.billableClients[j].id;
            //            comma = ",";
            //        }
            //    }
            //}
            //var projects = "";
            //if (self.viewModel.projects == "All")
            //    projects = "All";
            //else {
            //    var comma = "";
            //    if (self.viewModel.includeInactiveProjects) {
            //        for (let j = 0; j < self.filterOptions.allProjects.length; j++) {
            //            if (self.filterOptions.allProjects[j].selected) {
            //                projects += comma + self.filterOptions.allProjects[j].id;
            //                comma = ",";
            //            }
            //        }
            //    }
            //    else {
            //        for (let j = 0; j < self.filterOptions.projects.length; j++) {
            //            if (self.filterOptions.projects[j].selected) {
            //                projects += comma + self.filterOptions.projects[j].id;
            //                comma = ",";
            //            }
            //        }
            //    }
            //}
            //var projectWildCardSearch = "*";
            //if (self.viewModel.projectWildCardSearch != "") {
            //    projectWildCardSearch = self.viewModel.projectWildCardSearch;
            //}
            //var employees = "";
            //if (self.viewModel.employees == "All")
            //    employees = "All";
            //else {
            //    var comma = "";
            //    for (let j = 0; j < self.filterOptions.employees.length; j++) {
            //        if (self.filterOptions.employees[j].selected) {
            //            employees += comma + self.filterOptions.employees[j].id;
            //            comma = ",";
            //        }
            //    }
            //}
            //var employers = "";
            //if (self.viewModel.employers == "All")
            //    employers = "All";
            //else {
            //    var comma = "";
            //    for (let j = 0; j < self.filterOptions.employers.length; j++) {
            //        if (self.filterOptions.employers[j].selected) {
            //            employers += comma + self.filterOptions.employers[j].id;
            //            comma = ",";
            //        }
            //    }
            //}
            //self.$window.open(self.ReportService.reportApi() +
            //    reportType +
            //    "?startDate=" + startDate +
            //    "&endDate=" + endDate +
            //    "&projectId=" + self.viewModel.projectId +
            //    "&userAccountId=" + employees +
            //    "&clients=" + billableClients +
            //    "&projects=" + projects +
            //    "&projectWildCardSearch=" + projectWildCardSearch +
            //    "&employers=" + employers +
            //    "&showUnassigned=" + self.viewModel.showUnassigned +
            //    "&showRates=" + self.viewModel.showRates +
            //    "&showPhases=" + self.viewModel.showPhases,
            //    "_blank");
        };
        this.cancelForm = function () {
            _this.$state.transitionTo("mainState.scorecard.grid");
        };
        var self = this;
        if (!SecurityService.userHasPrivileges) {
            AccountService.getCurrentUser()
                .then(function (result) {
                if (!SecurityService.isAllowed("ReportGenerationUserProjects"))
                    $state.go("mainState.home");
            }, function (e) {
                this.$state.go("root.login");
            });
        }
        else {
            if (!SecurityService.isAllowed("ReportGenerationUserProjects"))
                $state.go("mainState.home");
        }
        var currentUser = this.SecurityService.getCurrentUserDetails();
        self.filterOptions = {
            evaluators: [],
            employees: [],
            allEmployers: [],
            employers: [],
        };
        UserService.allUserDropdownList()
            .then(function (result) {
            self.filterOptions.allemployees = result;
            _this.updateActiveUsers();
        }, function (error) {
            self.handleError(error);
        });
    }
    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
    UserProjectSummaryController.prototype.open = function (verb, url, data, target) {
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
    };
    return UserProjectSummaryController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("UserProjectSummaryController", [
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
//# sourceMappingURL=~user-project-summary-controller.js.map