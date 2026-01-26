module AccountServiceModule {

    export interface IAccountService {

        init: () => ng.IPromise<any>;
        getCurrentUser: () => ng.IPromise<CurrentUserModel>;
        login: () => ng.IPromise<any>;
        logout: () => ng.IPromise<any>;
        getMyProfile: () => ng.IPromise<ProfileViewModel>;
        saveMyProfile: (viewModel: any) => ng.IPromise<any>;
        subscriberSignup: (viewModel: any) => ng.IPromise<any>;
    }

    export class AccountService extends CHServiceBase implements IAccountService {

        //#region ctor

        constructor(private $http: angular.IHttpService,
            private $q: ng.IQService,
            private SecurityService: SecurityServiceModule.SecurityService,
            private ENV: any) {
            super(ENV.serverLocation + "api/Account/");
        }

        //#endregion

        init = (): ng.IPromise<any> => {
            const self = this;
            //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.
            const deferred = this.$q.defer();
            if (this.SecurityService.init()) {
                this.getCurrentUser()
                    .then(
                        result => {
                            deferred.resolve(result);
                        },
                        err => {
                            deferred.reject(err);
                        });
            } else {
                this.logout();
                deferred.reject("Not logged in!");
            }
            return deferred.promise;
        };

        getCurrentUser = (): ng.IPromise<CurrentUserModel> => {
            //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetCurrentUser")
                .then(
                    (result: any) => {
                        this.SecurityService
                            .setCurrentUserDetails(result.data.id,
                                result.data.displayName,
                                result.data.allowedPrivileges,
                                result.data.isSystemAdmin,
                                result.data.isUserProfileComplete,
                                result.data.isUserApproved);
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });

            return deferred.promise;
        };

        login = (): ng.IPromise<any> => {
            const data = "grant_type=password&username=demo&password=demo&scope=account";
            const deferred = this.$q.defer();
            const self = this;
            //this is required when used in callback functions, since 'this' refers to the global scope and not the class level scope.

            this.$http.post(this.ENV.serverLocation + "/token",
                    data,
                    {
                        headers: { 'Content-Type': "application/x-www-form-urlencoded" }
                    })
                .then((response: any) => {
                    if (!self.SecurityService.init()) {
                        self.SecurityService.login(response.data.access_token, "demo", false);
                    }
                    this.getCurrentUser()
                        .then(
                            result => {
                                self.SecurityService.setAccount(result.userName, result.isSystemAdmin);
                                deferred.resolve(response);
                            },
                            err => {
                                deferred.reject(err);
                            });
                },
                (error: any) => {
                    self.logout();
                    deferred.reject(error.data.error_description);
                });

            return deferred.promise;
        };

        logout = (): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.SecurityService.logout();
            deferred.resolve("ok");

            return deferred.promise;
        };

        getMyProfile = (): ng.IPromise<ProfileViewModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetMyProfile")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });

            return deferred.promise;
        };

        subscriberSignup(viewModel) {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SubscriberSignup", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );

            return deferred.promise;
        }

        saveMyProfile = (viewModel: any): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveMyProfile", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    });

            return deferred.promise;
        };
    }

    function getInstance($http: angular.IHttpService,
        $q: ng.IQService,
        SecurityService: SecurityServiceModule.SecurityService,
        ENV: any) {
        return new AccountService($http, $q, SecurityService, ENV);
    }

    angular.module("AngularApp")
        .factory("AccountService",
        [
            "$http",
            "$q",
            "SecurityService",
            "ENV",
            getInstance
        ]);
}