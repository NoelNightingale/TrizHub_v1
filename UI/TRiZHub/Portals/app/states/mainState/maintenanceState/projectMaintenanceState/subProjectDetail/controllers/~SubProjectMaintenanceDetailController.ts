
class SubProjectMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    parentProjectId: string;
    projectId: string;
    clientDropdown: any;
    userDropdown: any;
    projectTypeDropdown: any;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private EnumService: EnumServiceModule.EnumService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.parentProjectId = self.$stateParams["id"];
        self.projectId = self.$stateParams["subProjectId"];
        self.viewModel = {};

        ProjectService.projectTypeDropdownList()
            .then(
                result => {
                    self.projectTypeDropdown = result;
                },
                error => {
                    self.handleError(error);
                });

        if (self.projectId !== "new") {
            ProjectService.subProjectGet(self.projectId)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
            ProjectService.projectGet(self.parentProjectId)
                .then(
                    result => {
                        self.viewModel.parentProjectName = result.projectName;
                        self.viewModel.parentProjectNumber = result.projectNumber;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.projectId = null;
            self.viewModel.entityType = 0;
            self.viewModel.isActive = true;
            ProjectService.projectGet(self.parentProjectId)
                .then(
                    result => {
                        self.viewModel.parentProjectName = result.projectName;
                        self.viewModel.parentProjectNumber = result.projectNumber;
                        self.viewModel.subProjectTypeId = result.projectTypeId;
                        self.viewModel.parentAllowSubProjectAlternativeType = result.allowSubProjectAlternativeType;
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    }

    //#endregion

    cancelForm = (): any => {
        const self = this;
        self.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectGrid", { "id": this.parentProjectId });

    }

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;
        self.viewModel.projectId = self.parentProjectId;
        self.ProjectService.subProjectSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => { self.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectGrid", { "id": this.parentProjectId }); },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("SubProjectMaintenanceDetailController",
        [
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