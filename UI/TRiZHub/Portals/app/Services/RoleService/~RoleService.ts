
module RoleServiceModule {

    export interface IRoleService {

        roleSave: (viewModel: RoleViewModel) => ng.IPromise<RoleViewModel>;
        roleGrid: (req: GridModel) => ng.IPromise<GridResultModel<RoleGridModel>>;
        roleGet: (id: string) => ng.IPromise<RoleViewModel>;
        rolePrivileges: (id: string) => ng.IPromise<List<PermissionViewModel>>;
    }

    export class RoleService extends CHServiceBase implements IRoleService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Role/");
        }

        //#endregion

        roleSave = (viewModel: RoleViewModel): ng.IPromise<RoleViewModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "RoleSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        roleGrid = (req: GridResultModel<RoleGridModel>): ng.IPromise<GridResultModel<RoleGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "RoleGrid", req)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        roleGet = (id: string): ng.IPromise<RoleViewModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "RoleGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        rolePrivileges = (id: string): ng.IPromise<List<PermissionViewModel>> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "RolePrivileges/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new RoleService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("RoleService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}