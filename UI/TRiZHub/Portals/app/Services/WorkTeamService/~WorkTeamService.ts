
module WorkTeamServiceModule {

    export interface IWorkTeamService {
        workTeamDropdownList: () => ng.IPromise<WorkTeamDropdownModel>;
        workTeamGrid: (req: any) => ng.IPromise<GridResultModel<WorkTeamGridModel>>;
        getWorkTeam: (id: string) => ng.IPromise<WorkTeamEditModel>;
        saveWorkTeam: (viewModel: WorkTeamEditModel) => ng.IPromise<WorkTeamEditModel>;
        workTeamMembers: (id: string, includeEnded: boolean) => ng.IPromise<Array<WorkTeamMemberModel>>;
        saveWorkTeamMember: (viewModel: WorkTeamMemberModel) => ng.IPromise<WorkTeamMemberModel>;
        deleteWorkTeamMember: (id: string) => ng.IPromise<boolean>;
        workTeamAllocationTree: (id: string, includeInactive: boolean) => ng.IPromise<Array<any>>;
        saveWorkTeamAllocations: (workTeamId: string, selection: Array<any>) => ng.IPromise<boolean>;
        teamTypesForUser: (userId: string, start: string, end: string) => ng.IPromise<Array<WorkTeamTypePeriodModel>>;
    }

    export class WorkTeamService extends CHServiceBase implements IWorkTeamService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/WorkTeam/");
        }

        //#endregion

        private resolveGet = (url: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + url)
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

        private resolvePost = (url: string, body: any): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + url, body)
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

        workTeamDropdownList = (): ng.IPromise<WorkTeamDropdownModel> => {
            return this.resolveGet("WorkTeamDropdown/");
        };

        workTeamGrid = (req: any): ng.IPromise<GridResultModel<WorkTeamGridModel>> => {
            return this.resolvePost("WorkTeamGrid", req);
        };

        getWorkTeam = (id: string): ng.IPromise<WorkTeamEditModel> => {
            return this.resolveGet("GetWorkTeam/" + id);
        };

        saveWorkTeam = (viewModel: WorkTeamEditModel): ng.IPromise<WorkTeamEditModel> => {
            return this.resolvePost("SaveWorkTeam", viewModel);
        };

        workTeamMembers = (id: string, includeEnded: boolean): ng.IPromise<Array<WorkTeamMemberModel>> => {
            return this.resolveGet("WorkTeamMembers/" + id + "?includeEnded=" + includeEnded);
        };

        saveWorkTeamMember = (viewModel: WorkTeamMemberModel): ng.IPromise<WorkTeamMemberModel> => {
            return this.resolvePost("SaveWorkTeamMember", viewModel);
        };

        deleteWorkTeamMember = (id: string): ng.IPromise<boolean> => {
            return this.resolveGet("DeleteWorkTeamMember/" + id);
        };

        workTeamAllocationTree = (id: string, includeInactive: boolean): ng.IPromise<Array<any>> => {
            return this.resolveGet("WorkTeamAllocationTree/" + id + "?includeInactive=" + includeInactive);
        };

        saveWorkTeamAllocations = (workTeamId: string, selection: Array<any>): ng.IPromise<boolean> => {
            return this.resolvePost("SaveWorkTeamAllocations", { workTeamId: workTeamId, selection: selection });
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
