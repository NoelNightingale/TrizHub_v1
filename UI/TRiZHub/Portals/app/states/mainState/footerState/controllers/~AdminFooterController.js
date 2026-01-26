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
var AdminFooterController = /** @class */ (function (_super) {
    __extends(AdminFooterController, _super);
    //#region Ctor
    function AdminFooterController($scope, $state, SecurityService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        //#endregion
        _this.isAllowed = function (privilegeType) {
            return _this.SecurityService.isAllowed(privilegeType);
        };
        _this.currentUser = _this.SecurityService.getCurrentUserDetails();
        return _this;
    }
    return AdminFooterController;
}(CHControllerBase));
;
angular.module("AngularApp")
    .controller("AdminFooterController", [
    "$scope",
    "$state",
    "SecurityService",
    "Popups",
    AdminFooterController
]);
//# sourceMappingURL=~AdminFooterController.js.map