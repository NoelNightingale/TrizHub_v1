var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var TeamJobDesignationDetailController = (function (_super) {
    __extends(TeamJobDesignationDetailController, _super);
    //#endregion
    //#region Ctor
    function TeamJobDesignationDetailController($scope, $stateParams, $timeout, $window, $state, UserService, ClientService, EmployerService, Popups) {
        var _this = this;
        _super.call(this, $scope, Popups, $state);
        this.$scope = $scope;
        this.$stateParams = $stateParams;
        this.$timeout = $timeout;
        this.$window = $window;
        this.$state = $state;
        this.UserService = UserService;
        this.ClientService = ClientService;
        this.EmployerService = EmployerService;
        this.Popups = Popups;
        //#region members
        this.successMessage = "Saved Successfully";
        this.saveSuccess = false;
        //#endregion
        this.submitForm = function () {
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
        this.getBasic = function (date) {
            var dateFormat = new Date(date);
            dateFormat.setMinutes(dateFormat.getMinutes() - dateFormat.getTimezoneOffset());
            return dateFormat.toUTCString();
        };
        var self = this;
        this.viewModel = {};
        this.employedBy = []; // = String[2] = ["Triz SA", "Triz USA"];
        this.viewModel.userAccountId = this.$stateParams["userid"];
        this.viewModel.id = this.$stateParams["id"];
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
        if (this.viewModel.id !== "new") {
            this.UserService.teamJobDesignationGet(this.viewModel.id)
                .then(function (result) {
                self.viewModel = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        else {
            this.viewModel.id = null;
        }
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