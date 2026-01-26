class SubProjectGridController extends CHControllerBase {

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
    parentProjectId: any;
    parentProjectName: any;
    parentProjectNumber: any;
    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];

        this.parentProjectId = this.$stateParams["id"];

        ProjectService.projectGet(self.parentProjectId)
            .then(
            result => {
//                alert(result.projectName);
                this.parentProjectName = result.projectName;
                this.parentProjectNumber = result.projectNumber;
            },
            error => {
                this.handleError(error);
            });

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "subProjectNumber",
            this.ProjectService.subProjectGrid,
            this.onDataLoaded,
            model => {
                model.parentId = this.viewModel.id;
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
        this.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectDetail",
            { "id": this.parentProjectId, "subProjectId": "new" });
    };

    deleteRecord = (record) => {
/*        const me = this;
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
                        //me.gridModel
                        const index = me.gridModel.data.indexOf(record);
                        me.gridModel.data.splice(index, 1);
                    }

            },
            error => {
                me.handleError(error);
            }); */
    };
    
    reloadGrid = () => {
        const me = this;
        me.pageGrid.loadGrid();

    };
}

angular.module("AngularApp")
    .controller("SubProjectGridController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "ProjectService",
        "Popups",
        SubProjectGridController
    ]);