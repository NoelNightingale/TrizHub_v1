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
var ClientMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(ClientMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function ClientMaintenanceDetailController($stateParams, $scope, $state, $timeout, $window, ClientService, UserService, EnumService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.ClientService = ClientService;
        _this.UserService = UserService;
        _this.EnumService = EnumService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        _this.addReporter = function () {
            var self = _this;
            console.log(self.selectedReporter);
            if (!self.selectedReporter)
                return;
            self.ClientService.addClientReporter(self.clientId, self.selectedReporter)
                .then(function (result) {
                self.clientReporters = result;
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.saveSuccess = false;
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.removeReporter = function (userId) {
            var self = _this;
            console.log("Remove of user : " + userId);
            self.ClientService.removeClientReporter(self.clientId, userId)
                .then(function (result) {
                self.clientReporters = result;
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.saveSuccess = false;
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            self.ClientService.clientSave(self.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.clientMaintenance.grid");
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        self.clientId = self.$stateParams["id"];
        self.viewModel = {};
        if (self.clientId !== "new") {
            ClientService.clientGet(self.clientId)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
            ClientService.getClientReporters(self.clientId)
                .then(function (result) {
                self.clientReporters = result;
                console.log("Client Reporters : %1", result);
            }, function (error) {
                self.handleError(error);
            });
            UserService.userDropdownList()
                .then(function (result) {
                self.userDropdown = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            self.clientId = null;
        }
        return _this;
    }
    return ClientMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ClientMaintenanceDetailController", [
    "$stateParams",
    "$scope",
    "$state",
    "$timeout",
    "$window",
    "ClientService",
    "UserService",
    "EnumService",
    "Popups",
    ClientMaintenanceDetailController
]);
//# sourceMappingURL=~ClientMaintenanceDetailController.js.map