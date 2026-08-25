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
var ProjectTeamRatesEditController = /** @class */ (function (_super) {
    __extends(ProjectTeamRatesEditController, _super);
    //#endregion
    //#region Ctor
    function ProjectTeamRatesEditController($scope, $state, $stateParams, $timeout, BillingRatesService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.BillingRatesService = BillingRatesService;
        _this.Popups = Popups;
        _this.loading = false;
        _this.saveSuccess = false;
        _this.editingScope = null;
        _this.editModel = null;
        //#endregion
        _this.loadContext = function () {
            var self = _this;
            self.loading = true;
            self.BillingRatesService.userRatesForProjectContext(self.userId, self.projectId)
                .then(function (result) {
                self.context = result;
                self.loading = false;
            }, function (error) {
                self.loading = false;
                self.handleError(error);
            });
        };
        _this.backToRoster = function () {
            _this.$state.go("mainState.maintenance.projectMaintenance.teamRates", { id: _this.projectId });
        };
        _this.startAdd = function (scope) {
            _this.editingScope = scope;
            _this.editModel = {
                id: null,
                userAccountId: _this.userId,
                rate: null,
                startDate: null,
                endDate: null,
                clientId: scope === "Client" ? _this.context.clientId : null,
                projectId: scope === "Project" ? _this.projectId : null
            };
        };
        _this.startEdit = function (scope, row) {
            _this.editingScope = scope;
            _this.editModel = {
                id: row.id,
                userAccountId: _this.userId,
                rate: row.rate,
                startDate: row.startDate,
                endDate: row.endDate,
                clientId: scope === "Client" ? _this.context.clientId : null,
                projectId: scope === "Project" ? _this.projectId : null
            };
        };
        _this.cancelEdit = function () {
            _this.editingScope = null;
            _this.editModel = null;
        };
        _this.savePeriod = function () {
            var self = _this;
            if (!self.editModel || !self.editModel.rate || !self.editModel.startDate || !self.editModel.endDate) {
                self.Popups.showError(self.$scope, "Rate, Start Date and End Date are required.");
                return;
            }
            if (self.editingScope === "Project") {
                self.editModel.projectId = self.projectId;
                self.editModel.clientId = null;
            }
            else if (self.editingScope === "Client") {
                self.editModel.clientId = self.context.clientId;
                self.editModel.projectId = null;
            }
            else {
                self.editModel.clientId = null;
                self.editModel.projectId = null;
            }
            self.editModel.userAccountId = self.userId;
            self.BillingRatesService.billingRatesSave(self.editModel)
                .then(function () {
                self.saveSuccess = true;
                self.cancelEdit();
                self.loadContext();
                self.$timeout(function () { self.saveSuccess = false; }, 1500);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.deletePeriod = function (row) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this rate period...")
                .then(function (action) {
                if (!action)
                    return;
                self.BillingRatesService.billingRatesDelete({ id: row.id })
                    .then(function () {
                    self.loadContext();
                }, function (error) {
                    self.handleError(error);
                });
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.projectId = _this.$stateParams["projectId"];
        _this.userId = _this.$stateParams["userId"];
        _this.context = {
            projectRates: [],
            clientRates: [],
            defaultRates: []
        };
        _this.loadContext();
        return _this;
    }
    return ProjectTeamRatesEditController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ProjectTeamRatesEditController", [
    "$scope",
    "$state",
    "$stateParams",
    "$timeout",
    "BillingRatesService",
    "Popups",
    ProjectTeamRatesEditController
]);
//# sourceMappingURL=~ProjectTeamRatesEditController.js.map