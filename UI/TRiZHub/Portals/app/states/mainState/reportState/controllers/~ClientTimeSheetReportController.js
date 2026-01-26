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
var ClientTimeSheetReportController = /** @class */ (function (_super) {
    __extends(ClientTimeSheetReportController, _super);
    //#endregion
    //#region Ctor
    function ClientTimeSheetReportController($stateParams, $scope, $state, $timeout, $window, $filter, BillingCycleService, ClientService, ProjectService, EnumService, UserService, ReportService, SecurityService, AccountService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$filter = $filter;
        _this.BillingCycleService = BillingCycleService;
        _this.ClientService = ClientService;
        _this.ProjectService = ProjectService;
        _this.EnumService = EnumService;
        _this.UserService = UserService;
        _this.ReportService = ReportService;
        _this.SecurityService = SecurityService;
        _this.AccountService = AccountService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.disableFilter = false;
        _this.updateActiveUsers = function () {
            var self = _this;
            self.filterOptions.employees = [];
            for (var i = 0; i < self.filterOptions.allemployees.length; i++) {
                if (self.filterOptions.allemployees[i].accountName == "Yes" || self.viewModel.includeInactive)
                    self.filterOptions.employees.push(self.filterOptions.allemployees[i]);
            }
        };
        _this.updateActiveClients = function () {
            var self = _this;
            self.filterOptions.allClients = [];
            for (var i = 0; i < self.filterOptions.billableClients.length; i++) {
                if (self.filterOptions.billableClients[i].isActive || self.viewModel.includeInactiveClients)
                    self.filterOptions.allClients.push(self.filterOptions.billableClients[i]);
            }
        };
        _this.updateActiveProjects = function () {
            var self = _this;
            //self.filterOptions.allProjects = [];
            //for (var i = 0; i < self.filterOptions.projects.length; i++) {
            //    if (self.filterOptions.projects[i].isActive || self.viewModel.includeInactiveProjects)
            //        self.filterOptions.allProjects.push(self.filterOptions.projects[i]);
            //}
            //console.log(self.filterOptions.allProjects.length);
        };
        _this.updateFilter = function (popupModel) {
            var self = _this;
            self.disableFilter = false;
            if (self.viewModel.billingCycleId == 0)
                return;
            for (var j = 0; j < self.filterOptions.billingCycles.length; j++) {
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
        _this.clearDate = function (type) {
            var self = _this;
            if (type == 'start') {
                self.viewModel.startDate = null;
            }
            else if (type == 'end') {
                self.viewModel.endDate = null;
            }
        };
        _this.submitForm = function (reportType) {
            var self = _this;
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
                for (var j = 0; j < self.filterOptions.billableClients.length; j++) {
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
                for (var j = 0; j < self.filterOptions.projects.length; j++) {
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
                for (var j = 0; j < self.filterOptions.employees.length; j++) {
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
                "&showPhases=" + self.viewModel.showPhases, "_blank");
        };
        _this.cancelForm = function () {
            _this.$state.transitionTo("mainState.scorecard.grid");
        };
        if (!SecurityService.userHasPrivileges) {
            AccountService.getCurrentUser()
                .then(function (result) {
                if (!SecurityService.isAllowed("CustomerReportAccess"))
                    $state.go("mainState.home");
            }, function (e) {
                this.$state.go("root.login");
            });
        }
        else {
            if (!SecurityService.isAllowed("CustomerReportAccess"))
                $state.go("mainState.home");
        }
        var self = _this;
        self.viewModel = {
            showRates: true,
            showPhases: false,
            clients: "All",
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
            .then(function (result) {
            self.filterOptions.allemployees = result;
            _this.updateActiveUsers();
        }, function (error) {
            self.handleError(error);
        });
        ClientService.clientReporterDropdownList()
            .then(function (result) {
            self.filterOptions.billableClients = result;
            self.filterOptions.allClients = result;
        }, function (error) {
            self.handleError(error);
        });
        ProjectService.projectDropdownListForClientReporter()
            .then(function (result) {
            self.filterOptions.projects = result;
        }, function (error) {
            self.handleError(error);
        });
        ProjectService.allProjectDropdownListForClientReporter()
            .then(function (result) {
            self.filterOptions.allProjects = result;
        }, function (error) {
            self.handleError(error);
        });
        BillingCycleService.billingCycleDropdownList()
            .then(function (results) {
            self.filterOptions.billingCycles = results;
            self.filterOptions.billingCycles.splice(0, 0, {
                id: 0,
                description: "Manual Date"
            });
            self.viewModel.billingCycleId = 0;
        }, function (error) {
            self.handleError(error);
        });
        return _this;
    }
    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
    ClientTimeSheetReportController.prototype.open = function (verb, url, data, target) {
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
    return ClientTimeSheetReportController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientTimeSheetReportController", [
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
//# sourceMappingURL=~ClientTimeSheetReportController.js.map