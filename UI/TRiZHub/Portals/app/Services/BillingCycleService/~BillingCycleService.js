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
var BillingCycleServiceModule;
(function (BillingCycleServiceModule) {
    var BillingCycleService = /** @class */ (function (_super) {
        __extends(BillingCycleService, _super);
        //#region Ctor
        function BillingCycleService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/BillingCycle/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.billingCycleGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "BillingCycleGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingCycleGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingCycleGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingCycleSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingCycleSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingCycleListSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingCycleListSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingCycleDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "BillingCycleDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.billingCycleDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "BillingCycleDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return BillingCycleService;
    }(CHServiceBase));
    BillingCycleServiceModule.BillingCycleService = BillingCycleService;
    function getInstance($http, $q, ENV) {
        return new BillingCycleService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("BillingCycleService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(BillingCycleServiceModule || (BillingCycleServiceModule = {}));
//# sourceMappingURL=~BillingCycleService.js.map