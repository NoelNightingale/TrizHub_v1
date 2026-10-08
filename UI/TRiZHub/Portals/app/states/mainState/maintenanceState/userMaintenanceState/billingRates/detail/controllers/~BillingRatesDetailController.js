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
        _this.filteredProjectDropdown = [];
        // UI-only: Project-scope rates are saved with ClientId null.
        _this.projectClientFilterId = null;
        // Set when opened from a work team: only that team's clients/projects, and no employee (default) rates.
        _this.workTeamId = null;
        /** Team mode: the client/project options come from the team's allocations. */
        _this.loadTeamOptions = function () {
            var self = _this;
            self.BillingRatesService.workTeamMemberRates(self.workTeamId, self.viewModel.userAccountId)
                .then(function (result) {
                self.clientDropdown = (result.clients || []).map(function (c) { return ({ id: c.id, entityName: c.name }); });
                self.projectDropdown = (result.projects || []).map(function (p) { return ({ id: p.id, description: p.name, clientId: p.clientId }); });
                var seen = {};
                self.projectClientDropdown = [];
                (result.projects || []).forEach(function (p) {
                    if (!seen[p.clientId]) {
                        seen[p.clientId] = true;
                        self.projectClientDropdown.push({ id: p.clientId, entityName: p.clientName });
                    }
                });
                self.projectClientDropdown.sort(function (a, b) { return (a.entityName || "").localeCompare(b.entityName || ""); });
                self.syncProjectClientFilter();
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.goBack = function () {
            if (_this.workTeamId) {
                _this.$state.go("mainState.maintenance.workTeamMaintenance.detail", { id: _this.workTeamId, tab: "people", ratesFor: _this.viewModel.userAccountId });
                return;
            }
            _this.$state.go("mainState.maintenance.userMaintenance.billingRatesGrid", { id: _this.viewModel.userAccountId });
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
            else if (_this.workTeamId) {
                _this.scopeType = "Client";
                _this.viewModel.clientId = null;
                _this.viewModel.projectId = null;
            }
            else {
                _this.scopeType = "Default";
                _this.viewModel.clientId = null;
                _this.viewModel.projectId = null;
            }
        };
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
                if (!_this.viewModel.clientId && _this.projectClientFilterId)
                    _this.viewModel.clientId = _this.projectClientFilterId;
                _this.viewModel.projectId = null;
            }
            else if (_this.scopeType === "Project") {
                if (_this.viewModel.clientId && !_this.projectClientFilterId)
                    _this.projectClientFilterId = _this.viewModel.clientId;
                _this.viewModel.clientId = null;
                _this.applyProjectClientFilter();
            }
        };
        _this.syncProjectClientFilter = function () {
            if (!_this.projectClientFilterId && _this.viewModel.projectClientId)
                _this.projectClientFilterId = _this.viewModel.projectClientId;
            if (!_this.projectClientFilterId && _this.projectDropdown && _this.viewModel.projectId) {
                var project = _this.findProject(_this.viewModel.projectId);
                if (project && project.clientId)
                    _this.projectClientFilterId = project.clientId;
            }
            _this.applyProjectClientFilter();
        };
        _this.applyProjectClientFilter = function () {
            var all = _this.projectDropdown || [];
            var clientId = _this.projectClientFilterId;
            _this.filteredProjectDropdown = clientId
                ? all.filter(function (p) { return p.clientId === clientId; })
                : [];
            // Leave projects that aren't in the dropdown at all (e.g. inactive) untouched.
            var selected = _this.findProject(_this.viewModel.projectId);
            if (selected && _this.filteredProjectDropdown.indexOf(selected) < 0)
                _this.viewModel.projectId = null;
        };
        _this.findProject = function (projectId) {
            if (!projectId || !_this.projectDropdown)
                return null;
            var matches = _this.projectDropdown.filter(function (p) { return p.id === projectId; });
            return matches.length ? matches[0] : null;
        };
        _this.submitForm = function () {
            var self = _this;
            if (_this.viewModel.isLocked)
                return;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            _this.onScopeChanged();
            _this.BillingRatesService.billingRatesSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.goBack();
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.userAccountId = _this.$stateParams["userid"];
        _this.viewModel.id = _this.$stateParams["id"];
        _this.workTeamId = _this.$stateParams["workTeamId"] || null;
        if (_this.workTeamId) {
            _this.loadTeamOptions();
        }
        else {
            ClientService.clientDropdownList()
                .then(function (result) {
                self.clientDropdown = result;
                self.projectClientDropdown = result;
            }, function (error) {
                self.handleError(error);
            });
            ProjectService.projectDropdownList()
                .then(function (result) {
                self.projectDropdown = result;
                self.syncProjectClientFilter();
            }, function (error) {
                self.handleError(error);
            });
        }
        if (_this.viewModel.id !== "new") {
            _this.BillingRatesService.billingRatesGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
                self.scopeType = self.resolveScopeType(result);
                self.syncProjectClientFilter();
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