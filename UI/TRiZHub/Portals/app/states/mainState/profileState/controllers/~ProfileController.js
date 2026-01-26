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
var ProfileController = /** @class */ (function (_super) {
    __extends(ProfileController, _super);
    //#endregion
    //#region Ctor
    function ProfileController($stateParams, $timeout, $window, $state, $scope, $uibModal, $log, MasterDataService, AccountService, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.$scope = $scope;
        _this.$uibModal = $uibModal;
        _this.$log = $log;
        _this.MasterDataService = MasterDataService;
        _this.AccountService = AccountService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        //#region Members
        _this.successMessage = "Saved Successfully";
        _this.saveSuccess = false;
        //#endregion
        _this.submitForm = function () {
            var me = _this;
            _this.MasterDataService.profileSave(_this.viewModel)
                .then(function (result) {
                _this.saveSuccess = true;
                if (result.emailAddress !== _this.existingEmail) {
                    me.AccountService.logout();
                    me.SecurityService.getCurrentUserDetails().loggedIn = false;
                    me.$state.transitionTo("root.login");
                }
                else {
                    me.$timeout(function () { me.$state.transitionTo("mainState.home"); }, 1000);
                }
            }, function (error) {
                me.handleError(error);
            });
        };
        _this.cancelForm = function () {
            _this.$state.transitionTo("mainState.home");
        };
        var me = _this;
        _this.viewModel = {};
        MasterDataService.profileGet()
            .then(function (result) {
            me.viewModel = result;
            me.existingEmail = result.emailAddress;
        }, function (error) {
            me.handleError(error);
        });
        return _this;
    }
    return ProfileController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ProfileController", [
    "$stateParams",
    "$timeout",
    "$window",
    "$state",
    "$scope",
    "$uibModal",
    "$log",
    "MasterDataService",
    "AccountService",
    "SecurityService",
    "Popups",
    ProfileController
]);
//# sourceMappingURL=~ProfileController.js.map