class TravelInformationController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private UserService: UserServiceModule.UserService,
        private Popups: any) {
        super($scope, Popups);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];

        if (this.viewModel.id !== "new") {
            this.UserService.travelInformationGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.viewModel.id = null;
        }
    }


    //#endregion

    submitForm = () => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;
        this.UserService.travelInformationSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function() {
                            self.$state.go("mainState.maintenance.userMaintenance.grid");
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };
    cancelForm = (): void => {
        this.$state.go("mainState.maintenance.userMaintenance.grid");
    };
}

angular.module("AngularApp")
    .controller("TravelInformationController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "UserService",
        "Popups",
        TravelInformationController
    ]);