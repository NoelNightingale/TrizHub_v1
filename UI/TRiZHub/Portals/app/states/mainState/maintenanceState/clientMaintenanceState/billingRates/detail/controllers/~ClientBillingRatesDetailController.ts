class ClientBillingRatesDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    userDropdown: any;
    clientId: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $state: ng.ui.IStateService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private UserService: UserServiceModule.UserService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.clientId = this.$stateParams["clientId"];
        this.viewModel.clientId = this.clientId;
        this.viewModel.projectId = null;
        this.viewModel.id = this.$stateParams["id"];

        UserService.userDropdownList()
            .then(
                result => {
                    self.userDropdown = result;
                },
                error => {
                    self.handleError(error);
                });

        if (this.viewModel.id !== "new") {
            this.BillingRatesService.billingRatesGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                        self.clientId = result.clientId;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.viewModel.id = null;
        }
    }

    //#endregion

    submitForm = () => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;

        this.viewModel.clientId = this.clientId;
        this.viewModel.projectId = null;

        this.BillingRatesService.billingRatesSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                            self.$state.go("mainState.maintenance.clientMaintenance.billingRatesGrid",
                                { "id": self.clientId });
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    cancelForm = () => {
        this.$state.go("mainState.maintenance.clientMaintenance.billingRatesGrid",
            { "id": this.clientId });
    };
}

angular.module("AngularApp")
    .controller("ClientBillingRatesDetailController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$state",
        "BillingRatesService",
        "UserService",
        "Popups",
        ClientBillingRatesDetailController
    ]);
