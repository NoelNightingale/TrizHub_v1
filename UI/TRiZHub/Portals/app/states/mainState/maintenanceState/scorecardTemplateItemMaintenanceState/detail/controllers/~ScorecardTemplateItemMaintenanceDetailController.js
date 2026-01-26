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
var ScorecardTemplateItemMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ScorecardTemplateItemMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ScorecardTemplateItemMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ScorecardTemplateService, EnumService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.EnumService = EnumService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.summernoteOptions = {
            height: 110,
            focus: false,
            airMode: false,
            shortcuts: true,
            toolbar: [
                ['style', ['bold', 'italic', 'underline']],
                ['textsize', ['fontsize']],
                ['fontclr', ['color']],
                ['alignment', ['ul', 'ol', 'paragraph', 'lineheight']],
            ],
            disableDragAndDrop: true
        };
        _this.percentageFormatting = function (value) {
            return value.toString() + "%";
        };
        _this.checkSliderWeightValue = function (existing) {
            var self = _this;
            if (existing)
                self.filterOptions.maxWeight += self.viewModel.weight;
        };
        //#endregion
        _this.cancelForm = function () {
            var self = _this;
            self.$state.transitionTo("mainState.maintenance.scorecardTemplateItemMaintenance.grid", { "scorecardTemplateId": self.scorecardTemplateId });
        };
        _this.submitForm = function () {
            var self = _this;
            if (self.viewModel.weight > 0) {
                self.$scope.$broadcast("show-errors-check-validity");
                if (self.$scope["EditForm"].$invalid)
                    return;
                self.viewModel.scorecardTemplateId = self.scorecardTemplateId;
                self.ScorecardTemplateService.scorecardTemplateItemSave(self.viewModel)
                    .then(function (result) {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                        self.$state.transitionTo("mainState.maintenance.scorecardTemplateItemMaintenance.grid", { "scorecardTemplateId": self.scorecardTemplateId });
                    }, 1000);
                }, function (error) {
                    self.handleError(error);
                });
            }
            else {
                self.Popups.showError(self.$scope, "Cannot add item with 0% weight", "Error");
                self.handleError("Cannot add item with 0% weight");
            }
        };
        var self = _this;
        self.scorecardTemplateId = self.$stateParams["scorecardTemplateId"];
        self.scorecardTemplateItemId = self.$stateParams["id"];
        self.viewModel = {};
        self.filterOptions = { maxWeight: 0 };
        if (self.scorecardTemplateItemId !== "new") {
            ScorecardTemplateService.scorecardTemplateItemGet(self.scorecardTemplateItemId)
                .then(function (result) {
                self.viewModel = result;
                ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                    .then(function (results) {
                    self.filterOptions.maxWeight = results.totalAvailableWeight;
                    self.viewModel.scorecardName = results.scorecardName;
                    self.checkSliderWeightValue(true);
                }, function (error) {
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.scorecardTemplateItemId = null;
            self.viewModel.scorecardScoring = 0;
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(function (results) {
                self.filterOptions.maxWeight = results.totalAvailableWeight;
                self.viewModel.scorecardName = results.scorecardName;
                self.checkSliderWeightValue(false);
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    return ScorecardTemplateItemMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardTemplateItemMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ScorecardTemplateService",
    "EnumService",
    "Popups",
    ScorecardTemplateItemMaintenanceDetailController
]);
//# sourceMappingURL=~ScorecardTemplateItemMaintenanceDetailController.js.map