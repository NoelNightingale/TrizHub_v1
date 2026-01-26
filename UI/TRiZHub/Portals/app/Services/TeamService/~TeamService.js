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
var TeamServiceModule;
(function (TeamServiceModule) {
    var TeamService = /** @class */ (function (_super) {
        __extends(TeamService, _super);
        //#region Ctor
        function TeamService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Team/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.teamDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "TeamDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.teamGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TeamGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.saveTeam = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SaveTeam", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.getTeam = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetTeam/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return TeamService;
    }(CHServiceBase));
    TeamServiceModule.TeamService = TeamService;
    function getInstance($http, $q, ENV) {
        return new TeamService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("TeamService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(TeamServiceModule || (TeamServiceModule = {}));
//# sourceMappingURL=~TeamService.js.map