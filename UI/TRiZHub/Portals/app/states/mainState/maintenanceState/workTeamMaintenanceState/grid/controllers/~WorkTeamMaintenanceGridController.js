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
var WorkTeamMaintenanceGridController = /** @class */ (function (_super) {
    __extends(WorkTeamMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function WorkTeamMaintenanceGridController($scope, $state, $stateParams, WorkTeamService, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.WorkTeamService = WorkTeamService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        _this.loadingIsDone = false;
        _this.canCreate = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.toggleInactive = function () {
            _this.pageGrid.loadGrid();
        };
        _this.newRecord = function () {
            _this.$state.transitionTo("mainState.maintenance.workTeamMaintenance.detail", { "id": "new" });
        };
        _this.canCreate = SecurityService.isAllowed("TeamMaintenance");
        _this.pageGrid = new TcrGridServiceModule
            .TcrGridService("name", _this.WorkTeamService.workTeamGrid, _this.onDataLoaded, null, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    WorkTeamMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return WorkTeamMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("WorkTeamMaintenanceGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "WorkTeamService",
    "SecurityService",
    "Popups",
    WorkTeamMaintenanceGridController
]);
//# sourceMappingURL=~WorkTeamMaintenanceGridController.js.map