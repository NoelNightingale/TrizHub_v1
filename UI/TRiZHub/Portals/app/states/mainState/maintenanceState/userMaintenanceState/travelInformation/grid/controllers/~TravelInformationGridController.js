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
var TravelInformationGridController = /** @class */ (function (_super) {
    __extends(TravelInformationGridController, _super);
    //#endregion
    //#region Ctor
    function TravelInformationGridController($scope, $state, $stateParams, TravelInformationService, UserService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.TravelInformationService = TravelInformationService;
        _this.UserService = UserService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newTravelInfo = function () {
            _this.$state.transitionTo("mainState.maintenance.userMaintenance.travelInformationDetail", { userid: _this.viewModel.id, "id": "new" });
        };
        _this.deleteRecord = function (record) {
            var me = _this;
            me.Popups.confirmationDialog(me.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (action)
                    if (!record.new) {
                        me.TravelInformationService.travelInformationDelete(record)
                            .then(function (result) {
                            me.saveSuccess = true;
                            me.reloadGrid();
                        }, function (error) {
                            me.handleError(error);
                        });
                    }
                    else {
                        //me.gridModel
                        var index = me.gridModel.data.indexOf(record);
                        me.gridModel.data.splice(index, 1);
                    }
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.reloadGrid = function () {
            var me = _this;
            me.pageGrid.loadGrid();
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        UserService.userGet(_this.viewModel.id)
            .then(function (result) {
            self.user = result;
        }, function (error) {
            self.handleError(error);
        });
        _this.pageGrid = new TcrGridServiceModule.TcrGridService("expiryDate", _this.TravelInformationService.travelInformationGrid, _this.onDataLoaded, function (model) {
            model.Id = self.viewModel.id;
        }, null, $state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    TravelInformationGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return TravelInformationGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("TravelInformationGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "TravelInformationService",
    "UserService",
    "Popups",
    TravelInformationGridController
]);
//# sourceMappingURL=~TravelInformationGridController.js.map