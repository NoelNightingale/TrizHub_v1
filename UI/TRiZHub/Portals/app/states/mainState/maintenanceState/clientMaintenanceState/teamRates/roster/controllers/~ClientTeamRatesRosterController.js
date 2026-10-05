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
var ClientTeamRatesRosterController = /** @class */ (function (_super) {
    __extends(ClientTeamRatesRosterController, _super);
    //#endregion
    //#region Ctor
    function ClientTeamRatesRosterController($scope, $state, $stateParams, BillingRatesService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$stateParams = $stateParams;
        _this.BillingRatesService = BillingRatesService;
        _this.Popups = Popups;
        _this.loading = false;
        _this.viewMode = "asOf"; // "periods" | "asOf"
        _this.periodGroups = [];
        _this.periodsLoading = false;
        //#endregion
        _this.setViewMode = function (mode) {
            _this.viewMode = mode;
            if (mode === "periods") {
                _this.loadPeriods();
            }
            else {
                _this.loadTeam();
            }
        };
        _this.loadTeam = function () {
            var self = _this;
            self.loading = true;
            self.BillingRatesService.clientTeamRates(self.clientId, self.asOfDate)
                .then(function (result) {
                self.viewModel = result;
                self.team = result.team || [];
                self.loading = false;
            }, function (error) {
                self.loading = false;
                self.handleError(error);
            });
        };
        _this.loadPeriods = function () {
            var self = _this;
            self.periodsLoading = true;
            self.BillingRatesService.billingRatesGrid({
                clientId: self.clientId,
                scope: "Client",
                sortKey: "user",
                sortOrder: "ASC",
                currentPage: 1,
                recordsPerPage: 10000
            })
                .then(function (result) {
                self.periodGroups = self.groupByUser(result.results || []);
                self.periodsLoading = false;
            }, function (error) {
                self.periodsLoading = false;
                self.handleError(error);
            });
        };
        _this.groupByUser = function (rows) {
            var groups = [];
            var byUser = {};
            rows.forEach(function (r) {
                var group = byUser[r.userAccountId];
                if (!group) {
                    group = { userAccountId: r.userAccountId, userName: r.userName, rates: [] };
                    byUser[r.userAccountId] = group;
                    groups.push(group);
                }
                group.rates.push(r);
            });
            groups.forEach(function (g) { return g.rates.sort(function (a, b) {
                return new Date(a.startDate).getTime() - new Date(b.startDate).getTime();
            }); });
            return groups;
        };
        _this.projectOverridesLabel = function (count) {
            if (!count || count <= 0)
                return "—";
            return count === 1 ? "1 project" : (count + " projects");
        };
        _this.editRates = function (row) {
            _this.$state.go("mainState.maintenance.clientMaintenance.teamRatesEdit", { clientId: _this.clientId, userId: row.userAccountId });
        };
        _this.clientId = _this.$stateParams["id"];
        _this.asOfDate = new Date();
        _this.viewModel = {};
        _this.team = [];
        _this.loadTeam();
        return _this;
    }
    return ClientTeamRatesRosterController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientTeamRatesRosterController", [
    "$scope",
    "$state",
    "$stateParams",
    "BillingRatesService",
    "Popups",
    ClientTeamRatesRosterController
]);
//# sourceMappingURL=~ClientTeamRatesRosterController.js.map