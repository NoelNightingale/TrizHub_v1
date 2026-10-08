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
var WorkTeamServiceModule;
(function (WorkTeamServiceModule) {
    var WorkTeamService = /** @class */ (function (_super) {
        __extends(WorkTeamService, _super);
        //#region Ctor
        function WorkTeamService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/WorkTeam/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.resolveGet = function (url) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + url)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.resolvePost = function (url, body) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + url, body)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.workTeamDropdownList = function () {
                return _this.resolveGet("WorkTeamDropdown/");
            };
            _this.workTeamGrid = function (req) {
                return _this.resolvePost("WorkTeamGrid", req);
            };
            _this.getWorkTeam = function (id) {
                return _this.resolveGet("GetWorkTeam/" + id);
            };
            _this.saveWorkTeam = function (viewModel) {
                return _this.resolvePost("SaveWorkTeam", viewModel);
            };
            _this.workTeamMembers = function (id, includeEnded) {
                return _this.resolveGet("WorkTeamMembers/" + id + "?includeEnded=" + includeEnded);
            };
            _this.saveWorkTeamMember = function (viewModel) {
                return _this.resolvePost("SaveWorkTeamMember", viewModel);
            };
            _this.deleteWorkTeamMember = function (id) {
                return _this.resolveGet("DeleteWorkTeamMember/" + id);
            };
            _this.workTeamAllocationTree = function (id, includeInactive) {
                return _this.resolveGet("WorkTeamAllocationTree/" + id + "?includeInactive=" + includeInactive);
            };
            _this.saveWorkTeamAllocations = function (workTeamId, selection) {
                return _this.resolvePost("SaveWorkTeamAllocations", { workTeamId: workTeamId, selection: selection });
            };
            /** start/end are yyyy-MM-dd keys. */
            _this.teamTypesForUser = function (userId, start, end) {
                return _this.resolveGet("TeamTypesForUser/" + userId + "?start=" + start + "&end=" + end);
            };
            return _this;
        }
        return WorkTeamService;
    }(CHServiceBase));
    WorkTeamServiceModule.WorkTeamService = WorkTeamService;
    function getInstance($http, $q, ENV) {
        return new WorkTeamService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("WorkTeamService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(WorkTeamServiceModule || (WorkTeamServiceModule = {}));
//# sourceMappingURL=~WorkTeamService.js.map