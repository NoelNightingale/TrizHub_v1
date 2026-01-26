class AdminFooterController extends CHControllerBase {

    currentUser: any;

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.currentUser = this.SecurityService.getCurrentUserDetails();
    }

    //#endregion

    isAllowed = (privilegeType: string): boolean => {
        return this.SecurityService.isAllowed(privilegeType);
    };
};

angular.module("AngularApp")
    .controller("AdminFooterController",
    [
        "$scope",
        "$state",
        "SecurityService",
        "Popups",
        AdminFooterController
    ]);