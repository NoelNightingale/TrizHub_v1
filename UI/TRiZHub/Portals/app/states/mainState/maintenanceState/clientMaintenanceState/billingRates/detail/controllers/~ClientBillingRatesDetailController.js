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
var ClientBillingRatesDetailController = /** @class */ (function (_super) {
    __extends(ClientBillingRatesDetailController, _super);
    //#endregion
    //#region Ctor
    function ClientBillingRatesDetailController($scope, $stateParams, $timeout, $state, BillingRatesService, UserService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$state = $state;
        _this.BillingRatesService = BillingRatesService;
        _this.UserService = UserService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            _this.viewModel.clientId = _this.clientId;
            _this.viewModel.projectId = null;
            _this.BillingRatesService.billingRatesSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.go("mainState.maintenance.clientMaintenance.billingRatesGrid", { "id": self.clientId });
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.cancelForm = function () {
            _this.$state.go("mainState.maintenance.clientMaintenance.billingRatesGrid", { "id": _this.clientId });
        };
        var self = _this;
        _this.viewModel = {};
        _this.clientId = _this.$stateParams["clientId"];
        _this.viewModel.clientId = _this.clientId;
        _this.viewModel.projectId = null;
        _this.viewModel.id = _this.$stateParams["id"];
        UserService.userDropdownList()
            .then(function (result) {
            self.userDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        if (_this.viewModel.id !== "new") {
            _this.BillingRatesService.billingRatesGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
                self.clientId = result.clientId;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.viewModel.id = null;
        }
        return _this;
    }
    return ClientBillingRatesDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientBillingRatesDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$state",
    "BillingRatesService",
    "UserService",
    "Popups",
    ClientBillingRatesDetailController
]);
//# sourceMappingURL=~ClientBillingRatesDetailController.js.map