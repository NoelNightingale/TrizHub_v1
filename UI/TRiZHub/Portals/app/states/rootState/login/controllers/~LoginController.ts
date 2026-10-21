class LoginController extends CHControllerBase {

    //#region Members

    headerMessage = "Loading...";
    viewModel = null;
    successMessage = "Login Successfull";
    canImpersonate = false;
    usersLoaded = false;
    switchingUser = false;
    pickerReady = false;
    users: any[] = [];
    visibleUsers: any[] = [];
    userFilter = "";
    selectedAccount = "";
    appliedAccount = "";

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
                                            me.canImpersonate = !!result.canImpersonate;
                                            me.appliedAccount = result.impersonating ? result.userName : "";
                                            me.selectedAccount = me.appliedAccount;
                                            me.applyWelcome();
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

    applyWelcome = (): void => {
        const me = this;
        if (!me.viewModel)
            return;
        if (me.viewModel.isUserProfileComplete) {
            me.headerMessage = `Welcome back ${me.viewModel.displayName} (${me.viewModel.userName})`;
        } else {
            me.headerMessage = `Please setup your account (${me.viewModel.userName})`;
        }
    };

    loadCandidates = (): void => {
        const me = this;
        me.AccountService.impersonationCandidates()
            .then(
                users => {
                    me.users = users || [];
                    me.applyUserFilter();
                    me.usersLoaded = true;
                    me.$timeout(() => { me.pickerReady = true; }, 0);
                },
                error => {
                    me.headerMessage = `Could not load users. ${error}`;
                });
    };

    applyUserFilter = (): void => {
        const q = (this.userFilter || "").toLowerCase();
        const selected = this.selectedAccount || "";
        const matches = (this.users || []).filter(user => {
            if (selected && user.accountName === selected)
                return true;
            if (!q)
                return true;
            const haystack = ((user.firstname || "") + " " + (user.surname || "") + " " + (user.accountName || "")).toLowerCase();
            return haystack.indexOf(q) !== -1;
        }).map(user => ({
            accountName: user.accountName,
            label: (user.firstname || "") + " " + (user.surname || "") + " (" + user.accountName + ")"
        }));
        this.visibleUsers = matches;
    };

    switchUser = (): void => {
        const me = this;
        if (!me.pickerReady)
            return;
        const selected = me.selectedAccount || "";
        if (selected === (me.appliedAccount || "") || me.switchingUser)
            return;
        me.switchingUser = true;
        me.AccountService.impersonate(selected)
            .then(() => me.AccountService.getCurrentUser())
            .then(
                result => {
                    me.viewModel = result;
                    me.appliedAccount = result.impersonating ? result.userName : "";
                    me.selectedAccount = me.appliedAccount;
                    me.switchingUser = false;
                    me.applyWelcome();
                    me.applyUserFilter();
                },
                error => {
                    me.switchingUser = false;
                    me.selectedAccount = me.appliedAccount;
                    me.applyUserFilter();
                    me.headerMessage = `Could not switch user. ${error}`;
                });
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