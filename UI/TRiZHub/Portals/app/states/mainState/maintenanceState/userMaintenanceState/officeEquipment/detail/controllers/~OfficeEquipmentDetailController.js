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
var OfficeEquipmentDetailController = /** @class */ (function (_super) {
    __extends(OfficeEquipmentDetailController, _super);
    //#endregion
    //#region Ctor
    function OfficeEquipmentDetailController($scope, $stateParams, $timeout, $window, $state, UserService, Popups) {
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
            _this.UserService.officeEquipmentSave(_this.viewModel)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.$state.transitionTo("mainState.maintenance.userMaintenance.officeEquipmentGrid", { "id": result.userAccountId });
                }, 1000);
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.userAccountId = _this.$stateParams["userid"];
        _this.viewModel.id = _this.$stateParams["id"];
        if (_this.viewModel.id !== "new") {
            _this.UserService.officeEquipmentGet(_this.viewModel.id)
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
    return OfficeEquipmentDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("OfficeEquipmentDetailController", [
    "$scope",
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "UserService",
    "Popups",
    OfficeEquipmentDetailController
]);
//# sourceMappingURL=~OfficeEquipmentDetailController.js.map