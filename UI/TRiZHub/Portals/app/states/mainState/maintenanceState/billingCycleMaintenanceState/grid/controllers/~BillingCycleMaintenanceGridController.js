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
var BillingCycleMaintenanceGridController = /** @class */ (function (_super) {
    __extends(BillingCycleMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function BillingCycleMaintenanceGridController($stateParams, $timeout, $window, $state, $scope, $uibModal, $log, $filter, SecurityService, BillingCycleService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.$scope = $scope;
        _this.$uibModal = $uibModal;
        _this.$log = $log;
        _this.$filter = $filter;
        _this.SecurityService = SecurityService;
        _this.BillingCycleService = BillingCycleService;
        _this.Popups = Popups;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.loadingIsDone = false;
        _this.getCurrentYear = function () {
            var date = new Date(Date.now());
            var year = date.getFullYear();
            return year;
        };
        _this.reloadGrid = function () {
            var me = _this;
            if (!me.filterModel.year) {
            }
            me.pageGrid.loadGrid();
        };
        _this.incYear = function () {
            _this.viewModel = _this.viewModel + 1;
            _this.gridModel.searchFor = String(_this.viewModel);
            _this.pageGrid.loadGrid();
        };
        _this.decYear = function () {
            _this.viewModel = _this.viewModel - 1;
            _this.gridModel.searchFor = String(_this.viewModel);
            _this.pageGrid.loadGrid();
        };
        _this.deleteRecord = function (record) {
            var me = _this;
            me.Popups.confirmationDialog(me.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (action)
                    if (!record.new) {
                        me.BillingCycleService.billingCycleDelete(record)
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
        _this.newBillingCycle = function () {
            _this.$state.transitionTo("mainState.maintenance.billingCycleMaintenance.detail", { "id": "new" });
        };
        var me = _this;
        _this.filterModel = {};
        me.viewModel = _this.getCurrentYear();
        me.pageGrid = new TcrGridServiceModule.TcrGridService("cycle", _this.BillingCycleService.billingCycleGrid, _this.onDataLoaded, function (model) {
            model.searchFor = _this.viewModel;
        }, null, $state);
        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;
        me.pageGrid.loadGrid();
        return _this;
    }
    BillingCycleMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return BillingCycleMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("BillingCycleMaintenanceGridController", [
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "$scope",
    "$uibModal",
    "$log",
    "$filter",
    "SecurityService",
    "BillingCycleService",
    "Popups",
    BillingCycleMaintenanceGridController
]);
//# sourceMappingURL=~BillingCycleMaintenanceGridController.js.map