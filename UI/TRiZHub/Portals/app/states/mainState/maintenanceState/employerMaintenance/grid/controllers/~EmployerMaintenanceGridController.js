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
var EmployerMaintenanceGridController = /** @class */ (function (_super) {
    __extends(EmployerMaintenanceGridController, _super);
    function EmployerMaintenanceGridController($scope, $state, EmployerService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.EmployerService = EmployerService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.toggleInactive = function () {
            _this.pageGrid.loadGrid();
        };
        _this.toggleActivation = function (employer) {
            var self = _this;
            if (employer.isActive) {
                _this.EmployerService.deactivateEmployer(employer.id)
                    .then(function (result) {
                    employer.isActive = false;
                }, function (error) {
                    self.handleError(error);
                });
            }
            else {
                _this.EmployerService.activateEmployer(employer.id)
                    .then(function (result) {
                    employer.isActive = true;
                }, function (error) {
                    self.handleError(error);
                });
            }
        };
        _this.deleteRecord = function (record) {
            var me = _this;
            me.Popups.confirmationDialog(me.$scope, "Are you sure you want to delete?", "You are about to delete this record...")
                .then(function (action) {
                if (action)
                    me.EmployerService.deleteEmployer(record.id)
                        .then(function (result) {
                        me.pageGrid.loadGrid();
                    }, function (error) {
                        me.handleError(error);
                    });
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.newEmployer = function () {
            _this.$state.transitionTo("mainState.maintenance.employerMaintenance.detail", { "id": "new" });
        };
        var self = _this;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("name", self.EmployerService.employerGrid, self.onDataLoaded, null, null, $state);
        self.pageGrid.loadGrid();
        return _this;
    }
    EmployerMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            console.log(this.gridModel);
        }
    };
    return EmployerMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("EmployerMaintenanceGridController", [
    "$scope",
    "$state",
    "EmployerService",
    "Popups",
    EmployerMaintenanceGridController
]);
//# sourceMappingURL=~EmployerMaintenanceGridController.js.map