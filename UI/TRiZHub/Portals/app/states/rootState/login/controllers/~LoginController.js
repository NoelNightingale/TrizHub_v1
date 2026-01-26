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
var LoginController = /** @class */ (function (_super) {
    __extends(LoginController, _super);
    //#endregion
    //#region Ctor
    function LoginController($scope, $state, $timeout, AccountService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.AccountService = AccountService;
        _this.Popups = Popups;
        //#region Members
        _this.headerMessage = "Loading...";
        _this.viewModel = null;
        _this.successMessage = "Login Successfull";
        //#endregion
        _this.initHome = function () {
            var me = _this;
            me.AccountService.logout()
                .then(function (result) {
                console.log(result);
                me.AccountService.login()
                    .then(function (result) {
                    console.log(result);
                    me.AccountService.init()
                        .then(function (result) {
                        me.viewModel = result;
                        if (me.viewModel.isUserProfileComplete) {
                            me
                                .headerMessage = "Welcome back ".concat(me.viewModel.displayName, " (").concat(me.viewModel.userName, ")");
                        }
                        else {
                            me
                                .headerMessage = "Please setup your account (".concat(me.viewModel.userName, ")");
                        }
                    }, function (e) {
                        me.headerMessage = "Oops something went wrong...".concat(e);
                    });
                }, function (error) {
                    me.headerMessage = "Oops something went wrong... ".concat(error);
                    me.AccountService.logout();
                });
            }, function (error) {
                me.headerMessage = "Oops something went wrong... ".concat(error);
                me.AccountService.logout();
            });
        };
        _this.goTohome = function () {
            _this.$state.go("mainState.home");
        };
        _this.goToProfile = function () {
            _this.$state.go("mainState.profile");
        };
        _this.reload = function () {
            _this.initHome();
        };
        _this.initHome();
        return _this;
    }
    return LoginController;
}(CHControllerBase));
;
angular.module("AngularApp")
    .controller("LoginController", [
    "$scope",
    "$state",
    "$timeout",
    "AccountService",
    "Popups",
    LoginController
]);
//# sourceMappingURL=~LoginController.js.map