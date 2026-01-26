
class ScorecardTemplatePeriodMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    scorecardTemplateId: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        self.scorecardTemplateId = self.$stateParams["scorecardTemplateId"];
        self.pageGrid = new TcrGridServiceModule.TcrGridService("description",
            self.ScorecardTemplateService.scorecardTemplatePeriodGrid,
            self.onDataLoaded,
            model => {
                model.id = self.scorecardTemplateId;
            },
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

    newScorecardTemplatePeriod = () => {
        const self = this;
        this.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.detail",
            { "id": "new", "scorecardTemplateId": self.scorecardTemplateId });
    };

    deleteTemplatePeriod = (templatePeriod: any) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this period")
            .then(
                action => {
                    if (action) {
                        this.ScorecardTemplateService.scorecardTemplatePeriodDelete(templatePeriod)
                            .then(
                                result => {
                                    // Remove item from list
                                    for (var i = 0; i < self.gridModel.data.length; i++) {
                                        if (self.gridModel.data[i].id == templatePeriod.id) {
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
    .controller("ScorecardTemplatePeriodMaintenanceGridController",
        [
            "$scope",
            "$state",
            "$stateParams",
            "ScorecardTemplateService",
            "Popups",
            ScorecardTemplatePeriodMaintenanceGridController
        ]);