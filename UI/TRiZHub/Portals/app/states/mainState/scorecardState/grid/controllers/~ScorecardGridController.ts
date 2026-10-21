
class ScorecardGridController extends CHControllerBase {

    pageGridScorecards: any;
    pageGridPersonalScorecards: any;
    pageGridTeamScorecards: any;
    pageGridAdminScorecards: any;

    loadingIsDone = false;

    gridScorecards: TcrGridModel;
    onDataLoadedGridScorecards = (event) => { this.onLoadEventGridScorecards(event); };

    gridPersonalScorecards: TcrGridModel;
    onDataLoadedGridPersonalScorecards = (event) => { this.onLoadEventGridPersonalScorecards(event); };

    gridTeamScorecards: TcrGridModel;
    onDataLoadedGridTeamScorecards = (event) => { this.onLoadEventGridTeamScorecards(event); };

    gridAdminScorecards: TcrGridModel;
    onDataLoadedGridAdminScorecards = (event) => { this.onLoadEventGridAdminScorecards(event); };

    /** Which list is on screen: create, mine, team, or admin. */
    view = "create";
    createLoaded = false;
    mineLoaded = false;
    teamLoaded = false;
    adminLoaded = false;

    reassignUsers = [];

    isAdmin: boolean;

    filterModel: any;
    filterOptions: any;

    filterModelPersonal: any;
    filterOptionsPersonal: any;

    filterModelTeam: any;
    filterOptionsTeam: any;

    filterModelAdmin: any;
    filterOptionsAdmin: any;

    constructor(

        private $scope: ng.IScope,
        private $timeout: ng.ITimeoutService,
        private $state: ng.ui.IStateService,
        private ScorecardService: ScorecardServiceModule.ScorecardService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private UserService: UserServiceModule.UserService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);

        const self = this;

        // Setup initial filter options
        self.setupFilters();

        if (self.canCreateScorecards())
            self.view = "create";
        else if (self.isAllowed("PerformanceManagementViewMyScoreCards"))
            self.view = "mine";
        else if (self.canViewTeamScorecards())
            self.view = "team";
        else if (self.isAllowed("PerformanceManagementAdmin"))
            self.view = "admin";

        // Retrieve scorecard template period years
        this.ScorecardTemplateService.scorecardTemplatePeriodDropdownYear()
            .then(
                result => {

                    for (var i = 0; i < result.length; i++) {
                        self.filterOptions.allYears.push(result[i]);
                    }

                    result.unshift("All");

                    self.filterOptions.years = result;
                    self.filterOptionsPersonal.years = result;
                    self.filterOptionsTeam.years = result;
                    self.filterOptionsAdmin.years = result;
                },
                error => {
                    self.handleError(error);
                });

        self.isAdmin = self.isAllowed("PerformanceManagementAdmin");

        // Retrieve scorecard template periods
        self.getPeriods([self.filterModel.year], self.filterOptions, self.filterModel, function () { self.loadGrids(); });
        self.getPeriods([self.filterModelPersonal.year], self.filterOptionsPersonal, self.filterModelPersonal, function () { });
        self.getPeriods([self.filterModelTeam.year], self.filterOptionsTeam, self.filterModelTeam, function () { });

        self.bindFilters();

        // Privileges and the security list arrive after a refresh, so re-check
        // until Admin can be decided. The other tabs call isAllowed from the view.
        self.$scope.$watch(() => self.isAllowed("PerformanceManagementAdmin"), () => self.syncAccess());
    }

    private adminStarted = false;

    syncAccess = () => {
        const admin = this.isAllowed("PerformanceManagementAdmin");
        this.isAdmin = admin;
        if (admin)
            this.startAdmin();
        this.ensureViewAllowed();
    };

    private startAdmin = () => {
        if (this.adminStarted)
            return;
        this.adminStarted = true;
        const self = this;

        self.getPeriods([self.filterModelAdmin.year], self.filterOptionsAdmin, self.filterModelAdmin, () => {
            if (self.pageGridAdminScorecards)
                return;
            self.pageGridAdminScorecards = new TcrGridServiceModule
                .TcrGridService("employeename", self.ScorecardService.adminScorecardGrid, self.onDataLoadedGridAdminScorecards, model => {
                    model.customSearchModel = self.filterModelAdmin;
                }, null, self.$state);
            self.pageGridAdminScorecards.loadGrid();
        });

        self.UserService.userDropdownList()
            .then(
                result => {
                    self.reassignUsers = result;
                },
                error => {
                    self.handleError(error);
                });
    };

    private ensureViewAllowed = () => {
        const user = this.SecurityService.getCurrentUserDetails();
        if (!user || (!user.isSystemAdmin && !this.SecurityService.userHasPrivileges()))
            return;

        const any = this.canCreateScorecards()
            || this.isAllowed("PerformanceManagementViewMyScoreCards")
            || this.canViewTeamScorecards()
            || this.isAllowed("PerformanceManagementAdmin");
        if (!any)
            return;

        const currentOk =
            (this.view === "create" && this.canCreateScorecards())
            || (this.view === "mine" && this.isAllowed("PerformanceManagementViewMyScoreCards"))
            || (this.view === "team" && this.canViewTeamScorecards())
            || (this.view === "admin" && this.isAllowed("PerformanceManagementAdmin"));
        if (currentOk)
            return;

        if (this.canCreateScorecards())
            this.setView("create");
        else if (this.isAllowed("PerformanceManagementViewMyScoreCards"))
            this.setView("mine");
        else if (this.canViewTeamScorecards())
            this.setView("team");
        else if (this.isAllowed("PerformanceManagementAdmin"))
            this.setView("admin");
    };

    setupFilters = () => {
        const self = this;

        self.filterOptions = {};
        self.filterOptions.allYears = [];
        self.filterOptions.years = [];
        self.filterOptions.periods = [];
        self.filterOptions.period = {};

        self.filterModel = {};
        self.filterModel.searchText = "";
        self.filterModel.employeeName = "";
        self.filterModel.scoreCardName = "";
        self.filterModel.evaluatorName = "";
        self.filterModel.locked = false;
        self.filterModel.submitted = false;
        self.filterModel.year = new Date().getFullYear();
        self.filterModel.periodStart = null;
        self.filterModel.periodEnd = null;
        self.filterModel.periodId = null;
        self.filterModel.variablePeriod = false;

        self.filterOptionsPersonal = {};
        self.filterOptionsPersonal.years = [];
        self.filterOptionsPersonal.periods = [];
        self.filterOptionsPersonal.period = {};

        self.filterModelPersonal = {};
        self.filterModelPersonal.searchText = "";
        self.filterModelPersonal.employeeName = "";
        self.filterModelPersonal.scoreCardName = "";
        self.filterModelPersonal.evaluatorName = "";
        self.filterModelPersonal.locked = false;
        self.filterModelPersonal.submitted = true;
        self.filterModelPersonal.year = new Date().getFullYear();
        self.filterModelPersonal.periodStart = null;
        self.filterModelPersonal.periodEnd = null;
        self.filterModelPersonal.periodId = null;
        self.filterModelPersonal.variablePeriod = false;

        self.filterOptionsTeam = {};
        self.filterOptionsTeam.years = [];
        self.filterOptionsTeam.periods = [];
        self.filterOptionsTeam.period = {};

        self.filterModelTeam = {};
        self.filterModelTeam.searchText = "";
        self.filterModelTeam.employeeName = "";
        self.filterModelTeam.scoreCardName = "";
        self.filterModelTeam.evaluatorName = "";
        self.filterModelTeam.locked = false;
        self.filterModelTeam.submitted = true;
        self.filterModelTeam.year = new Date().getFullYear();
        self.filterModelTeam.periodStart = null;
        self.filterModelTeam.periodEnd = null;
        self.filterModelTeam.periodId = null;
        self.filterModelTeam.variablePeriod = false;

        self.filterOptionsAdmin = {};
        self.filterOptionsAdmin.years = [];
        self.filterOptionsAdmin.periods = [];
        self.filterOptionsAdmin.period = {};

        self.filterModelAdmin = {};
        self.filterModelAdmin.searchText = "";
        self.filterModelAdmin.employeeName = "";
        self.filterModelAdmin.scoreCardName = "";
        self.filterModelAdmin.evaluatorName = "";
        self.filterModelAdmin.locked = false;
        self.filterModelAdmin.submitted = false;
        self.filterModelAdmin.year = new Date().getFullYear();
        self.filterModelAdmin.periodStart = null;
        self.filterModelAdmin.periodEnd = null;
        self.filterModelAdmin.periodId = null;
        self.filterModelAdmin.variablePeriod = false;
    };

    getPeriods = (years: Array<number>, options: any, model: any, callback: Function) => {
        const self = this;

        options.periods = [];
        options.period = {};

        self.ScorecardTemplateService.scorecardTemplatePeriodDropdownListYearMultiple(years)
            .then(
                result => {
                    // Format dates
                    for (var i = 0; i < result.length; i++) {

                        if (result[i].isVariable) {
                            result[i].displayVal = result[i].scorecardName + " : " + result[i].name + " (Variable)";
                        }
                        else {
                            result[i].displayVal = result[i].scorecardName + " : " + result[i].name + " (" + self.formatDate(new Date(result[i].startDate)) + " - " + self.formatDate(new Date(result[i].endDate)) + ")";
                        }
                    }

                    result.unshift({ id: 0, startDate: null, endDate: null, displayVal: "All" });

                    options.periods = result;
                    options.period = result[0];
                    model.periodStart = null;
                    model.periodEnd = null;
                    callback();

                },
                error => {
                    self.handleError(error);
                });
    };

    loadGrids = () => {
        const self = this;

        self.pageGridScorecards = new TcrGridServiceModule
            .TcrGridService("employeename", self.ScorecardService.scorecardGrid, self.onDataLoadedGridScorecards, model => {                
                model.customSearchModel = self.filterModel;
            }, null, self.$state);
        self.pageGridScorecards.loadGrid();

        self.pageGridPersonalScorecards = new TcrGridServiceModule
            .TcrGridService("period", self.ScorecardService.myScorecardGrid, self.onDataLoadedGridPersonalScorecards, model => {
                model.customSearchModel = self.filterModelPersonal;
            }, null, self.$state);
        self.pageGridPersonalScorecards.loadGrid();

        self.pageGridTeamScorecards = new TcrGridServiceModule
            .TcrGridService("employeename", self.ScorecardService.teamScorecardGrid, self.onDataLoadedGridTeamScorecards, model => {
                model.customSearchModel = self.filterModelTeam;
            }, null, self.$state);
        self.pageGridTeamScorecards.loadGrid();
    };

    //#endregion

    private onLoadEventGridScorecards(event: TcrGridModel): void {
        this.gridScorecards = event;
        this.createLoaded = true;
        if (this.gridScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    private onLoadEventGridPersonalScorecards(event: TcrGridModel): void {
        this.gridPersonalScorecards = event;
        this.mineLoaded = true;
        if (this.gridPersonalScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    private onLoadEventGridTeamScorecards(event: TcrGridModel): void {
        this.gridTeamScorecards = event;
        this.teamLoaded = true;
        if (this.gridTeamScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    private onLoadEventGridAdminScorecards(event: TcrGridModel): void {
        this.gridAdminScorecards = event;
        this.adminLoaded = true;
        if (this.gridAdminScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    newScorecard = () => {
        this.$state.transitionTo("mainState.scorecard.detail", { "id": "new" });
    };

    isAllowed = (privilegeType: string): boolean => {
        return this.SecurityService.isAllowed(privilegeType);
    };

    /** Create tab: the global privilege, or a work-team lead/manager with the scorecards flag. */
    canCreateScorecards = (): boolean => {
        return this.isAllowed("PerformanceManagementCreateScoreCards") || this.SecurityService.hasTeamCapability("scorecards");
    };

    /** Team tab: line leaders, or a work-team lead/manager with the scorecards flag. */
    canViewTeamScorecards = (): boolean => {
        return this.isAllowed("PerformanceManagementViewMyTeamScoreCards") || this.SecurityService.hasTeamCapability("scorecards");
    };

    searchValueChange = () => {
        this.applySearchText(this.filterModel, this.gridScorecards);
    };

    searchValuePersonalChange = () => {
        this.applySearchText(this.filterModelPersonal, this.gridPersonalScorecards);
    };

    searchValueTeamChange = () => {
        this.applySearchText(this.filterModelTeam, this.gridTeamScorecards);
    };

    searchValueAdminChange = () => {
        this.applySearchText(this.filterModelAdmin, this.gridAdminScorecards);
    };

    private applySearchText = (model: any, grid: TcrGridModel) => {
        const text = model.searchText || "";
        model.employeeName = text;
        model.scoreCardName = text;
        model.evaluatorName = text;
        if (grid)
            grid.searchFor = text;
    };

    searchActive = () => {
        if (this.view === "mine") {
            this.searchValuePersonalChange();
            if (this.pageGridPersonalScorecards)
                this.pageGridPersonalScorecards.search();
            return;
        }
        if (this.view === "team") {
            this.searchValueTeamChange();
            if (this.pageGridTeamScorecards)
                this.pageGridTeamScorecards.search();
            return;
        }
        if (this.view === "admin") {
            this.searchValueAdminChange();
            if (this.pageGridAdminScorecards)
                this.pageGridAdminScorecards.search();
            return;
        }
        this.searchValueChange();
        if (this.pageGridScorecards)
            this.pageGridScorecards.search();
    };

    /** Bindings for the list that is on screen. Rebuilt when the view changes. */
    filters: any;

    setView = (view: string) => {
        this.view = view;
        this.bindFilters();
    };

    private bindFilters = () => {
        if (this.view === "mine")
            this.filters = {
                model: this.filterModelPersonal,
                options: this.filterOptionsPersonal,
                showSubmitted: false,
                onSearch: this.searchValuePersonalChange,
                onLock: this.lockedSubmittedPersonalChange,
                onYear: this.yearChangePersonal,
                onPeriod: this.periodChangePersonal
            };
        else if (this.view === "team")
            this.filters = {
                model: this.filterModelTeam,
                options: this.filterOptionsTeam,
                showSubmitted: false,
                onSearch: this.searchValueTeamChange,
                onLock: this.lockedSubmittedTeamChange,
                onYear: this.yearChangeTeam,
                onPeriod: this.periodChangeTeam
            };
        else if (this.view === "admin")
            this.filters = {
                model: this.filterModelAdmin,
                options: this.filterOptionsAdmin,
                showSubmitted: true,
                onSearch: this.searchValueAdminChange,
                onLock: this.lockedSubmittedAdminChange,
                onYear: this.yearChangeAdmin,
                onPeriod: this.periodChangeAdmin
            };
        else
            this.filters = {
                model: this.filterModel,
                options: this.filterOptions,
                showSubmitted: true,
                onSearch: this.searchValueChange,
                onLock: this.lockedSubmittedChange,
                onYear: this.yearChange,
                onPeriod: this.periodChange
            };
    };

    activeGrid = (): TcrGridModel => {
        if (this.view === "mine")
            return this.gridPersonalScorecards;
        if (this.view === "team")
            return this.gridTeamScorecards;
        if (this.view === "admin")
            return this.gridAdminScorecards;
        return this.gridScorecards;
    };

    isViewLoaded = (): boolean => {
        if (this.view === "mine")
            return this.mineLoaded;
        if (this.view === "team")
            return this.teamLoaded;
        if (this.view === "admin")
            return this.adminLoaded;
        return this.createLoaded;
    };

    sortKey = (): string => {
        const grid = this.activeGrid();
        return grid && grid.sortKeyOrder ? grid.sortKeyOrder.key : "";
    };

    sortOrder = (): string => {
        const grid = this.activeGrid();
        return grid && grid.sortKeyOrder ? grid.sortKeyOrder.order : "";
    };

    toggleSort = (key: string) => {
        const page = this.view === "mine" ? this.pageGridPersonalScorecards
            : this.view === "team" ? this.pageGridTeamScorecards
            : this.view === "admin" ? this.pageGridAdminScorecards
            : this.pageGridScorecards;
        if (page)
            page.sort(key);
    };

    currentSearch = (): string => {
        if (this.view === "mine")
            return this.filterModelPersonal.searchText;
        if (this.view === "team")
            return this.filterModelTeam.searchText;
        if (this.view === "admin")
            return this.filterModelAdmin.searchText;
        return this.filterModel.searchText;
    };

    setLockFilter = (model: any, locked: boolean, reload: () => void) => {
        if (model.locked === locked)
            return;
        model.locked = locked;
        reload();
    };

    setSubmittedFilter = (model: any, submitted: boolean, reload: () => void) => {
        if (model.submitted === submitted)
            return;
        model.submitted = submitted;
        reload();
    };

    openScorecard = (record) => {
        const readOnly = this.view === "team" ? true : !!record.locked;
        this.$state.go("mainState.scorecard.submitScorecard", {
            id: record.scorecardId,
            readOnly: readOnly ? "true" : "false"
        });
    };

    lockedSubmittedChange = () => {
        const self = this;
        self.pageGridScorecards.loadGrid();
    };

    lockedSubmittedPersonalChange = () => {
        const self = this;
        self.pageGridPersonalScorecards.loadGrid();
    };

    lockedSubmittedTeamChange = () => {
        const self = this;
        self.pageGridTeamScorecards.loadGrid();
    };

    lockedSubmittedAdminChange = () => {
        const self = this;
        self.pageGridAdminScorecards.loadGrid();
    };

    periodChange = (period: any) => {
        const self = this;
        self.filterModel.periodStart = period.startDate;
        self.filterModel.periodEnd = period.endDate;
        self.filterModel.periodId = period.id;
        self.filterModel.variablePeriod = period.isVariable;
        self.pageGridScorecards.loadGrid();
    };

    periodChangePersonal = (period: any) => {
        const self = this;
        self.filterModelPersonal.periodStart = period.startDate;
        self.filterModelPersonal.periodEnd = period.endDate;
        self.filterModelPersonal.periodId = period.id;
        self.filterModelPersonal.variablePeriod = period.isVariable;
        self.pageGridPersonalScorecards.loadGrid();
    };

    periodChangeTeam = (period: any) => {
        const self = this;
        self.filterModelTeam.periodStart = period.startDate;
        self.filterModelTeam.periodEnd = period.endDate;
        self.filterModelTeam.periodId = period.id;
        self.filterModelTeam.variablePeriod = period.isVariable;
        self.pageGridTeamScorecards.loadGrid();
    };

    periodChangeAdmin = (period: any) => {
        const self = this;
        self.filterModelAdmin.periodStart = period.startDate;
        self.filterModelAdmin.periodEnd = period.endDate;
        self.filterModelAdmin.periodId = period.id;
        self.filterModelAdmin.variablePeriod = period.isVariable;
        self.pageGridAdminScorecards.loadGrid();
    };

    yearChange = (year: any) => {
        const self = this;
        if (year == "All") {
            year = self.filterOptions.allYears;
        }
        else {
            year = [year];
        }
        self.getPeriods(year, self.filterOptions, self.filterModel, () => { self.pageGridScorecards.loadGrid(); });
    };

    yearChangePersonal = (year: any) => {
        const self = this;
        if (year == "All") {
            year = self.filterOptions.allYears;
        }
        else {
            year = [year];
        }
        self.getPeriods(year, self.filterOptionsPersonal, self.filterModelPersonal, () => { self.pageGridPersonalScorecards.loadGrid(); });
    };

    yearChangeTeam = (year: any) => {
        const self = this;
        if (year == "All") {
            year = self.filterOptions.allYears;
        }
        else {
            year = [year];
        }
        self.getPeriods(year, self.filterOptionsTeam, self.filterModelTeam, () => { self.pageGridTeamScorecards.loadGrid(); });
    };

    yearChangeAdmin = (year: any) => {
        const self = this;
        if (year == "All") {
            year = self.filterOptions.allYears;
        }
        else {
            year = [year];
        }
        self.getPeriods(year, self.filterOptionsAdmin, self.filterModelAdmin, () => { self.pageGridAdminScorecards.loadGrid(); });
    };

    deleteScoreCard = (record) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this scoreCard")
            .then(
                action => {
                    if (action) {
                        self.ScorecardService.scoreCardDelete(record)
                            .then(
                                result => {

                                    self.pageGridScorecards.loadGrid();

                                    if (self.isAdmin == true) {
                                        self.pageGridAdminScorecards.loadGrid();
                                    }
                                },
                                error => {
                                    self.handleError(error);
                                });
                    }

                },
                error => {
                    self.handleError(error);
                });
    };


    toggleScoreCardLock = (record) => {
        const self = this;

        self.ScorecardService.scoreCardLock(record)
            .then(
                result => {
                    self.pageGridTeamScorecards.loadGrid();
                    self.pageGridScorecards.loadGrid();
                    self.pageGridPersonalScorecards.loadGrid();

                    if (self.isAdmin == true) {
                        self.pageGridAdminScorecards.loadGrid();
                    }
                },
                error => {
                    self.handleError(error);
                });
    }


    unsubmitScoreCard = (record) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to Unsubmit?",
            "You are about to Unsubmit this scorecard")
            .then(
                action => {
                    if (action) {
                        self.ScorecardService.scoreCardUnsubmit(record)
                            .then(
                                result => {

                                    self.pageGridTeamScorecards.loadGrid();
                                    self.pageGridScorecards.loadGrid();
                                    self.pageGridPersonalScorecards.loadGrid();

                                    if (self.isAdmin == true) {
                                        self.pageGridAdminScorecards.loadGrid();
                                    }
                                },
                                error => {
                                    self.handleError(error);
                                });
                    }

                },
                error => {
                    self.handleError(error);
                });
    };

    submitScoreCard = (record) => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope,
            "Are you sure you want to Submit?",
            "You are about to Submit this scorecard")
            .then(
                action => {
                    if (action) {
                        self.ScorecardService.scoreCardSubmit(record)
                            .then(
                                result => {
                                    self.pageGridTeamScorecards.loadGrid();
                                    self.pageGridScorecards.loadGrid();
                                    self.pageGridPersonalScorecards.loadGrid();

                                    if (self.isAdmin == true) {
                                        self.pageGridAdminScorecards.loadGrid();
                                    }
                                },
                                error => {
                                    self.handleError(error);
                                });
                    }

                },
                error => {
                    self.handleError(error);
                });
    };

    formatDate = (date): string => {
        return date.getFullYear() + "/" + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);
    };

    reassignScorecard = (scorecard) => {
        const self = this;
        self.Popups.scorecardReassignDailog(self.$scope, "Reassign Scorecard", "Ok", "Cancel", scorecard, self.reassignUsers)
            .then(
                action => {
                    if (action) {

                        let scoreCard = { scorecardId: action.scorecard.scorecardId, evaluatorId: action.evaluator.id };

                        self.ScorecardService.scoreCardReassign(scoreCard)
                            .then(
                                result => {
                                    self.pageGridAdminScorecards.search();
                                },
                                error => {
                                    self.handleError(error);
                                });
                    }
                },
                error => {
                    self.handleError(error);
                });
    };
}

angular.module("AngularApp")
    .controller("ScorecardGridController",
        [
            "$scope",
            "$timeout",
            "$state",
            "ScorecardService",
            "SecurityService",
            "ScorecardTemplateService",
            "UserService",
            "Popups",
            ScorecardGridController
        ]);