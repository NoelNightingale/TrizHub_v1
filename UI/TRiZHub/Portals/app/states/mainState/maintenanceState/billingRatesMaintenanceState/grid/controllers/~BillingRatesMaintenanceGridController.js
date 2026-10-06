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
var BillingRatesMaintenanceGridController = /** @class */ (function (_super) {
    __extends(BillingRatesMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function BillingRatesMaintenanceGridController($scope, $state, $timeout, $uibModal, BillingRatesService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.$uibModal = $uibModal;
        _this.BillingRatesService = BillingRatesService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.gridModel = {
            data: [],
            totalItems: 0,
            sortKeyOrder: { order: "ASC", key: "userName" },
            currentPage: 1,
            maxSize: 5,
            recordsPerPage: 60
        };
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.effectiveGridModel = {
            data: [],
            totalItems: 0,
            sortKeyOrder: { order: "ASC", key: "userName" },
            currentPage: 1,
            maxSize: 5,
            recordsPerPage: 60
        };
        _this.onEffectiveLoaded = function (event) { _this.effectiveGridModel = event; };
        _this.viewMode = "periods"; // "periods" | "effective"
        /** True when filters changed since the last successful Apply / grid load. */
        _this.filtersDirty = false;
        _this.filters = {
            userAccountIds: [],
            clientIds: [],
            projectIds: [],
            scope: "",
            activeOn: new Date(),
            rangeStart: new Date(),
            rangeEnd: new Date(),
            userStatus: "active",
            clientStatus: "active",
            projectStatus: "active"
        };
        _this.optionUsers = [];
        _this.optionClients = [];
        _this.optionProjects = [];
        _this.userFilterText = "";
        _this.clientFilterText = "";
        _this.projectFilterText = "";
        _this.optionsLoading = false;
        _this.cascadeTimer = null;
        _this.exporting = false;
        /** Id of the row just saved from the edit modal; drives a brief highlight. */
        _this.highlightId = null;
        _this.highlightTimer = null;
        _this.markFiltersDirty = function () {
            _this.filtersDirty = true;
        };
        _this.onActiveOnChanged = function () {
            _this.markFiltersDirty();
        };
        _this.clearDate = function (field) {
            _this.filters[field] = null;
            _this.markFiltersDirty();
        };
        _this.setViewMode = function (mode) {
            if (_this.viewMode === mode) {
                return;
            }
            _this.viewMode = mode;
            _this.applyFilters();
        };
        _this.setStatusFilter = function (dimension, status) {
            if (dimension === "user") {
                if (_this.filters.userStatus === status) {
                    return;
                }
                _this.filters.userStatus = status;
            }
            else if (dimension === "client") {
                if (_this.filters.clientStatus === status) {
                    return;
                }
                _this.filters.clientStatus = status;
            }
            else if (dimension === "project") {
                if (_this.filters.projectStatus === status) {
                    return;
                }
                _this.filters.projectStatus = status;
            }
            else {
                return;
            }
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.applyFilters = function () {
            if (!_this.validateDates()) {
                return;
            }
            _this.filtersDirty = false;
            var grid = _this.viewMode === "effective" ? _this.effectiveGrid : _this.pageGrid;
            if (grid && grid.gridModel) {
                grid.gridModel.currentPage = 1;
            }
            grid.loadGrid();
        };
        _this.validateDates = function () {
            if (_this.viewMode === "effective") {
                if (!_this.filters.activeOn) {
                    _this.Popups.showError(_this.$scope, "Select an Effective On date.");
                    return false;
                }
                return true;
            }
            if (_this.filters.rangeStart && _this.filters.rangeEnd
                && new Date(_this.filters.rangeStart).getTime() > new Date(_this.filters.rangeEnd).getTime()) {
                _this.Popups.showError(_this.$scope, "Start Date must be on or before End Date.");
                return false;
            }
            return true;
        };
        _this.clearAllFilters = function () {
            _this.filters.userAccountIds = [];
            _this.filters.clientIds = [];
            _this.filters.projectIds = [];
            _this.filters.scope = "";
            _this.filters.activeOn = new Date();
            _this.filters.rangeStart = new Date();
            _this.filters.rangeEnd = new Date();
            _this.filters.userStatus = "active";
            _this.filters.clientStatus = "active";
            _this.filters.projectStatus = "active";
            _this.refreshFilterOptions(false);
            _this.applyFilters();
        };
        _this.isSelected = function (list, id) {
            return !!list && list.indexOf(id) >= 0;
        };
        _this.toggleUser = function (id) {
            _this.toggleInList(_this.filters.userAccountIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.toggleClient = function (id) {
            _this.toggleInList(_this.filters.clientIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.toggleProject = function (id) {
            _this.toggleInList(_this.filters.projectIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.removeUserChip = function (id) {
            _this.removeFromList(_this.filters.userAccountIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.removeClientChip = function (id) {
            _this.removeFromList(_this.filters.clientIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.removeProjectChip = function (id) {
            _this.removeFromList(_this.filters.projectIds, id);
            _this.markFiltersDirty();
            _this.scheduleCascade();
        };
        _this.chipName = function (options, id) {
            if (!options) {
                return id;
            }
            for (var i = 0; i < options.length; i++) {
                if (options[i].id === id) {
                    return options[i].name;
                }
            }
            return id;
        };
        _this.filteredUsers = function () {
            return _this.filterByText(_this.optionUsers, _this.userFilterText);
        };
        _this.filteredClients = function () {
            return _this.filterByText(_this.optionClients, _this.clientFilterText);
        };
        _this.filteredProjects = function () {
            return _this.filterByText(_this.optionProjects, _this.projectFilterText);
        };
        _this.filterByText = function (options, text) {
            if (!options) {
                return [];
            }
            var q = (text || "").toLowerCase().trim();
            if (!q) {
                return options;
            }
            return options.filter(function (o) { return (o.name || "").toLowerCase().indexOf(q) >= 0; });
        };
        _this.toggleInList = function (list, id) {
            var idx = list.indexOf(id);
            if (idx >= 0) {
                list.splice(idx, 1);
            }
            else {
                list.push(id);
            }
        };
        _this.removeFromList = function (list, id) {
            var idx = list.indexOf(id);
            if (idx >= 0) {
                list.splice(idx, 1);
            }
        };
        _this.scheduleCascade = function () {
            var self = _this;
            if (self.cascadeTimer) {
                self.$timeout.cancel(self.cascadeTimer);
            }
            self.cascadeTimer = self.$timeout(function () {
                self.refreshFilterOptions(false);
            }, 250);
        };
        _this.refreshFilterOptions = function (loadGridAfter) {
            var self = _this;
            self.optionsLoading = true;
            self.BillingRatesService.filterOptions({
                userAccountIds: self.filters.userAccountIds,
                clientIds: self.filters.clientIds,
                projectIds: self.filters.projectIds,
                userStatus: self.filters.userStatus || "active",
                clientStatus: self.filters.clientStatus || "active",
                projectStatus: self.filters.projectStatus || "active"
            }).then(function (result) {
                self.optionUsers = result.users || [];
                self.optionClients = result.clients || [];
                self.optionProjects = result.projects || [];
                self.pruneSelections();
                self.optionsLoading = false;
                if (loadGridAfter) {
                    self.applyFilters();
                }
            }, function (error) {
                self.optionsLoading = false;
                self.handleError(error);
            });
        };
        _this.pruneSelections = function () {
            var beforeUsers = _this.filters.userAccountIds.length;
            var beforeClients = _this.filters.clientIds.length;
            var beforeProjects = _this.filters.projectIds.length;
            _this.filters.userAccountIds = _this.pruneList(_this.filters.userAccountIds, _this.optionUsers);
            _this.filters.clientIds = _this.pruneList(_this.filters.clientIds, _this.optionClients);
            _this.filters.projectIds = _this.pruneList(_this.filters.projectIds, _this.optionProjects);
            if (_this.filters.userAccountIds.length !== beforeUsers
                || _this.filters.clientIds.length !== beforeClients
                || _this.filters.projectIds.length !== beforeProjects) {
                _this.markFiltersDirty();
            }
        };
        _this.pruneList = function (selected, options) {
            if (!selected || selected.length === 0) {
                return [];
            }
            var allowed = {};
            for (var i = 0; i < (options || []).length; i++) {
                allowed[options[i].id] = true;
            }
            return selected.filter(function (id) { return !!allowed[id]; });
        };
        _this.newRecord = function (scope, clientId, projectId, userId) {
            var params = { id: "new" };
            if (userId) {
                params.userId = userId;
            }
            else if (_this.filters.userAccountIds.length === 1) {
                params.userId = _this.filters.userAccountIds[0];
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
            _this.openEditor(params);
        };
        _this.editRecord = function (rateId) {
            if (!rateId) {
                return;
            }
            _this.openEditor({ id: rateId });
        };
        _this.openEditor = function (params) {
            var self = _this;
            var modal = _this.$uibModal.open({
                animation: false,
                templateUrl: "Portals/app/states/mainState/maintenanceState/billingRatesMaintenanceState/detail/views/mainView.html?" + APP_CACHE_VER,
                controller: "BillingRatesMaintenanceDetailController",
                controllerAs: "vm",
                backdrop: "static",
                windowClass: "br-edit-modal",
                resolve: {
                    modalParams: function () { return params; }
                }
            });
            modal.result.then(function (result) {
                self.onEditorClosed(result);
            }, function () { });
        };
        _this.onEditorClosed = function (result) {
            var self = _this;
            if (!result || !result.id) {
                return;
            }
            if (_this.viewMode === "effective") {
                if (result.action === "saved") {
                    _this.flashRow(result.id);
                }
                _this.effectiveGrid.loadGrid();
                return;
            }
            if (result.action === "deleted") {
                _this.removeRow(result.id);
                return;
            }
            _this.BillingRatesService.billingRatesGridRow(result.id)
                .then(function (row) {
                if (row) {
                    self.upsertRow(row);
                }
                else {
                    self.pageGrid.loadGrid();
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.findRowIndex = function (id) {
            var data = _this.gridModel.data || [];
            for (var i = 0; i < data.length; i++) {
                if (data[i].id === id) {
                    return i;
                }
            }
            return -1;
        };
        _this.removeRow = function (id) {
            var idx = _this.findRowIndex(id);
            if (idx < 0) {
                return;
            }
            _this.gridModel.data.splice(idx, 1);
            _this.gridModel.totalItems = Math.max(0, (_this.gridModel.totalItems || 0) - 1);
        };
        _this.upsertRow = function (row) {
            if (!_this.gridModel.data) {
                _this.gridModel.data = [];
            }
            var idx = _this.findRowIndex(row.id);
            if (idx >= 0) {
                _this.gridModel.data[idx] = row;
            }
            else {
                _this.gridModel.data.unshift(row);
                _this.gridModel.totalItems = (_this.gridModel.totalItems || 0) + 1;
            }
            _this.flashRow(row.id);
        };
        _this.flashRow = function (id) {
            var self = _this;
            if (self.highlightTimer) {
                self.$timeout.cancel(self.highlightTimer);
            }
            self.highlightId = id;
            self.highlightTimer = self.$timeout(function () {
                self.highlightId = null;
                self.highlightTimer = null;
            }, 2500);
        };
        _this.exportExcel = function () {
            var self = _this;
            if (self.exporting || !self.validateDates()) {
                return;
            }
            var effective = self.viewMode === "effective";
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
            }).then(function (response) {
                self.exporting = false;
                var disposition = response.headers("content-disposition") || "";
                var filename = "BillingRates.xlsx";
                var match = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/i.exec(disposition);
                if (match && match[1]) {
                    filename = match[1].replace(/['"]/g, "");
                }
                var blob = new Blob([response.data], {
                    type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });
                var url = window.URL.createObjectURL(blob);
                var link = document.createElement("a");
                link.href = url;
                link.download = filename;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            }, function (error) {
                self.exporting = false;
                self.handleError(error);
            });
        };
        var self = _this;
        _this.pageGrid = new TcrGridServiceModule.TcrGridService("userName", _this.BillingRatesService.billingRatesGrid, _this.onDataLoaded, function (model) {
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
        }, null, _this.$state);
        _this.effectiveGrid = new TcrGridServiceModule.TcrGridService("userName", _this.BillingRatesService.effectiveRatesGrid, _this.onEffectiveLoaded, function (model) {
            model.userAccountIds = self.filters.userAccountIds || [];
            model.clientIds = self.filters.clientIds || [];
            model.projectIds = self.filters.projectIds || [];
            model.activeOn = self.filters.activeOn;
        }, null, _this.$state);
        _this.refreshFilterOptions(false);
        _this.applyFilters();
        return _this;
    }
    //#endregion
    BillingRatesMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return BillingRatesMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("BillingRatesMaintenanceGridController", [
    "$scope",
    "$state",
    "$timeout",
    "$uibModal",
    "BillingRatesService",
    "Popups",
    BillingRatesMaintenanceGridController
]);
//# sourceMappingURL=~BillingRatesMaintenanceGridController.js.map