var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var ScorecardTemplateServiceModule;
(function (ScorecardTemplateServiceModule) {
    var ScorecardTemplateService = /** @class */ (function (_super) {
        __extends(ScorecardTemplateService, _super);
        //#region Ctor
        function ScorecardTemplateService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/Scorecard/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.scorecardTemplateGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateItemGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateItemGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateItemSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateItemSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateItemGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateItemGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateItemGetSkeleton = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateItemGetSkeleton")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateItemDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateItemDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplatePeriodGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplatePeriodSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplatePeriodGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplatePeriodDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateDropdownListAll = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateDropdownAll/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateDropdownListYearMultiple = function (years) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplateDropdownListYearMultiple", years)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodDropdownList = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplatePeriodDropdown/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodDropdownListYear = function (year) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplatePeriodDropdownYear/" + year)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodDropdownListYearMultiple = function (years) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplatePeriodDropdownYearMultiple", years)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodDropdownYear = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplatePeriodDropdownYear")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplateYearDropdownList = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "ScorecardTemplateYearDropdownList/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.scorecardTemplatePeriodSearchDropdownList = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ScorecardTemplatePeriodSearchDropdownList", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return ScorecardTemplateService;
    }(CHServiceBase));
    ScorecardTemplateServiceModule.ScorecardTemplateService = ScorecardTemplateService;
    function getInstance($http, $q, ENV) {
        return new ScorecardTemplateService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("ScorecardTemplateService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(ScorecardTemplateServiceModule || (ScorecardTemplateServiceModule = {}));
//# sourceMappingURL=~ScorecardTemplateService.js.map