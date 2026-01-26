
class RoleMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private RoleService: RoleServiceModule.RoleService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        this.pageGrid = new TcrGridServiceModule
            .TcrGridService("rolename", this.RoleService.roleGrid, this.onDataLoaded, null, null, $state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }

    }


    newRole = () => {
        this.$state.transitionTo("mainState.maintenance.roleMaintenance.detail", { "id": "new" });
    };
}

angular.module("AngularApp")
    .controller("RoleMaintenanceGridController",
    [
        "$scope",
        "$state",
        "RoleService",
        "Popups",
        RoleMaintenanceGridController
    ]);