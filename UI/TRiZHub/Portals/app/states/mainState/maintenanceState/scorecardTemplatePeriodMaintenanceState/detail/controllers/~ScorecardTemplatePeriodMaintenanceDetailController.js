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
var ScorecardTemplatePeriodMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ScorecardTemplatePeriodMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ScorecardTemplatePeriodMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ScorecardTemplateService, EnumService, Popups) {
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
        //#endregion
        _this.cancelForm = function () {
            var self = _this;
            self.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.grid", { "scorecardTemplateId": self.scorecardTemplateId });
        };
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.viewModel.reviewYear = self.reviewYearDate.getFullYear();
            // Set details to today
            if (self.scorecardTemplateItemId == "new" && self.viewModel.isVariable) {
                self.viewModel.startDate = new Date();
                self.viewModel.endDate = new Date();
                //self.reviewYearDate = new Date();
            }
            self.viewModel.scorecardTemplateId = self.scorecardTemplateId;
            self.ScorecardTemplateService.scorecardTemplatePeriodSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.grid", { "scorecardTemplateId": self.scorecardTemplateId });
                }, 1000);
            }, function (error) {
                self.Popups.showError(self.$scope, error, "Error");
                self.handleError(error);
            });
        };
        _this.variableChange = function () {
            if (_this.scorecardTemplateItemId == "new" && _this.viewModel.isVariable) {
                _this.viewModel.startDate = new Date();
                _this.viewModel.endDate = new Date();
                _this.reviewYearDate = new Date();
            }
        };
        var self = _this;
        self.scorecardTemplateId = self.$stateParams["scorecardTemplateId"];
        self.scorecardTemplateItemId = self.$stateParams["id"];
        self.viewModel = {
            scorecardName: "",
            description: "",
            isVariable: false,
            startDate: new Date(),
            endDate: new Date(),
            reviewYear: new Date().getFullYear()
        };
        self.filterOptions = {};
        self.reviewYearDate = new Date();
        if (self.scorecardTemplateItemId !== "new") {
            ScorecardTemplateService.scorecardTemplatePeriodGet(self.scorecardTemplateItemId)
                .then(function (result) {
                self.viewModel = result;
                self.reviewYearDate.setFullYear(self.viewModel.reviewYear);
                ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                    .then(function (results) {
                    self.viewModel.scorecardName = results.scorecardName;
                }, function (error) {
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.scorecardTemplateItemId = null;
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(function (results) {
                self.viewModel.scorecardName = results.scorecardName;
                self.reviewYearDate.setFullYear(self.viewModel.reviewYear);
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    return ScorecardTemplatePeriodMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardTemplatePeriodMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ScorecardTemplateService",
    "EnumService",
    "Popups",
    ScorecardTemplatePeriodMaintenanceDetailController
]);
//# sourceMappingURL=~ScorecardTemplatePeriodMaintenanceDetailController.js.map