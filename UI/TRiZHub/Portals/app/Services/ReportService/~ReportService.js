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
var ReportServiceModule;
(function (ReportServiceModule) {
    var ReportService = /** @class */ (function (_super) {
        __extends(ReportService, _super);
        //#region Ctor
        function ReportService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Report/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.timesheetSummaryReport = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TimesheetSummaryExcel", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userSummary = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserSummaryExcel", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scoreCardSummary = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardEmployeeSummaryExcel", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userAssetRegisterSummary = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserAssetRegisterSummaryExcel", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.reportApi = function () {
                return _this.mvcRoot;
            };
            _this.mvcRoot = ENV.serverLocation + "api/Report/";
            return _this;
        }
        return ReportService;
    }(CHServiceBase));
    ReportServiceModule.ReportService = ReportService;
    function getInstance($http, $q, ENV) {
        return new ReportService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("ReportService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(ReportServiceModule || (ReportServiceModule = {}));
//# sourceMappingURL=~ReportService.js.map