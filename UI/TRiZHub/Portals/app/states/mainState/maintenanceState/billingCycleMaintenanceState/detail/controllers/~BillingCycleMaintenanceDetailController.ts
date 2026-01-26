class BillingCycleMaintenanceDetailController extends CHControllerBase {

    //#region members

    successMessage = "Saved Successfully";
    saveSuccess = false;
    viewModel: any;
    billingCycleId: string;

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.billingCycleId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.billingCycleId !== "new") {
            BillingCycleService.billingCycleGet(self.billingCycleId)
                .then(
                result => {
                    result.startDate = this.fixTimezone(result.startDate);
                    result.endDate = this.fixTimezone(result.endDate);
                    self.viewModel = result;
                    console.log("Billing Cycle Record Loaded :");
                    console.log(self.viewModel);
                },
                error => {
                    self.handleError(error);
                });
        } else {
            self.billingCycleId = null;
        }
    }

    //#endregion


    public fixTimezone(dateTime: string): string {
        // Get local timezone
        // getTimezoneOffset returns difference between UTC and Local Timezone so have to swap minus value 
        // +2:00 timezone will be returned as -120
        var timezone = (new Date().getTimezoneOffset() / 60) * -1;
        var timezoneString = ':00';

        // Check if single or double digit
        if (Math.abs(timezone) > 9) {
            if (timezone < 0)
                timezoneString = timezone.toString() + timezoneString;
            else
                timezoneString = '+' + timezone.toString() + timezoneString;
        }
        else {
            if (timezone < 0)
                timezoneString = '-0' + timezone.toString()[1] + timezoneString;
            else
                timezoneString = '+0' + timezone.toString() + timezoneString;
        }

        dateTime = dateTime + timezoneString;
        return dateTime;

    }


    calckWeekDays = (): any => {
        const self = this;
        if (!self.viewModel.startDate || !self.viewModel.endDate) return; 
        // Calculate days between dates
        var millisecondsPerDay = 86400 * 1000; // Day in milliseconds
        console.log(self.viewModel.startDate);
        var startDate = new Date(self.viewModel.startDate);
        var endDate = new Date(self.viewModel.endDate);
        endDate.setHours(23, 59, 59, 999);  // End just before midnight
        var weekdays = 0;
        while (startDate < endDate) {
            if (startDate.getDay() < 6 && startDate.getDay() > 0)
                weekdays = weekdays + 1;           
            startDate.setDate(startDate.getDate() + 1);
        }
        self.viewModel.weekdays = weekdays;
    };


    calckWorkDays = (): any => {
        const self = this;
        if (self.viewModel.weekdays && self.viewModel.weekdays > 0)
            self.viewModel.workDays = self.viewModel.weekdays - self.viewModel.publicHolidays;
    };


    submitForm = (): any => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        if (self.viewModel.startDate.getTime)
            self.viewModel.startDate = self.viewModel.startDate.getFullYear() + "-" + (self.viewModel.startDate.getMonth() + 1) + "-" + self.viewModel.startDate.getDate() + "T00:00:00";
        if (self.viewModel.endDate.getTime)
            self.viewModel.endDate = self.viewModel.endDate.getFullYear() + "-" + (self.viewModel.endDate.getMonth() + 1) + "-" + self.viewModel.endDate.getDate() + "T00:00:00";



        self.BillingCycleService.billingCycleSave(self.viewModel)
            .then(
            result => {
                self.saveSuccess = true;
                self.$timeout(() => {
                    self.$state.transitionTo("mainState.maintenance.billingCycleMaintenance.grid");
                },
                    1000);
            },
            error => {
                self.handleError(error);
            });

    };
};



angular.module("AngularApp")
    .controller("BillingCycleMaintenanceDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "$window",
        "BillingCycleService",
        "Popups",
        BillingCycleMaintenanceDetailController
    ]);
