class ClientTeamRatesRosterController extends CHControllerBase {

    //#region Members

    clientId: string;
    asOfDate: any;
    viewModel: any;
    team: any[];
    loading = false;

    viewMode: string = "asOf"; // "periods" | "asOf"
    periodGroups: any[] = [];
    periodsLoading = false;

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

    setViewMode = (mode: string) => {
        this.viewMode = mode;
        if (mode === "periods") {
            this.loadPeriods();
        } else {
            this.loadTeam();
        }
    };

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

    loadPeriods = () => {
        const self = this;
        self.periodsLoading = true;
        self.BillingRatesService.billingRatesGrid(<any>{
                clientId: self.clientId,
                scope: "Client",
                sortKey: "user",
                sortOrder: "ASC",
                currentPage: 1,
                recordsPerPage: 10000
            })
            .then(
                result => {
                    self.periodGroups = self.groupByUser((<any>result).results || []);
                    self.periodsLoading = false;
                },
                error => {
                    self.periodsLoading = false;
                    self.handleError(error);
                });
    };

    groupByUser = (rows: any[]): any[] => {
        const groups: any[] = [];
        const byUser: { [id: string]: any } = {};
        rows.forEach(r => {
            let group = byUser[r.userAccountId];
            if (!group) {
                group = { userAccountId: r.userAccountId, userName: r.userName, rates: [] };
                byUser[r.userAccountId] = group;
                groups.push(group);
            }
            group.rates.push(r);
        });
        groups.forEach(g => g.rates.sort((a, b) =>
            new Date(a.startDate).getTime() - new Date(b.startDate).getTime()));
        return groups;
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
