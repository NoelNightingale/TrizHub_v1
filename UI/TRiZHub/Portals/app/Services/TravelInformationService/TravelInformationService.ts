module TravelInformationServiceModule {

    export interface ITravelInformationService {

        travelInformationSave: (viewModel: TravelInformationEditModel) => ng.IPromise<TravelInformationEditModel>;
        travelInformationGrid: (req: GridModel) => ng.IPromise<GridResultModel<TravelInformationGridModel>>;
        travelInformationGet: (id: string) => ng.IPromise<TravelInformationEditModel>;
        travelInformationDelete: (viewModel: TravelInformationEditModel) => ng.IPromise<TravelInformationEditModel>;

    }

    export class TravelInformationService extends CHServiceBase implements ITravelInformationService {

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/TravelInformation/");
        }

        //#endregion

        travelInformationGrid = (req: GridResultModel<TravelInformationGridModel>): ng.
            IPromise<GridResultModel<TravelInformationGridModel>> => {
                var deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "TravelInformationGrid", req)
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

        travelInformationSave = (viewModel: TravelInformationEditModel): ng.IPromise<TravelInformationEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveTravelInformation", viewModel)
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

        travelInformationGet = (id: string): ng.IPromise<TravelInformationEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "TravelInformationGet/" + id)
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

        travelInformationDelete = (viewModel: TravelInformationEditModel): ng.IPromise<TravelInformationEditModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TravelInformationDelete", viewModel)
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
        return new TravelInformationService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("TravelInformationService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}