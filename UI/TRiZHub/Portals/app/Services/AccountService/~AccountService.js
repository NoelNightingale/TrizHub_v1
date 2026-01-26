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
var AccountServiceModule;
(function (AccountServiceModule) {
    var AccountService = /** @class */ (function (_super) {
        __extends(AccountService, _super);
        //#region ctor
        function AccountService($http, $q, SecurityService, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Account/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.SecurityService = SecurityService;
            _this.ENV = ENV;
            //#endregion
            _this.init = function () {
                var self = _this;
                //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.
                var deferred = _this.$q.defer();
                if (_this.SecurityService.init()) {
                    _this.getCurrentUser()
                        .then(function (result) {
                        deferred.resolve(result);
                    }, function (err) {
                        deferred.reject(err);
                    });
                }
                else {
                    _this.logout();
                    deferred.reject("Not logged in!");
                }
                return deferred.promise;
            };
            _this.getCurrentUser = function () {
                //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetCurrentUser")
                    .then(function (result) {
                    _this.SecurityService
                        .setCurrentUserDetails(result.data.id, result.data.displayName, result.data.allowedPrivileges, result.data.isSystemAdmin, result.data.isUserProfileComplete, result.data.isUserApproved);
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.login = function () {
                var data = "grant_type=password&username=demo&password=demo&scope=account";
                var deferred = _this.$q.defer();
                var self = _this;
                //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.
                _this.$http.post(_this.ENV.serverLocation + "/token", data, {
                    headers: { 'Content-Type': "application/x-www-form-urlencoded" }
                })
                    .then(function (response) {
                    if (!self.SecurityService.init()) {
                        self.SecurityService.login(response.data.access_token, "demo", false);
                    }
                    _this.getCurrentUser()
                        .then(function (result) {
                        self.SecurityService.setAccount(result.userName, result.isSystemAdmin);
                        deferred.resolve(response);
                    }, function (err) {
                        deferred.reject(err);
                    });
                }, function (error) {
                    self.logout();
                    deferred.reject(error.data.error_description);
                });
                return deferred.promise;
            };
            _this.logout = function () {
                var deferred = _this.$q.defer();
                _this.SecurityService.logout();
                deferred.resolve("ok");
                return deferred.promise;
            };
            _this.getMyProfile = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetMyProfile")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.saveMyProfile = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SaveMyProfile", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        AccountService.prototype.subscriberSignup = function (viewModel) {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SubscriberSignup", viewModel)
                .then(function (result) {
                deferred.resolve(result.data);
            }, function (error) {
                deferred.reject(error.data.message);
            });
            return deferred.promise;
        };
        return AccountService;
    }(CHServiceBase));
    AccountServiceModule.AccountService = AccountService;
    function getInstance($http, $q, SecurityService, ENV) {
        return new AccountService($http, $q, SecurityService, ENV);
    }
    angular.module("AngularApp")
        .factory("AccountService", [
        "$http",
        "$q",
        "SecurityService",
        "ENV",
        getInstance
    ]);
})(AccountServiceModule || (AccountServiceModule = {}));
//# sourceMappingURL=~AccountService.js.map