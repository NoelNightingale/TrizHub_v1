var SecurityServiceModule;
(function (SecurityServiceModule) {
    var CurrentUser = /** @class */ (function () {
        //#endregion
        //#region Ctor
        function CurrentUser() {
            this.id = null;
            this.loggedIn = false;
            this.accessToken = "";
            this.userName = "";
            this.displayName = "";
            this.allowedPrivileges = [];
            this.isSystemAdmin = false;
        }
        return CurrentUser;
    }());
    SecurityServiceModule.CurrentUser = CurrentUser;
    var SecurityService = /** @class */ (function () {
        //#region Ctor
        function SecurityService($rootScope, $localStorage, EnumService) {
            var _this = this;
            this.$rootScope = $rootScope;
            this.$localStorage = $localStorage;
            this.EnumService = EnumService;
            //#endregion
            this.init = function () {
                SecurityService.currentUser = new CurrentUser();
                var authData = _this.$localStorage.authData;
                if (authData) {
                    SecurityService.currentUser.loggedIn = true;
                    SecurityService.currentUser.userName = authData.userName;
                    SecurityService.currentUser.accessToken = authData.token;
                    SecurityService.currentUser.isSystemAdmin = authData.isSystemAdmin;
                }
                _this.$rootScope.$emit(_this.scopeUpdateEvent, SecurityService.currentUser);
                return SecurityService.currentUser.loggedIn;
            };
            this.login = function (accessToken, userName, isSystemAdmin) {
                SecurityService.currentUser.userName = userName;
                SecurityService.currentUser.accessToken = accessToken;
                SecurityService.currentUser.loggedIn = true;
                _this.$localStorage.authData = { token: accessToken, userName: userName, isSystemAdmin: isSystemAdmin };
                _this.$rootScope.$emit(_this.scopeUpdateEvent, SecurityService.currentUser);
            };
            this.logout = function () {
                SecurityService.currentUser = new CurrentUser();
                _this.$localStorage.authData = null;
                _this.$rootScope.$emit(_this.scopeUpdateEvent, SecurityService.currentUser);
            };
            this.getCurrentUserDetails = function () {
                return SecurityService.currentUser;
            };
            this.setCurrentUserDetails = function (id, displayName, allowedPrivileges, isSystemAdmin, isUserProfileComplete, isUserApproved) {
                SecurityService.currentUser.id = id;
                SecurityService.currentUser.displayName = displayName;
                SecurityService.currentUser.allowedPrivileges = allowedPrivileges;
                SecurityService.currentUser.isSystemAdmin = isSystemAdmin;
                SecurityService.currentUser.isUserProfileComplete = isUserProfileComplete;
                SecurityService.currentUser.isUserApproved = isUserApproved;
                _this.$rootScope.$emit(_this.scopeUpdateEvent, SecurityService.currentUser);
            };
            this.setAccount = function (username, isSystemAdmin) {
                SecurityService.currentUser.userName = username;
                SecurityService.currentUser.isSystemAdmin = isSystemAdmin;
                _this.$rootScope.$emit(_this.scopeUpdateEvent, SecurityService.currentUser);
            };
            this.getToken = function () {
                if (SecurityService.currentUser.loggedIn)
                    return SecurityService.currentUser.accessToken;
                else
                    return null;
            };
            this.userHasPrivileges = function () {
                if (!SecurityService.currentUser || SecurityService.currentUser.allowedPrivileges == null)
                    return false;
                if (!(SecurityService.currentUser.allowedPrivileges.length > 0))
                    return false;
                return true;
            };
            this.isAllowed = function (privilegeType) {
                if (SecurityService.currentUser.isSystemAdmin)
                    return true;
                if (!_this.userHasPrivileges())
                    return false;
                var allValues = _this.EnumService.getSecurityTypes();
                var myEnum = -1;
                for (var _i = 0, allValues_1 = allValues; _i < allValues_1.length; _i++) {
                    var value = allValues_1[_i];
                    if (value.name === privilegeType) {
                        myEnum = value.ordinalValue;
                        break;
                    }
                }
                if (myEnum < 0)
                    return false;
                var result = SecurityService.currentUser.allowedPrivileges.indexOf(myEnum) > -1;
                return result;
            };
            this.scopeUpdateEvent = "current-user-updated";
        }
        return SecurityService;
    }());
    SecurityServiceModule.SecurityService = SecurityService;
    function getInstance($rootScope, $localStorage, EnumService) {
        return new SecurityService($rootScope, $localStorage, EnumService);
    }
    angular.module("AngularApp")
        .factory("SecurityService", [
        "$rootScope",
        "$localStorage",
        "EnumService",
        getInstance
    ]);
})(SecurityServiceModule || (SecurityServiceModule = {}));
//# sourceMappingURL=~SecurityService.js.map