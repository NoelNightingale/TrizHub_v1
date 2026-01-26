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
var ScorecardTemplateItemMaintenanceGridController = /** @class */ (function (_super) {
    __extends(ScorecardTemplateItemMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function ScorecardTemplateItemMaintenanceGridController($scope, $state, $stateParams, ScorecardTemplateService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newScorecardTemplateItem = function () {
            var self = _this;
            self.ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(function (results) {
                if (results.totalAvailableWeight > 0) {
                    _this.$state.transitionTo("mainState.maintenance.scorecardTemplateItemMaintenance.detail", { "id": "new", "scorecardTemplateId": _this.scorecardTemplateId });
                }
                else {
                    self.Popups.showError(self.$scope, "Cannot add more items, weight would exceed 100%", "Error");
                    self.handleError("Cannot add more items, weight would exceed 100%");
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.deleteTemplateItem = function (templateItem) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this item")
                .then(function (action) {
                if (action) {
                    _this.ScorecardTemplateService.scorecardTemplateItemDelete(templateItem)
                        .then(function (result) {
                        // Remove item from list
                        for (var i = 0; i < self.gridModel.data.length; i++) {
                            if (self.gridModel.data[i].id == templateItem.id) {
                                self.gridModel.data.splice(i, 1);
                                return;
                            }
                        }
                    }, function (error) {
                        self.Popups.showError(self.$scope, error, "Error")
                            .then(function (action) {
                            console.log("Error completed");
                        }, function (error) {
                            // No need for action
                        });
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.scorecardTemplateId = self.$stateParams["scorecardTemplateId"];
        self.pageGrid = new TcrGridServiceModule.TcrGridService("order", self.ScorecardTemplateService.scorecardTemplateItemGrid, self.onDataLoaded, function (model) {
            model.id = self.scorecardTemplateId;
        }, null, $state);
        self.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    ScorecardTemplateItemMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return ScorecardTemplateItemMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardTemplateItemMaintenanceGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "ScorecardTemplateService",
    "Popups",
    ScorecardTemplateItemMaintenanceGridController
]);
//# sourceMappingURL=~ScorecardTemplateItemMaintenanceGridController.js.map