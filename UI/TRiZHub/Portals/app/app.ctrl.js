/**
 * @ngdoc function
 * @name app.controller:AppCtrl
 * @description
 * # MainCtrl
 * Controller of the app
 */

(function() {
    "use strict";
    angular
        .module("AngularApp")
        .controller("AppCtrl", AppCtrl);

    AppCtrl.$inject = ["$scope", "$localStorage", "$location", "$rootScope", "$anchorScroll", "$timeout", "$window"];

    function AppCtrl($scope, $localStorage, $location, $rootScope, $anchorScroll, $timeout, $window) {
        var vm = $scope;
        vm.isIE = isIE();
        vm.isSmart = isSmart();
        // config
        vm.app = {
            name: "TAP",
            version: "1.1.7",
            // for chart colors
            color: {
                'primary': "#0cc2aa",
                'accent': "#a88add",
                'warn': "#fcc100",
                'info': "#6887ff",
                'success': "#6cc788",
                'warning': "#f77a99",
                'danger': "#f44455",
                'white': "#ffffff",
                'light': "#f1f2f3",
                'dark': "#2e3e4e",
                'black': "#2a2b3c"
            },
            setting: {
                theme: {
                    primary: "primary",
                    accent: "accent",
                    warn: "warn",
                    info: "info",
                    success: "success",
                    warning: "warning",
                    danger: "danger",
                    white: "white",
                    light: "light",
                    dark: "dark",
                    black: "black",
                },
                folded: false,
                boxed: false,
                container: false,
                themeID: 1,
                bg: ""
            }
        };

        var setting = vm.app.name + "-Setting";
        // save settings to local storage
        $localStorage[setting] = vm.app.setting;

        // watch changes
        $scope.$watch("app.setting",
            function() {
                $localStorage[setting] = vm.app.setting;
            },
            true);

        getParams("bg") && (vm.app.setting.bg = getParams("bg"));

        vm.setTheme = setTheme;
        setColor();

        function setTheme(theme) {
            vm.app.setting.theme = theme.theme;
            setColor();
            if (theme.url) {
                $timeout(function() {
                        $window.location.href = theme.url;
                    },
                    100,
                    false);
            }
        };

        function setColor() {
            vm.app.setting.color = {
                primary: getColor(vm.app.setting.theme.primary),
                accent: getColor(vm.app.setting.theme.accent),
                warn: getColor(vm.app.setting.theme.warn),
                info: getColor(vm.app.setting.theme.info),
                success: getColor(vm.app.setting.theme.success),
                warning: getColor(vm.app.setting.theme.warning),
                danger: getColor(vm.app.setting.theme.danger),
                white: getColor(vm.app.setting.theme.white),
                light: getColor(vm.app.setting.theme.light),
                dark: getColor(vm.app.setting.theme.dark),
                black: getColor(vm.app.setting.theme.black),
            };
        };

        function getColor(name) {
            const temp = vm.app.color[name];
            return vm.app.color[name] ? vm.app.color[name] : palette.find(name);
        };

        $rootScope.$on("$stateChangeSuccess", openPage);

        function openPage() {
            // goto top
            $location.hash("content");
            $anchorScroll();
            $location.hash("");
            // hide open menu
            $("#aside").modal("hide");
            $("body").removeClass("modal-open").find(".modal-backdrop").remove();
            $(".navbar-toggleable-sm").collapse("hide");
        };

        vm.goBack = function() {
            $window.history.back();
        };

        function isIE() {
            return !!navigator.userAgent.match(/MSIE/i) || !!navigator.userAgent.match(/Trident.*rv:11\./);
        }

        function isSmart() {
            // Adapted from http://www.detectmobilebrowsers.com
            const ua = $window["navigator"]["userAgent"] || $window["navigator"]["vendor"] || $window["opera"];
            // Checks for iOs, Android, Blackberry, Opera Mini, and Windows mobile devices
            return (/iPhone|iPod|iPad|Silk|Android|BlackBerry|Opera Mini|IEMobile/).test(ua);
        }

        function getParams(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            const regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
            const results = regex.exec(location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
    }
})();