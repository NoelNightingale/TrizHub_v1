
class ScorecardSubmitScorecardController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    currentUser: any;
    scorecardId: string;
    clientDropdown: any;
    userDropdown: any;

    filterOptions: any;
    filterModel: any;

    readOnly = false;
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
    summernoteOptionsDisabled = {
        height: 110,
        focus: false,
        airMode: false,
        shortcuts: false,
        toolbar: [],
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
        private ScorecardService: ScorecardServiceModule.ScorecardService,
        private EnumService: EnumServiceModule.EnumService,
        private UserService: UserServiceModule.UserService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.scorecardId = self.$stateParams["id"];
        self.readOnly = self.$stateParams["readOnly"] == 'true';
        self.viewModel = { isActive: true };
        self.currentUser = SecurityService.getCurrentUserDetails();
        self.filterOptions = { scorecardScoreTypes: EnumService.getScorecardScoreTypes() };

        ScorecardService.scorecardGet(self.scorecardId)
            .then(
                result => {
                    self.viewModel = result;

                    // Allow admin to edit any field an evaluator would
                    if (this.SecurityService.isAllowed("PerformanceManagementAdmin")) {
                        self.viewModel.scorecardModel.evaluatorId = self.currentUser.id;
                    }

                    // Disable evaluator comment if it is the employee that is logged in
                    if (self.currentUser.id == self.viewModel.scorecardModel.employeeId) {
                        self.$timeout(() => { $(".evaluatorComment .note-editable").attr("contenteditable", "false") }, 500);
                    }
                    else {
                        self.$timeout(() => { $(".employeeComment .note-editable").attr("contenteditable", "false") }, 500);
                    }
                },
                error => {
                    self.handleError(error);
                });

        if (self.readOnly == true) {
            self.$timeout(() => { $(".note-editable").attr("contenteditable", "false") }, 500);
        }
    }

    //#endregion

    /** E, A, I score is item weight times the rating weight. A manual entry is already the weighted score. */
    itemEarned = (item): number => {
        if (!item)
            return null;
        if (Number(item.scorecardScoring) === 1) {
            if (item.value === null || item.value === undefined || item.value === "")
                return null;
            const manual = Number(item.value);
            return isNaN(manual) ? null : manual;
        }
        const rating = this.ratingWeight(item.scoreType);
        if (rating === null)
            return null;
        return (Number(item.weight) || 0) * rating / 100;
    };

    totalEarned = (): number => {
        const items = this.scoreItems();
        let total = 0;
        for (let i = 0; i < items.length; i++) {
            const earned = this.itemEarned(items[i]);
            if (earned !== null)
                total += earned;
        }
        return total;
    };

    totalWeight = (): number => {
        const items = this.scoreItems();
        let total = 0;
        for (let i = 0; i < items.length; i++)
            total += Number(items[i].weight) || 0;
        return total;
    };

    formatScore = (value: number): string => {
        if (value === null || value === undefined || isNaN(value))
            return "\u2014";
        const rounded = Math.round(value * 100) / 100;
        if (Math.abs(rounded - Math.round(rounded)) < 0.001)
            return String(Math.round(rounded));
        return rounded.toFixed(2).replace(/0+$/, "").replace(/\.$/, "");
    };

    itemScoreLabel = (item): string => {
        const earned = this.itemEarned(item);
        const weight = Number(item && item.weight) || 0;
        return this.formatScore(earned) + " / " + this.formatScore(weight);
    };

    totalScoreLabel = (): string => {
        return this.formatScore(this.totalEarned()) + " / " + this.formatScore(this.totalWeight());
    };

    private scoreItems = (): any[] => {
        const records = this.viewModel && this.viewModel.scorecardRecordModels;
        return records || [];
    };

    /** Rating weights live on the template: E = 0, A = 1, I = 2. */
    private ratingWeight = (scoreType): number => {
        const card = this.viewModel && this.viewModel.scorecardModel;
        if (!card || scoreType === null || scoreType === undefined || scoreType === "")
            return null;
        const rating = Number(scoreType);
        if (rating === 0)
            return Number(card.excellentWeight) || 0;
        if (rating === 1)
            return Number(card.adequateWeight) || 0;
        if (rating === 2)
            return Number(card.inadequateWeight) || 0;
        return null;
    };

    showMeasureDefinition = (definition): any => {
        const self = this;
        self.Popups.scorecardDefinitionDailog(self.$scope, self.$timeout, "Definition", "Ok", definition)
            .then(
                action => {},
                error => {
                    self.handleError(error);
                });
        self.$timeout(() => { $(".modal-dialog .note-editable").attr("contenteditable", "false") }, 200);
    };

    submitForm = (complete): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.readOnly)
            return;
        self.viewModel.scorecardModel.completed = complete;
        self.ScorecardService.scorecardRecordSave(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    saveComment = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.readOnly)
            return;
        self.ScorecardService.scorecardCommmentSave(self.viewModel)
            .then(
            result => {
                    // Save scorecard employee comments
                    self.ScorecardService.scorecardRecordCommentSave(self.viewModel)
                        .then(
                            result => {
                                self.saveSuccess = true;
                                self.$timeout(() => { self.$state.transitionTo("mainState.scorecard.grid"); }, 1000);
                            },
                            error => {
                                self.handleError(error);
                            });
                },
                error => {
                    self.handleError(error);
                });
    };

}

angular.module("AngularApp")
    .controller("ScorecardSubmitScorecardController",
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
            ScorecardSubmitScorecardController
        ]);