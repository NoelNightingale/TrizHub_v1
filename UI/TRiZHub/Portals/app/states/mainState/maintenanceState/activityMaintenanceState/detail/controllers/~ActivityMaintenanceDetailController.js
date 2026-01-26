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
var ActivityMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ActivityMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ActivityMaintenanceDetailController($scope, $stateParams, $state, $timeout, $window, ActivityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ActivityService = ActivityService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.ActivityService.saveActivity(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.activityMaintenance.grid");
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.activityId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.activityId !== "new") {
            ActivityService.getActivity(self.activityId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.activityId = null;
        }
        return _this;
    }
    return ActivityMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ActivityMaintenanceDetailController", [
    "$scope",
    "$stateParams",
    "$state",
    "$timeout",
    "$window",
    "ActivityService",
    "Popups",
    ActivityMaintenanceDetailController
]);
//# sourceMappingURL=~ActivityMaintenanceDetailController.js.map