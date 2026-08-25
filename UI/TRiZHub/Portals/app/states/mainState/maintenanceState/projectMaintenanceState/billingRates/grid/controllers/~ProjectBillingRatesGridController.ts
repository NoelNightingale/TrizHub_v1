class ProjectBillingRatesGridController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    viewModel: any;
    project: any;

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];

        ProjectService.projectGet(this.viewModel.id)
            .then(
                result => {
                    self.project = result;
                },
                error => {
                    self.handleError(error);
                });

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "startDate",
            this.BillingRatesService.billingRatesGrid,
            this.onDataLoaded,
            model => {
                model.projectId = self.viewModel.id;
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
        this.$state.transitionTo("mainState.maintenance.projectMaintenance.billingRatesDetail",
            { projectId: this.viewModel.id, id: "new" });
    };

    reloadGrid = () => {
        this.pageGrid.loadGrid();
    };
}

angular.module("AngularApp")
    .controller("ProjectBillingRatesGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "BillingRatesService",
        "ProjectService",
        "Popups",
        ProjectBillingRatesGridController
    ]);
