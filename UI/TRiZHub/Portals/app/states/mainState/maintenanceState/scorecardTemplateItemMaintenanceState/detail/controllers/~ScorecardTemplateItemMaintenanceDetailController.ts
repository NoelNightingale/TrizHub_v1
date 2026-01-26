
class ScorecardTemplateItemMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scorecardTemplateId: string;
    scorecardTemplateItemId: string;
    clientDropdown: any;
    userDropdown: any;

    filterOptions: any;

    summernoteOptions = {
        height: 110,
        focus: false,
        airMode: false,
        shortcuts: true,
        toolbar: [
            ['style', ['bold', 'italic', 'underline']],
            ['textsize', ['fontsize']],
            ['fontclr', ['color']],
            ['alignment', ['ul', 'ol', 'paragraph', 'lineheight']],
        ],
        disableDragAndDrop: true
    };

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
        self.viewModel = {};

        self.filterOptions = { maxWeight: 0 };
        if (self.scorecardTemplateItemId !== "new") {
            ScorecardTemplateService.scorecardTemplateItemGet(self.scorecardTemplateItemId)
                .then(
                    result => {
                        self.viewModel = result;
                        ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                            .then(
                                results => {
                                    self.filterOptions.maxWeight = results.totalAvailableWeight;
                                    self.viewModel.scorecardName = results.scorecardName;
                                    self.checkSliderWeightValue(true);
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
            self.viewModel.scorecardScoring = 0;
            ScorecardTemplateService.scorecardTemplateGet(self.scorecardTemplateId)
                .then(
                    results => {
                        self.filterOptions.maxWeight = results.totalAvailableWeight;
                        self.viewModel.scorecardName = results.scorecardName;
                        self.checkSliderWeightValue(false);
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    }

    percentageFormatting = (value): string => {
        return value.toString() + "%";
    };
    checkSliderWeightValue = (existing) => {
        const self = this;
        if (existing)
            self.filterOptions.maxWeight += self.viewModel.weight;
    };

    //#endregion

    cancelForm = (): any => {
        const self = this;
        self.$state.transitionTo("mainState.maintenance.scorecardTemplateItemMaintenance.grid",
            { "scorecardTemplateId": self.scorecardTemplateId });
    };

    submitForm = (): any => {
        const self = this;

        if (self.viewModel.weight > 0) {
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.viewModel.scorecardTemplateId = self.scorecardTemplateId;
            self.ScorecardTemplateService.scorecardTemplateItemSave(self.viewModel)
                .then(
                    result => {
                        self.saveSuccess = true;
                        self.$timeout(() => {
                            self.$state.transitionTo("mainState.maintenance.scorecardTemplateItemMaintenance.grid",
                                { "scorecardTemplateId": self.scorecardTemplateId });
                        },
                            1000);
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {

            self.Popups.showError(self.$scope, "Cannot add item with 0% weight", "Error");
            self.handleError("Cannot add item with 0% weight");
        }

    };
}

angular.module("AngularApp")
    .controller("ScorecardTemplateItemMaintenanceDetailController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "$timeout",
            "$window",
            "ScorecardTemplateService",
            "EnumService",
            "Popups",
            ScorecardTemplateItemMaintenanceDetailController
        ]);