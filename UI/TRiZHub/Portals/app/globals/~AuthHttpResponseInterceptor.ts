module AuthHttpResponseInterceptorModule {

    export interface IAuthHttpResponseInterceptor {
        request: (config: any) => any;
        responseError: (rejection: any) => ng.IPromise<any>;
    };

    export class AuthHttpResponseInterceptor implements IAuthHttpResponseInterceptor {

        constructor(private $q: ng.IQService, private $location: ng.ILocationService, private $localStorage: any) {
        }

        request = (config: any): any => {
            return config;
        };

        responseError = (rejection: any): ng.IPromise<any> => {

            if (rejection.status === 401) {
                this.$location.path("/login");
            }

            return this.$q.reject(rejection);
        };
    };

    function getInstance($q: ng.IQService, $location: ng.ILocationService, $localStorage: any) {
        return new AuthHttpResponseInterceptor($q, $location, $localStorage);
    }

    angular.module("AngularApp")
        .factory("AuthHttpResponseInterceptor", ["$q", "$location", "$localStorage", getInstance]);
};