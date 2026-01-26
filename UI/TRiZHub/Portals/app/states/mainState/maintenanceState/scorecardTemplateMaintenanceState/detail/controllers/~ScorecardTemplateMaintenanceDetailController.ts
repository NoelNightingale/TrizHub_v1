
class ScorecardTemplateMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scorecardTemplateId: string;
    clientDropdown: any;
    userDropdown: any;

    filterOptions: any;

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
        self.scorecardTemplateId = self.$stateParams["id"];
        self.filterOptions = { maxWeight: 100 };
        if (self.scorecardTemplateId !== "new") {
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(
                    result => {
                        self.viewModel = result;

                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.scorecardTemplateId = null;

        }
    }

    percentageFormatting = (value): string => {
        return value.toString() + "%";
    };

    //#endregion

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        console.log(self.viewModel.excellentWeight);
        console.log(self.viewModel.adequateWeight);
        console.log(self.viewModel.inadequateWeight);

        // Validate
        if ((self.viewModel.excellentWeight < 1) && (self.viewModel.adequateWeight < 1) && (self.viewModel.inadequateWeight < 1)) {
            self.Popups.showError(self.$scope, "Please specify at least one E,A,I weight.", "Error");
            return;
        }

        self.ScorecardTemplateService.scorecardTemplateSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => {
                        self.$state.transitionTo("mainState.maintenance.scorecardTemplateMaintenance.grid");
                    },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("ScorecardTemplateMaintenanceDetailController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "$timeout",
            "$window",
            "ScorecardTemplateService",
            "EnumService",
            "Popups",
            ScorecardTemplateMaintenanceDetailController
        ]);