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
var TeamMaintenanceGridController = /** @class */ (function (_super) {
    __extends(TeamMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function TeamMaintenanceGridController($scope, $state, $stateParams, TeamService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.TeamService = TeamService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newRecord = function () {
            _this.$state.transitionTo("mainState.maintenance.teamMaintenance.detail", { "id": "new" });
        };
        var self = _this;
        _this.pageGrid = new TcrGridServiceModule
            .TcrGridService("teamname", _this.TeamService.teamGrid, _this.onDataLoaded, null, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    TeamMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return TeamMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("TeamMaintenanceGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "TeamService",
    "Popups",
    TeamMaintenanceGridController
]);
//# sourceMappingURL=~TeamMaintenanceGridController.js.map