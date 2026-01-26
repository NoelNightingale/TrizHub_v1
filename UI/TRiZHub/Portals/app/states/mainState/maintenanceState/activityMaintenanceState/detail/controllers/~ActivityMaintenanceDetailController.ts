class ActivityMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    activityId: string;

    //#endregion

    //#region Ctor
    constructor(
        
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private ActivityService: ActivityServiceModule.ActivityService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.activityId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.activityId !== "new") {
            ActivityService.getActivity(self.activityId)
                .then(
                    result => {
                        self.viewModel = result;
                    },
                    error => {
                        self.handleError(error);
                    });
        } else {
            self.activityId = null;

        }
        
    }

    //#endregion

    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        self.ActivityService.saveActivity(self.viewModel)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(() => {
                            self.$state.transitionTo("mainState.maintenance.activityMaintenance.grid");
                        },
                        1000);
                },
                error => {
                    self.handleError(error);
                });
    };

}

angular.module("AngularApp")
    .controller("ActivityMaintenanceDetailController",
    [
       
        "$scope",
        "$stateParams",
        "$state",
        "$timeout",
        "$window",
        "ActivityService",
        "Popups",
        ActivityMaintenanceDetailController
    ]);
