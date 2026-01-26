
module ScorecardServiceModule {

    export interface IScorecardService {
        scorecardGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardGridModel>>;
        myScorecardGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardGridModel>>;
        teamScorecardGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardGridModel>>;
        adminScorecardGrid: (req: GridModel) => ng.IPromise<GridResultModel<ScorecardGridModel>>;
        scorecardGet: (id: string) => ng.IPromise<ScorecardModel>;
        scorecardSave: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scorecardRecordSave: (viewModel: ScorecardPeriodModel) => ng.IPromise<ScorecardPeriodModel>;
        scorecardCommmentSave: (viewModel: ScorecardPeriodModel) => ng.IPromise<ScorecardPeriodModel>;
        scorecardRecordCommentSave: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scorecardPeriodGet: (id: string) => ng.IPromise<ScorecardPeriodModel>;
        scorecardDropdownList: () => ng.IPromise<ScorecardDropdownModel>;
        scoreCardDelete: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scoreCardLock: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scoreCardUnsubmit: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scoreCardSubmit: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
        scoreCardReassign: (viewModel: ScorecardModel) => ng.IPromise<ScorecardModel>;
    }

    export class ScorecardService extends CHServiceBase implements IScorecardService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Scorecard/");
        }

        //#endregion

        scorecardGrid = (req: GridResultModel<ScorecardGridModel>): ng.IPromise<GridResultModel<ScorecardGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardGrid", req)
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

        myScorecardGrid = (req: GridResultModel<ScorecardGridModel>): ng.IPromise<GridResultModel<ScorecardGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "MyScorecardGrid", req)
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


        teamScorecardGrid = (req: GridResultModel<ScorecardGridModel>): ng.IPromise<GridResultModel<ScorecardGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TeamScorecardGrid", req)
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

        adminScorecardGrid = (req: GridResultModel<ScorecardGridModel>): ng.IPromise<GridResultModel<ScorecardGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardGridAdmin", req)
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

        scorecardGet = (id: string): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardGet/" + id)
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

        scorecardSave = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardSave", viewModel)
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

        scorecardRecordSave = (viewModel: ScorecardPeriodModel): ng.IPromise<ScorecardPeriodModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardRecordSave", viewModel)
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

        scorecardCommmentSave = (viewModel: ScorecardPeriodModel): ng.IPromise<ScorecardPeriodModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardCommentSave", viewModel)
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

        scorecardRecordCommentSave = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveScoreCardRecordEmployeeComment", viewModel)
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


        scorecardPeriodGet = (id: string): ng.IPromise<ScorecardPeriodModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardPeriodGet/" + id)
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

        scorecardDropdownList = (): ng.IPromise<ScorecardDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ScorecardDropdown/")
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

        scoreCardDelete = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardDelete", viewModel)
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

        scoreCardLock = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScoreCardLock", viewModel)
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

        scoreCardUnsubmit = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardUnsubmit", viewModel)
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

        scoreCardSubmit = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardSubmit", viewModel)
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

        scoreCardReassign = (viewModel: ScorecardModel): ng.IPromise<ScorecardModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ScorecardReassign", viewModel)
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
        return new ScorecardService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ScorecardService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}