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
var UserMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(UserMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function UserMaintenanceDetailController($scope, $stateParams, $timeout, $window, $state, UserService, SecurityService, ReportService, RoleService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.UserService = UserService;
        _this.SecurityService = SecurityService;
        _this.ReportService = ReportService;
        _this.RoleService = RoleService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            if (_this.viewModel.id == null) {
                _this.UserService.signUp(_this.viewModel)
                    .then(function (result) {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                        self.$state.go("mainState.maintenance.userMaintenance.detail", { "id": result.id });
                    }, 1000);
                }, function (error) {
                    self.handleError(error);
                });
            }
            else {
                _this.UserService.userSave(_this.viewModel)
                    .then(function (result) {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                        self.$state.go("mainState.maintenance.userMaintenance.grid");
                    }, 1000);
                }, function (error) {
                    self.handleError(error);
                });
            }
        };
        _this.isAllowed = function (privilegeType) {
            return _this.SecurityService.isAllowed(privilegeType);
        };
        _this.userSummary = function () {
            var self = _this;
            self.$window.open(self.ReportService.reportApi() +
                "UserSummaryExcel?UserAccountId=" +
                _this.viewModel.id +
                "&allUsers=false", "_blank");
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        if (_this.viewModel.id !== "new") {
            _this.UserService.userGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.viewModel.id = null;
            _this.RoleService.rolePrivileges("")
                .then(function (result) {
                self.viewModel.permissions = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    return UserMaintenanceDetailController;
}(CHControllerBase));
;
angular.module("AngularApp")
    .controller("UserMaintenanceDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "UserService",
    "SecurityService",
    "ReportService",
    "RoleService",
    "Popups",
    UserMaintenanceDetailController
]);
//# sourceMappingURL=~UserMaintenanceDetailController.js.map