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
var EmployerServiceModule;
(function (EmployerServiceModule) {
    var EmployerService = /** @class */ (function (_super) {
        __extends(EmployerService, _super);
        //#region Ctor
        function EmployerService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Employer/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.employerGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "EmployerGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.employerDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "EmployerDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.allEmployerDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "AllEmployerDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.employerGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "EmployerGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.employerSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "EmployerSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deactivateEmployer = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "Deactivate/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.activateEmployer = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "Activate/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deleteEmployer = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.delete(_this.urlRoot + "Delete/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return EmployerService;
    }(CHServiceBase));
    EmployerServiceModule.EmployerService = EmployerService;
    function getInstance($http, $q, ENV) {
        return new EmployerService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("EmployerService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(EmployerServiceModule || (EmployerServiceModule = {}));
//# sourceMappingURL=~EmployerService.js.map