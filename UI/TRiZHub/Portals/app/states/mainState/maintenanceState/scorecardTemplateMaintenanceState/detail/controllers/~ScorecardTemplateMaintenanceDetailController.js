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
var ScorecardTemplateMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ScorecardTemplateMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ScorecardTemplateMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ScorecardTemplateService, EnumService, Popups) {
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
        _this.percentageFormatting = function (value) {
            return value.toString() + "%";
        };
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            console.log(self.viewModel.excellentWeight);
            console.log(self.viewModel.adequateWeight);
            console.log(self.viewModel.inadequateWeight);
            // Validate
            if ((self.viewModel.excellentWeight < 1) && (self.viewModel.adequateWeight < 1) && (self.viewModel.inadequateWeight < 1)) {
                self.Popups.showError(self.$scope, "Please specify at least one E,A,I weight.", "Error");
                return;
            }
            self.ScorecardTemplateService.scorecardTemplateSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.scorecardTemplateMaintenance.grid");
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.scorecardTemplateId = self.$stateParams["id"];
        self.filterOptions = { maxWeight: 100 };
        if (self.scorecardTemplateId !== "new") {
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.scorecardTemplateId = null;
        }
        return _this;
    }
    return ScorecardTemplateMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardTemplateMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ScorecardTemplateService",
    "EnumService",
    "Popups",
    ScorecardTemplateMaintenanceDetailController
]);
//# sourceMappingURL=~ScorecardTemplateMaintenanceDetailController.js.map