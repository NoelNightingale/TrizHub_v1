
module WorkGroupServiceModule {

    export interface IWorkGroupService {
        tree: () => ng.IPromise<WorkGroupTreeModel>;
        saveGroup: (model: WorkGroupSaveModel) => ng.IPromise<string>;
        deleteGroup: (id: string) => ng.IPromise<boolean>;
        saveTeam: (model: WorkTeamSaveModel) => ng.IPromise<string>;
        deleteTeam: (id: string) => ng.IPromise<boolean>;
        savePerson: (model: WorkTeamPersonSaveModel) => ng.IPromise<string>;
        deletePerson: (id: string) => ng.IPromise<boolean>;
    }

    export class WorkGroupService extends CHServiceBase implements IWorkGroupService {

        urlRoot: string;

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/WorkGroup/");
        }

        private resolveGet = (url: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + url)
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };

        private resolvePost = (url: string, body: any): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + url, body)
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };

        tree = (): ng.IPromise<WorkGroupTreeModel> => {
            return this.resolveGet("Tree");
        };

        saveGroup = (model: WorkGroupSaveModel): ng.IPromise<string> => {
            return this.resolvePost("SaveGroup", model);
        };

        deleteGroup = (id: string): ng.IPromise<boolean> => {
            return this.resolvePost("DeleteGroup", { id: id });
        };

        saveTeam = (model: WorkTeamSaveModel): ng.IPromise<string> => {
            return this.resolvePost("SaveTeam", model);
        };

        deleteTeam = (id: string): ng.IPromise<boolean> => {
            return this.resolvePost("DeleteTeam", { id: id });
        };

        savePerson = (model: WorkTeamPersonSaveModel): ng.IPromise<string> => {
            return this.resolvePost("SavePerson", model);
        };

        deletePerson = (id: string): ng.IPromise<boolean> => {
            return this.resolvePost("DeletePerson", { id: id });
        };
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new WorkGroupService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("WorkGroupService",
            [
                "$http",
                "$q",
                "ENV",
                getInstance
            ]);
}
