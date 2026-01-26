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
var RoleMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(RoleMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function RoleMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, RoleService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
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
            _this.RoleService.roleSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () { self.$state.transitionTo("mainState.maintenance.roleMaintenance.grid"); }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.categoryId = _this.$stateParams["id"];
        _this.viewModel = {};
        if (_this.categoryId !== "new") {
            RoleService.roleGet(_this.categoryId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.categoryId = null;
            _this.RoleService.rolePrivileges("")
                .then(function (result) {
                self.viewModel.permissions = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    return RoleMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("RoleMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "RoleService",
    "Popups",
    RoleMaintenanceDetailController
]);
//# sourceMappingURL=~RoleMaintenanceDetailController.js.map