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
    /**
     * Runs inside a $uibModal opened by BillingRatesMaintenanceGridController.
     * modalParams: { id: string | "new", userId?, scope?, clientId?, projectId? }
     * Closes with { action: "saved" | "deleted", id } or dismisses on cancel.
     */
    function BillingRatesMaintenanceDetailController($scope, modalParams, $uibModalInstance, $state, BillingRatesService, ClientService, ProjectService, UserService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.modalParams = modalParams;
        _this.$uibModalInstance = $uibModalInstance;
        _this.$state = $state;
        _this.BillingRatesService = BillingRatesService;
        _this.ClientService = ClientService;
        _this.ProjectService = ProjectService;
        _this.UserService = UserService;
        _this.Popups = Popups;
        _this.scopeType = "Default";
        /** Client chosen to narrow the project list for Project rates; not saved. */
        _this.projectClientFilter = null;
        _this.projectClientOptions = [];
        _this.filteredProjects = [];
        _this.isNew = false;
        _this.userLocked = false;
        _this.saving = false;
        _this.applyNewPrefill = function () {
            var params = _this.modalParams || {};
            var scope = params.scope;
            var clientId = params.clientId;
            var projectId = params.projectId;
            if (scope === "Client" || (clientId && !projectId)) {
                _this.scopeType = "Client";
                _this.viewModel.clientId = clientId || null;
                _this.viewModel.projectId = null;
            }
            else if (scope === "Project" || projectId) {
                _this.scopeType = "Project";
                _this.viewModel.projectId = projectId || null;
                _this.viewModel.clientId = null;
                _this.projectClientFilter = clientId || null;
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
                if (!_this.viewModel.clientId && _this.projectClientFilter) {
                    _this.viewModel.clientId = _this.projectClientFilter;
                }
                _this.viewModel.projectId = null;
            }
            else if (_this.scopeType === "Project") {
                if (!_this.projectClientFilter && _this.viewModel.clientId) {
                    _this.projectClientFilter = _this.viewModel.clientId;
                    _this.refreshProjectOptions();
                }
                _this.viewModel.clientId = null;
            }
        };
        _this.refreshProjectOptions = function () {
            var projects = _this.projectDropdown || [];
            var clients = _this.clientDropdown || [];
            if (!_this.projectClientFilter && _this.viewModel.projectId) {
                var current = projects.filter(function (p) { return p.id === _this.viewModel.projectId; })[0];
                if (current) {
                    _this.projectClientFilter = current.clientId;
                }
            }
            var clientHasProjects = {};
            projects.forEach(function (p) { clientHasProjects[p.clientId] = true; });
            _this.projectClientOptions = clients.filter(function (c) { return !!clientHasProjects[c.id]; });
            var clientId = _this.projectClientFilter;
            _this.filteredProjects = clientId ? projects.filter(function (p) { return p.clientId === clientId; }) : [];
        };
        _this.onProjectClientChanged = function () {
            var projects = _this.projectDropdown || [];
            var current = projects.filter(function (p) { return p.id === _this.viewModel.projectId; })[0];
            if (!current || current.clientId !== _this.projectClientFilter) {
                _this.viewModel.projectId = null;
            }
            _this.refreshProjectOptions();
        };
        _this.cancel = function () {
            _this.$uibModalInstance.dismiss("cancel");
        };
        _this.submitForm = function () {
            var self = _this;
            if (_this.viewModel.isLocked || _this.saving)
                return;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            _this.onScopeChanged();
            _this.saving = true;
            _this.BillingRatesService.billingRatesSave(_this.viewModel)
                .then(function (result) {
                self.saving = false;
                self.$uibModalInstance.close({ action: "saved", id: result.id });
            }, function (error) {
                self.saving = false;
                self.handleError(error);
            });
        };
        _this.deleteRecord = function () {
            var _a;
            var self = _this;
            if (self.isNew || !((_a = self.viewModel) === null || _a === void 0 ? void 0 : _a.id) || self.viewModel.isLocked) {
                return;
            }
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (!action) {
                    return;
                }
                self.BillingRatesService.billingRatesDelete(self.viewModel)
                    .then(function (result) {
                    self.$uibModalInstance.close({ action: "deleted", id: self.viewModel.id });
                }, function (error) {
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        var params = _this.modalParams || {};
        _this.viewModel = {};
        _this.viewModel.id = params.id || "new";
        _this.viewModel.userAccountId = params.userId || null;
        _this.isNew = _this.viewModel.id === "new";
        _this.userLocked = !_this.isNew && !!_this.viewModel.userAccountId;
        UserService.userDropdownList()
            .then(function (result) {
            self.userDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        ClientService.clientDropdownList()
            .then(function (result) {
            self.clientDropdown = result;
            self.refreshProjectOptions();
        }, function (error) {
            self.handleError(error);
        });
        ProjectService.projectDropdownList()
            .then(function (result) {
            self.projectDropdown = result;
            self.refreshProjectOptions();
        }, function (error) {
            self.handleError(error);
        });
        if (!_this.isNew) {
            _this.BillingRatesService.billingRatesGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
                self.scopeType = self.resolveScopeType(result);
                self.userLocked = true;
                self.projectClientFilter = result.projectClientId || null;
                self.refreshProjectOptions();
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
    "modalParams",
    "$uibModalInstance",
    "$state",
    "BillingRatesService",
    "ClientService",
    "ProjectService",
    "UserService",
    "Popups",
    BillingRatesMaintenanceDetailController
]);
//# sourceMappingURL=~BillingRatesMaintenanceDetailController.js.map