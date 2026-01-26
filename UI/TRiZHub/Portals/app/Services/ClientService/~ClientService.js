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
var ClientServiceModule;
(function (ClientServiceModule) {
    var ClientService = /** @class */ (function (_super) {
        __extends(ClientService, _super);
        //#region Ctor
        function ClientService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Client/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.clientGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ClientsGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.getClientReporters = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetClientReporters/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.addClientReporter = function (id, userId) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "AddClientReporter?clientid=" + id + "&userId=" + userId)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.removeClientReporter = function (id, userId) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "RemoveClientReporter?clientid=" + id + "&userId=" + userId)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.clientGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ClientGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.clientSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ClientSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.clientDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ClientDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.clientReporterDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ClientReporterDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deleteClient = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "DeleteClient/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return ClientService;
    }(CHServiceBase));
    ClientServiceModule.ClientService = ClientService;
    function getInstance($http, $q, ENV) {
        return new ClientService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("ClientService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(ClientServiceModule || (ClientServiceModule = {}));
//# sourceMappingURL=~ClientService.js.map