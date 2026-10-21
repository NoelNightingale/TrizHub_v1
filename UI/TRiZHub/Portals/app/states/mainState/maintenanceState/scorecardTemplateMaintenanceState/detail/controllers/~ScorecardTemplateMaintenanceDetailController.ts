class ScorecardTemplateMaintenanceDetailController extends CHControllerBase {

    //#region Members

    templateId: string = null;
    isNew = false;
    loading = false;

    /** Last template state returned by the server, including item, period and scorecard counts. */
    template: any = null;

    /** Name of the template this one was just copied from; drives a one-off notice. */
    copiedFrom: string = null;

    /** True while a template delete request is in flight. */
    busy = false;

    settings: any = null;
    settingsForm: any;
    settingsSaving = false;
    settingsSubmitted = false;
    settingsError: string = null;
    private settingsSnapshot: string = null;

    items: any[] = [];
    itemsLoading = false;
    selectedItemId: string = null;

    itemForm: any;
    editor: any = { key: 0, loading: false, saving: false, submitted: false, isNew: false, error: null, record: null };
    private itemSnapshot: string = null;

    periods: any[] = [];
    periodsLoading = false;

    /** Set once the user has agreed to leave, so the state-change guard lets the next transition through. */
    private allowLeave = false;

    summernoteOptions = {
        height: 110,
        focus: false,
        airMode: false,
        shortcuts: true,
        toolbar: [
            ["style", ["bold", "italic", "underline"]],
            ["textsize", ["fontsize"]],
            ["fontclr", ["color"]],
            ["alignment", ["ul", "ol", "paragraph", "lineheight"]]
        ],
        disableDragAndDrop: true
    };

    //#endregion

    //#region Ctor

    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private Popups: any) {
        super($scope, Popups, $state);
        const me = this;
        const id = me.$stateParams["id"];
        me.copiedFrom = me.$stateParams["copiedFrom"] || null;

        if (!id || id === "new") {
            me.isNew = true;
            me.settings = {
                id: null,
                scorecardName: "",
                scorecardCode: null,
                excellentWeight: 0,
                adequateWeight: 0,
                inadequateWeight: 0,
                isActive: true
            };
            me.settingsSnapshot = me.snapshotSettings();
        } else {
            me.templateId = id;
            me.loadTemplate();
            me.loadItems(null);
            me.loadPeriods();
        }

        me.$scope.$on("$stateChangeStart", (event: any, toState: any, toParams: any) => {
            if (me.allowLeave || !me.hasUnsavedChanges()) {
                return;
            }
            event.preventDefault();
            me.confirmDiscard("You have unsaved changes on this template.").then(discard => {
                if (discard) {
                    me.allowLeave = true;
                    me.$state.go(toState.name, toParams);
                }
            });
        });
    }

    //#endregion

    //#region Template settings

    private loadTemplate = (): void => {
        const me = this;
        me.loading = true;
        me.ScorecardTemplateService.scorecardTemplateGet(me.templateId)
            .then(
                result => {
                    me.applyTemplate(result);
                    me.loading = false;
                },
                error => {
                    me.loading = false;
                    me.handleError(error);
                });
    };

    private applyTemplate = (result: any): void => {
        this.template = result;
        this.settings = {
            id: result.id,
            scorecardName: result.scorecardName,
            scorecardCode: result.scorecardCode,
            excellentWeight: result.excellentWeight,
            adequateWeight: result.adequateWeight,
            inadequateWeight: result.inadequateWeight,
            isActive: result.isActive
        };
        this.settingsSnapshot = this.snapshotSettings();
    };

    private snapshotSettings = (): string => {
        const s = this.settings;
        if (!s) {
            return null;
        }
        return angular.toJson({
            name: (s.scorecardName || "").trim(),
            e: Number(s.excellentWeight) || 0,
            a: Number(s.adequateWeight) || 0,
            i: Number(s.inadequateWeight) || 0,
            active: !!s.isActive
        });
    };

    isSettingsDirty = (): boolean => {
        return !!this.settings && this.snapshotSettings() !== this.settingsSnapshot;
    };

    percentageFormatting = (value: any): string => {
        return value.toString() + "%";
    };

    setActive = (isActive: boolean): void => {
        if (this.settings) {
            this.settings.isActive = isActive;
        }
    };

    revertSettings = (): void => {
        if (this.isNew) {
            this.$state.go("mainState.maintenance.scorecardTemplateMaintenance.grid");
            return;
        }
        if (this.template) {
            this.applyTemplate(this.template);
            this.settingsError = null;
            this.settingsSubmitted = false;
        }
    };

    saveSettings = (): void => {
        const me = this;
        const s = me.settings;
        if (!s || me.settingsSaving) {
            return;
        }
        me.settingsSubmitted = true;
        me.settingsError = null;
        if (!(s.scorecardName || "").trim()) {
            me.settingsError = "Please enter a template name.";
            return;
        }
        if ((Number(s.excellentWeight) || 0) < 1 && (Number(s.adequateWeight) || 0) < 1 && (Number(s.inadequateWeight) || 0) < 1) {
            me.settingsError = "Please specify at least one E, A or I weight.";
            return;
        }

        const payload = {
            id: s.id || null,
            scorecardName: s.scorecardName.trim(),
            scorecardCode: s.scorecardCode,
            excellentWeight: Number(s.excellentWeight) || 0,
            adequateWeight: Number(s.adequateWeight) || 0,
            inadequateWeight: Number(s.inadequateWeight) || 0,
            isActive: !!s.isActive
        };

        me.settingsSaving = true;
        me.ScorecardTemplateService.scorecardTemplateSave(payload)
            .then(
                result => {
                    me.settingsSaving = false;
                    me.settingsSubmitted = false;
                    me.applyTemplate(result);
                    if (me.isNew) {
                        me.allowLeave = true;
                        me.$state.go("mainState.maintenance.scorecardTemplateMaintenance.detail",
                            { id: result.id }, { location: "replace" });
                    }
                },
                error => {
                    me.settingsSaving = false;
                    me.settingsError = error;
                });
    };

    //#endregion

    //#region Template actions

    deleteTemplate = (): void => {
        const me = this;
        if (me.busy || !me.template || me.template.scorecardCount > 0) {
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Delete \u201c" + me.template.scorecardName + "\u201d?",
            message: "This removes the template together with its " + me.items.length + " item"
                + (me.items.length === 1 ? "" : "s") + " and " + me.periods.length + " period"
                + (me.periods.length === 1 ? "" : "s") + ". This cannot be undone.",
            confirmText: "Delete template",
            cancelText: "Keep",
            tone: "danger",
            icon: "fa-trash"
        }).then(action => {
            if (!action) {
                return;
            }
            me.busy = true;
            me.ScorecardTemplateService.scorecardTemplateDelete({ id: me.templateId })
                .then(
                    () => {
                        me.busy = false;
                        me.allowLeave = true;
                        me.$state.go("mainState.maintenance.scorecardTemplateMaintenance.grid");
                    },
                    error => {
                        me.busy = false;
                        me.handleError(error);
                    });
        });
    };

    dismissCopiedNotice = (): void => {
        this.copiedFrom = null;
    };

    //#endregion

    //#region Items list

    private loadItems = (selectId: string): void => {
        const me = this;
        me.itemsLoading = true;
        me.ScorecardTemplateService.scorecardTemplateItemGrid({
            id: me.templateId,
            searchFor: "",
            sortKey: "order",
            sortOrder: "ASC",
            currentPage: 1,
            recordsPerPage: 1000
        } as any).then(
            result => {
                me.items = ((result && result.results) || []).slice().sort((a, b) => me.compareOrder(a.order, b.order));
                me.itemsLoading = false;
                const target = selectId || (me.editor.record ? null : (me.items.length ? me.items[0].id : null));
                if (target) {
                    me.openItem(target);
                }
            },
            error => {
                me.itemsLoading = false;
                me.handleError(error);
            });
    };

    /** Natural order so "2" sorts before "10" and "1.2" before "1.10". */
    private compareOrder = (a: string, b: string): number => {
        const re = /(\d+|\D+)/g;
        const pa = String(a == null ? "" : a).match(re) || [];
        const pb = String(b == null ? "" : b).match(re) || [];
        for (let i = 0; i < Math.min(pa.length, pb.length); i++) {
            const na = parseInt(pa[i], 10);
            const nb = parseInt(pb[i], 10);
            if (!isNaN(na) && !isNaN(nb)) {
                if (na !== nb) {
                    return na - nb;
                }
                continue;
            }
            const sa = pa[i].toLowerCase();
            const sb = pb[i].toLowerCase();
            if (sa !== sb) {
                return sa < sb ? -1 : 1;
            }
        }
        return pa.length - pb.length;
    };

    totalWeight = (): number => {
        let total = 0;
        for (let i = 0; i < this.items.length; i++) {
            total += Number(this.items[i].weight) || 0;
        }
        return Math.round(total * 100) / 100;
    };

    weightState = (): string => {
        const total = this.totalWeight();
        if (Math.abs(total - 100) < 0.001) {
            return "is-complete";
        }
        return total > 100 ? "is-over" : "is-under";
    };

    weightPercent = (): number => {
        return Math.max(0, Math.min(100, this.totalWeight()));
    };

    weightSummary = (): string => {
        const total = this.totalWeight();
        const state = this.weightState();
        if (state === "is-complete") {
            return "Fully allocated";
        }
        return state === "is-over"
            ? (Math.round((total - 100) * 100) / 100) + "% over"
            : (Math.round((100 - total) * 100) / 100) + "% unallocated";
    };

    /** Highest weight the open item can take without the template exceeding 100%. */
    maxItemWeight = (): number => {
        const r = this.editor.record;
        let others = 0;
        for (let i = 0; i < this.items.length; i++) {
            if (!r || this.items[i].id !== r.id) {
                others += Number(this.items[i].weight) || 0;
            }
        }
        return Math.max(0, Math.round((100 - others) * 100) / 100);
    };

    isSelected = (item: any): boolean => {
        return !this.editor.isNew && this.selectedItemId === item.id;
    };

    //#endregion

    //#region Item editor

    selectItem = (item: any): void => {
        const me = this;
        if (me.isSelected(item) || me.editor.saving) {
            return;
        }
        me.guardItemChange(() => me.openItem(item.id));
    };

    addItem = (): void => {
        const me = this;
        if (me.editor.saving) {
            return;
        }
        if (me.totalWeight() >= 100) {
            me.handleError("Cannot add more items, weight would exceed 100%. Lower the weight of another item first.");
            return;
        }
        me.guardItemChange(() => {
            me.resetEditor(true);
            me.selectedItemId = null;
            me.editor.record = {
                id: null,
                description: "",
                definition: "",
                order: me.nextOrder(),
                weight: 0,
                scorecardScoring: 0,
                minimum: null,
                maximum: null,
                manualDefinition: "",
                excellentDefinition: "",
                adequateDefinition: "",
                inadequateDefinition: ""
            };
            me.itemSnapshot = me.snapshotItem();
        });
    };

    private nextOrder = (): string => {
        let max = 0;
        for (let i = 0; i < this.items.length; i++) {
            const n = parseInt(this.items[i].order, 10);
            if (!isNaN(n) && n > max) {
                max = n;
            }
        }
        return String(max + 1);
    };

    private openItem = (itemId: string): void => {
        const me = this;
        me.resetEditor(false);
        me.selectedItemId = itemId;
        me.editor.loading = true;
        me.ScorecardTemplateService.scorecardTemplateItemGet(itemId)
            .then(
                result => {
                    if (me.selectedItemId !== itemId || me.editor.isNew) {
                        return;
                    }
                    me.editor.record = {
                        id: result.id,
                        description: result.description,
                        definition: result.definition || "",
                        order: result.order,
                        weight: Number(result.weight) || 0,
                        scorecardScoring: Number(result.scorecardScoring) || 0,
                        minimum: result.minimum,
                        maximum: result.maximum,
                        manualDefinition: result.manualDefinition || "",
                        excellentDefinition: result.excellentDefinition || "",
                        adequateDefinition: result.adequateDefinition || "",
                        inadequateDefinition: result.inadequateDefinition || ""
                    };
                    me.itemSnapshot = me.snapshotItem();
                    me.editor.loading = false;
                },
                error => {
                    me.editor.loading = false;
                    me.editor.error = error;
                });
    };

    private resetEditor = (isNew: boolean): void => {
        this.editor = {
            key: this.editor.key + 1,
            loading: false,
            saving: false,
            submitted: false,
            isNew: isNew,
            error: null,
            record: null
        };
        this.itemSnapshot = null;
    };

    private snapshotItem = (): string => {
        const r = this.editor.record;
        if (!r) {
            return null;
        }
        const text = (v: any): string => (v == null ? "" : String(v)).trim();
        const num = (v: any): any => (v === null || v === undefined || v === "" ? null : Number(v));
        return angular.toJson({
            description: text(r.description),
            definition: text(r.definition).replace(/<p><br\s*\/?><\/p>/g, ""),
            order: text(r.order),
            weight: Number(r.weight) || 0,
            scoring: Number(r.scorecardScoring) || 0,
            minimum: num(r.minimum),
            maximum: num(r.maximum),
            manual: text(r.manualDefinition),
            e: text(r.excellentDefinition),
            a: text(r.adequateDefinition),
            i: text(r.inadequateDefinition)
        });
    };

    isItemDirty = (): boolean => {
        return !!this.editor.record && this.snapshotItem() !== this.itemSnapshot;
    };

    hasUnsavedChanges = (): boolean => {
        return this.isSettingsDirty() || this.isItemDirty();
    };

    setScoring = (scoring: number): void => {
        if (this.editor.record) {
            this.editor.record.scorecardScoring = scoring;
        }
    };

    private guardItemChange = (proceed: () => void): void => {
        if (!this.isItemDirty()) {
            proceed();
            return;
        }
        this.confirmDiscard("You have unsaved changes to this item.").then(discard => {
            if (discard) {
                proceed();
            }
        });
    };

    private confirmDiscard = (message: string): ng.IPromise<boolean> => {
        return this.Popups.actionDialog(this.$scope, {
            title: "Discard changes?",
            message: message,
            confirmText: "Discard",
            cancelText: "Keep editing",
            tone: "danger",
            icon: "fa-undo"
        });
    };

    cancelItem = (): void => {
        const me = this;
        if (me.editor.saving) {
            return;
        }
        if (me.editor.isNew) {
            me.guardItemChange(() => {
                me.resetEditor(false);
                if (me.items.length) {
                    me.openItem(me.items[0].id);
                }
            });
            return;
        }
        if (me.selectedItemId) {
            me.guardItemChange(() => me.openItem(me.selectedItemId));
        }
    };

    saveItem = (): void => {
        const me = this;
        const r = me.editor.record;
        if (!r || me.editor.saving) {
            return;
        }
        me.editor.submitted = true;
        me.editor.error = null;

        const missing: string[] = [];
        if (!(r.description || "").trim()) {
            missing.push("Performance measure");
        }
        if (!String(r.order == null ? "" : r.order).trim()) {
            missing.push("Order");
        }
        if (missing.length) {
            me.editor.error = "Please complete the required fields: " + missing.join(", ") + ".";
            return;
        }
        if (!(Number(r.weight) > 0)) {
            me.editor.error = "An item needs a weight greater than 0%.";
            return;
        }
        if (Number(r.weight) > me.maxItemWeight()) {
            me.editor.error = "This weight would take the template over 100%. The most this item can have is " + me.maxItemWeight() + "%.";
            return;
        }
        const isManual = Number(r.scorecardScoring) === 1;
        const min = r.minimum === "" ? null : r.minimum;
        const max = r.maximum === "" ? null : r.maximum;
        if (isManual && min != null && max != null && Number(max) < Number(min)) {
            me.editor.error = "The maximum score must be greater than or equal to the minimum score.";
            return;
        }

        const payload: any = {
            scorecardTemplateId: me.templateId,
            description: r.description.trim(),
            definition: r.definition,
            order: String(r.order).trim(),
            weight: Number(r.weight),
            scorecardScoring: isManual ? 1 : 0,
            minimum: min,
            maximum: max,
            manualDefinition: r.manualDefinition,
            excellentDefinition: r.excellentDefinition,
            adequateDefinition: r.adequateDefinition,
            inadequateDefinition: r.inadequateDefinition
        };
        if (r.id) {
            payload.id = r.id;
        }

        me.editor.saving = true;
        me.ScorecardTemplateService.scorecardTemplateItemSave(payload)
            .then(
                result => {
                    me.editor.saving = false;
                    me.itemSnapshot = me.snapshotItem();
                    me.loadItems(result.id);
                },
                error => {
                    me.editor.saving = false;
                    me.editor.error = error;
                });
    };

    deleteItem = (): void => {
        const me = this;
        const r = me.editor.record;
        if (!r || !r.id || me.editor.saving) {
            return;
        }
        me.Popups.actionDialog(me.$scope, {
            title: "Delete item?",
            message: "\u201c" + (r.description || "This item") + "\u201d will be removed from the template.",
            confirmText: "Delete item",
            cancelText: "Keep",
            tone: "danger",
            icon: "fa-trash"
        }).then(action => {
            if (!action) {
                return;
            }
            me.editor.saving = true;
            me.ScorecardTemplateService.scorecardTemplateItemDelete({ id: r.id })
                .then(
                    () => {
                        me.resetEditor(false);
                        me.selectedItemId = null;
                        me.loadItems(null);
                    },
                    error => {
                        me.editor.saving = false;
                        me.editor.error = error;
                    });
        });
    };

    //#endregion

    //#region Periods

    private loadPeriods = (): void => {
        const me = this;
        me.periodsLoading = true;
        me.ScorecardTemplateService.scorecardTemplatePeriodGrid({
            id: me.templateId,
            searchFor: "",
            sortKey: "startdate",
            sortOrder: "ASC",
            currentPage: 1,
            recordsPerPage: 1000
        } as any).then(
            result => {
                me.periods = (result && result.results) || [];
                me.periodsLoading = false;
            },
            error => {
                me.periodsLoading = false;
                me.handleError(error);
            });
    };

    //#endregion
}

angular.module("AngularApp")
    .controller("ScorecardTemplateMaintenanceDetailController",
        [
            "$stateParams",
            "$scope",
            "$state",
            "ScorecardTemplateService",
            "Popups",
            ScorecardTemplateMaintenanceDetailController
        ]);
