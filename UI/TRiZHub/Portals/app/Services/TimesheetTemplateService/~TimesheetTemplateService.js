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
var TimesheetTemplateServiceModule;
(function (TimesheetTemplateServiceModule) {
    var TimesheetTemplateService = /** @class */ (function (_super) {
        __extends(TimesheetTemplateService, _super);
        function TimesheetTemplateService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/TimesheetTemplate/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            _this.list = function (userAccountId) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "List", { userAccountId: userAccountId })
                    .then(function (result) { deferred.resolve(result.data); }, function (error) { deferred.reject(error.data.message); });
                return deferred.promise;
            };
            _this.save = function (model) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "Save", model)
                    .then(function (result) { deferred.resolve(result.data); }, function (error) { deferred.reject(error.data.message); });
                return deferred.promise;
            };
            _this.rename = function (id, label) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "Rename", { id: id, label: label })
                    .then(function (result) { deferred.resolve(result.data); }, function (error) { deferred.reject(error.data.message); });
                return deferred.promise;
            };
            _this.deleteTemplate = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "Delete", { id: id })
                    .then(function (result) { deferred.resolve(result.data); }, function (error) { deferred.reject(error.data.message); });
                return deferred.promise;
            };
            return _this;
        }
        return TimesheetTemplateService;
    }(CHServiceBase));
    TimesheetTemplateServiceModule.TimesheetTemplateService = TimesheetTemplateService;
    function getInstance($http, $q, ENV) {
        return new TimesheetTemplateService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("TimesheetTemplateService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(TimesheetTemplateServiceModule || (TimesheetTemplateServiceModule = {}));
//# sourceMappingURL=~TimesheetTemplateService.js.map