class ScorecardTemplateMaintenanceGridController extends CHControllerBase {

    //#region Members

    templates: any[] = [];
    loading = false;
    loaded = false;
    searchText = "";
    statusFilter = "active";
    summary: any = { total: 0, active: 0, inactive: 0 };

    /** Id of the template whose duplicate or delete request is in flight. */
    busyId: string = null;

    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.loadTemplates();
    }

    //#endregion

    //#region Template list

    loadTemplates = (): void => {
        const me = this;
        me.loading = true;
        me.ScorecardTemplateService.scorecardTemplateGrid({
            searchFor: "",
            sortKey: "scorecardname",
            sortOrder: "ASC",
            currentPage: 1,
            recordsPerPage: 1000
        } as any).then(
            result => {
                me.templates = (result && result.results) || [];
                me.refreshSummary();
                me.loading = false;
                me.loaded = true;
            },
            error => {
                me.loading = false;
                me.handleError(error);
            });
    };

    private refreshSummary = (): void => {
        const s = { total: this.templates.length, active: 0, inactive: 0 };
        for (let i = 0; i < this.templates.length; i++) {
            if (this.templates[i].isActive) {
                s.active++;
            } else {
                s.inactive++;
            }
        }
        this.summary = s;
    };

    filteredTemplates = (): any[] => {
        const q = (this.searchText || "").toLowerCase().trim();
        const status = this.statusFilter;
        return this.templates.filter(t =>
            (!q || (t.scorecardName || "").toLowerCase().indexOf(q) >= 0
                || (t.scorecardCode || "").toLowerCase().indexOf(q) >= 0)
            && (status === "all" || (status === "active") === !!t.isActive));
    };

    clearFilters = (): void => {
        this.searchText = "";
        this.statusFilter = "all";
    };

    weightState = (total: number): string => {
        const value = Number(total) || 0;
        if (Math.abs(value - 100) < 0.001) {
            return "is-complete";
        }
        return value > 100 ? "is-over" : "is-under";
    };

    weightPercent = (total: number): number => {
        return Math.max(0, Math.min(100, Number(total) || 0));
    };

    weightTooltip = (total: number): string => {
        const value = Number(total) || 0;
        const state = this.weightState(value);
        if (state === "is-complete") {
            return "Item weights add up to 100%.";
        }
        return state === "is-over"
            ? "Item weights add up to " + value + "%, which is more than 100%."
            : "Item weights add up to " + value + "%. " + (100 - value) + "% is still unallocated.";
    };

    //#endregion

    //#region Actions

    newTemplate = (): void => {
        this.$state.go("mainState.maintenance.scorecardTemplateMaintenance.detail", { id: "new" });
    };

    open = (template: any): void => {
        this.$state.go("mainState.maintenance.scorecardTemplateMaintenance.detail", { id: template.id });
    };

    duplicate = (template: any): void => {
        const me = this;
        if (me.busyId) {
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Duplicate \u201c" + template.scorecardName + "\u201d?",
            message: "Creates a new template with the same settings and all " + template.itemCount
                + " item" + (template.itemCount === 1 ? "" : "s") + ". Periods are not copied.",
            confirmText: "Duplicate",
            cancelText: "Cancel",
            tone: "primary",
            icon: "fa-clone",
            input: { label: "New template name", value: "Copy of " + template.scorecardName, maxLength: 500 },
            note: "The copy starts inactive. Activate it from its settings when it is ready."
        }).then(name => {
            if (!name) {
                return;
            }
            me.busyId = template.id;
            me.ScorecardTemplateService.scorecardTemplateDuplicate({ sourceId: template.id, scorecardName: name })
                .then(
                    result => {
                        me.busyId = null;
                        me.$state.go("mainState.maintenance.scorecardTemplateMaintenance.detail",
                            { id: result.id, copiedFrom: template.scorecardName });
                    },
                    error => {
                        me.busyId = null;
                        me.handleError(error);
                    });
        });
    };

    deleteTemplate = (template: any): void => {
        const me = this;
        if (me.busyId || template.scorecardCount > 0) {
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Delete \u201c" + template.scorecardName + "\u201d?",
            message: "This removes the template together with its " + template.itemCount + " item"
                + (template.itemCount === 1 ? "" : "s") + " and " + template.periodCount + " period"
                + (template.periodCount === 1 ? "" : "s") + ". This cannot be undone.",
            confirmText: "Delete template",
            cancelText: "Keep",
            tone: "danger",
            icon: "fa-trash"
        }).then(action => {
            if (!action) {
                return;
            }
            me.busyId = template.id;
            me.ScorecardTemplateService.scorecardTemplateDelete({ id: template.id })
                .then(
                    () => {
                        me.busyId = null;
                        const idx = me.templates.indexOf(template);
                        if (idx >= 0) {
                            me.templates.splice(idx, 1);
                        }
                        me.refreshSummary();
                    },
                    error => {
                        me.busyId = null;
                        me.handleError(error);
                    });
        });
    };

    //#endregion
}

angular.module("AngularApp")
    .controller("ScorecardTemplateMaintenanceGridController",
        [
            "$scope",
            "$state",
            "ScorecardTemplateService",
            "Popups",
            ScorecardTemplateMaintenanceGridController
        ]);
