class ProjectTeamRatesEditController extends CHControllerBase {

    //#region Members

    projectId: string;
    userId: string;
    context: any;
    loading = false;
    saveSuccess = false;

    editingScope: string = null;
    editModel: any = null;

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
        this.projectId = this.$stateParams["projectId"];
        this.userId = this.$stateParams["userId"];
        this.context = {
            projectRates: [],
            clientRates: [],
            defaultRates: []
        };
        this.loadContext();
    }

    //#endregion

    loadContext = () => {
        const self = this;
        self.loading = true;
        self.BillingRatesService.userRatesForProjectContext(self.userId, self.projectId)
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
        this.$state.go("mainState.maintenance.projectMaintenance.teamRates",
            { id: this.projectId });
    };

    startAdd = (scope: string) => {
        this.editingScope = scope;
        this.editModel = {
            id: null,
            userAccountId: this.userId,
            rate: null,
            startDate: null,
            endDate: null,
            clientId: scope === "Client" ? this.context.clientId : null,
            projectId: scope === "Project" ? this.projectId : null
        };
    };

    startEdit = (scope: string, row: any) => {
        this.editingScope = scope;
        this.editModel = {
            id: row.id,
            userAccountId: this.userId,
            rate: row.rate,
            startDate: row.startDate,
            endDate: row.endDate,
            clientId: scope === "Client" ? this.context.clientId : null,
            projectId: scope === "Project" ? this.projectId : null
        };
    };

    cancelEdit = () => {
        this.editingScope = null;
        this.editModel = null;
    };

    savePeriod = () => {
        const self = this;
        if (!self.editModel || !self.editModel.rate || !self.editModel.startDate || !self.editModel.endDate) {
            self.Popups.showError(self.$scope, "Rate, Start Date and End Date are required.");
            return;
        }

        if (self.editingScope === "Project") {
            self.editModel.projectId = self.projectId;
            self.editModel.clientId = null;
        } else if (self.editingScope === "Client") {
            self.editModel.clientId = self.context.clientId;
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
    .controller("ProjectTeamRatesEditController",
    [
        "$scope",
        "$state",
        "$stateParams",
        "$timeout",
        "BillingRatesService",
        "Popups",
        ProjectTeamRatesEditController
    ]);
