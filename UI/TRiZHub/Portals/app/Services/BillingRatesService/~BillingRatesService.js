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
var BillingRatesServiceModule;
(function (BillingRatesServiceModule) {
    var BillingRatesService = /** @class */ (function (_super) {
        __extends(BillingRatesService, _super);
        //#region Ctor
        function BillingRatesService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/BillingRates/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.billingRatesGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingRatesGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.effectiveRatesGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "EffectiveRatesGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingRatesSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SaveBillingRates", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingRatesGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "BillingRatesGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingRatesDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingRatesDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectTeamRates = function (projectId, asOfDate) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ProjectTeamRates", { projectId: projectId, asOfDate: asOfDate })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userRatesForProjectContext = function (userId, projectId) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserRatesForProjectContext?userId=" + userId + "&projectId=" + projectId)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.clientTeamRates = function (clientId, asOfDate) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ClientTeamRates", { clientId: clientId, asOfDate: asOfDate })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userRatesForClientContext = function (userId, clientId) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserRatesForClientContext?userId=" + userId + "&clientId=" + clientId)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userRatesAsOf = function (userAccountId, asOfDate) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserRatesAsOf", { userAccountId: userAccountId, asOfDate: asOfDate })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.filterOptions = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "FilterOptions", req || {})
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return BillingRatesService;
    }(CHServiceBase));
    BillingRatesServiceModule.BillingRatesService = BillingRatesService;
    function getInstance($http, $q, ENV) {
        return new BillingRatesService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("BillingRatesService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(BillingRatesServiceModule || (BillingRatesServiceModule = {}));
//# sourceMappingURL=~BillingRatesService.js.map