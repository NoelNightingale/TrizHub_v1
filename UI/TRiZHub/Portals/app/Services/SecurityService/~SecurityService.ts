module SecurityServiceModule {

    export class CurrentUser {

        //#region Properties

        id: string;
        loggedIn: boolean;
        accessToken: string;
        userName: string;
        displayName: string;
        allowedPrivileges: any;
        isUserProfileComplete: boolean;
        isUserApproved: boolean;
        isSystemAdmin: boolean;

        //#endregion

        //#region Ctor

        constructor() {
            this.id = null;
            this.loggedIn = false;
            this.accessToken = "";
            this.userName = "";
            this.displayName = "";
            this.allowedPrivileges = [];
            this.isSystemAdmin = false;
        }

        //#endregion
    }

    export interface ISecurityService {

        scopeUpdateEvent: string;

        init: () => boolean;
        login: (accessToken: string, userName: string, isSystemAdmin: boolean) => void;
        logout: () => void;
        getCurrentUserDetails: () => CurrentUser;
        setCurrentUserDetails: (id: string,
            displayName: string,
            allowedPrivileges: string,
            isSystemAdmin: boolean,
            isUserProfileComplete: boolean,
            isUserApproved: boolean) => void;
        setAccount: (username: string, isSystemAdmin: boolean) => void;
        getToken: () => string;
        userHasPrivileges: () => boolean;
        isAllowed: (privilegeType: string) => boolean;
    }

    export class SecurityService implements ISecurityService {
        static currentUser: CurrentUser;
        scopeUpdateEvent: string;

        //#region Ctor

        constructor(private $rootScope: angular.IRootScopeService,
            private $localStorage: any,
            private EnumService: EnumServiceModule.EnumService) {
            this.scopeUpdateEvent = "current-user-updated";
        }

        //#endregion

        init = (): boolean => {

            SecurityService.currentUser = new CurrentUser();
            const authData = this.$localStorage.authData;
            if (authData) {
                SecurityService.currentUser.loggedIn = true;
                SecurityService.currentUser.userName = authData.userName;
                SecurityService.currentUser.accessToken = authData.token;
                SecurityService.currentUser.isSystemAdmin = authData.isSystemAdmin;
            }

            this.$rootScope.$emit(this.scopeUpdateEvent, SecurityService.currentUser);
            return SecurityService.currentUser.loggedIn;
        };

        login = (accessToken: string, userName: string, isSystemAdmin: boolean): void => {

            SecurityService.currentUser.userName = userName;
            SecurityService.currentUser.accessToken = accessToken;
            SecurityService.currentUser.loggedIn = true;

            this.$localStorage.authData = { token: accessToken, userName: userName, isSystemAdmin: isSystemAdmin };
            this.$rootScope.$emit(this.scopeUpdateEvent, SecurityService.currentUser);
        };

        logout = (): void => {

            SecurityService.currentUser = new CurrentUser();

            this.$localStorage.authData = null;
            this.$rootScope.$emit(this.scopeUpdateEvent, SecurityService.currentUser);
        };

        getCurrentUserDetails = (): CurrentUser => {
            return SecurityService.currentUser;
        };

        setCurrentUserDetails = (id: string,
            displayName: string,
            allowedPrivileges: any,
            isSystemAdmin: boolean,
            isUserProfileComplete: boolean,
            isUserApproved: boolean): void => {
            SecurityService.currentUser.id = id;
            SecurityService.currentUser.displayName = displayName;
            SecurityService.currentUser.allowedPrivileges = allowedPrivileges;
            SecurityService.currentUser.isSystemAdmin = isSystemAdmin;
            SecurityService.currentUser.isUserProfileComplete = isUserProfileComplete;
            SecurityService.currentUser.isUserApproved = isUserApproved;
            this.$rootScope.$emit(this.scopeUpdateEvent, SecurityService.currentUser);
        };

        setAccount = (username: string, isSystemAdmin: boolean): void => {
            SecurityService.currentUser.userName = username;
            SecurityService.currentUser.isSystemAdmin = isSystemAdmin;
            this.$rootScope.$emit(this.scopeUpdateEvent, SecurityService.currentUser);
        };

        getToken = (): string => {
            if (SecurityService.currentUser.loggedIn)
                return SecurityService.currentUser.accessToken;
            else
                return null;
        };

        userHasPrivileges = (): boolean => {
            if (!SecurityService.currentUser || SecurityService.currentUser.allowedPrivileges == null)
                return false;

            if (!(SecurityService.currentUser.allowedPrivileges.length > 0))
                return false;

            return true;
        };

        isAllowed = (privilegeType: string): boolean => {
            if (SecurityService.currentUser.isSystemAdmin)
                return true;

            if (!this.userHasPrivileges())
                return false;

            var allValues = this.EnumService.getSecurityTypes();
            var myEnum = -1;

            for (let value of allValues) {
                if (value.name === privilegeType) {
                    myEnum = value.ordinalValue;
                    break;
                }
            }

            if (myEnum < 0)
                return false;

            var result = SecurityService.currentUser.allowedPrivileges.indexOf(myEnum) > -1;
            return result;
        };
    }

    function getInstance($rootScope: angular.IRootScopeService, $localStorage: any, EnumService: any) {
        return new SecurityService($rootScope, $localStorage, EnumService);
    }

    angular.module("AngularApp")
        .factory("SecurityService",
        [
            "$rootScope",
            "$localStorage",
            "EnumService",
            getInstance
        ]);
}