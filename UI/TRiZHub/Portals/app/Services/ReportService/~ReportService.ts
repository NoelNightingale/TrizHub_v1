
module ReportServiceModule {

    export interface IReportService {
        timesheetSummaryReport: (req: TimeSheetReportFillterModel) => ng.IPromise<TimeSheetReportFillterModel>;
        userSummary: (req: UserSummaryUserAccountIdModel) => ng.IPromise<UserSummaryUserAccountIdModel>;
        scoreCardSummary: (req: ScorecardReportModel) => ng.IPromise<ScorecardReportModel>;
        userAssetRegisterSummary: (req: UserSummaryAssetRegisterModel) => ng.IPromise<UserSummaryAssetRegisterModel>;

        reportApi: () => string;
    }

    export class ReportService extends CHServiceBase implements IReportService {

        urlRoot: string;
        mvcRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Report/");
            this.mvcRoot = ENV.serverLocation + "api/Report/";
        }

        //#endregion

        timesheetSummaryReport = (req: TimeSheetReportFillterModel): ng.IPromise<TimeSheetReportFillterModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TimesheetSummaryExcel", req)
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

        userSummary = (req: UserSummaryUserAccountIdModel): ng.IPromise<UserSummaryUserAccountIdModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "UserSummaryExcel", req)
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

        scoreCardSummary = (req: ScorecardReportModel): ng.IPromise<ScorecardReportModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardEmployeeSummaryExcel", req)
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

        userAssetRegisterSummary = (req: UserSummaryAssetRegisterModel): ng.IPromise<UserSummaryAssetRegisterModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "UserAssetRegisterSummaryExcel", req)
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



        reportApi = (): string => {
            return this.mvcRoot;
        };

    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new ReportService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ReportService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}