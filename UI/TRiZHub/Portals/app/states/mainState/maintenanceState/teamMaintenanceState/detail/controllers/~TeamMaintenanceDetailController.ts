class TeamMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    teamId: string;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private teamService: TeamServiceModule.TeamService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.teamId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.teamId !== "new") {
            teamService.getTeam(self.teamId)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.teamId = null;

        };
        
    }

    //#endregion

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        self.teamService.saveTeam(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => {
                            self.$state.transitionTo("mainState.maintenance.teamMaintenance.grid");
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

}

angular.module("AngularApp")
    .controller("TeamMaintenanceDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "$window",
        "TeamService",
        "Popups",
        TeamMaintenanceDetailController
    ]);

