
class ScorecardTemplatePeriodMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scorecardTemplateId: string;
    scorecardTemplateItemId: string;
    clientDropdown: any;
    userDropdown: any;
    filterOptions: any;
    reviewYearDate: any;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private EnumService: EnumServiceModule.EnumService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.scorecardTemplateId = self.$stateParams["scorecardTemplateId"];
        self.scorecardTemplateItemId = self.$stateParams["id"];

        self.viewModel = {
            scorecardName: "",
            description: "",
            isVariable: false,
            startDate: new Date(),
            endDate: new Date(),
            reviewYear: new Date().getFullYear()
        };

        self.filterOptions = {};
        self.reviewYearDate = new Date();


        if (self.scorecardTemplateItemId !== "new") {
            ScorecardTemplateService.scorecardTemplatePeriodGet(self.scorecardTemplateItemId)
                .then(
                    result => {
                        self.viewModel = result;
                        self.reviewYearDate.setFullYear(self.viewModel.reviewYear);

                        ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                            .then(
                                results => {
                                    self.viewModel.scorecardName = results.scorecardName;
                                },
                                error => {
                                    self.handleError(error);
                                });
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.scorecardTemplateItemId = null;
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(
                    results => {
                        self.viewModel.scorecardName = results.scorecardName;
                        self.reviewYearDate.setFullYear(self.viewModel.reviewYear);
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    }

    //#endregion

    cancelForm = (): any => {
        const self = this;
        self.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.grid",
            { "scorecardTemplateId": self.scorecardTemplateId });
    };

    submitForm = (): any => {
        const self = this;

        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        self.viewModel.reviewYear = self.reviewYearDate.getFullYear();

        // Set details to today
        if (self.scorecardTemplateItemId == "new" && self.viewModel.isVariable) {
            self.viewModel.startDate = new Date();
            self.viewModel.endDate = new Date();
            //self.reviewYearDate = new Date();
        }

        self.viewModel.scorecardTemplateId = self.scorecardTemplateId;
        self.ScorecardTemplateService.scorecardTemplatePeriodSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => {
                        self.$state.transitionTo("mainState.maintenance.scorecardTemplatePeriodMaintenance.grid",
                            { "scorecardTemplateId": self.scorecardTemplateId });
                    },
                        1000);
                },
                error => {
                    self.Popups.showError(self.$scope, error, "Error");
                    self.handleError(error);
                });
    };

    variableChange = (): any => {
        if (this.scorecardTemplateItemId == "new" && this.viewModel.isVariable) {
            this.viewModel.startDate = new Date();
            this.viewModel.endDate = new Date();
            this.reviewYearDate = new Date();
        }
    };
}

angular.module("AngularApp")
    .controller("ScorecardTemplatePeriodMaintenanceDetailController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "$timeout",
            "$window",
            "ScorecardTemplateService",
            "EnumService",
            "Popups",
            ScorecardTemplatePeriodMaintenanceDetailController
        ]);