class BillingRatesDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    scopeType = "Default";
    clientDropdown: any;
    projectDropdown: any;

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
            this.viewModel.projectId = null;
        } else if (this.scopeType === "Project") {
            this.viewModel.clientId = null;
        }
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
