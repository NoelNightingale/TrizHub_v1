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

        ClientService.clientDropdownList()
            .then(
                result => {
                    self.clientDropdown = result;
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
                            self.$state.go("mainState.maintenance.userMaintenance.billingRatesGrid",
                            { "id": result.userAccountId });
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
