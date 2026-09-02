class BillingRatesMaintenanceGridController extends CHControllerBase {

    //#region Members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any = {
        data: [],
        totalItems: 0,
        sortKeyOrder: { order: "ASC", key: "startdate" },
        currentPage: 1,
        maxSize: 5,
        recordsPerPage: 60
    };
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    /** True when filters changed since the last successful Apply / grid load. */
    filtersDirty = false;

    filters: any = {
        userAccountIds: [] as string[],
        clientIds: [] as string[],
        projectIds: [] as string[],
        scope: "",
        activeOn: null
    };

    optionUsers: any[] = [];
    optionClients: any[] = [];
    optionProjects: any[] = [];

    userFilterText: string = "";
    clientFilterText: string = "";
    projectFilterText: string = "";

    optionsLoading = false;
    cascadeTimer: any = null;
    exporting = false;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "startDate",
            this.BillingRatesService.billingRatesGrid,
            this.onDataLoaded,
            model => {
                model.userAccountIds = self.filters.userAccountIds || [];
                model.clientIds = self.filters.clientIds || [];
                model.projectIds = self.filters.projectIds || [];
                model.scope = self.filters.scope || null;
                model.activeOn = self.filters.activeOn || null;
            },
            null,
            this.$state);

        this.refreshFilterOptions(false);
        this.applyFilters();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    }

    markFiltersDirty = () => {
        this.filtersDirty = true;
    };

    onActiveOnChanged = () => {
        this.markFiltersDirty();
    };

    applyFilters = () => {
        this.filtersDirty = false;
        if (this.pageGrid && this.pageGrid.gridModel) {
            this.pageGrid.gridModel.currentPage = 1;
        }
        this.pageGrid.loadGrid();
    };

    clearAllFilters = () => {
        this.filters.userAccountIds = [];
        this.filters.clientIds = [];
        this.filters.projectIds = [];
        this.filters.scope = "";
        this.filters.activeOn = null;
        this.refreshFilterOptions(false);
        this.applyFilters();
    };

    isSelected = (list: string[], id: string): boolean => {
        return !!list && list.indexOf(id) >= 0;
    };

    toggleUser = (id: string) => {
        this.toggleInList(this.filters.userAccountIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    toggleClient = (id: string) => {
        this.toggleInList(this.filters.clientIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    toggleProject = (id: string) => {
        this.toggleInList(this.filters.projectIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    removeUserChip = (id: string) => {
        this.removeFromList(this.filters.userAccountIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    removeClientChip = (id: string) => {
        this.removeFromList(this.filters.clientIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    removeProjectChip = (id: string) => {
        this.removeFromList(this.filters.projectIds, id);
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    chipName = (options: any[], id: string): string => {
        if (!options) {
            return id;
        }
        for (let i = 0; i < options.length; i++) {
            if (options[i].id === id) {
                return options[i].name;
            }
        }
        return id;
    };

    filteredUsers = (): any[] => {
        return this.filterByText(this.optionUsers, this.userFilterText);
    };

    filteredClients = (): any[] => {
        return this.filterByText(this.optionClients, this.clientFilterText);
    };

    filteredProjects = (): any[] => {
        return this.filterByText(this.optionProjects, this.projectFilterText);
    };

    private filterByText = (options: any[], text: string): any[] => {
        if (!options) {
            return [];
        }
        const q = (text || "").toLowerCase().trim();
        if (!q) {
            return options;
        }
        return options.filter(o => (o.name || "").toLowerCase().indexOf(q) >= 0);
    };

    private toggleInList = (list: string[], id: string) => {
        const idx = list.indexOf(id);
        if (idx >= 0) {
            list.splice(idx, 1);
        } else {
            list.push(id);
        }
    };

    private removeFromList = (list: string[], id: string) => {
        const idx = list.indexOf(id);
        if (idx >= 0) {
            list.splice(idx, 1);
        }
    };

    private scheduleCascade = () => {
        const self = this;
        if (self.cascadeTimer) {
            self.$timeout.cancel(self.cascadeTimer);
        }
        self.cascadeTimer = self.$timeout(() => {
            self.refreshFilterOptions(false);
        }, 250);
    };

    refreshFilterOptions = (loadGridAfter: boolean) => {
        const self = this;
        self.optionsLoading = true;
        self.BillingRatesService.filterOptions({
            userAccountIds: self.filters.userAccountIds,
            clientIds: self.filters.clientIds,
            projectIds: self.filters.projectIds
        }).then(
            result => {
                self.optionUsers = result.users || [];
                self.optionClients = result.clients || [];
                self.optionProjects = result.projects || [];
                self.pruneSelections();
                self.optionsLoading = false;

                if (loadGridAfter) {
                    self.applyFilters();
                }
            },
            error => {
                self.optionsLoading = false;
                self.handleError(error);
            });
    };

    private pruneSelections = () => {
        const beforeUsers = this.filters.userAccountIds.length;
        const beforeClients = this.filters.clientIds.length;
        const beforeProjects = this.filters.projectIds.length;

        this.filters.userAccountIds = this.pruneList(this.filters.userAccountIds, this.optionUsers);
        this.filters.clientIds = this.pruneList(this.filters.clientIds, this.optionClients);
        this.filters.projectIds = this.pruneList(this.filters.projectIds, this.optionProjects);

        if (this.filters.userAccountIds.length !== beforeUsers
            || this.filters.clientIds.length !== beforeClients
            || this.filters.projectIds.length !== beforeProjects) {
            this.markFiltersDirty();
        }
    };

    private pruneList = (selected: string[], options: any[]): string[] => {
        if (!selected || selected.length === 0) {
            return [];
        }
        const allowed: any = {};
        for (let i = 0; i < (options || []).length; i++) {
            allowed[options[i].id] = true;
        }
        return selected.filter(id => !!allowed[id]);
    };

    newRecord = (scope?: string, clientId?: string, projectId?: string, userId?: string) => {
        const params: any = { id: "new" };

        if (userId) {
            params.userId = userId;
        } else if (this.filters.userAccountIds.length === 1) {
            params.userId = this.filters.userAccountIds[0];
        }

        if (scope) {
            params.scope = scope;
        }
        if (clientId) {
            params.clientId = clientId;
        }
        if (projectId) {
            params.projectId = projectId;
        }

        this.$state.transitionTo("mainState.maintenance.billingRatesMaintenance.detail", params);
    };

    editRecord = (rateId: string) => {
        if (!rateId) {
            return;
        }
        this.$state.transitionTo("mainState.maintenance.billingRatesMaintenance.detail", { id: rateId });
    };

    exportExcel = () => {
        const self = this;
        if (self.exporting) {
            return;
        }

        self.exporting = true;
        self.BillingRatesService.exportExcel({
            userAccountIds: self.filters.userAccountIds || [],
            clientIds: self.filters.clientIds || [],
            projectIds: self.filters.projectIds || [],
            scope: self.filters.scope || null,
            activeOn: self.filters.activeOn || null,
            resultMode: "periods"
        }).then(
            (response: any) => {
                self.exporting = false;
                const disposition = response.headers("content-disposition") || "";
                let filename = "BillingRates.xlsx";
                const match = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/i.exec(disposition);
                if (match && match[1]) {
                    filename = match[1].replace(/['"]/g, "");
                }

                const blob = new Blob([response.data], {
                    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement("a");
                link.href = url;
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            },
            error => {
                self.exporting = false;
                self.handleError(error);
            });
    };
}

angular.module("AngularApp")
    .controller("BillingRatesMaintenanceGridController",
    [
        "$scope",
        "$state",
        "$timeout",
        "BillingRatesService",
        "Popups",
        BillingRatesMaintenanceGridController
    ]);
