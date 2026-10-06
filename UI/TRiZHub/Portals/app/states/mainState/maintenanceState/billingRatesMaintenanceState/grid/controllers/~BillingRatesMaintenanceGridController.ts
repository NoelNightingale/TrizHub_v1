declare var APP_CACHE_VER: string;

class BillingRatesMaintenanceGridController extends CHControllerBase {

    //#region Members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: any = {
        data: [],
        totalItems: 0,
        sortKeyOrder: { order: "ASC", key: "userName" },
        currentPage: 1,
        maxSize: 5,
        recordsPerPage: 60
    };
    onDataLoaded = (event) => { this.onLoadEvent(event); };

    effectiveGrid: any;
    effectiveGridModel: any = {
        data: [],
        totalItems: 0,
        sortKeyOrder: { order: "ASC", key: "userName" },
        currentPage: 1,
        maxSize: 5,
        recordsPerPage: 60
    };
    onEffectiveLoaded = (event) => { this.effectiveGridModel = event; };

    viewMode: string = "periods"; // "periods" | "effective"

    /** True when filters changed since the last successful Apply / grid load. */
    filtersDirty = false;

        filters: any = {
        userAccountIds: [] as string[],
        clientIds: [] as string[],
        projectIds: [] as string[],
        scope: "",
        activeOn: new Date(),
        rangeStart: new Date(),
        rangeEnd: new Date(),
        userStatus: "active",
        clientStatus: "active",
        projectStatus: "active"
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

    /** Id of the row just saved from the edit modal; drives a brief highlight. */
    highlightId: string = null;
    private highlightTimer: any = null;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private $uibModal: any,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {

        super($scope, Popups, $state);
        const self = this;

        this.pageGrid = new TcrGridServiceModule.TcrGridService(
            "userName",
            this.BillingRatesService.billingRatesGrid,
            this.onDataLoaded,
            model => {
                model.userAccountIds = self.filters.userAccountIds || [];
                model.clientIds = self.filters.clientIds || [];
                model.projectIds = self.filters.projectIds || [];
                model.scope = self.filters.scope || null;
                model.activeOn = null;
                model.rangeStart = self.filters.rangeStart || null;
                model.rangeEnd = self.filters.rangeEnd || null;
                model.userStatus = self.filters.userStatus || "active";
                model.clientStatus = self.filters.clientStatus || "active";
                model.projectStatus = self.filters.projectStatus || "active";
            },
            null,
            this.$state);

        this.effectiveGrid = new TcrGridServiceModule.TcrGridService(
            "userName",
            this.BillingRatesService.effectiveRatesGrid,
            this.onEffectiveLoaded,
            model => {
                model.userAccountIds = self.filters.userAccountIds || [];
                model.clientIds = self.filters.clientIds || [];
                model.projectIds = self.filters.projectIds || [];
                model.activeOn = self.filters.activeOn;
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

    clearDate = (field: string) => {
        this.filters[field] = null;
        this.markFiltersDirty();
    };

    setViewMode = (mode: string) => {
        if (this.viewMode === mode) {
            return;
        }
        this.viewMode = mode;
        this.applyFilters();
    };

    setStatusFilter = (dimension: string, status: string) => {
        if (dimension === "user") {
            if (this.filters.userStatus === status) {
                return;
            }
            this.filters.userStatus = status;
        } else if (dimension === "client") {
            if (this.filters.clientStatus === status) {
                return;
            }
            this.filters.clientStatus = status;
        } else if (dimension === "project") {
            if (this.filters.projectStatus === status) {
                return;
            }
            this.filters.projectStatus = status;
        } else {
            return;
        }
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    applyFilters = () => {
        if (!this.validateDates()) {
            return;
        }
        this.filtersDirty = false;
        const grid = this.viewMode === "effective" ? this.effectiveGrid : this.pageGrid;
        if (grid && grid.gridModel) {
            grid.gridModel.currentPage = 1;
        }
        grid.loadGrid();
    };

    private validateDates = (): boolean => {
        if (this.viewMode === "effective") {
            if (!this.filters.activeOn) {
                this.Popups.showError(this.$scope, "Select an Effective On date.");
                return false;
            }
            return true;
        }
        if (this.filters.rangeStart && this.filters.rangeEnd
            && new Date(this.filters.rangeStart).getTime() > new Date(this.filters.rangeEnd).getTime()) {
            this.Popups.showError(this.$scope, "Start Date must be on or before End Date.");
            return false;
        }
        return true;
    };

    clearAllFilters = () => {
        this.filters.userAccountIds = [];
        this.filters.clientIds = [];
        this.filters.projectIds = [];
        this.filters.scope = "";
        this.filters.activeOn = new Date();
        this.filters.rangeStart = new Date();
        this.filters.rangeEnd = new Date();
        this.filters.userStatus = "active";
        this.filters.clientStatus = "active";
        this.filters.projectStatus = "active";
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

    /** True when every option currently visible in the list (after search) is selected. */
    areAllSelected = (dimension: string): boolean => {
        const visible = this.visibleOptions(dimension);
        const selected = this.selectedList(dimension);
        if (!visible.length || !selected) {
            return false;
        }
        return visible.every(o => selected.indexOf(o.id) >= 0);
    };

    toggleSelectAll = (dimension: string) => {
        const visible = this.visibleOptions(dimension);
        const selected = this.selectedList(dimension);
        if (!visible.length || !selected) {
            return;
        }
        if (this.areAllSelected(dimension)) {
            visible.forEach(o => this.removeFromList(selected, o.id));
        } else {
            visible.forEach(o => {
                if (selected.indexOf(o.id) < 0) {
                    selected.push(o.id);
                }
            });
        }
        this.markFiltersDirty();
        this.scheduleCascade();
    };

    private visibleOptions = (dimension: string): any[] => {
        if (dimension === "user") {
            return this.filteredUsers();
        }
        if (dimension === "client") {
            return this.filteredClients();
        }
        if (dimension === "project") {
            return this.filteredProjects();
        }
        return [];
    };

    private selectedList = (dimension: string): string[] => {
        if (dimension === "user") {
            return this.filters.userAccountIds;
        }
        if (dimension === "client") {
            return this.filters.clientIds;
        }
        if (dimension === "project") {
            return this.filters.projectIds;
        }
        return null;
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
            projectIds: self.filters.projectIds,
            userStatus: self.filters.userStatus || "active",
            clientStatus: self.filters.clientStatus || "active",
            projectStatus: self.filters.projectStatus || "active"
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

        this.openEditor(params);
    };

    editRecord = (rateId: string) => {
        if (!rateId) {
            return;
        }
        this.openEditor({ id: rateId });
    };

    private openEditor = (params: any) => {
        const self = this;
        const modal = this.$uibModal.open({
            animation: false,
            templateUrl: "Portals/app/states/mainState/maintenanceState/billingRatesMaintenanceState/detail/views/mainView.html?" + APP_CACHE_VER,
            controller: "BillingRatesMaintenanceDetailController",
            controllerAs: "vm",
            backdrop: "static",
            windowClass: "br-edit-modal",
            resolve: {
                modalParams: () => params
            }
        });
        modal.result.then(
            result => {
                self.onEditorClosed(result);
            },
            () => { });
    };

    private onEditorClosed = (result: any) => {
        const self = this;
        if (!result || !result.id) {
            return;
        }
        if (this.viewMode === "effective") {
            if (result.action === "saved") {
                this.flashRow(result.id);
            }
            this.effectiveGrid.loadGrid();
            return;
        }
        if (result.action === "deleted") {
            this.removeRow(result.id);
            return;
        }
        this.BillingRatesService.billingRatesGridRow(result.id)
            .then(
                row => {
                    if (row) {
                        self.upsertRow(row);
                    } else {
                        self.pageGrid.loadGrid();
                    }
                },
                error => {
                    self.handleError(error);
                });
    };

    private findRowIndex = (id: string): number => {
        const data = this.gridModel.data || [];
        for (let i = 0; i < data.length; i++) {
            if (data[i].id === id) {
                return i;
            }
        }
        return -1;
    };

    private removeRow = (id: string) => {
        const idx = this.findRowIndex(id);
        if (idx < 0) {
            return;
        }
        this.gridModel.data.splice(idx, 1);
        this.gridModel.totalItems = Math.max(0, (this.gridModel.totalItems || 0) - 1);
    };

    private upsertRow = (row: any) => {
        if (!this.gridModel.data) {
            this.gridModel.data = [];
        }
        const idx = this.findRowIndex(row.id);
        if (idx >= 0) {
            this.gridModel.data[idx] = row;
        } else {
            this.gridModel.data.unshift(row);
            this.gridModel.totalItems = (this.gridModel.totalItems || 0) + 1;
        }
        this.flashRow(row.id);
    };

    private flashRow = (id: string) => {
        const self = this;
        if (self.highlightTimer) {
            self.$timeout.cancel(self.highlightTimer);
        }
        self.highlightId = id;
        self.highlightTimer = self.$timeout(() => {
            self.highlightId = null;
            self.highlightTimer = null;
        }, 2500);
    };

    exportExcel = () => {
        const self = this;
        if (self.exporting || !self.validateDates()) {
            return;
        }

        const effective = self.viewMode === "effective";
        self.exporting = true;
        self.BillingRatesService.exportExcel({
            userAccountIds: self.filters.userAccountIds || [],
            clientIds: self.filters.clientIds || [],
            projectIds: self.filters.projectIds || [],
            scope: effective ? null : (self.filters.scope || null),
            activeOn: effective ? self.filters.activeOn : null,
            rangeStart: effective ? null : (self.filters.rangeStart || null),
            rangeEnd: effective ? null : (self.filters.rangeEnd || null),
            resultMode: effective ? "effective" : "periods",
            userStatus: self.filters.userStatus || "active",
            clientStatus: self.filters.clientStatus || "active",
            projectStatus: self.filters.projectStatus || "active"
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
        "$uibModal",
        "BillingRatesService",
        "Popups",
        BillingRatesMaintenanceGridController
    ]);
