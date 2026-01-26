
class ScorecardDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scorecardPeriodId: string;
    clientDropdown: any;
    userDropdown: any;

    filterOptions: any;
    filterModel: any;

    readOnly = false;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private ScorecardService: ScorecardServiceModule.ScorecardService,
        private EnumService: EnumServiceModule.EnumService,
        private UserService: UserServiceModule.UserService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.scorecardPeriodId = self.$stateParams["id"];
        self.SecurityService = SecurityService;
        self.viewModel = {
            isActive: true,
            variableStart: new Date(),
            variableEnd: new Date(),
            variableYear: new Date().getFullYear()
        };

        // Set variable ende date to 4 weeks from now
        let fourWeeks = 28;
        var variableEnd = self.viewModel.variableEnd;
        variableEnd = variableEnd.setDate((variableEnd.getDate() + fourWeeks));
        self.viewModel.variableEnd = new Date(variableEnd);

        self.filterOptions = {
            evaluators: [],
            employees: [],
            scorecardTemplatePeriods: [],
            variablePeriod: false
        };

        UserService.userScorecardEvaluatorFilterDropdown()
            .then(
                result => {
                    self.filterOptions.evaluators = result;
                },
                error => {
                    self.handleError(error);
                });
        UserService.userScorecardEmployeeFilterDropdown()
            .then(
                result => {
                    self.filterOptions.employees = result;
                },
                error => {
                    self.handleError(error);
                });
        ScorecardTemplateService.scorecardTemplateDropdownList()
            .then(
                result => {
                    self.filterOptions.scorecardTemplates = result;
                },
                error => {
                    self.handleError(error);
                });
        if (self.scorecardPeriodId !== "new") {
            ScorecardService.scorecardGet(self.scorecardPeriodId)
                .then(
                    result => {
                        self.viewModel = result;
                        self.changedScorecardTemplate();
                        if (self.viewModel.employeeId === self.SecurityService.getCurrentUserDetails().id) {
                            self.readOnly = true;
                        }
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.scorecardPeriodId = null;
        }
    }

    //#endregion

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;
        if (self.readOnly)
            return;

        self.viewModel.variableYear = self.viewModel.variableStart.getFullYear();

        self.ScorecardService.scorecardSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    changedScorecardTemplate = (): void => {
        const self = this;
        self.viewModel.createdBy = self.SecurityService.getCurrentUserDetails().id;
        self.viewModel.DateCreated = new Date();
        self.ScorecardTemplateService.scorecardTemplatePeriodDropdownList(self.viewModel.scorecardTemplateId)
            .then(
                result => {
                    self.filterOptions.scorecardTemplatePeriods = result;
                },
                error => {
                    self.handleError(error);
                });
    };

    changedScorecardTemplatePeriod = (): void => {
        for (var i = 0; i < this.filterOptions.scorecardTemplatePeriods.length; i++) {
            if (this.viewModel.scorecardTemplatePeriodId == this.filterOptions.scorecardTemplatePeriods[i].id) {
                this.filterOptions.variablePeriod = this.filterOptions.scorecardTemplatePeriods[i].isVariable;
                return;
            }
        }
    };
}

angular.module("AngularApp")
    .controller("ScorecardDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "$window",
        "ScorecardTemplateService",
        "ScorecardService",
        "EnumService",
        "UserService",
        "SecurityService",
        "Popups",
        ScorecardDetailController
    ]);