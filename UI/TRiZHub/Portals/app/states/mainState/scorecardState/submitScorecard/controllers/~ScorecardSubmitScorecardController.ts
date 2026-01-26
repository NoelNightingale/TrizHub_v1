
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
                        self.$timeout(() => { $(".evaluatorComment .note-editable").attr("contenteditable", "false") }, 200);
                    }
                    // Disable employee comment if it is the evaluator that is logged in
                    else {
                        self.$timeout(() => { $(".employeeComment .note-editable").attr("contenteditable", "false") }, 200);
                    }
                },
                error => {
                    self.handleError(error);
                });

        if (self.readOnly == true) {
            self.$timeout(() => { $(".note-editable").attr("contenteditable", "false") }, 200);
        }
    }

    //#endregion

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
                    self.Popups.showError(self.$scope, error, "Error");
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
                                self.Popups.showError(self.$scope, error, "Error");
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