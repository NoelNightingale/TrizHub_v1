class BillingRatesMaintenanceDetailController extends CHControllerBase {

    //#region members

    viewModel: any;
    scopeType = "Default";
    clientDropdown: any;
    projectDropdown: any;
    userDropdown: any;
    /** Client chosen to narrow the project list for Project rates; not saved. */
    projectClientFilter: string = null;
    projectClientOptions: any[] = [];
    filteredProjects: any[] = [];
    isNew = false;
    userLocked = false;
    saving = false;

    //#endregion

    //#region Ctor

    /**
     * Runs inside a $uibModal opened by BillingRatesMaintenanceGridController.
     * modalParams: { id: string | "new", userId?, scope?, clientId?, projectId? }
     * Closes with { action: "saved" | "deleted", id } or dismisses on cancel.
     */
    constructor(
        private $scope: ng.IScope,
        private modalParams: any,
        private $uibModalInstance: any,
        private $state: ng.ui.IStateService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private ClientService: ClientServiceModule.ClientService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private UserService: UserServiceModule.UserService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        const params = this.modalParams || {};
        this.viewModel = {};
        this.viewModel.id = params.id || "new";
        this.viewModel.userAccountId = params.userId || null;
        this.isNew = this.viewModel.id === "new";
        this.userLocked = !this.isNew && !!this.viewModel.userAccountId;

        UserService.userDropdownList()
            .then(
                result => {
                    self.userDropdown = result;
                },
                error => {
                    self.handleError(error);
                });

        ClientService.clientDropdownList()
            .then(
                result => {
                    self.clientDropdown = result;
                    self.refreshProjectOptions();
                },
                error => {
                    self.handleError(error);
                });

        ProjectService.projectDropdownList()
            .then(
                result => {
                    self.projectDropdown = result;
                    self.refreshProjectOptions();
                },
                error => {
                    self.handleError(error);
                });

        if (!this.isNew) {
            this.BillingRatesService.billingRatesGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                        self.scopeType = self.resolveScopeType(result);
                        self.userLocked = true;
                        self.projectClientFilter = result.projectClientId || null;
                        self.refreshProjectOptions();
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
        const params = this.modalParams || {};
        const scope = params.scope;
        const clientId = params.clientId;
        const projectId = params.projectId;

        if (scope === "Client" || (clientId && !projectId)) {
            this.scopeType = "Client";
            this.viewModel.clientId = clientId || null;
            this.viewModel.projectId = null;
        } else if (scope === "Project" || projectId) {
            this.scopeType = "Project";
            this.viewModel.projectId = projectId || null;
            this.viewModel.clientId = null;
            this.projectClientFilter = clientId || null;
        } else {
            this.scopeType = "Default";
            this.viewModel.clientId = null;
            this.viewModel.projectId = null;
        }
    };

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
            if (!this.viewModel.clientId && this.projectClientFilter) {
                this.viewModel.clientId = this.projectClientFilter;
            }
            this.viewModel.projectId = null;
        } else if (this.scopeType === "Project") {
            if (!this.projectClientFilter && this.viewModel.clientId) {
                this.projectClientFilter = this.viewModel.clientId;
                this.refreshProjectOptions();
            }
            this.viewModel.clientId = null;
        }
    };

    refreshProjectOptions = () => {
        const projects: any[] = this.projectDropdown || [];
        const clients: any[] = this.clientDropdown || [];

        if (!this.projectClientFilter && this.viewModel.projectId) {
            const current = projects.filter(p => p.id === this.viewModel.projectId)[0];
            if (current) {
                this.projectClientFilter = current.clientId;
            }
        }

        const clientHasProjects: any = {};
        projects.forEach(p => { clientHasProjects[p.clientId] = true; });
        this.projectClientOptions = clients.filter(c => !!clientHasProjects[c.id]);

        const clientId = this.projectClientFilter;
        this.filteredProjects = clientId ? projects.filter(p => p.clientId === clientId) : [];
    };

    onProjectClientChanged = () => {
        const projects: any[] = this.projectDropdown || [];
        const current = projects.filter(p => p.id === this.viewModel.projectId)[0];
        if (!current || current.clientId !== this.projectClientFilter) {
            this.viewModel.projectId = null;
        }
        this.refreshProjectOptions();
    };

    cancel = () => {
        this.$uibModalInstance.dismiss("cancel");
    };

    submitForm = () => {
        const self = this;
        if (this.viewModel.isLocked || this.saving)
            return;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;

        this.onScopeChanged();

        this.saving = true;
        this.BillingRatesService.billingRatesSave(this.viewModel)
            .then(
                result => {
                    self.saving = false;
                    self.$uibModalInstance.close({ action: "saved", id: result.id });
                },
                error => {
                    self.saving = false;
                    self.handleError(error);
                });
    };

    deleteRecord = () => {
        const self = this;

        if (self.isNew || !self.viewModel?.id || self.viewModel.isLocked) {
            return;
        }

        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this record...")
            .then(
                action => {
                    if (!action) {
                        return;
                    }

                    self.BillingRatesService.billingRatesDelete(self.viewModel)
                        .then(
                            result => {
                                self.$uibModalInstance.close({ action: "deleted", id: self.viewModel.id });
                            },
                            error => {
                                self.handleError(error);
                            });
                },
                error => {
                    self.handleError(error);
                });
    };

    //#endregion
}

angular.module("AngularApp")
    .controller("BillingRatesMaintenanceDetailController",
    [
        "$scope",
        "modalParams",
        "$uibModalInstance",
        "$state",
        "BillingRatesService",
        "ClientService",
        "ProjectService",
        "UserService",
        "Popups",
        BillingRatesMaintenanceDetailController
    ]);
