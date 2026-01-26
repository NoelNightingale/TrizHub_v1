
class ScorecardTemplateMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("scorecardname",
                self.ScorecardTemplateService.scorecardTemplateGrid,
                self.onDataLoaded,
                null,
                null,
                $state);
        self.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    newScorecardTemplate = () => {
        this.$state.transitionTo("mainState.maintenance.scorecardTemplateMaintenance.detail", { "id": "new" });
    };

    deleteTemplate = (template: any) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this template")
            .then(
                action => {
                    if (action) {
                        this.ScorecardTemplateService.scorecardTemplateDelete(template)
                            .then(
                                result => {
                                    // Remove item from list
                                    for (var i = 0; i < self.gridModel.data.length; i++) {
                                        if (self.gridModel.data[i].id == template.id) {
                                            self.gridModel.data.splice(i, 1);
                                            return;
                                        }
                                    }
                                },
                                error => {

                                    self.Popups.showError(self.$scope, error, "Error")
                                        .then(action => {
                                            console.log("Error completed");
                                        },
                                        error => {
                                            // No need for action
                                        });
                                    self.handleError(error);
                                });
                    }

                },
                error => {
                    self.handleError(error);
                });


    };
}

angular.module("AngularApp")
    .controller("ScorecardTemplateMaintenanceGridController",
        [
            "$scope",
            "$state",
            "ScorecardTemplateService",
            "Popups",
            ScorecardTemplateMaintenanceGridController
        ]);