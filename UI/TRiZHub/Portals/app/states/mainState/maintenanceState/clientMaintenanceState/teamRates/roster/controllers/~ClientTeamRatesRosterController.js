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
        //#endregion
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