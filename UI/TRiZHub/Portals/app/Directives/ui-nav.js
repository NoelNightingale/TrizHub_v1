(function() {
    "use strict";
    angular
        .module("AngularApp")
        .directive("uiNav", uiNav);

    function uiNav() {
        const directive = {
            restrict: "AC",
            link: link
        };
        return directive;
    }

    function link(scope, el, attr) {
        el.find("a")
            .bind("click",
                function(e) {
                    const li = angular.element(this).parent();
                    const active = li.parent()[0].querySelectorAll(".active");
                    li.toggleClass("active");
                    angular.element(active).removeClass("active");
                });
    }
})();