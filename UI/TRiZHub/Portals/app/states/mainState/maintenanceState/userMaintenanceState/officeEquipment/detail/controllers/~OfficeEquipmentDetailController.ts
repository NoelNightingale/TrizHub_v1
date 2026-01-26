class OfficeEquipmentDetailController extends CHControllerBase {

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
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.userAccountId = this.$stateParams["userid"];
        this.viewModel.id = this.$stateParams["id"];

        if (this.viewModel.id !== "new") {
            this.UserService.officeEquipmentGet(this.viewModel.id)
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
        this.UserService.officeEquipmentSave(this.viewModel)
            .then(
            result => {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.userMaintenance.officeEquipmentGrid",
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
    .controller("OfficeEquipmentDetailController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "UserService",
        "Popups",
        OfficeEquipmentDetailController
    ]);