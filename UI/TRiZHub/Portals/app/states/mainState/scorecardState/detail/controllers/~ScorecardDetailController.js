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
var ScorecardDetailController = /** @class */ (function (_super) {
    __extends(ScorecardDetailController, _super);
    //#endregion
    //#region Ctor
    function ScorecardDetailController($stateParams, $scope, $state, $timeout, $window, ScorecardTemplateService, ScorecardService, EnumService, UserService, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.ScorecardService = ScorecardService;
        _this.EnumService = EnumService;
        _this.UserService = UserService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.readOnly = false;
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            if (self.readOnly)
                return;
            self.viewModel.variableYear = self.viewModel.variableStart.getFullYear();
            self.ScorecardService.scorecardSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.changedScorecardTemplate = function () {
            var self = _this;
            self.viewModel.createdBy = self.SecurityService.getCurrentUserDetails().id;
            self.viewModel.DateCreated = new Date();
            self.ScorecardTemplateService.scorecardTemplatePeriodDropdownList(self.viewModel.scorecardTemplateId)
                .then(function (result) {
                self.filterOptions.scorecardTemplatePeriods = result;
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.changedScorecardTemplatePeriod = function () {
            for (var i = 0; i < _this.filterOptions.scorecardTemplatePeriods.length; i++) {
                if (_this.viewModel.scorecardTemplatePeriodId == _this.filterOptions.scorecardTemplatePeriods[i].id) {
                    _this.filterOptions.variablePeriod = _this.filterOptions.scorecardTemplatePeriods[i].isVariable;
                    return;
                }
            }
        };
        var self = _this;
        self.scorecardPeriodId = self.$stateParams["id"];
        self.SecurityService = SecurityService;
        self.viewModel = {
            isActive: true,
            variableStart: new Date(),
            variableEnd: new Date(),
            variableYear: new Date().getFullYear()
        };
        // Set variable ende date to 4 weeks from now
        var fourWeeks = 28;
        var variableEnd = self.viewModel.variableEnd;
        variableEnd = variableEnd.setDate((variableEnd.getDate() + fourWeeks));
        self.viewModel.variableEnd = new Date(variableEnd);
        self.filterOptions = {
            evaluators: [],
            employees: [],
            scorecardTemplatePeriods: [],
            variablePeriod: false
        };
        UserService.userScorecardEvaluatorFilterDropdown()
            .then(function (result) {
            self.filterOptions.evaluators = result;
        }, function (error) {
            self.handleError(error);
        });
        UserService.userScorecardEmployeeFilterDropdown()
            .then(function (result) {
            self.filterOptions.employees = result;
        }, function (error) {
            self.handleError(error);
        });
        ScorecardTemplateService.scorecardTemplateDropdownList()
            .then(function (result) {
            self.filterOptions.scorecardTemplates = result;
        }, function (error) {
            self.handleError(error);
        });
        if (self.scorecardPeriodId !== "new") {
            ScorecardService.scorecardGet(self.scorecardPeriodId)
                .then(function (result) {
                self.viewModel = result;
                self.changedScorecardTemplate();
                if (self.viewModel.employeeId === self.SecurityService.getCurrentUserDetails().id) {
                    self.readOnly = true;
                }
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.scorecardPeriodId = null;
        }
        return _this;
    }
    return ScorecardDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ScorecardTemplateService",
    "ScorecardService",
    "EnumService",
    "UserService",
    "SecurityService",
    "Popups",
    ScorecardDetailController
]);
//# sourceMappingURL=~ScorecardDetailController.js.map