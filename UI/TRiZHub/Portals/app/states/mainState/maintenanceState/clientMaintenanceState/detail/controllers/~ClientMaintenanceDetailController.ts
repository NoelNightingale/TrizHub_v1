class ClientMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    userDropdown: any;
    clientReporters: any;
    selectedReporter: any;
    clientId: string;
    clientTypes: any;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private ClientService: ClientServiceModule.ClientService,
        private UserService: UserServiceModule.UserService,
        private EnumService: EnumServiceModule.EnumService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.clientId = self.$stateParams["id"];
        self.viewModel = {};

        if (self.clientId !== "new") {
            ClientService.clientGet(self.clientId)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });

            ClientService.getClientReporters(self.clientId)
                .then(
                result => {
                    self.clientReporters = result;
                    console.log("Client Reporters : %1", result);
                },
                error => {
                    self.handleError(error);
                });

            UserService.userDropdownList()
                .then(
                result => {
                    self.userDropdown = result;
                },
                error => {
                    self.handleError(error);
                });

        } else {
            self.clientId = null;
        }
    }

    addReporter = (): any => {
        const self = this;

        console.log(self.selectedReporter);
        if (!self.selectedReporter) return;
        self.ClientService.addClientReporter(self.clientId, self.selectedReporter)
            .then(
            result => {
                self.clientReporters = result;
                self.saveSuccess = true;
                self.$timeout(() => {
                    self.saveSuccess = false;
                },
                    1000);
            },
            error => {
                self.handleError(error);
            });
    }


    removeReporter = (userId): any => {
        const self = this;

        console.log("Remove of user : " + userId);
        self.ClientService.removeClientReporter(self.clientId, userId)
            .then(
            result => {
                self.clientReporters = result;
                self.saveSuccess = true;
                self.$timeout(() => {
                    self.saveSuccess = false;
                },
                    1000);
            },
            error => {
                self.handleError(error);
            });
    }


    //#endregion
    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

            self.ClientService.clientSave(self.viewModel)
                .then(
                    result => {
                        self.saveSuccess = true;
                        self.$timeout(() => {
                                self.$state.transitionTo("mainState.maintenance.clientMaintenance.grid");
                            },
                            1000);
                    },
                    error => {
                        self.handleError(error);
                    });

    };

    isAllowed = (privilegeType: string): boolean => {
        return this.SecurityService.isAllowed(privilegeType);
    };
}

angular.module("AngularApp")
    .controller("ClientMaintenanceDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "$window",
        "ClientService",
        "UserService",
        "EnumService",
        "SecurityService",
        "Popups",
        ClientMaintenanceDetailController
    ]);