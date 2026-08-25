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
var ProjectTeamRatesRosterController = /** @class */ (function (_super) {
    __extends(ProjectTeamRatesRosterController, _super);
    //#endregion
    //#region Ctor
    function ProjectTeamRatesRosterController($scope, $state, $stateParams, BillingRatesService, Popups) {
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
            self.BillingRatesService.projectTeamRates(self.projectId, self.asOfDate)
                .then(function (result) {
                self.viewModel = result;
                self.team = result.team || [];
                self.loading = false;
            }, function (error) {
                self.loading = false;
                self.handleError(error);
            });
        };
        _this.formatRate = function (rate) {
            if (rate === null || rate === undefined)
                return "—";
            return rate;
        };
        _this.editRates = function (row) {
            _this.$state.go("mainState.maintenance.projectMaintenance.teamRatesEdit", { projectId: _this.projectId, userId: row.userAccountId });
        };
        _this.projectId = _this.$stateParams["id"];
        _this.asOfDate = new Date();
        _this.viewModel = {};
        _this.team = [];
        _this.loadTeam();
        return _this;
    }
    return ProjectTeamRatesRosterController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ProjectTeamRatesRosterController", [
    "$scope",
    "$state",
    "$stateParams",
    "BillingRatesService",
    "Popups",
    ProjectTeamRatesRosterController
]);
//# sourceMappingURL=~ProjectTeamRatesRosterController.js.map