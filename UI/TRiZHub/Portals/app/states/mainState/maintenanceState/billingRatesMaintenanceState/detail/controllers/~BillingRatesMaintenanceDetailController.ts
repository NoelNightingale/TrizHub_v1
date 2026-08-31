class BillingRatesMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scopeType = "Default";
    clientDropdown: any;
    projectDropdown: any;
    userDropdown: any;
    isNew = false;
    userLocked = false;

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
        private UserService: UserServiceModule.UserService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];
        this.viewModel.userAccountId = this.$stateParams["userId"] || null;
        this.isNew = this.viewModel.id === "new";
        this.userLocked = !this.isNew && !!this.viewModel.userAccountId;

        UserService.userDropdownList()
            .then(
                result => {
                    self.userDropdown = result;
                    if (self.userLocked) {
                        // Dropdown will still render the selected value even if it's not in list.
                    }
                },
                error => {
                    self.handleError(error);
                });

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
            this.viewModel.projectId = null;
        } else if (this.scopeType === "Project") {
            this.viewModel.clientId = null;
        }
    };

    cancel = () => {
        this.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
    };

    submitForm = () => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;

        this.onScopeChanged();

        this.BillingRatesService.billingRatesSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                            self.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    deleteRecord = () => {
        const self = this;

        if (self.isNew || !self.viewModel?.id) {
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
                                self.saveSuccess = false;
                                self.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
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
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "BillingRatesService",
        "ClientService",
        "ProjectService",
        "UserService",
        "Popups",
        BillingRatesMaintenanceDetailController
    ]);

