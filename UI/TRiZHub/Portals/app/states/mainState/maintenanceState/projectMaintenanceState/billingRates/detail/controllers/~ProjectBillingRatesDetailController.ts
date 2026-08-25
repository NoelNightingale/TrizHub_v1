class ProjectBillingRatesDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    userDropdown: any;
    projectId: any;

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
        this.projectId = this.$stateParams["projectId"];
        this.viewModel.projectId = this.projectId;
        this.viewModel.clientId = null;
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
                        self.projectId = result.projectId;
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

        this.viewModel.projectId = this.projectId;
        this.viewModel.clientId = null;

        this.BillingRatesService.billingRatesSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                            self.$state.go("mainState.maintenance.projectMaintenance.billingRatesGrid",
                                { "id": self.projectId });
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    cancelForm = () => {
        this.$state.go("mainState.maintenance.projectMaintenance.billingRatesGrid",
            { "id": this.projectId });
    };
}

angular.module("AngularApp")
    .controller("ProjectBillingRatesDetailController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$state",
        "BillingRatesService",
        "UserService",
        "Popups",
        ProjectBillingRatesDetailController
    ]);
