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
var ActivityServiceModule;
(function (ActivityServiceModule) {
    var ActivityService = /** @class */ (function (_super) {
        __extends(ActivityService, _super);
        //#region Ctor
        function ActivityService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Activity/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.activityDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ActivityDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.saveActivity = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SaveActivity", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.getActivity = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetActivity/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.activityGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ActivityGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return ActivityService;
    }(CHServiceBase));
    ActivityServiceModule.ActivityService = ActivityService;
    function getInstance($http, $q, ENV) {
        return new ActivityService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("ActivityService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(ActivityServiceModule || (ActivityServiceModule = {}));
//# sourceMappingURL=~ActivityService.js.map