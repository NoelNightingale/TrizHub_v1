
class WorkTeamMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    canCreate = false;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private WorkTeamService: WorkTeamServiceModule.WorkTeamService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.canCreate = SecurityService.isAllowed("TeamMaintenance");
        this.pageGrid = new TcrGridServiceModule
            .TcrGridService("name", this.WorkTeamService.workTeamGrid, this.onDataLoaded, null, null, $state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    toggleInactive = () => {
        this.pageGrid.loadGrid();
    };

    newRecord = () => {
        this.$state.transitionTo("mainState.maintenance.workTeamMaintenance.detail", { "id": "new" });
    };
}

angular.module("AngularApp")
    .controller("WorkTeamMaintenanceGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "WorkTeamService",
        "SecurityService",
        "Popups",
        WorkTeamMaintenanceGridController
    ]);
