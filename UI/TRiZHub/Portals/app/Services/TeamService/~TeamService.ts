
module TeamServiceModule {

    export interface ITeamService {
        teamDropdownList: () => ng.IPromise<TeamDropdownModel>;
        getTeam: (id: string) => ng.IPromise<TeamEditModel>;
        saveTeam: (viewModel: ProjectModel) => ng.IPromise<TeamEditModel>;
        teamGrid: (req: GridModel) => ng.IPromise<GridResultModel<TeamGridModel>>;
    }

    export class TeamService extends CHServiceBase implements ITeamService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Team/");
        }

        //#endregion

        teamDropdownList = (): ng.IPromise<TeamDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "TeamDropdown/")
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

        teamGrid = (req: GridResultModel<TeamGridModel>): ng.
            IPromise<GridResultModel<TeamGridModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TeamGrid", req)
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

        saveTeam = (viewModel: TeamEditModel): ng.IPromise<TeamEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveTeam", viewModel)
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

        getTeam = (id: string): ng.IPromise<TeamEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetTeam/" + id)
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
        return new TeamService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("TeamService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}