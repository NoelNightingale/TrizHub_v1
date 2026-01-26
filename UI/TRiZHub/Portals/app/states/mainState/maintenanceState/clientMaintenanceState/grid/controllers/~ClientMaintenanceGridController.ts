
class ClientMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private ClientService: ClientServiceModule.ClientService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        this.pageGrid = new TcrGridServiceModule
            .TcrGridService("entityName", this.ClientService.clientGrid, this.onDataLoaded, null, null, $state);
        this.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    newClient = () => {
        this.$state.transitionTo("mainState.maintenance.clientMaintenance.detail", { "id": "new" });
    };

    deleteClient = (client: any, index: number) => {
        this.Popups.confirmationDialog(this.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this client")
            .then(
                action => {
                    if (action) {
                        this.ClientService.deleteClient(client.id)
                            .then(
                                result => {
                                    if (result == 0) {
                                        this.Popups.showError(this.$scope, "The Client could not be deleted as it has projects assigned to it.", null, null);
                                    }
                                    else {
                                        this.gridModel.data.splice(index, 1);
                                    }
                                },
                                error => {
                                    this.handleError(error);
                                });
                    }

                },
                error => {
                    this.handleError(error);
                });
    };

}

angular.module("AngularApp")
    .controller("ClientMaintenanceGridController",
    [
        "$scope",
        "$state",
        "ClientService",
        "Popups",
        ClientMaintenanceGridController
    ]);