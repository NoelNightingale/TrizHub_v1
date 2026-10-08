
module TimesheetTemplateServiceModule {

    export interface ITimesheetTemplateService {
        list: (userAccountId: string) => ng.IPromise<TimesheetTemplateModel[]>;
        save: (model: any) => ng.IPromise<TimesheetTemplateModel>;
        rename: (id: string, label: string) => ng.IPromise<TimesheetTemplateModel>;
        deleteTemplate: (id: string) => ng.IPromise<any>;
    }

    export class TimesheetTemplateService extends CHServiceBase implements ITimesheetTemplateService {

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/TimesheetTemplate/");
        }

        list = (userAccountId: string): ng.IPromise<TimesheetTemplateModel[]> => {
            const deferred = this.$q.defer<any>();
            this.$http.post(this.urlRoot + "List", { userAccountId: userAccountId })
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };

        save = (model: any): ng.IPromise<TimesheetTemplateModel> => {
            const deferred = this.$q.defer<any>();
            this.$http.post(this.urlRoot + "Save", model)
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };

        rename = (id: string, label: string): ng.IPromise<TimesheetTemplateModel> => {
            const deferred = this.$q.defer<any>();
            this.$http.post(this.urlRoot + "Rename", { id: id, label: label })
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };

        deleteTemplate = (id: string): ng.IPromise<any> => {
            const deferred = this.$q.defer<any>();
            this.$http.post(this.urlRoot + "Delete", { id: id })
                .then(
                    result => { deferred.resolve(result.data); },
                    error => { deferred.reject(error.data.message); }
                );
            return deferred.promise;
        };
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new TimesheetTemplateService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("TimesheetTemplateService",
            [
                "$http",
                "$q",
                "ENV",
                getInstance
            ]);
}
