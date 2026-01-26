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
var EmployerMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(EmployerMaintenanceDetailController, _super);
    //userDropdown: any;
    //clientDropdown: any;
    //employedBy: any;
    //#endregion
    //#region Ctor
    function EmployerMaintenanceDetailController($scope, $stateParams, $timeout, $window, $state, EmployerService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.EmployerService = EmployerService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.EmployerService.employerSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$state.transitionTo("mainState.maintenance.employerMaintenance.grid");
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        if (_this.viewModel.id !== "new") {
            _this.EmployerService.employerGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.viewModel.id = null;
            _this.viewModel.isActive = true;
        }
        return _this;
    }
    return EmployerMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("EmployerMaintenanceDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "EmployerService",
    "Popups",
    EmployerMaintenanceDetailController
]);
//# sourceMappingURL=~EmployerMaintenanceDetailController.js.map