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
var ProjectServiceModule;
(function (ProjectServiceModule) {
    var ProjectService = /** @class */ (function (_super) {
        __extends(ProjectService, _super);
        //#region Ctor
        function ProjectService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Project/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.projectGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ProjectsGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ProjectGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ProjectSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.subProjectGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SubProjectsGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.subProjectSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SubProjectSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.subProjectGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "SubProjectGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectAndSubProjectDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ProjectAndSubProjectDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.getUserAllocatedProjects = function (id, includeInactive) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "GetUserAllocatedProjects/" + id + "?includeInactive=" + includeInactive)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ProjectDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectDropdownListForClientReporter = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ProjectDropdownForClientReporter/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.allProjectDropdownListForClientReporter = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "AllProjectDropdownForClientReporter/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.allProjectDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "AllProjectDropdownForClientReporter/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.projectTypeDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ProjectTypeDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deleteProject = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "DeleteProject/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deleteSubProject = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "DeleteSubProject/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userIdentityProjects = function (id, includeInactive) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserIdentityProjectList/?id=" + id + '&includeInactive=' + includeInactive)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.saveUserIdentityProjects = function (id, projects) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SaveUserIdentityProjectList", { userId: id, projects: projects })
                    .then(function (result) {
                    deferred.resolve(result);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return ProjectService;
    }(CHServiceBase));
    ProjectServiceModule.ProjectService = ProjectService;
    function getInstance($http, $q, ENV) {
        return new ProjectService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("ProjectService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(ProjectServiceModule || (ProjectServiceModule = {}));
//# sourceMappingURL=~ProjectService.js.map