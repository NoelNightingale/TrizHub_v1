class AdminMaintenanceSettingsController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    pageGrid: any;
    viewModel: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: angular.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private MasterDataService: MasterDataServiceModule.MasterDataService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};

        this.MasterDataService.settingsGet()
            .then(
                result => {
                    self.viewModel = result;
                },
                error => {
                    self.handleError(error);
                });
    }

    //#endregion

    cancelForm = () => {
        this.$state.transitionTo("mainState.home");
    };

    submitForm = (): any => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;

        this.MasterDataService.settingsSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function() { self.$state.transitionTo("mainState.home"); }, 1000);
                },
                (error) => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("AdminMaintenanceSettingsController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "$timeout",
        "$window",
        "MasterDataService",
        "Popups",
        AdminMaintenanceSettingsController
    ]);