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
var BillingRatesDetailController = /** @class */ (function (_super) {
    __extends(BillingRatesDetailController, _super);
    //#endregion
    //#region Ctor
    function BillingRatesDetailController($scope, $stateParams, $timeout, $window, $state, BillingRatesService, ClientService, ProjectService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.BillingRatesService = BillingRatesService;
        _this.ClientService = ClientService;
        _this.ProjectService = ProjectService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.scopeType = "Default";
        //#endregion
        _this.resolveScopeType = function (model) {
            if (model.projectId)
                return "Project";
            if (model.clientId)
                return "Client";
            return "Default";
        };
        _this.onScopeChanged = function () {
            if (_this.scopeType === "Default") {
                _this.viewModel.clientId = null;
                _this.viewModel.projectId = null;
            }
            else if (_this.scopeType === "Client") {
                _this.viewModel.projectId = null;
            }
            else if (_this.scopeType === "Project") {
                _this.viewModel.clientId = null;
            }
        };
        _this.submitForm = function () {
            var self = _this;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            _this.onScopeChanged();
            _this.BillingRatesService.billingRatesSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.go("mainState.maintenance.userMaintenance.billingRatesGrid", { "id": result.userAccountId });
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.applyNewPrefill = function () {
            var scope = _this.$stateParams["scope"];
            var clientId = _this.$stateParams["clientId"];
            var projectId = _this.$stateParams["projectId"];
            if (scope === "Client" || (clientId && !projectId)) {
                _this.scopeType = "Client";
                _this.viewModel.clientId = clientId || null;
                _this.viewModel.projectId = null;
            }
            else if (scope === "Project" || projectId) {
                _this.scopeType = "Project";
                _this.viewModel.projectId = projectId || null;
                _this.viewModel.clientId = null;
            }
            else {
                _this.scopeType = "Default";
                _this.viewModel.clientId = null;
                _this.viewModel.projectId = null;
            }
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.userAccountId = _this.$stateParams["userid"];
        _this.viewModel.id = _this.$stateParams["id"];
        ClientService.clientDropdownList()
            .then(function (result) {
            self.clientDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        ProjectService.projectDropdownList()
            .then(function (result) {
            self.projectDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        if (_this.viewModel.id !== "new") {
            _this.BillingRatesService.billingRatesGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
                self.scopeType = self.resolveScopeType(result);
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.viewModel.id = null;
            _this.applyNewPrefill();
        }
        return _this;
    }
    return BillingRatesDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("BillingRatesDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "BillingRatesService",
    "ClientService",
    "ProjectService",
    "Popups",
    BillingRatesDetailController
]);
//# sourceMappingURL=~BillingRatesDetailController.js.map