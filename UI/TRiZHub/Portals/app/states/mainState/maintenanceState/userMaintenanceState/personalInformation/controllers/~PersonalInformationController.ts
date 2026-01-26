class PersonalInformationController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    genders: any;
    races: any;

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
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.genders = String[2] = ["Male", "Female"];
        this.races = String[4] = ["White", "Black", "Colored", "Asian"];
        this.viewModel.id = this.$stateParams["id"];

        this.UserService.personalInformationGet(this.viewModel.id)
            .then(
            result => {
                self.viewModel = result;
            },
            error => {
                self.handleError(error);
            });
    }

    //#endregion

    submitForm = () => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;
        this.UserService.personalInformationSave(this.viewModel)
            .then(
            result => {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.go("mainState.maintenance.userMaintenance.detail",
                        { "id": result.userAccountId });
                },
                    1000);
            },
            error => {
                self.handleError(error);
            });
    };
}

angular.module("AngularApp")
    .controller("PersonalInformationController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "UserService",
        "Popups",
        PersonalInformationController
    ]);