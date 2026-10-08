class ProjectTeamRatesRosterController extends CHControllerBase {

    //#region Members

    projectId: string;
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
        this.projectId = this.$stateParams["id"];
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

    loadPeriods = () => {
        const self = this;
        self.periodsLoading = true;
        self.BillingRatesService.billingRatesGrid(<any>{
                projectId: self.projectId,
                scope: "Project",
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
