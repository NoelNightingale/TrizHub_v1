module MasterDataServiceModule {

    export interface IMasterDataService {
        settings: () => ng.IPromise<SettingsModel>;
        profileSave: (viewModel: ProfileViewModel) => ng.IPromise<ProfileViewModel>;
        profileGet: () => ng.IPromise<ProfileViewModel>;
        settingsSave: (viewModel: SettingsModel) => ng.IPromise<SettingsModel>;
        settingsGet: () => ng.IPromise<SettingsModel>;
    }

    export class MasterDataService extends CHServiceBase implements IMasterDataService {

        //#region ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/MasterData/");
        }

        //#endregion

        settings = (): ng.IPromise<SettingsModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "Settings")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });

            return deferred.promise;
        };

        //#region Profile Methods

        profileGet = (): ng.IPromise<ProfileViewModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProfileGet")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });
            return deferred.promise;
        };

        profileSave = (viewModel: ProfileViewModel): ng.IPromise<ProfileViewModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ProfileSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });
            return deferred.promise;
        };

        //#endregion

        //#region Settings methods

        settingsGet = (): ng.IPromise<SettingsModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "SettingsGet")
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

        settingsSave = (viewModel: SettingsModel): ng.IPromise<SettingsModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SettingsSave", viewModel)
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

        //#endregion
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new MasterDataService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("MasterDataService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}