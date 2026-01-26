class BillingCycleMaintenanceGridController extends CHControllerBase {

    //#region Members

    successMessage = "Saved Successfully";
    saveSuccess = false;

    filterModel: any;
    pageGrid: any;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    gridModel: TcrGridModel;
    loadingIsDone = false;
    viewModel: any;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private $scope: ng.IScope,
        private $uibModal: any,
        private $log: ng.ILogService,
        private $filter: ng.IFilterService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        this.filterModel = {};
        me.viewModel = this.getCurrentYear();
        me.pageGrid = new TcrGridServiceModule.TcrGridService("cycle", this.BillingCycleService.billingCycleGrid, this.onDataLoaded,
            model => {
                model.searchFor = this.viewModel;

            }, null, $state);
        me.filterModel.userId = SecurityService.getCurrentUserDetails().id;
        me.pageGrid.loadGrid();
    }

    getCurrentYear = (): number => {
        var date = new Date(Date.now());
        var year = date.getFullYear();
        return year;
    }

    reloadGrid = () => {
        const me = this;
        if (!me.filterModel.year) {
        }
        me.pageGrid.loadGrid();
    };

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    incYear = () => {
        this.viewModel = this.viewModel + 1;
        this.gridModel.searchFor = String(this.viewModel);
        this.pageGrid.loadGrid();
    };


    decYear = () => {
        this.viewModel = this.viewModel - 1;
        this.gridModel.searchFor = String(this.viewModel);
        this.pageGrid.loadGrid();
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
                            me.BillingCycleService.billingCycleDelete(record)
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

    newBillingCycle = () => {
        this.$state.transitionTo("mainState.maintenance.billingCycleMaintenance.detail", { "id": "new" });
    };

}


angular.module("AngularApp")
    .controller("BillingCycleMaintenanceGridController",
        [
            "$stateParams",
            "$timeout",
            "$window",
            "$state",
            "$scope",
            "$uibModal",
            "$log",
            "$filter",
            "SecurityService",
            "BillingCycleService",
            "Popups",
            BillingCycleMaintenanceGridController
        ]);