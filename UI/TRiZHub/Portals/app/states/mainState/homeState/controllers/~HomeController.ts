class HomeController extends CHControllerBase {

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private AccountService: AccountServiceModule.AccountService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);

        if (this.SecurityService.getCurrentUserDetails() == undefined) {
            this.AccountService.init()
                .then(
                function (result) { },
                function (e) { this.$state.go("root.login"); });
        }

        else {
            if (this.SecurityService.getCurrentUserDetails().allowedPrivileges.indexOf(7) != -1 && this.SecurityService.getCurrentUserDetails().allowedPrivileges.length == 1) {
                this.$state.go("mainState.timesheet")
            }
        }
    }


    //#endregion



};

angular.module("AngularApp")
    .controller("HomeController",
    [
        "$scope",
        "$state",
        "AccountService",
        "SecurityService",
        "Popups",
        HomeController
    ]);