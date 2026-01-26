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
var EnumServiceModule;
(function (EnumServiceModule) {
    var EnumService = /** @class */ (function (_super) {
        __extends(EnumService, _super);
        //#region ctor
        function EnumService($http, $rootScope, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Enums/") || this;
            _this.$http = $http;
            _this.$rootScope = $rootScope;
            _this.ENV = ENV;
            //#endregion
            _this.loadStatusType = function () {
                return _this.$http.get(_this.urlRoot + "StatusTypeEnum")
                    .then(function (result) {
                    _this.statusType = result.data;
                    _this.checkAllEnumsLoaded();
                });
            };
            _this.loadSecurityType = function () {
                return _this.$http.get(_this.urlRoot + "SecurityEnum")
                    .then(function (result) {
                    _this.securityTypes = result.data;
                    _this.checkAllEnumsLoaded();
                });
            };
            _this.loadClientType = function () {
                return _this.$http.get(_this.urlRoot + "ClientTypeEnum")
                    .then(function (result) {
                    _this.clientTypes = result.data;
                    _this.checkAllEnumsLoaded();
                });
            };
            _this.loadScorecardScoreType = function () {
                return _this.$http.get(_this.urlRoot + "ScorecardScoreTypeEnum")
                    .then(function (result) {
                    _this.scorecardScoreTypes = result.data;
                    _this.checkAllEnumsLoaded();
                });
            };
            _this.checkAllEnumsLoaded = function () {
                if (_this.statusType.length > 0 &&
                    _this.securityTypes.length > 0 &&
                    _this.clientTypes.length > 0 &&
                    _this.scorecardScoreTypes.length > 0) {
                    _this.allEnumsLoaded = true;
                    _this.$rootScope.$emit(_this.enumsLoadedEvent, _this.allEnumsLoaded);
                }
            };
            _this.getStatusTypes = function () {
                return _this.statusType;
            };
            _this.getSecurityTypes = function () {
                return _this.securityTypes;
            };
            _this.getClientTypes = function () {
                return _this.clientTypes;
            };
            _this.getScorecardScoreTypes = function () {
                return _this.scorecardScoreTypes;
            };
            _this.getAllEnumsLoaded = function () {
                return _this.allEnumsLoaded;
            };
            _this.statusType = [];
            _this.securityTypes = [];
            _this.clientTypes = [];
            _this.scorecardScoreTypes = [];
            _this.loadStatusType();
            _this.loadSecurityType();
            _this.loadClientType();
            _this.loadScorecardScoreType();
            return _this;
        }
        return EnumService;
    }(CHServiceBase));
    EnumServiceModule.EnumService = EnumService;
    function getInstance($http, $rootScope, ENV) {
        return new EnumService($http, $rootScope, ENV);
    }
    angular.module("AngularApp")
        .factory("EnumService", [
        "$http",
        "$rootScope",
        "ENV",
        getInstance
    ]);
})(EnumServiceModule || (EnumServiceModule = {}));
//# sourceMappingURL=~EnumService.js.map