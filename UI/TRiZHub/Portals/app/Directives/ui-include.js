(function() {
    "use strict";
    angular
        .module("AngularApp")
        .directive("uiInclude", uiInclude);
    uiInclude.$inject = ["$http", "$templateCache", "$compile"];

    function uiInclude($http, $templateCache, $compile) {
        const directive = {
            restrict: "A",
            link: link
        };
        return directive;

        function link(scope, el, attr) {
            const templateUrl = scope.$eval(attr.uiInclude);
            $http.get(templateUrl, { cache: $templateCache })
                .success(
                    function(tplContent) {
                        el.replaceWith($compile(tplContent)(scope));
                    }
                );
        }
    }
})();