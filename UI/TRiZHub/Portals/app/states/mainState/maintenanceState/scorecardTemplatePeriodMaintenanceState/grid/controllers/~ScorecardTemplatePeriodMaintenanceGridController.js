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
var ScorecardTemplatePeriodMaintenanceGridController = /** @class */ (function (_super) {
    __extends(ScorecardTemplatePeriodMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function ScorecardTemplatePeriodMaintenanceGridController($scope, $state, $stateParams, ScorecardTemplateService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.newScorecardTemplatePeriod = function () {
            var self = _this;
            _this.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.detail", { "id": "new", "scorecardTemplateId": self.scorecardTemplateId });
        };
        _this.deleteTemplatePeriod = function (templatePeriod) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this period")
                .then(function (action) {
                if (action) {
                    _this.ScorecardTemplateService.scorecardTemplatePeriodDelete(templatePeriod)
                        .then(function (result) {
                        // Remove item from list
                        for (var i = 0; i < self.gridModel.data.length; i++) {
                            if (self.gridModel.data[i].id == templatePeriod.id) {
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
        self.pageGrid = new TcrGridServiceModule.TcrGridService("description", self.ScorecardTemplateService.scorecardTemplatePeriodGrid, self.onDataLoaded, function (model) {
            model.id = self.scorecardTemplateId;
        }, null, $state);
        self.pageGrid.loadGrid();
        return _this;
    }
    //#endregion
    ScorecardTemplatePeriodMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return ScorecardTemplatePeriodMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardTemplatePeriodMaintenanceGridController", [
    "$scope",
    "$state",
    "$stateParams",
    "ScorecardTemplateService",
    "Popups",
    ScorecardTemplatePeriodMaintenanceGridController
]);
//# sourceMappingURL=~ScorecardTemplatePeriodMaintenanceGridController.js.map