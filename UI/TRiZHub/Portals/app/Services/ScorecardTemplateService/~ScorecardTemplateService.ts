
module ScorecardTemplateServiceModule {

    export interface IScorecardTemplateService {
        scorecardTemplateGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardTemplateGridModel>>;
        scorecardTemplateGet: (id: string) => ng.IPromise<ScorecardTemplateModel>;
        scorecardTemplateSave: (viewModel: ScorecardTemplateModel) => ng.IPromise<ScorecardTemplateModel>;
        scorecardTemplateDelete: (viewModel: ScorecardTemplateModel) => ng.IPromise<ScorecardTemplateModel>;

        scorecardTemplateItemGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardTemplateItemGridModel>>;
        scorecardTemplateItemSave: (viewModel: ScorecardTemplateItemModel) => ng.IPromise<ScorecardTemplateItemModel>;
        scorecardTemplateItemGet: (id: string) => ng.IPromise<ScorecardTemplateItemModel>;
        scorecardTemplateItemGetSkeleton: () => ng.IPromise<ScorecardTemplateItemModel>;
        scorecardTemplateItemDelete: (viewModel: ScorecardTemplateItemModel) => ng.IPromise<ScorecardTemplateItemModel>;        

        scorecardTemplatePeriodGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardTemplatePeriodGridModel>>;
        scorecardTemplatePeriodSave: (viewModel: ScorecardTemplateItemModel) => ng.
        IPromise<ScorecardTemplatePeriodModel>;
        scorecardTemplatePeriodGet: (id: string) => ng.IPromise<ScorecardTemplatePeriodModel>;
        scorecardTemplatePeriodDelete: (viewModel: ScorecardTemplatePeriodModel) => ng.IPromise<ScorecardTemplatePeriodModel>;

        scorecardTemplateDropdownList: () => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplateDropdownListAll: () => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplateDropdownListYearMultiple: (year: Array<number>) => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplatePeriodDropdownList: (id: string) => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplatePeriodDropdownListYear: (year: number) => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplatePeriodDropdownListYearMultiple: (year: Array<number>) => ng.IPromise<ScorecardTemplateDropdownModel>;
        scorecardTemplatePeriodDropdownYear: () => ng.IPromise<ScorecardTemplateDropdownModel>;

        scorecardTemplateYearDropdownList: (id: string) => ng.IPromise<ScorecardTemplateModel>;
        scorecardTemplatePeriodSearchDropdownList: (viewModel: any) => ng.IPromise<ScorecardTemplatePeriodModel>;
    }

    export class ScorecardTemplateService extends CHServiceBase implements IScorecardTemplateService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Scorecard/");
        }

        //#endregion

        scorecardTemplateGrid = (req: GridResultModel<ScorecardTemplateGridModel>): ng.
            IPromise<GridResultModel<ScorecardTemplateGridModel>> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "ScorecardTemplateGrid", req)
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

        scorecardTemplateGet = (id: string): ng.IPromise<ScorecardTemplateModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateGet/" + id)
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

        scorecardTemplateSave = (viewModel: ScorecardTemplateModel): ng.IPromise<ScorecardTemplateModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplateSave", viewModel)
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

        scorecardTemplateDelete = (viewModel: ScorecardTemplateModel): ng.IPromise<ScorecardTemplateModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplateDelete", viewModel)
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

        scorecardTemplateItemGrid = (req: GridResultModel<ScorecardTemplateItemGridModel>): ng.
            IPromise<GridResultModel<ScorecardTemplateItemGridModel>> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "ScorecardTemplateItemGrid", req)
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

        scorecardTemplateItemSave = (viewModel: ScorecardTemplateItemModel): ng.IPromise<ScorecardTemplateItemModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplateItemSave", viewModel)
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

        scorecardTemplateItemGet = (id: string): ng.IPromise<ScorecardTemplateItemModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateItemGet/" + id)
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

        scorecardTemplateItemGetSkeleton = (): ng.IPromise<ScorecardTemplateItemModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateItemGetSkeleton")
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

        scorecardTemplateItemDelete = (viewModel: ScorecardTemplateItemModel): ng.IPromise<ScorecardTemplateItemModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplateItemDelete", viewModel)
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

        scorecardTemplatePeriodGrid = (req: GridResultModel<ScorecardTemplatePeriodGridModel>): ng.
            IPromise<GridResultModel<ScorecardTemplatePeriodGridModel>> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "ScorecardTemplatePeriodGrid", req)
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

        scorecardTemplatePeriodSave = (viewModel: ScorecardTemplatePeriodModel): ng.
            IPromise<ScorecardTemplatePeriodModel> => {
                const deferred = this.$q.defer();
                this.$http.post(this.urlRoot + "ScorecardTemplatePeriodSave", viewModel)
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

        scorecardTemplatePeriodGet = (id: string): ng.IPromise<ScorecardTemplatePeriodModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplatePeriodGet/" + id)
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

        scorecardTemplatePeriodDelete = (viewModel: ScorecardTemplatePeriodModel): ng.IPromise<ScorecardTemplatePeriodModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplatePeriodDelete", viewModel)
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

        scorecardTemplateDropdownList = (): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateDropdown/")
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

        scorecardTemplateDropdownListAll = (): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateDropdownAll/")
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

        scorecardTemplateDropdownListYearMultiple = (years: Array<number>): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplateDropdownListYearMultiple", years)
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

        

        scorecardTemplatePeriodDropdownList = (id: string): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplatePeriodDropdown/" + id)
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

        scorecardTemplatePeriodDropdownListYear = (year: number): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplatePeriodDropdownYear/" + year)
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

        scorecardTemplatePeriodDropdownListYearMultiple = (years: Array<number>): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplatePeriodDropdownYearMultiple", years)
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

        scorecardTemplatePeriodDropdownYear = (): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplatePeriodDropdownYear")
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

        scorecardTemplateYearDropdownList = (id: string): ng.IPromise<ScorecardTemplateModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardTemplateYearDropdownList/" + id)
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

        scorecardTemplatePeriodSearchDropdownList = (viewModel: any): ng.IPromise<ScorecardTemplateDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardTemplatePeriodSearchDropdownList", viewModel)
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
        return new ScorecardTemplateService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ScorecardTemplateService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}