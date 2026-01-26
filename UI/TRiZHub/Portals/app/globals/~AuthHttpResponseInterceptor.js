var AuthHttpResponseInterceptorModule;
(function (AuthHttpResponseInterceptorModule) {
    ;
    var AuthHttpResponseInterceptor = /** @class */ (function () {
        function AuthHttpResponseInterceptor($q, $location, $localStorage) {
            var _this = this;
            this.$q = $q;
            this.$location = $location;
            this.$localStorage = $localStorage;
            this.request = function (config) {
                return config;
            };
            this.responseError = function (rejection) {
                if (rejection.status === 401) {
                    _this.$location.path("/login");
                }
                return _this.$q.reject(rejection);
            };
        }
        return AuthHttpResponseInterceptor;
    }());
    AuthHttpResponseInterceptorModule.AuthHttpResponseInterceptor = AuthHttpResponseInterceptor;
    ;
    function getInstance($q, $location, $localStorage) {
        return new AuthHttpResponseInterceptor($q, $location, $localStorage);
    }
    angular.module("AngularApp")
        .factory("AuthHttpResponseInterceptor", ["$q", "$location", "$localStorage", getInstance]);
})(AuthHttpResponseInterceptorModule || (AuthHttpResponseInterceptorModule = {}));
;
//# sourceMappingURL=~AuthHttpResponseInterceptor.js.map