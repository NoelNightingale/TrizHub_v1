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
var BillingRatesMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(BillingRatesMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function BillingRatesMaintenanceDetailController($scope, $stateParams, $timeout, $window, $state, BillingRatesService, ClientService, ProjectService, UserService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.BillingRatesService = BillingRatesService;
        _this.ClientService = ClientService;
        _this.ProjectService = ProjectService;
        _this.UserService = UserService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.scopeType = "Default";
        _this.isNew = false;
        _this.userLocked = false;
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
        _this.cancel = function () {
            _this.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
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
                    self.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.deleteRecord = function () {
            var _a;
            var self = _this;
            if (self.isNew || !((_a = self.viewModel) === null || _a === void 0 ? void 0 : _a.id)) {
                return;
            }
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (!action) {
                    return;
                }
                self.BillingRatesService.billingRatesDelete(self.viewModel)
                    .then(function (result) {
                    self.saveSuccess = false;
                    self.$state.go("mainState.maintenance.billingRatesMaintenance.grid");
                }, function (error) {
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        _this.viewModel.userAccountId = _this.$stateParams["userId"] || null;
        _this.isNew = _this.viewModel.id === "new";
        _this.userLocked = !_this.isNew && !!_this.viewModel.userAccountId;
        UserService.userDropdownList()
            .then(function (result) {
            self.userDropdown = result;
            if (self.userLocked) {
                // Dropdown will still render the selected value even if it's not in list.
            }
        }, function (error) {
            self.handleError(error);
        });
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
        if (!_this.isNew) {
            _this.BillingRatesService.billingRatesGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
                self.scopeType = self.resolveScopeType(result);
                self.userLocked = true;
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
    return BillingRatesMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("BillingRatesMaintenanceDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "BillingRatesService",
    "ClientService",
    "ProjectService",
    "UserService",
    "Popups",
    BillingRatesMaintenanceDetailController
]);
//# sourceMappingURL=~BillingRatesMaintenanceDetailController.js.map