
class RoleMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    categoryId: string;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private RoleService: RoleServiceModule.RoleService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.categoryId = this.$stateParams["id"];
        this.viewModel = {};

        if (this.categoryId !== "new") {
            RoleService.roleGet(this.categoryId)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.categoryId = null;
            this.RoleService.rolePrivileges("")
                .then(
                    result => {
                        self.viewModel.permissions = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    }

    //#endregion

    submitForm = (): any => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;
        this.RoleService.roleSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => { self.$state.transitionTo("mainState.maintenance.roleMaintenance.grid"); },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("RoleMaintenanceDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "$window",
        "RoleService",
        "Popups",
        RoleMaintenanceDetailController
    ]);