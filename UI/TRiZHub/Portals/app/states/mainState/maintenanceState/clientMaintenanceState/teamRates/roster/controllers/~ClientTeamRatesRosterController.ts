class ClientTeamRatesRosterController extends CHControllerBase {

    //#region Members

    clientId: string;
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
        this.clientId = this.$stateParams["id"];
        this.asOfDate = new Date();
        this.viewModel = {};
        this.team = [];
        this.loadTeam();
    }

    //#endregion

    loadTeam = () => {
        const self = this;
        self.loading = true;
        self.BillingRatesService.clientTeamRates(self.clientId, self.asOfDate)
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

    projectOverridesLabel = (count: number): string => {
        if (!count || count <= 0)
            return "—";
        return count === 1 ? "1 project" : (count + " projects");
    };

    editRates = (row: any) => {
        this.$state.go("mainState.maintenance.clientMaintenance.teamRatesEdit",
            { clientId: this.clientId, userId: row.userAccountId });
    };
}

angular.module("AngularApp")
    .controller("ClientTeamRatesRosterController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "BillingRatesService",
        "Popups",
        ClientTeamRatesRosterController
    ]);
