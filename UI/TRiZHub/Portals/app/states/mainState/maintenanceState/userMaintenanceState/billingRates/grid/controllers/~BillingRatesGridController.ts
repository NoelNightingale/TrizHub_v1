class BillingRatesGridController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any;
    idGridModel: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    viewModel: any;
    user: any;

    viewMode: string = "all"; // "all" | "asOf"
    asOfDate: any;
    asOfModel: any;
    asOfLoading = false;
    asOfError: string = null;

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private UserService: UserServiceModule.UserService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];
        this.asOfDate = new Date();

        UserService.userGet(this.viewModel.id)
            .then(
                result => {
                    self.user = result;
                },
                error => {
                    self.handleError(error);
                });

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "startDate",
            this.BillingRatesService.billingRatesGrid,
            this.onDataLoaded,
            model => {
                model.userAccountId = self.viewModel.id;
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

    setViewMode = (mode: string) => {
        this.viewMode = mode;
        this.asOfError = null;
        if (mode === "asOf") {
            this.loadAsOf();
        } else {
            this.reloadGrid();
        }
    };

    loadAsOf = () => {
        const self = this;
        self.asOfLoading = true;
        self.asOfError = null;
        self.BillingRatesService.userRatesAsOf(self.viewModel.id, self.asOfDate)
            .then(
                result => {
                    self.asOfModel = result;
                    self.asOfLoading = false;
                },
                error => {
                    self.asOfLoading = false;
                    self.asOfError = error;
                    self.handleError(error);
                });
    };

    newRecord = (scope?: string, clientId?: string, projectId?: string) => {
        const params: any = { userid: this.viewModel.id, id: "new" };
        if (scope) {
            params.scope = scope;
        }
        if (clientId) {
            params.clientId = clientId;
        }
        if (projectId) {
            params.projectId = projectId;
        }
        this.$state.transitionTo("mainState.maintenance.userMaintenance.billingRatesDetail", params);
    };

    editRecord = (rateId: string) => {
        if (!rateId) {
            return;
        }
        this.$state.transitionTo("mainState.maintenance.userMaintenance.billingRatesDetail",
            { userid: this.viewModel.id, id: rateId });
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
                            me.BillingRatesService.billingRatesDelete(record)
                                .then(
                                    result => {
                                        me.saveSuccess = true;
                                        me.reloadGrid();
                                    },
                                    error => {
                                        me.handleError(error);
                                    });
                        } else {
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
    .controller("BillingRatesGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "BillingRatesService",
        "UserService",
        "Popups",
        BillingRatesGridController
    ]);
