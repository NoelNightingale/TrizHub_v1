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
var TeamJobDesignationDetailController = /** @class */ (function (_super) {
    __extends(TeamJobDesignationDetailController, _super);
    //#endregion
    //#region Ctor
    function TeamJobDesignationDetailController($scope, $stateParams, $timeout, $window, $state, UserService, ClientService, EmployerService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.UserService = UserService;
        _this.ClientService = ClientService;
        _this.EmployerService = EmployerService;
        _this.Popups = Popups;
        //#region members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.submitForm = function () {
            var self = _this;
            _this.$scope.$broadcast("show-errors-check-validity");
            if (_this.$scope["EditForm"].$invalid)
                return;
            // Set location
            for (var i = 0; i < _this.employedBy.length; i++) {
                if (_this.employedBy[i].id == _this.viewModel.employerId) {
                    _this.viewModel.location = _this.employedBy[i].name;
                    break;
                }
            }
            if (_this.viewModel.startDate)
                _this.viewModel.startDate = _this.getBasic(_this.viewModel.startDate);
            if (_this.viewModel.endDate)
                _this.viewModel.endDate = _this.getBasic(_this.viewModel.endDate);
            _this.UserService.teamJobDesignationSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.go("mainState.maintenance.userMaintenance.teamJobDesignationGrid", { "id": result.userAccountId });
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.getBasic = function (date) {
            var dateFormat = new Date(date);
            dateFormat.setMinutes(dateFormat.getMinutes() - dateFormat.getTimezoneOffset());
            return dateFormat.toUTCString();
        };
        var self = _this;
        _this.viewModel = {};
        _this.employedBy = []; // = String[2] = ["Triz SA", "Triz USA"];
        _this.viewModel.userAccountId = _this.$stateParams["userid"];
        _this.viewModel.id = _this.$stateParams["id"];
        UserService.userDropdownList()
            .then(function (result) {
            self.userDropdown = result;
            self.userDropdown.splice(0, 0, { id: null, description: "N/A" });
        }, function (error) {
            self.handleError(error);
        });
        ClientService.clientDropdownList()
            .then(function (result) {
            self.clientDropdown = result;
        }, function (error) {
            self.handleError(error);
        });
        EmployerService.employerDropdownList()
            .then(function (result) {
            self.employedBy = result;
        }, function (error) {
            self.handleError(error);
        });
        if (_this.viewModel.id !== "new") {
            _this.UserService.teamJobDesignationGet(_this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            _this.viewModel.id = null;
        }
        return _this;
    }
    return TeamJobDesignationDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("TeamJobDesignationDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "UserService",
    "ClientService",
    "EmployerService",
    "Popups",
    TeamJobDesignationDetailController
]);
//# sourceMappingURL=~TeamJobDesignationDetailController.js.map