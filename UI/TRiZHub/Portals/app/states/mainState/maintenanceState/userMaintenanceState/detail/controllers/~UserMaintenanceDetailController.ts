class UserMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    showProjects: any;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private UserService: UserServiceModule.UserService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private ReportService: ReportServiceModule.ReportService,
        private RoleService: RoleServiceModule.RoleService,

        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        this.viewModel = {};
        this.viewModel.id = this.$stateParams["id"];

        if (this.viewModel.id !== "new") {
            this.UserService.userGet(this.viewModel.id)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.viewModel.id = null;
            this.RoleService.rolePrivileges("")
                .then(
                    result => {
                        self.viewModel.permissions = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        }
    }


    //#endregion

    submitForm = () => {
        const self = this;
        this.$scope.$broadcast("show-errors-check-validity");
        if (this.$scope["EditForm"].$invalid)
            return;
        if (this.viewModel.id == null) {
            this.UserService.signUp(this.viewModel)
                .then(
                    result => {
                        self.saveSuccess = true;
                        self.$timeout(function() {
                                self.$state.go("mainState.maintenance.userMaintenance.detail", { "id": result.id });
                            },
                            1000);
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            this.UserService.userSave(this.viewModel)
                .then(
                    result => {
                        self.saveSuccess = true;
                        self.$timeout(function() {
                                self.$state.go("mainState.maintenance.userMaintenance.grid");
                            },
                            1000);
                    },
                    error => {
                        self.handleError(error);
                    });

        }
    };

    isAllowed = (privilegeType: string): boolean => {
        return this.SecurityService.isAllowed(privilegeType);
    };

    userSummary = (): void => {
        const self = this;

        self.$window.open(self.ReportService.reportApi() +
            "UserSummaryExcel?UserAccountId=" +
            this.viewModel.id +
            "&allUsers=false", 
            "_blank");
    };
};

angular.module("AngularApp")
    .controller("UserMaintenanceDetailController",
    [
        "$scope",
        "$stateParams",
        "$timeout",
        "$window",
        "$state",
        "UserService",
        "SecurityService",
        "ReportService",
        "RoleService",
        "Popups",
        UserMaintenanceDetailController
    ]);