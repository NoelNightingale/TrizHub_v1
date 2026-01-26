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
var PersonalInformationController = /** @class */ (function (_super) {
    __extends(PersonalInformationController, _super);
    //#endregion
    //#region Ctor
    function PersonalInformationController($scope, $stateParams, $timeout, $window, $state, UserService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.UserService = UserService;
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
            _this.UserService.personalInformationSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.go("mainState.maintenance.userMaintenance.detail", { "id": result.userAccountId });
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.genders = String[2] = ["Male", "Female"];
        _this.races = String[4] = ["White", "Black", "Colored", "Asian"];
        _this.viewModel.id = _this.$stateParams["id"];
        _this.UserService.personalInformationGet(_this.viewModel.id)
            .then(function (result) {
            self.viewModel = result;
        }, function (error) {
            self.handleError(error);
        });
        return _this;
    }
    return PersonalInformationController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("PersonalInformationController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "UserService",
    "Popups",
    PersonalInformationController
]);
//# sourceMappingURL=~PersonalInformationController.js.map