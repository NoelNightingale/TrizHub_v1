
module ClientServiceModule {

    export interface IClientService {
        clientGrid: (req: GridModel) => ng.IPromise<GridResultModel<ClientGridModel>>;
        clientGet: (id: string) => ng.IPromise<ClientModel>;
        clientSave: (viewModel: ClientModel) => ng.IPromise<ClientModel>;

        clientDropdownList: () => ng.IPromise<ClientDropdownModel>;

        clientReporterDropdownList: () => ng.IPromise<ClientDropdownModel>;

        getClientReporters: (id: string) => ng.IPromise<UserDropdownModel>;

        addClientReporter: (id: string, userId: string) => ng.IPromise<UserDropdownModel>;
        removeClientReporter: (id: string, userId: string) => ng.IPromise<UserDropdownModel>;

    }

    export class ClientService extends CHServiceBase implements IClientService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Client/");
        }

        //#endregion

        clientGrid = (req: GridResultModel<ClientGridModel>): ng.IPromise<GridResultModel<ClientGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ClientsGrid", req)
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

        getClientReporters = (id: string): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetClientReporters/" + id)
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


        addClientReporter = (id: string, userId: string): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "AddClientReporter?clientid=" + id + "&userId=" + userId)
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

        removeClientReporter = (id: string, userId: string): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "RemoveClientReporter?clientid=" + id + "&userId=" + userId)
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

        clientGet = (id: string): ng.IPromise<ClientModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ClientGet/" + id)
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

        clientSave = (viewModel: ClientModel): ng.IPromise<ClientModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ClientSave", viewModel)
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


        clientDropdownList = (): ng.IPromise<ClientDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ClientDropdown/")
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


        clientReporterDropdownList = (): ng.IPromise<ClientDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ClientReporterDropdown/")
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

        deleteClient = (id: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "DeleteClient/" + id)
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
        return new ClientService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ClientService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}