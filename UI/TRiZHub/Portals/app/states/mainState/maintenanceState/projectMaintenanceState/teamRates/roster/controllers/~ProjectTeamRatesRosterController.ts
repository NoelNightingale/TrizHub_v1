class ProjectTeamRatesRosterController extends CHControllerBase {

    //#region Members

    projectId: string;
    asOfDate: any;
    viewModel: any;
    team: any[];
    loading = false;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.projectId = this.$stateParams["id"];
        this.asOfDate = new Date();
        this.viewModel = {};
        this.team = [];
        this.loadTeam();
    }

    //#endregion

    loadTeam = () => {
        const self = this;
        self.loading = true;
        self.BillingRatesService.projectTeamRates(self.projectId, self.asOfDate)
            .then(
                result => {
                    self.viewModel = result;
                    self.team = result.team || [];
                    self.loading = false;
                },
                error => {
                    self.loading = false;
                    self.handleError(error);
                });
    };

    formatRate = (rate: any): string => {
        if (rate === null || rate === undefined)
            return "—";
        return rate;
    };

    editRates = (row: any) => {
        this.$state.go("mainState.maintenance.projectMaintenance.teamRatesEdit",
            { projectId: this.projectId, userId: row.userAccountId });
    };
}

angular.module("AngularApp")
    .controller("ProjectTeamRatesRosterController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "BillingRatesService",
        "Popups",
        ProjectTeamRatesRosterController
    ]);
