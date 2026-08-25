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
var ClientBillingRatesGridController = /** @class */ (function (_super) {
    __extends(ClientBillingRatesGridController, _super);
    //#endregion
    //#region Ctor
    function ClientBillingRatesGridController($scope, $state, $stateParams, BillingRatesService, ClientService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.BillingRatesService = BillingRatesService;
        _this.ClientService = ClientService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newRecord = function () {
            _this.$state.transitionTo("mainState.maintenance.clientMaintenance.billingRatesDetail", { clientId: _this.viewModel.id, id: "new" });
        };
        _this.reloadGrid = function () {
            _this.pageGrid.loadGrid();
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        ClientService.clientGet(_this.viewModel.id)
            .then(function (result) {
            self.client = result;
        }, function (error) {
            self.handleError(error);
        });
        _this.pageGrid = new TcrGridServiceModule.TcrGridService("startDate", _this.BillingRatesService.billingRatesGrid, _this.onDataLoaded, function (model) {
            model.clientId = self.viewModel.id;
        }, null, _this.$state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    ClientBillingRatesGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return ClientBillingRatesGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientBillingRatesGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "BillingRatesService",
    "ClientService",
    "Popups",
    ClientBillingRatesGridController
]);
//# sourceMappingURL=~ClientBillingRatesGridController.js.map