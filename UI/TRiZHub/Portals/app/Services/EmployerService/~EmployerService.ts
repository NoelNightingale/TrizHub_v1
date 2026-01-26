module EmployerServiceModule {
    export interface IEmployerService {
        employerGrid: (req: GridModel) => ng.IPromise<GridResultModel<EmployerGridModel>>;
        employerDropdownList: () => ng.IPromise<EmployerModel>;
        allEmployerDropdownList: () => ng.IPromise<EmployerModel>;

        employerGet: (id: string) => ng.IPromise<EmployerModel>;
        employerSave: (viewModel: EmployerModel) => ng.IPromise<EmployerModel>;

        activateEmployer: (id: string) => ng.IPromise<EmployerModel>;
        deactivateEmployer: (id: string) => ng.IPromise<EmployerModel>;
        deleteEmployer: (id: string) => ng.IPromise<any>;
    }

    export class EmployerService extends CHServiceBase implements IEmployerService {
        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Employer/");
        }

        //#endregion

        employerGrid = (req: GridResultModel<EmployerGridModel>): ng.IPromise<GridResultModel<EmployerGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "EmployerGrid", req)
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

        employerDropdownList = (): ng.IPromise<EmployerModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "EmployerDropdown/")
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

        allEmployerDropdownList = (): ng.IPromise<EmployerModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "AllEmployerDropdown/")
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

        employerGet = (id: string): ng.IPromise<EmployerModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "EmployerGet/" + id)
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

        employerSave = (viewModel: EmployerModel): ng.IPromise<EmployerModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "EmployerSave", viewModel)
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

        deactivateEmployer = (id: string): ng.IPromise<EmployerModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "Deactivate/" + id)
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

        activateEmployer = (id: string): ng.IPromise<EmployerModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "Activate/" + id)
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

        deleteEmployer = (id: string): ng.IPromise<any> => {
            var deferred = this.$q.defer();
            this.$http.delete(this.urlRoot + "Delete/" + id)
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
        return new EmployerService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("EmployerService",
            [
                "$http",
                "$q",
                "ENV",
                getInstance
            ]);
}