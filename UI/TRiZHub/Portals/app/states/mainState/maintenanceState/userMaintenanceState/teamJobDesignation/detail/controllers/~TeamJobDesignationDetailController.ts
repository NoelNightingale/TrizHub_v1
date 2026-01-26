class TeamJobDesignationDetailController extends CHControllerBase {
    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    userDropdown: any;
    clientDropdown: any;
    employedBy: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private UserService: UserServiceModule.UserService,
        private ClientService: ClientServiceModule.ClientService,
        private EmployerService: EmployerServiceModule.EmployerService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.employedBy = []; // = String[2] = ["Triz SA", "Triz USA"];
        this.viewModel.userAccountId = this.$stateParams["userid"];
        this.viewModel.id = this.$stateParams["id"];
        UserService.userDropdownList()
            .then(
                result => {
                    self.userDropdown = result;
                    self.userDropdown.splice(0, 0, { id: null, description: "N/A" });
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

        EmployerService.employerDropdownList()
            .then(
                result => {
                    self.employedBy = result;
                },
                error => {
                    self.handleError(error);
                });

        if (this.viewModel.id !== "new") {
            this.UserService.teamJobDesignationGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
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

        // Set location
        for (var i = 0; i < this.employedBy.length; i++) {
            if (this.employedBy[i].id == this.viewModel.employerId) {
                this.viewModel.location = this.employedBy[i].name
                break;
            }
        }

        if (this.viewModel.startDate)
            this.viewModel.startDate = this.getBasic(this.viewModel.startDate);

        if (this.viewModel.endDate)
            this.viewModel.endDate = this.getBasic(this.viewModel.endDate);

        this.UserService.teamJobDesignationSave(this.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                        self.$state.go("mainState.maintenance.userMaintenance.teamJobDesignationGrid",
                            { "id": result.userAccountId });
                    },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

    getBasic = (date) => {

        let dateFormat = new Date(date);
        dateFormat.setMinutes(dateFormat.getMinutes() - dateFormat.getTimezoneOffset())
        return dateFormat.toUTCString();
    }
}

angular.module("AngularApp")
    .controller("TeamJobDesignationDetailController",
        [
            "$scope",
            "$stateParams",
            "$timeout",
            "$window",
            "$state",
            "UserService",
            "ClientService",
            "EmployerService",
            "Popups",
            TeamJobDesignationDetailController
        ]);