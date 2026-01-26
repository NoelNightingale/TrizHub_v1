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
var HomeController = /** @class */ (function (_super) {
    __extends(HomeController, _super);
    //#region Ctor
    function HomeController($scope, $state, AccountService, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.AccountService = AccountService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        if (_this.SecurityService.getCurrentUserDetails() == undefined) {
            _this.AccountService.init()
                .then(function (result) { }, function (e) { this.$state.go("root.login"); });
        }
        else {
            if (_this.SecurityService.getCurrentUserDetails().allowedPrivileges.indexOf(7) != -1 && _this.SecurityService.getCurrentUserDetails().allowedPrivileges.length == 1) {
                _this.$state.go("mainState.timesheet");
            }
        }
        return _this;
    }
    return HomeController;
}(CHControllerBase));
;
angular.module("AngularApp")
    .controller("HomeController", [
    "$scope",
    "$state",
    "AccountService",
    "SecurityService",
    "Popups",
    HomeController
]);
//# sourceMappingURL=~HomeController.js.map