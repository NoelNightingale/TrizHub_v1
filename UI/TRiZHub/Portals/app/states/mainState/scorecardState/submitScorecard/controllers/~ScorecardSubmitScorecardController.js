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
var ScorecardSubmitScorecardController = /** @class */ (function (_super) {
    __extends(ScorecardSubmitScorecardController, _super);
    //#endregion
    //#region Ctor
    function ScorecardSubmitScorecardController($stateParams, $scope, $state, $timeout, $window, ScorecardTemplateService, ScorecardService, EnumService, UserService, SecurityService, Popups) {
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
        _this.summernoteOptionsDisabled = {
            height: 110,
            focus: false,
            airMode: false,
            shortcuts: false,
            toolbar: [],
            disableDragAndDrop: true
        };
        //#endregion
        _this.showMeasureDefinition = function (definition) {
            var self = _this;
            self.Popups.scorecardDefinitionDailog(self.$scope, self.$timeout, "Definition", "Ok", definition)
                .then(function (action) { }, function (error) {
                self.handleError(error);
            });
            self.$timeout(function () { $(".modal-dialog .note-editable").attr("contenteditable", "false"); }, 200);
        };
        _this.submitForm = function (complete) {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.readOnly)
                return;
            self.viewModel.scorecardModel.completed = complete;
            self.ScorecardService.scorecardRecordSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
            }, function (error) {
                self.Popups.showError(self.$scope, error, "Error");
                self.handleError(error);
            });
        };
        _this.saveComment = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.readOnly)
                return;
            self.ScorecardService.scorecardCommmentSave(self.viewModel)
                .then(function (result) {
                // Save scorecard employee comments
                self.ScorecardService.scorecardRecordCommentSave(self.viewModel)
                    .then(function (result) {
                    self.saveSuccess = true;
                    self.$timeout(function () { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
                }, function (error) {
                    self.Popups.showError(self.$scope, error, "Error");
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.scorecardId = self.$stateParams["id"];
        self.readOnly = self.$stateParams["readOnly"] == 'true';
        self.viewModel = { isActive: true };
        self.currentUser = SecurityService.getCurrentUserDetails();
        self.filterOptions = { scorecardScoreTypes: EnumService.getScorecardScoreTypes() };
        ScorecardService.scorecardGet(self.scorecardId)
            .then(function (result) {
            self.viewModel = result;
            // Allow admin to edit any field an evaluator would
            if (_this.SecurityService.isAllowed("PerformanceManagementAdmin")) {
                self.viewModel.scorecardModel.evaluatorId = self.currentUser.id;
            }
            // Disable evaluator comment if it is the employee that is logged in
            if (self.currentUser.id == self.viewModel.scorecardModel.employeeId) {
                self.$timeout(function () { $(".evaluatorComment .note-editable").attr("contenteditable", "false"); }, 200);
            }
            // Disable employee comment if it is the evaluator that is logged in
            else {
                self.$timeout(function () { $(".employeeComment .note-editable").attr("contenteditable", "false"); }, 200);
            }
        }, function (error) {
            self.handleError(error);
        });
        if (self.readOnly == true) {
            self.$timeout(function () { $(".note-editable").attr("contenteditable", "false"); }, 200);
        }
        return _this;
    }
    return ScorecardSubmitScorecardController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardSubmitScorecardController", [
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
    ScorecardSubmitScorecardController
]);
//# sourceMappingURL=~ScorecardSubmitScorecardController.js.map