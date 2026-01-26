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
var TimesheetServiceModule;
(function (TimesheetServiceModule) {
    var TimesheetService = /** @class */ (function (_super) {
        __extends(TimesheetService, _super);
        //#region Ctor
        function TimesheetService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Timesheet/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.timesheetGrid = function (req) {
                var deferred = _this.$q.defer();
                var sd = new Date(Date.UTC(req.startDate.getFullYear(), req.startDate.getMonth(), req.startDate.getDate(), 0, 0, 0));
                var ed = new Date(Date.UTC(req.endDate.getFullYear(), req.endDate.getMonth(), req.endDate.getDate(), 0, 0, 0));
                req.startDate = sd;
                req.endDate = ed;
                _this.$http.post(_this.urlRoot + "TimesheetGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.timesheetGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "TimesheetGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.timesheetSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TimesheetSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.timesheetListSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TimesheetListSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.timesheetDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TimesheetDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return TimesheetService;
    }(CHServiceBase));
    TimesheetServiceModule.TimesheetService = TimesheetService;
    function getInstance($http, $q, ENV) {
        return new TimesheetService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("TimesheetService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(TimesheetServiceModule || (TimesheetServiceModule = {}));
//# sourceMappingURL=~TimesheetService.js.map