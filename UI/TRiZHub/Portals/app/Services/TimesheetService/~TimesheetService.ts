
module TimesheetServiceModule {

    export interface ITimesheetService {
        timesheetGrid: (req: GridModel) => ng.IPromise<GridResultModel<TimesheetGridModel>>;
        timesheetGet: (id: string) => ng.IPromise<TimesheetModel>;
        timesheetSave: (viewModel: TimesheetModel) => ng.IPromise<TimesheetModel>;
        timesheetListSave: (viewModel: TimesheetModel) => ng.IPromise<TimesheetModel>;
        timesheetDelete: (viewModel: TimesheetModel) => ng.IPromise<TimesheetModel>;
    }

    export class TimesheetService extends CHServiceBase implements ITimesheetService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Timesheet/");
        }

        //#endregion

        timesheetGrid = (req: GridResultModel<TimesheetGridModel>): ng.IPromise<GridResultModel<TimesheetGridModel>> => {
            const deferred = this.$q.defer();
            var sd = new Date(Date.UTC(req.startDate.getFullYear(), req.startDate.getMonth(), req.startDate.getDate(), 0, 0, 0));
            var ed = new Date(Date.UTC(req.endDate.getFullYear(), req.endDate.getMonth(), req.endDate.getDate(), 0, 0, 0));

            req.startDate = sd;
            req.endDate = ed;

            this.$http.post(this.urlRoot + "TimesheetGrid", req)
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

        timesheetGet = (id: string): ng.IPromise<TimesheetModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "TimesheetGet/" + id)
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

        timesheetSave = (viewModel: TimesheetModel): ng.IPromise<TimesheetModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TimesheetSave", viewModel)
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

        timesheetListSave = (viewModel: ICollection<TimesheetGridModel>): ng.IPromise<ICollection<TimesheetGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TimesheetListSave", viewModel)
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

        timesheetDelete = (viewModel: TimesheetModel): ng.IPromise<TimesheetModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TimesheetDelete", viewModel)
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
        return new TimesheetService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("TimesheetService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}