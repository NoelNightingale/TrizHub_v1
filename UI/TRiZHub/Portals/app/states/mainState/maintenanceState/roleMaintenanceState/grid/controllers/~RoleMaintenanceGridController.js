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
var RoleMaintenanceGridController = /** @class */ (function (_super) {
    __extends(RoleMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function RoleMaintenanceGridController($scope, $state, RoleService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.RoleService = RoleService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newRole = function () {
            _this.$state.transitionTo("mainState.maintenance.roleMaintenance.detail", { "id": "new" });
        };
        var self = _this;
        _this.pageGrid = new TcrGridServiceModule
            .TcrGridService("rolename", _this.RoleService.roleGrid, _this.onDataLoaded, null, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    RoleMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return RoleMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("RoleMaintenanceGridController", [
    "$scope",
    "$state",
    "RoleService",
    "Popups",
    RoleMaintenanceGridController
]);
//# sourceMappingURL=~RoleMaintenanceGridController.js.map