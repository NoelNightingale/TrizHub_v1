class LoginController extends CHControllerBase {

    //#region Members

    headerMessage = "Loading...";
    viewModel = null;
    successMessage = "Login Successfull";

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: any,
        private $timeout: ng.ITimeoutService,
        private AccountService: AccountServiceModule.AccountService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.initHome();
    }

    //#endregion

    initHome = (): void => {
        const me = this;
        me.AccountService.logout()
            .then(
                function (result) {

                    console.log(result);
                    me.AccountService.login()
                        .then(
                            function (result) {

                                console.log(result);

                                me.AccountService.init()
                                    .then(
                                        function(result) {
                                            me.viewModel = result;
                                            if (me.viewModel.isUserProfileComplete) {
                                                me
                                                    .headerMessage = `Welcome back ${me.viewModel.displayName} (${
                                                    me.viewModel.userName})`;
                                            } else {
                                                me
                                                    .headerMessage = `Please setup your account (${me.viewModel.userName
                                                    })`;
                                            }
                                        },
                                        function(e) {
                                            me.headerMessage = `Oops something went wrong...${e}`;
                                        });
                            },
                            function(error) {
                                me.headerMessage = `Oops something went wrong... ${error}`;
                                me.AccountService.logout();
                            }
                        );
                },
                function(error) {
                    me.headerMessage = `Oops something went wrong... ${error}`;
                    me.AccountService.logout();
                }
            );
    };

    goTohome = (): void => {
        this.$state.go("mainState.home");
    };

    goToProfile = (): void => {
        this.$state.go("mainState.profile");
    };

    reload = (): void => {
        this.initHome();
    };

};

angular.module("AngularApp")
    .controller("LoginController",
    [
        "$scope",
        "$state",
        "$timeout",
        "AccountService",
        "Popups",
        LoginController
    ]);