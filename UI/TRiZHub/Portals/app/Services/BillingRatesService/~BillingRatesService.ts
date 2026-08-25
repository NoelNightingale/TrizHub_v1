module BillingRatesServiceModule {

    export interface IBillingRateService {

        billingRatesSave: (viewModel: BillingRatesEditModel) => ng.IPromise<BillingRatesEditModel>;
        billingRatesGrid: (req: GridModel) => ng.IPromise<GridResultModel<BillingRatesGridModel>>;
        billingRatesGet: (id: string) => ng.IPromise<BillingRatesEditModel>;
        billingRatesDelete: (viewModel: BillingRatesEditModel) => ng.IPromise<BillingRatesEditModel>;
        projectTeamRates: (projectId: string, asOfDate: any) => ng.IPromise<any>;
        userRatesForProjectContext: (userId: string, projectId: string) => ng.IPromise<any>;

    }

    export class BillingRatesService extends CHServiceBase implements IBillingRateService {

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/BillingRates/");
        }

        //#endregion

        billingRatesGrid = (req: GridResultModel<BillingRatesGridModel>): ng.
            IPromise<GridResultModel<BillingRatesGridModel>> => {
                var deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "BillingRatesGrid", req)
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

        billingRatesSave = (viewModel: BillingRatesEditModel): ng.IPromise<BillingRatesEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveBillingRates", viewModel)
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

        billingRatesGet = (id: string): ng.IPromise<BillingRatesEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "BillingRatesGet/" + id)
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

        billingRatesDelete = (viewModel: BillingRatesEditModel): ng.IPromise<BillingRatesEditModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "BillingRatesDelete", viewModel)
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

        projectTeamRates = (projectId: string, asOfDate: any): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ProjectTeamRates", { projectId: projectId, asOfDate: asOfDate })
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

        userRatesForProjectContext = (userId: string, projectId: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserRatesForProjectContext?userId=" + userId + "&projectId=" + projectId)
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
        return new BillingRatesService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("BillingRatesService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}
