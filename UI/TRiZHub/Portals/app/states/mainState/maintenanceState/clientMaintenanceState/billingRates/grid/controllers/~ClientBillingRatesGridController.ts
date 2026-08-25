class ClientBillingRatesGridController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    viewModel: any;
    client: any;

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private ClientService: ClientServiceModule.ClientService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];

        ClientService.clientGet(this.viewModel.id)
            .then(
                result => {
                    self.client = result;
                },
                error => {
                    self.handleError(error);
                });

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "startDate",
            this.BillingRatesService.billingRatesGrid,
            this.onDataLoaded,
            model => {
                model.clientId = self.viewModel.id;
            },
            null,
            this.$state);
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
        this.$state.transitionTo("mainState.maintenance.clientMaintenance.billingRatesDetail",
            { clientId: this.viewModel.id, id: "new" });
    };

    reloadGrid = () => {
        this.pageGrid.loadGrid();
    };
}

angular.module("AngularApp")
    .controller("ClientBillingRatesGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "BillingRatesService",
        "ClientService",
        "Popups",
        ClientBillingRatesGridController
    ]);
