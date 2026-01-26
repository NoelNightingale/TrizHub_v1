class EmployerMaintenanceGridController extends CHControllerBase {

    pageGrid: any;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private EmployerService: EmployerServiceModule.EmployerService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("name", self.EmployerService.employerGrid, self.onDataLoaded, null, null, $state);
        self.pageGrid.loadGrid();
    }

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            console.log(this.gridModel);
        }
    }

    toggleInactive = () => {
        this.pageGrid.loadGrid();
    };

    toggleActivation = (employer: any) => {
        const self = this;
        if (employer.isActive) {
            this.EmployerService.deactivateEmployer(employer.id)
                .then(
                    result => {
                        employer.isActive = false;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.EmployerService.activateEmployer(employer.id)
                .then(
                    result => {
                        employer.isActive = true;
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    };

    deleteRecord = (record) => {
        const me = this;
        me.Popups.confirmationDialog(me.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this record...")
            .then(
                action => {
                    if (action)
                        me.EmployerService.deleteEmployer(record.id)
                            .then(
                                result => {
                                    me.pageGrid.loadGrid();
                                },
                                error => {
                                    me.handleError(error);
                                });

                },
                error => {
                    me.handleError(error);
                });
    };

    newEmployer = () => {
        this.$state.transitionTo("mainState.maintenance.employerMaintenance.detail", { "id": "new" });
    };
}

angular.module("AngularApp")
    .controller("EmployerMaintenanceGridController",
        [
            "$scope",
            "$state",
            "EmployerService",
            "Popups",
            EmployerMaintenanceGridController
        ]);