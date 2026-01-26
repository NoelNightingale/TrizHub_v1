class UserMaintenanceGridController extends CHControllerBase {

    //#region Members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    show: boolean;

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private UserService: UserServiceModule.UserService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        this.show = false;
        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "firstName",
            this.UserService.userGrid,
            model => {
                this.gridModel = model;
                if (this.gridModel.totalItems > 0) {
                    this.loadingIsDone = true;
                }
            },
            null,
            null,
            $state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    unLock = (user: any) => {
        const self = this;
        this.UserService.userUnlock(user.id)
            .then(
                result => {
                    user.lockedOut = null;
                },
                error => {
                    self.handleError(error);
                });
    };

    toggleUserActivation = (user: any) => {
        const self = this;
        if (user.active) {
            this.UserService.deactivateUser(user.id)
                .then(
                    result => {
                        user.active = false;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.UserService.activateUser(user.id)
                .then(
                    result => {
                        user.active = true;
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    };

    newUser = () => {
        this.$state.transitionTo("mainState.maintenance.userMaintenance.detail", { "id": "new" });
    };

    toggleInactiveUserShow = () => {
        const self = this;
        this.pageGrid.loadGrid();
    };


}

angular.module("AngularApp")
    .controller("UserMaintenanceGridController",
    [
        "$scope",
        "$state",
        "UserService",
        "Popups",
        UserMaintenanceGridController
    ]);