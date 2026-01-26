
class TeamMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private TeamService: TeamServiceModule.TeamService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        this.pageGrid = new TcrGridServiceModule
            .TcrGridService("teamname", this.TeamService.teamGrid, this.onDataLoaded, null, null, $state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }

    }


    newRecord = () => {
        this.$state.transitionTo("mainState.maintenance.teamMaintenance.detail", { "id": "new" });
    };


}

angular.module("AngularApp")
    .controller("TeamMaintenanceGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "TeamService",
        "Popups",
        TeamMaintenanceGridController
    ]);
