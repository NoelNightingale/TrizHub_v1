class BillingRatesDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scopeType = "Default";
    clientDropdown: any;
    projectDropdown: any;
    filteredProjectDropdown: any[] = [];
    // UI-only: Project-scope rates are saved with ClientId null.
    projectClientFilterId: string = null;
    // Clients offered as the Project-scope filter; the full client list outside team mode.
    projectClientDropdown: any;
    // Set when opened from a work team: only that team's clients/projects, and no employee (default) rates.
    workTeamId: string = null;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private ClientService: ClientServiceModule.ClientService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.userAccountId = this.$stateParams["userid"];
        this.viewModel.id = this.$stateParams["id"];
        this.workTeamId = this.$stateParams["workTeamId"] || null;

        if (this.workTeamId) {
            this.loadTeamOptions();
        } else {
            ClientService.clientDropdownList()
                .then(
                    result => {
                        self.clientDropdown = result;
                        self.projectClientDropdown = result;
                    },
                    error => {
                        self.handleError(error);
                    });

            ProjectService.projectDropdownList()
                .then(
                    result => {
                        self.projectDropdown = result;
                        self.syncProjectClientFilter();
                    },
                    error => {
                        self.handleError(error);
                    });
        }

        if (this.viewModel.id !== "new") {
            this.BillingRatesService.billingRatesGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                        self.scopeType = self.resolveScopeType(result);
                        self.syncProjectClientFilter();
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.viewModel.id = null;
            this.applyNewPrefill();
        }
    }

    /** Team mode: the client/project options come from the team's allocations. */
    loadTeamOptions = () => {
        const self = this;
        self.BillingRatesService.workTeamMemberRates(self.workTeamId, self.viewModel.userAccountId)
            .then(
                result => {
                    self.clientDropdown = (result.clients || []).map(c => ({ id: c.id, entityName: c.name }));
                    self.projectDropdown = (result.projects || []).map(p => ({ id: p.id, description: p.name, clientId: p.clientId }));
                    const seen = {};
                    self.projectClientDropdown = [];
                    (result.projects || []).forEach(p => {
                        if (!seen[p.clientId]) {
                            seen[p.clientId] = true;
                            self.projectClientDropdown.push({ id: p.clientId, entityName: p.clientName });
                        }
                    });
                    self.projectClientDropdown.sort((a, b) => (a.entityName || "").localeCompare(b.entityName || ""));
                    self.syncProjectClientFilter();
                },
                error => {
                    self.handleError(error);
                });
    };

    goBack = () => {
        if (this.workTeamId) {
            this.$state.go("mainState.maintenance.workTeamMaintenance.detail",
                { id: this.workTeamId, tab: "people", ratesFor: this.viewModel.userAccountId });
            return;
        }
        this.$state.go("mainState.maintenance.userMaintenance.billingRatesGrid", { id: this.viewModel.userAccountId });
    };

    applyNewPrefill = () => {
        const scope = this.$stateParams["scope"];
        const clientId = this.$stateParams["clientId"];
        const projectId = this.$stateParams["projectId"];

        if (scope === "Client" || (clientId && !projectId)) {
            this.scopeType = "Client";
            this.viewModel.clientId = clientId || null;
            this.viewModel.projectId = null;
        } else if (scope === "Project" || projectId) {
            this.scopeType = "Project";
            this.viewModel.projectId = projectId || null;
            this.viewModel.clientId = null;
        } else if (this.workTeamId) {
            this.scopeType = "Client";
            this.viewModel.clientId = null;
            this.viewModel.projectId = null;
        } else {
            this.scopeType = "Default";
            this.viewModel.clientId = null;
            this.viewModel.projectId = null;
        }
    };

   //#endregion

    resolveScopeType = (model: any): string => {
        if (model.projectId)
            return "Project";
        if (model.clientId)
            return "Client";
        return "Default";
    };

    onScopeChanged = () => {
        if (this.scopeType === "Default") {
            this.viewModel.clientId = null;
            this.viewModel.projectId = null;
        } else if (this.scopeType === "Client") {
            if (!this.viewModel.clientId && this.projectClientFilterId)
                this.viewModel.clientId = this.projectClientFilterId;
            this.viewModel.projectId = null;
        } else if (this.scopeType === "Project") {
            if (this.viewModel.clientId && !this.projectClientFilterId)
                this.projectClientFilterId = this.viewModel.clientId;
            this.viewModel.clientId = null;
            this.applyProjectClientFilter();
        }
    };

    syncProjectClientFilter = () => {
        if (!this.projectClientFilterId && this.viewModel.projectClientId)
            this.projectClientFilterId = this.viewModel.projectClientId;
        if (!this.projectClientFilterId && this.projectDropdown && this.viewModel.projectId) {
            const project = this.findProject(this.viewModel.projectId);
            if (project && project.clientId)
                this.projectClientFilterId = project.clientId;
        }
        this.applyProjectClientFilter();
    };

    applyProjectClientFilter = () => {
        const all: any[] = this.projectDropdown || [];
        const clientId = this.projectClientFilterId;
        this.filteredProjectDropdown = clientId
            ? all.filter(p => p.clientId === clientId)
            : [];

        // Leave projects that aren't in the dropdown at all (e.g. inactive) untouched.
        const selected = this.findProject(this.viewModel.projectId);
        if (selected && this.filteredProjectDropdown.indexOf(selected) < 0)
            this.viewModel.projectId = null;
    };

    findProject = (projectId: string): any => {
        if (!projectId || !this.projectDropdown)
            return null;
        const matches = (this.projectDropdown as any[]).filter(p => p.id === projectId);
        return matches.length ? matches[0] : null;
    };

    submitForm = () => {
        const self = this;
        if (this.viewModel.isLocked)
            return;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;

        this.onScopeChanged();

        this.BillingRatesService.billingRatesSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function() {
                            self.goBack();
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

}

angular.module("AngularApp")
    .controller("BillingRatesDetailController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "BillingRatesService",
        "ClientService",
        "ProjectService",
        "Popups",
        BillingRatesDetailController
    ]);
