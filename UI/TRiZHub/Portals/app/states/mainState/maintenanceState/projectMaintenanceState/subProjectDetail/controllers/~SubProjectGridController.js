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
var SubProjectGridController = /** @class */ (function (_super) {
    __extends(SubProjectGridController, _super);
    //#endregion
    //#region Ctor
    function SubProjectGridController($scope, $state, $stateParams, ProjectService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.ProjectService = ProjectService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newRecord = function () {
            _this.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectDetail", { "id": _this.parentProjectId, "subProjectId": "new" });
        };
        _this.deleteRecord = function (record) {
            /*        const me = this;
                    me.Popups.confirmationDialog(me.$scope,
                        "Are you sure you want to delete?",
                        "You are about to delete this record...")
                        .then(
                        action => {
                            if (action)
                                if (!record.new) {
                                    me.BillingRatesService.billingRatesDelete(record)
                                        .then(
                                        result => {
                                            me.saveSuccess = true;
                                            me.reloadGrid();
                                        },
                                        error => {
                                            me.handleError(error);
                                        });
                                } else {
                                    //me.gridModel
                                    const index = me.gridModel.data.indexOf(record);
                                    me.gridModel.data.splice(index, 1);
                                }
            
                        },
                        error => {
                            me.handleError(error);
                        }); */
        };
        _this.reloadGrid = function () {
            var me = _this;
            me.pageGrid.loadGrid();
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        _this.parentProjectId = _this.$stateParams["id"];
        ProjectService.projectGet(self.parentProjectId)
            .then(function (result) {
            //                alert(result.projectName);
            _this.parentProjectName = result.projectName;
            _this.parentProjectNumber = result.projectNumber;
        }, function (error) {
            _this.handleError(error);
        });
        _this.pageGrid = new TcrGridServiceModule.TcrGridService("subProjectNumber", _this.ProjectService.subProjectGrid, _this.onDataLoaded, function (model) {
            model.parentId = _this.viewModel.id;
        }, null, _this.$state);
        _this.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    SubProjectGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return SubProjectGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("SubProjectGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "ProjectService",
    "Popups",
    SubProjectGridController
]);
//# sourceMappingURL=~SubProjectGridController.js.map