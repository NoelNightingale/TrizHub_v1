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
var SubProjectMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(SubProjectMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function SubProjectMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ProjectService, EnumService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ProjectService = ProjectService;
        _this.EnumService = EnumService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.cancelForm = function () {
            var self = _this;
            self.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectGrid", { "id": _this.parentProjectId });
        };
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.viewModel.projectId = self.parentProjectId;
            self.ProjectService.subProjectSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () { self.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectGrid", { "id": _this.parentProjectId }); }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.parentProjectId = self.$stateParams["id"];
        self.projectId = self.$stateParams["subProjectId"];
        self.viewModel = {};
        ProjectService.projectTypeDropdownList()
            .then(function (result) {
            self.projectTypeDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        if (self.projectId !== "new") {
            ProjectService.subProjectGet(self.projectId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
            ProjectService.projectGet(self.parentProjectId)
                .then(function (result) {
                self.viewModel.parentProjectName = result.projectName;
                self.viewModel.parentProjectNumber = result.projectNumber;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.projectId = null;
            self.viewModel.entityType = 0;
            self.viewModel.isActive = true;
            ProjectService.projectGet(self.parentProjectId)
                .then(function (result) {
                self.viewModel.parentProjectName = result.projectName;
                self.viewModel.parentProjectNumber = result.projectNumber;
                self.viewModel.subProjectTypeId = result.projectTypeId;
                self.viewModel.parentAllowSubProjectAlternativeType = result.allowSubProjectAlternativeType;
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    return SubProjectMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("SubProjectMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ProjectService",
    "EnumService",
    "Popups",
    SubProjectMaintenanceDetailController
]);
//# sourceMappingURL=~SubProjectMaintenanceDetailController.js.map