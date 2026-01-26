module BillingCycleServiceModule {

    export interface IBillingCycleService1 {

        billingCycleGet: (id: string) => ng.IPromise<BillingCycleModel>;
        billingCycleSave: (viewModel: BillingCycleModel) => ng.IPromise<BillingCycleModel>;
        billingCycleGrid: (req: GridModel) => ng.IPromise<GridResultModel<BillingCycleGridModel>>;
        billingCycleDelete: (viewModel: BillingCycleModel) => ng.IPromise<BillingCycleModel>;
        billingCycleDropdownList: () => ng.IPromise<BillingCycleDropdownModel>;
    }

    export class BillingCycleService extends CHServiceBase implements IBillingCycleService1 {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/BillingCycle/");
        }

        //#endregion

        billingCycleGet = (id: string): ng.IPromise<BillingCycleModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "BillingCycleGet/" + id)
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


        billingCycleGrid = (req: GridResultModel<BillingCycleGridModel>): ng.
            IPromise<GridResultModel<BillingCycleGridModel>> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "BillingCycleGrid", req)
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


        billingCycleSave = (viewModel: BillingCycleModel): ng.IPromise<BillingCycleModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "BillingCycleSave", viewModel)
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


        billingCycleListSave = (viewModel: ICollection<TimesheetGridModel>): ng.
            IPromise<ICollection<TimesheetGridModel>> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "BillingCycleListSave", viewModel)
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

        billingCycleDelete = (viewModel: BillingCycleModel): ng.IPromise<BillingCycleModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "BillingCycleDelete", viewModel)
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

        billingCycleDropdownList = (): ng.IPromise<BillingCycleDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "BillingCycleDropdown/")
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
        return new BillingCycleService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("BillingCycleService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}