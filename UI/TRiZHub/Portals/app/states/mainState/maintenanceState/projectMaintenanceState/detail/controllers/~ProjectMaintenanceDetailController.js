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
var ProjectMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ProjectMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ProjectMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ClientService, UserService, ProjectService, EnumService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ClientService = ClientService;
        _this.UserService = UserService;
        _this.ProjectService = ProjectService;
        _this.EnumService = EnumService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.changedProjectType = false;
        //#endregion
        _this.projectTypeChange = function () {
            _this.changedProjectType = true;
        };
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            // Check if not invoiceable and billable
            var billableText = _this.viewModel.billable ? 'billable' : 'non-billalbe';
            for (var i = 0; i < _this.projectTypeDropdown.length; i++) {
                if (_this.projectTypeDropdown[i].id == _this.viewModel.projectTypeId && _this.projectTypeDropdown[i].allowSubProjectBillable != _this.viewModel.billable) {
                    _this.Popups.showError(_this.$scope, "Projects that are of type '" + _this.projectTypeDropdown[i].name + "', cannot be " + billableText + ".");
                    return;
                }
            }
            if (_this.changedProjectType && _this.viewModel.hasSubprojects) {
                _this.Popups.confirmationDialog(_this.$scope, "Are you sure you want to change the Project type?", "This Project has sub-projects, continuing will change their type as well and cannot be undone. Review and make a note of existing sub project types before changing the parent.").then(function (result) {
                    if (result) {
                        self.ProjectService.projectSave(self.viewModel)
                            .then(function (result) {
                            self.saveSuccess = true;
                            self.$state.transitionTo("mainState.maintenance.projectMaintenance.grid");
                        }, function (error) {
                            self.handleError(error);
                        });
                    }
                }, function (error) {
                });
            }
            else {
                self.ProjectService.projectSave(self.viewModel)
                    .then(function (result) {
                    self.saveSuccess = true;
                    self.$state.transitionTo("mainState.maintenance.projectMaintenance.grid");
                }, function (error) {
                    self.handleError(error);
                });
            }
        };
        var self = _this;
        self.projectId = self.$stateParams["id"];
        self.viewModel = {};
        ClientService.clientDropdownList()
            .then(function (result) {
            self.clientDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        UserService.userDropdownList()
            .then(function (result) {
            self.userDropdown = result;
            self.userDropdown.splice(0, 0, { id: null, description: "N/A" });
        }, function (error) {
            self.handleError(error);
        });
        ProjectService.projectTypeDropdownList()
            .then(function (result) {
            self.projectTypeDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        if (self.projectId !== "new") {
            ProjectService.projectGet(self.projectId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.projectId = null;
            self.viewModel.entityType = 0;
            self.viewModel.billable = true;
            self.viewModel.isActive = true;
        }
        return _this;
    }
    return ProjectMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ProjectMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ClientService",
    "UserService",
    "ProjectService",
    "EnumService",
    "Popups",
    ProjectMaintenanceDetailController
]);
//# sourceMappingURL=~ProjectMaintenanceDetailController.js.map