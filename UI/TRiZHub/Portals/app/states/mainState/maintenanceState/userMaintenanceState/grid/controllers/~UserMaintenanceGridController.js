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
var UserMaintenanceGridController = /** @class */ (function (_super) {
    __extends(UserMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function UserMaintenanceGridController($scope, $state, UserService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.UserService = UserService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        //#endregion
        _this.unLock = function (user) {
            var self = _this;
            _this.UserService.userUnlock(user.id)
                .then(function (result) {
                user.lockedOut = null;
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.toggleUserActivation = function (user) {
            var self = _this;
            if (user.active) {
                _this.UserService.deactivateUser(user.id)
                    .then(function (result) {
                    user.active = false;
                }, function (error) {
                    self.handleError(error);
                });
            }
            else {
                _this.UserService.activateUser(user.id)
                    .then(function (result) {
                    user.active = true;
                }, function (error) {
                    self.handleError(error);
                });
            }
        };
        _this.newUser = function () {
            _this.$state.transitionTo("mainState.maintenance.userMaintenance.detail", { "id": "new" });
        };
        _this.toggleInactiveUserShow = function () {
            var self = _this;
            _this.pageGrid.loadGrid();
        };
        _this.show = false;
        _this.pageGrid = new TcrGridServiceModule.TcrGridService("firstName", _this.UserService.userGrid, function (model) {
            _this.gridModel = model;
            if (_this.gridModel.totalItems > 0) {
                _this.loadingIsDone = true;
            }
        }, null, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    return UserMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("UserMaintenanceGridController", [
    "$scope",
    "$state",
    "UserService",
    "Popups",
    UserMaintenanceGridController
]);
//# sourceMappingURL=~UserMaintenanceGridController.js.map