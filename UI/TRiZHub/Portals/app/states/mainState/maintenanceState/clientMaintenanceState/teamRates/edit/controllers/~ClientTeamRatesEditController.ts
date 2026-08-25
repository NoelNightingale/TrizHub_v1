class ClientTeamRatesEditController extends CHControllerBase {

    //#region Members

    clientId: string;
    userId: string;
    context: any;
    loading = false;
    saveSuccess = false;

    editingScope: string = null;
    editModel: any = null;
    editingProjectId: string = null;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.clientId = this.$stateParams["clientId"];
        this.userId = this.$stateParams["userId"];
        this.context = {
            clientRates: [],
            defaultRates: [],
            projectRateGroups: [],
            clientProjects: []
        };
        this.loadContext();
    }

    //#endregion

    loadContext = () => {
        const self = this;
        self.loading = true;
        self.BillingRatesService.userRatesForClientContext(self.userId, self.clientId)
            .then(
                result => {
                    self.context = result;
                    self.loading = false;
                },
                error => {
                    self.loading = false;
                    self.handleError(error);
                });
    };

    backToRoster = () => {
        this.$state.go("mainState.maintenance.clientMaintenance.teamRates",
            { id: this.clientId });
    };

    startAdd = (scope: string, projectId?: string) => {
        this.editingScope = scope;
        this.editingProjectId = projectId || null;
        this.editModel = {
            id: null,
            userAccountId: this.userId,
            rate: null,
            startDate: null,
            endDate: null,
            clientId: scope === "Client" ? this.clientId : null,
            projectId: scope === "Project" ? (projectId || null) : null
        };
    };

    startEdit = (scope: string, row: any, projectId?: string) => {
        this.editingScope = scope;
        this.editingProjectId = projectId || row.projectId || null;
        this.editModel = {
            id: row.id,
            userAccountId: this.userId,
            rate: row.rate,
            startDate: row.startDate,
            endDate: row.endDate,
            clientId: scope === "Client" ? this.clientId : null,
            projectId: scope === "Project" ? (projectId || row.projectId) : null
        };
    };

    cancelEdit = () => {
        this.editingScope = null;
        this.editingProjectId = null;
        this.editModel = null;
    };

    savePeriod = () => {
        const self = this;
        if (!self.editModel || !self.editModel.rate || !self.editModel.startDate || !self.editModel.endDate) {
            self.Popups.showError(self.$scope, "Rate, Start Date and End Date are required.");
            return;
        }

        if (self.editingScope === "Project") {
            if (!self.editModel.projectId) {
                self.Popups.showError(self.$scope, "Project is required for a project-specific rate.");
                return;
            }
            self.editModel.clientId = null;
        } else if (self.editingScope === "Client") {
            self.editModel.clientId = self.clientId;
            self.editModel.projectId = null;
        } else {
            self.editModel.clientId = null;
            self.editModel.projectId = null;
        }

        self.editModel.userAccountId = self.userId;

        self.BillingRatesService.billingRatesSave(self.editModel)
            .then(
                () => {
                    self.saveSuccess = true;
                    self.cancelEdit();
                    self.loadContext();
                    self.$timeout(() => { self.saveSuccess = false; }, 1500);
                },
                error => {
                    self.handleError(error);
                });
    };

    deletePeriod = (row: any) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
                "Are you sure you want to delete?",
                "You are about to delete this rate period...")
            .then(
                action => {
                    if (!action)
                        return;
                    self.BillingRatesService.billingRatesDelete({ id: row.id })
                        .then(
                            () => {
                                self.loadContext();
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
    .controller("ClientTeamRatesEditController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "$timeout",
        "BillingRatesService",
        "Popups",
        ClientTeamRatesEditController
    ]);
