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
var ClientMaintenanceGridController = /** @class */ (function (_super) {
    __extends(ClientMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function ClientMaintenanceGridController($scope, $state, ClientService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.ClientService = ClientService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newClient = function () {
            _this.$state.transitionTo("mainState.maintenance.clientMaintenance.detail", { "id": "new" });
        };
        _this.deleteClient = function (client, index) {
            _this.Popups.confirmationDialog(_this.$scope, "Are you sure you want to delete?", "You are about to delete this client")
                .then(function (action) {
                if (action) {
                    _this.ClientService.deleteClient(client.id)
                        .then(function (result) {
                        if (result == 0) {
                            _this.Popups.showError(_this.$scope, "The Client could not be deleted as it has projects assigned to it.", null, null);
                        }
                        else {
                            _this.gridModel.data.splice(index, 1);
                        }
                    }, function (error) {
                        _this.handleError(error);
                    });
                }
            }, function (error) {
                _this.handleError(error);
            });
        };
        var self = _this;
        _this.pageGrid = new TcrGridServiceModule
            .TcrGridService("entityName", _this.ClientService.clientGrid, _this.onDataLoaded, null, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    ClientMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return ClientMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientMaintenanceGridController", [
    "$scope",
    "$state",
    "ClientService",
    "Popups",
    ClientMaintenanceGridController
]);
//# sourceMappingURL=~ClientMaintenanceGridController.js.map