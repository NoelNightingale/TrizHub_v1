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
    function BillingRatesMaintenanceGridController($scope, $state, $timeout, BillingRatesService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
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
        /** True when filters changed since the last successful Apply / grid load. */
        _this.filtersDirty = false;
        _this.filters = {
            userAccountIds: [],
            clientIds: [],
            projectIds: [],
            scope: "",
            activeOn: new Date(),
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
        _this.markFiltersDirty = function () {
            _this.filtersDirty = true;
        };
        _this.onActiveOnChanged = function () {
            _this.markFiltersDirty();
        };
        _this.clearEffectiveDate = function () {
            _this.filters.activeOn = null;
            _this.markFiltersDirty();
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
            _this.filtersDirty = false;
            if (_this.pageGrid && _this.pageGrid.gridModel) {
                _this.pageGrid.gridModel.currentPage = 1;
            }
            _this.pageGrid.loadGrid();
        };
        _this.clearAllFilters = function () {
            _this.filters.userAccountIds = [];
            _this.filters.clientIds = [];
            _this.filters.projectIds = [];
            _this.filters.scope = "";
            _this.filters.activeOn = new Date();
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
            _this.$state.transitionTo("mainState.maintenance.billingRatesMaintenance.detail", params);
        };
        _this.editRecord = function (rateId) {
            if (!rateId) {
                return;
            }
            _this.$state.transitionTo("mainState.maintenance.billingRatesMaintenance.detail", { id: rateId });
        };
        _this.exportExcel = function () {
            var self = _this;
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
                resultMode: "periods",
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
            model.activeOn = self.filters.activeOn || null;
            model.userStatus = self.filters.userStatus || "active";
            model.clientStatus = self.filters.clientStatus || "active";
            model.projectStatus = self.filters.projectStatus || "active";
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
    "BillingRatesService",
    "Popups",
    BillingRatesMaintenanceGridController
]);
//# sourceMappingURL=~BillingRatesMaintenanceGridController.js.map