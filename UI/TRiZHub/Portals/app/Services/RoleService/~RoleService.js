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
var RoleServiceModule;
(function (RoleServiceModule) {
    var RoleService = /** @class */ (function (_super) {
        __extends(RoleService, _super);
        //#region Ctor
        function RoleService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Role/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.roleSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "RoleSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.roleGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "RoleGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.roleGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "RoleGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.rolePrivileges = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "RolePrivileges/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return RoleService;
    }(CHServiceBase));
    RoleServiceModule.RoleService = RoleService;
    function getInstance($http, $q, ENV) {
        return new RoleService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("RoleService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(RoleServiceModule || (RoleServiceModule = {}));
//# sourceMappingURL=~RoleService.js.map