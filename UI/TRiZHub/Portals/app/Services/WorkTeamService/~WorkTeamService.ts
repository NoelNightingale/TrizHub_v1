
module WorkTeamServiceModule {

    export interface IWorkTeamService {
        teamTypesForUser: (userId: string, start: string, end: string) => ng.IPromise<Array<WorkTeamTypePeriodModel>>;
    }

    /** Timesheet team-type preselect. Returns no periods until work-group reach is specified. */
    export class WorkTeamService extends CHServiceBase implements IWorkTeamService {

        urlRoot: string;

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/WorkTeam/");
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

        /** start/end are yyyy-MM-dd keys. */
        teamTypesForUser = (userId: string, start: string, end: string): ng.IPromise<Array<WorkTeamTypePeriodModel>> => {
            return this.resolveGet("TeamTypesForUser/" + userId + "?start=" + start + "&end=" + end);
        };
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new WorkTeamService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("WorkTeamService",
            [
                "$http",
                "$q",
                "ENV",
                getInstance
            ]);
}
