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
var UserRoleSummaryController = /** @class */ (function (_super) {
    __extends(UserRoleSummaryController, _super);
    //#endregion
    //#region Ctor
    function UserRoleSummaryController($stateParams, $scope, $state, $timeout, $window, $filter, UserService, ReportService, SecurityService, AccountService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$filter = $filter;
        _this.UserService = UserService;
        _this.ReportService = ReportService;
        _this.SecurityService = SecurityService;
        _this.AccountService = AccountService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.viewModel = {
            employees: "All",
            includeInactiveRoles: false,
            includeInactiveUsers: false,
            showInactiveUsers: false,
        };
        _this.filterOptions = {
            allEmployees: [],
            employees: [],
        };
        _this.updateActiveUsers = function () {
            var self = _this;
            self.filterOptions.employees = [];
            for (var i = 0; i < self.filterOptions.allEmployees.length; i++) {
                if (self.filterOptions.allEmployees[i].accountName == "Yes" || self.viewModel.showInactiveUsers)
                    self.filterOptions.employees.push(self.filterOptions.allEmployees[i]);
            }
        };
        _this.submitForm = function () {
            var self = _this;
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
            self.$window.open(self.ReportService.reportApi() + '/RoleAllocation' +
                "?userAccounts=" + employees
                + "&includeInactiveRoles=" + self.viewModel.includeInactiveRoles
                + "&includeInactiveUsers=" + self.viewModel.includeInactiveUsers, "_blank");
        };
        var self = _this;
        if (!SecurityService.userHasPrivileges) {
            AccountService.getCurrentUser()
                .then(function (result) {
                if (!SecurityService.isAllowed("ReportGenerationUserRoles"))
                    $state.go("mainState.home");
            }, function (e) {
                this.$state.go("root.login");
            });
        }
        else {
            if (!SecurityService.isAllowed("ReportGenerationUserRoles"))
                $state.go("mainState.home");
        }
        UserService.allUserDropdownList()
            .then(function (result) {
            self.filterOptions.allEmployees = result;
            self.updateActiveUsers();
        }, function (error) {
            self.handleError(error);
        });
        return _this;
    }
    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
    UserRoleSummaryController.prototype.open = function (verb, url, data, target) {
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
    return UserRoleSummaryController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("UserRoleSummaryController", [
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
    UserRoleSummaryController
]);
//# sourceMappingURL=~UserRoleSummaryController.js.map