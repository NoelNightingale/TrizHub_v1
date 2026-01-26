
module ActivityServiceModule {

    export interface IActivityService {
        activityDropdownList: () => ng.IPromise<ActivityDropdownModel>;
        getActivity: (id: string) => ng.IPromise<ActivityEditModel>;
        saveActivity: (viewModel: ProjectModel) => ng.IPromise<ActivityEditModel>;
        activityGrid: (req: GridModel) => ng.IPromise<GridResultModel<ActivityGridModel>>;

    }

    export class ActivityService extends CHServiceBase implements IActivityService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Activity/");
        }

        //#endregion

        activityDropdownList = (): ng.IPromise<ActivityDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ActivityDropdown/")
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


        saveActivity = (viewModel: ActivityEditModel): ng.IPromise<ActivityEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveActivity", viewModel)
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

        getActivity = (id: string): ng.IPromise<ActivityEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetActivity/" + id)
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

        activityGrid = (req: GridResultModel<ActivityGridModel>): ng.
            IPromise<GridResultModel<ActivityGridModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ActivityGrid", req)
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
        return new ActivityService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ActivityService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}