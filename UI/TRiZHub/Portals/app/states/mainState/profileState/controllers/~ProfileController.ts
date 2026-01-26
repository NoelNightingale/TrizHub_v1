class ProfileController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    existingEmail: any;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private $scope: ng.IScope,
        private $uibModal: any,
        private $log: ng.ILogService,
        private MasterDataService: MasterDataServiceModule.MasterDataService,
        private AccountService: AccountServiceModule.AccountService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        this.viewModel = {};
        MasterDataService.profileGet()
            .then(
                result => {
                    me.viewModel = result;
                    me.existingEmail = result.emailAddress;
                },
                error => {
                    me.handleError(error);
                });
    }

    //#endregion

    submitForm = () => {
        const me = this;
        this.MasterDataService.profileSave(this.viewModel)
            .then(
                result => {
                    this.saveSuccess = true;
                    if (result.emailAddress !== this.existingEmail) {
                        me.AccountService.logout();
                        me.SecurityService.getCurrentUserDetails().loggedIn = false;
                        me.$state.transitionTo("root.login");
                    } else {
                        me.$timeout(function() { me.$state.transitionTo("mainState.home"); }, 1000);
                    }
                },
                error => {
                    me.handleError(error);
                });
    };

    cancelForm = (): void => {
        this.$state.transitionTo("mainState.home");
    };
}

angular.module("AngularApp")
    .controller("ProfileController",
    [
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "$scope",
        "$uibModal",
        "$log",
        "MasterDataService",
        "AccountService",
        "SecurityService",
        "Popups",
        ProfileController
    ]);