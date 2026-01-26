module EnumServiceModule {

    export interface IEnumService {

        allEnumsLoaded: boolean;
        enumsLoadedEvent: string;
        checkAllEnumsLoaded: () => void;
        getAllEnumsLoaded: () => any;

        statusType: any;
        getStatusTypes: () => List<EnumModel>;
        loadStatusType: () => ng.IPromise<List<EnumModel>>;

        securityTypes: any;
        getSecurityTypes: () => List<SecurityTypeModel>;
        loadSecurityType: () => ng.IPromise<List<SecurityTypeModel>>;

        clientTypes: any;
        getClientTypes: () => List<ClientTypeModel>;
        loadClientType: () => ng.IPromise<List<ClientTypeModel>>;

        scorecardScoreTypes: any;
        getScorecardScoreTypes: () => List<ScorecardScoreTypeModel>;
        loadScorecardScoreType: () => ng.IPromise<List<ScorecardScoreTypeModel>>;
    }

    export class EnumService extends CHServiceBase implements IEnumService {

        allEnumsLoaded: boolean;
        enumsLoadedEvent: string;

        statusType: any;
        securityTypes: any;
        clientTypes: any;
        scorecardScoreTypes: any;

        //#region ctor

        constructor(private $http: angular.IHttpService,
            private $rootScope: angular.IRootScopeService,
            private ENV: any) {
            super(ENV.serverLocation + "api/Enums/");

            this.statusType = [];
            this.securityTypes = [];
            this.clientTypes = [];
            this.scorecardScoreTypes = [];

            this.loadStatusType();
            this.loadSecurityType();
            this.loadClientType();
            this.loadScorecardScoreType();
        }

        //#endregion

        loadStatusType = (): ng.IPromise<List<EnumModel>> => {
            return this.$http.get(this.urlRoot + "StatusTypeEnum")
                .then(result => {
                    this.statusType = result.data;
                    this.checkAllEnumsLoaded();
                });
        };

        loadSecurityType = (): ng.IPromise<List<SecurityTypeModel>> => {
            return this.$http.get(this.urlRoot + "SecurityEnum")
                .then(result => {
                    this.securityTypes = result.data;
                    this.checkAllEnumsLoaded();
                });
        };

        loadClientType = (): ng.IPromise<List<ClientTypeModel>> => {
            return this.$http.get(this.urlRoot + "ClientTypeEnum")
                .then(result => {
                    this.clientTypes = result.data;
                    this.checkAllEnumsLoaded();
                });
        };

        loadScorecardScoreType = (): ng.IPromise<List<ScorecardScoreTypeModel>> => {
            return this.$http.get(this.urlRoot + "ScorecardScoreTypeEnum")
                .then(result => {
                    this.scorecardScoreTypes = result.data;
                    this.checkAllEnumsLoaded();
                });
        };

        checkAllEnumsLoaded = (): any => {

            if (this.statusType.length > 0 &&
                this.securityTypes.length > 0 &&
                this.clientTypes.length > 0 &&
                this.scorecardScoreTypes.length > 0) {
                this.allEnumsLoaded = true;
                this.$rootScope.$emit(this.enumsLoadedEvent, this.allEnumsLoaded);
            }

        };

        getStatusTypes = (): List<EnumModel> => {
            return this.statusType;
        };

        getSecurityTypes = (): List<SecurityTypeModel> => {
            return this.securityTypes;
        };
        getClientTypes = (): List<ClientTypeModel> => {
            return this.clientTypes;
        };
        getScorecardScoreTypes = (): List<ScorecardScoreTypeModel> => {
            return this.scorecardScoreTypes;
        };
        getAllEnumsLoaded = (): any => {
            return this.allEnumsLoaded;
        };
    }

    function getInstance($http: angular.IHttpService, $rootScope: angular.IRootScopeService, ENV: any) {
        return new EnumService($http, $rootScope, ENV);
    }

    angular.module("AngularApp")
        .factory("EnumService",
        [
            "$http",
            "$rootScope",
            "ENV",
            getInstance
        ]);
}