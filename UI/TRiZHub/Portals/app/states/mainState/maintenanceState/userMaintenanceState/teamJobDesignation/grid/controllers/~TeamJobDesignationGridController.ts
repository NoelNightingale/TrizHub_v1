class TeamJobDesignationGridController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    viewModel: any;
    user: any;

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private UserService: UserServiceModule.UserService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];
        UserService.userGet(this.viewModel.id)
            .then(
                result => {
                    self.user = result;
                },
                error => {
                    self.handleError(error);
                });

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "client",
            this.UserService.teamJobDesignationGrid,
            this.onDataLoaded,
            model => {
                model.Id = self.viewModel.id;
            },
            null,
            this.$state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;

        this.loadingIsDone = true;

    }

    newRecord = () => {
        this.$state.transitionTo("mainState.maintenance.userMaintenance.teamJobDesignationtDetail",
        { userid: this.viewModel.id, "id": "new" });
    };

    deleteRecord = (record) => {
        const me = this;
        me.Popups.confirmationDialog(me.$scope,
                "Are you sure you want to delete?",
                "You are about to delete this record...")
            .then(
                action => {
                    if (action)
                        if (!record.new) {
                            me.UserService.teamJobDesignationDelete(record)
                                .then(
                                    result => {
                                        me.saveSuccess = true;
                                        me.reloadGrid();
                                    },
                                    error => {
                                        me.handleError(error);
                                    });
                        } else {
                            //me.gridModel
                            const index = me.gridModel.data.indexOf(record);
                            me.gridModel.data.splice(index, 1);
                        }

                },
                error => {
                    me.handleError(error);
                });
    };

    reloadGrid = () => {
        const me = this;
        me.pageGrid.loadGrid();

    };
}

angular.module("AngularApp")
    .controller("TeamJobDesignationGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "UserService",
        "Popups",
        TeamJobDesignationGridController
    ]);