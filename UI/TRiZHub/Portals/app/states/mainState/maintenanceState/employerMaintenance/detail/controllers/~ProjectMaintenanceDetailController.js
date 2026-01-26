//class ProjectMaintenanceDetailController extends CHControllerBase {
//    //#region members
//    successMessage = "Saved Successfully";
//    saveSuccess = false;
//    viewModel: any;
//    projectId: string;
//    clientDropdown: any;
//    userDropdown: any;
//    projectTypeDropdown: any;
//    changedProjectType: boolean = false;
//    //#endregion
//    //#region Ctor
//    constructor(
//        private $stateParams: ng.ui.IStateParamsService,
//        private $scope: ng.IScope,
//        private $state: ng.ui.IStateService,
//        private $timeout: ng.ITimeoutService,
//        private $window: ng.IWindowService,
//        private ClientService: ClientServiceModule.ClientService,
//        private UserService: UserServiceModule.UserService,
//        private ProjectService: ProjectServiceModule.ProjectService,
//        private EnumService: EnumServiceModule.EnumService,
//        private Popups: any) {
//        super($scope, Popups, $state);
//        const self = this;
//        self.projectId = self.$stateParams["id"];
//        self.viewModel = {};
//        ClientService.clientDropdownList()
//            .then(
//                result => {
//                    self.clientDropdown = result;
//                },
//                error => {
//                    self.handleError(error);
//                });
//        UserService.userDropdownList()
//            .then(
//                result => {
//                    self.userDropdown = result;
//                    self.userDropdown.splice(0, 0, { id: null, description: "N/A" });
//                },
//                error => {
//                    self.handleError(error);
//                });
//        ProjectService.projectTypeDropdownList()
//            .then(
//                result => {
//                    self.projectTypeDropdown = result;
//                },
//                error => {
//                    self.handleError(error);
//                });
//        if (self.projectId !== "new") {
//            ProjectService.projectGet(self.projectId)
//                .then(
//                    result => {
//                        self.viewModel = result;
//                    },
//                    error => {
//                        self.handleError(error);
//                    });
//        } else {
//            self.projectId = null;
//            self.viewModel.entityType = 0;
//            self.viewModel.billable = true;
//            self.viewModel.isActive = true;
//        }
//    }
//    //#endregion
//    projectTypeChange = (): any => {
//        this.changedProjectType = true;
//    };
//    submitForm = (): any => {
//        const self = this;
//        self.$scope.$broadcast("show-errors-check-validity");
//        if (self.$scope["EditForm"].$invalid)
//            return;
//        // Check if not invoiceable and billable
//        let billableText: string = this.viewModel.billable ? 'billable' : 'non-billalbe';
//        for (var i = 0; i < this.projectTypeDropdown.length; i++) {
//            if (this.projectTypeDropdown[i].id == this.viewModel.projectTypeId && this.projectTypeDropdown[i].allowSubProjectBillable != this.viewModel.billable) {
//                this.Popups.showError(this.$scope, "Projects that are of type '" + this.projectTypeDropdown[i].name + "', cannot be " + billableText + ".")
//                return;
//            }
//        }
//        if (this.changedProjectType && this.viewModel.hasSubprojects) {
//            this.Popups.confirmationDialog(this.$scope, "Are you sure you want to change the Project type?", "This Project has sub-projects, continuing will change their type as well and cannot be undone. Review and make a note of existing sub project types before changing the parent.").then(
//                result => {
//                    if (result) {
//                        self.ProjectService.projectSave(self.viewModel)
//                            .then(
//                                result => {
//                                    self.saveSuccess = true;
//                                    self.$state.transitionTo("mainState.maintenance.projectMaintenance.grid");
//                                },
//                                error => {
//                                    self.handleError(error);
//                                });
//                    }
//                },
//                error => {
//                });
//        }
//        else {
//            self.ProjectService.projectSave(self.viewModel)
//                .then(
//                    result => {
//                        self.saveSuccess = true;
//                        self.$state.transitionTo("mainState.maintenance.projectMaintenance.grid");
//                    },
//                    error => {
//                        self.handleError(error);
//                    });
//        }
//    };
//}
//angular.module("AngularApp")
//    .controller("ProjectMaintenanceDetailController",
//        [
//            "$stateParams",
//            "$scope",
//            "$state",
//            "$timeout",
//            "$window",
//            "ClientService",
//            "UserService",
//            "ProjectService",
//            "EnumService",
//            "Popups",
//            ProjectMaintenanceDetailController
//        ]); 
//# sourceMappingURL=~ProjectMaintenanceDetailController.js.map