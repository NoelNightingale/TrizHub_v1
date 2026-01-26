class EmployerMaintenanceDetailController extends CHControllerBase {
    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;

    //userDropdown: any;
    //clientDropdown: any;
    //employedBy: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private EmployerService: EmployerServiceModule.EmployerService,
        private Popups: any) {
        super($scope, Popups, $state);

        const self = this;
        this.viewModel = {};

        this.viewModel.id = this.$stateParams["id"];

        if (this.viewModel.id !== "new") {
            this.EmployerService.employerGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.viewModel.id = null;
            this.viewModel.isActive = true;
        }
    }

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        self.EmployerService.employerSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$state.transitionTo("mainState.maintenance.employerMaintenance.grid");
                },
                error => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("EmployerMaintenanceDetailController",
        [
            "$scope",
            "$stateParams",
            "$timeout",
            "$window",
            "$state",
            "EmployerService",
            "Popups",
            EmployerMaintenanceDetailController
        ]);