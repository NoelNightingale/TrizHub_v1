var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var BillingCycleMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(BillingCycleMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function BillingCycleMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, BillingCycleService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.BillingCycleService = BillingCycleService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.calckWeekDays = function () {
            var self = _this;
            if (!self.viewModel.startDate || !self.viewModel.endDate)
                return;
            // Calculate days between dates
            var millisecondsPerDay = 86400 * 1000; // Day in milliseconds
            console.log(self.viewModel.startDate);
            var startDate = new Date(self.viewModel.startDate);
            var endDate = new Date(self.viewModel.endDate);
            endDate.setHours(23, 59, 59, 999); // End just before midnight
            var weekdays = 0;
            while (startDate < endDate) {
                if (startDate.getDay() < 6 && startDate.getDay() > 0)
                    weekdays = weekdays + 1;
                startDate.setDate(startDate.getDate() + 1);
            }
            self.viewModel.weekdays = weekdays;
        };
        _this.calckWorkDays = function () {
            var self = _this;
            if (self.viewModel.weekdays && self.viewModel.weekdays > 0)
                self.viewModel.workDays = self.viewModel.weekdays - self.viewModel.publicHolidays;
        };
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            if (self.viewModel.startDate.getTime)
                self.viewModel.startDate = self.viewModel.startDate.getFullYear() + "-" + (self.viewModel.startDate.getMonth() + 1) + "-" + self.viewModel.startDate.getDate() + "T00:00:00";
            if (self.viewModel.endDate.getTime)
                self.viewModel.endDate = self.viewModel.endDate.getFullYear() + "-" + (self.viewModel.endDate.getMonth() + 1) + "-" + self.viewModel.endDate.getDate() + "T00:00:00";
            self.BillingCycleService.billingCycleSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.billingCycleMaintenance.grid");
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.billingCycleId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.billingCycleId !== "new") {
            BillingCycleService.billingCycleGet(self.billingCycleId)
                .then(function (result) {
                result.startDate = _this.fixTimezone(result.startDate);
                result.endDate = _this.fixTimezone(result.endDate);
                self.viewModel = result;
                console.log("Billing Cycle Record Loaded :");
                console.log(self.viewModel);
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.billingCycleId = null;
        }
        return _this;
    }
    //#endregion
    BillingCycleMaintenanceDetailController.prototype.fixTimezone = function (dateTime) {
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
    };
    return BillingCycleMaintenanceDetailController;
}(CHControllerBase));
;
angular.module("AngularApp")
    .controller("BillingCycleMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "BillingCycleService",
    "Popups",
    BillingCycleMaintenanceDetailController
]);
//# sourceMappingURL=~BillingCycleMaintenanceDetailController.js.map