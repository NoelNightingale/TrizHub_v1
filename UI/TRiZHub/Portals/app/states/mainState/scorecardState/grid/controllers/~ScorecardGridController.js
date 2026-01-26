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
var ScorecardGridController = /** @class */ (function (_super) {
    __extends(ScorecardGridController, _super);
    function ScorecardGridController($scope, $timeout, $state, ScorecardService, SecurityService, ScorecardTemplateService, UserService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$timeout = $timeout;
        _this.$state = $state;
        _this.ScorecardService = ScorecardService;
        _this.SecurityService = SecurityService;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.UserService = UserService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoadedGridScorecards = function (event) { _this.onLoadEventGridScorecards(event); };
        _this.onDataLoadedGridPersonalScorecards = function (event) { _this.onLoadEventGridPersonalScorecards(event); };
        _this.onDataLoadedGridTeamScorecards = function (event) { _this.onLoadEventGridTeamScorecards(event); };
        _this.onDataLoadedGridAdminScorecards = function (event) { _this.onLoadEventGridAdminScorecards(event); };
        _this.pscActive = "";
        _this.yscActive = "";
        _this.cscActive = "";
        _this.ascActive = "";
        _this.reassignUsers = [];
        _this.setupFilters = function () {
            var self = _this;
            self.filterOptions = {};
            self.filterOptions.allYears = [];
            self.filterOptions.years = [];
            self.filterOptions.periods = [];
            self.filterOptions.period = {};
            self.filterModel = {};
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
        _this.getPeriods = function (years, options, model, callback) {
            var self = _this;
            options.periods = [];
            options.period = {};
            self.ScorecardTemplateService.scorecardTemplatePeriodDropdownListYearMultiple(years)
                .then(function (result) {
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
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.loadGrids = function () {
            var self = _this;
            self.pageGridScorecards = new TcrGridServiceModule
                .TcrGridService("employeename", self.ScorecardService.scorecardGrid, self.onDataLoadedGridScorecards, function (model) {
                model.customSearchModel = self.filterModel;
            }, null, self.$state);
            self.pageGridScorecards.loadGrid();
            self.pageGridPersonalScorecards = new TcrGridServiceModule
                .TcrGridService("period", self.ScorecardService.myScorecardGrid, self.onDataLoadedGridPersonalScorecards, function (model) {
                model.customSearchModel = self.filterModelPersonal;
            }, null, self.$state);
            self.pageGridPersonalScorecards.loadGrid();
            self.pageGridTeamScorecards = new TcrGridServiceModule
                .TcrGridService("employeename", self.ScorecardService.teamScorecardGrid, self.onDataLoadedGridTeamScorecards, function (model) {
                model.customSearchModel = self.filterModelTeam;
            }, null, self.$state);
            self.pageGridTeamScorecards.loadGrid();
            if (self.isAdmin) {
                self.pageGridAdminScorecards = new TcrGridServiceModule
                    .TcrGridService("employeename", self.ScorecardService.adminScorecardGrid, self.onDataLoadedGridAdminScorecards, function (model) {
                    model.customSearchModel = self.filterModelAdmin;
                }, null, self.$state);
                self.pageGridAdminScorecards.loadGrid();
            }
        };
        _this.newScorecard = function () {
            _this.$state.transitionTo("mainState.scorecard.detail", { "id": "new" });
        };
        _this.isAllowed = function (privilegeType) {
            return _this.SecurityService.isAllowed(privilegeType);
        };
        _this.searchValueChange = function () {
            var self = _this;
            self.filterModel.employeeName = self.gridScorecards.searchFor;
            self.filterModel.scoreCardName = self.gridScorecards.searchFor;
            self.filterModel.evaluatorName = self.gridScorecards.searchFor;
        };
        _this.searchValuePersonalChange = function () {
            var self = _this;
            self.filterModelPersonal.employeeName = self.gridPersonalScorecards.searchFor;
            self.filterModelPersonal.scoreCardName = self.gridPersonalScorecards.searchFor;
            self.filterModelPersonal.evaluatorName = self.gridPersonalScorecards.searchFor;
        };
        _this.searchValueTeamChange = function () {
            var self = _this;
            self.filterModelTeam.employeeName = self.gridTeamScorecards.searchFor;
            self.filterModelTeam.scoreCardName = self.gridTeamScorecards.searchFor;
            self.filterModelTeam.evaluatorName = self.gridTeamScorecards.searchFor;
        };
        _this.searchValueAdminChange = function () {
            var self = _this;
            self.filterModelAdmin.employeeName = self.gridAdminScorecards.searchFor;
            self.filterModelAdmin.scoreCardName = self.gridAdminScorecards.searchFor;
            self.filterModelAdmin.evaluatorName = self.gridAdminScorecards.searchFor;
        };
        _this.lockedSubmittedChange = function () {
            var self = _this;
            self.pageGridScorecards.loadGrid();
        };
        _this.lockedSubmittedPersonalChange = function () {
            var self = _this;
            self.pageGridPersonalScorecards.loadGrid();
        };
        _this.lockedSubmittedTeamChange = function () {
            var self = _this;
            self.pageGridTeamScorecards.loadGrid();
        };
        _this.lockedSubmittedAdminChange = function () {
            var self = _this;
            self.pageGridAdminScorecards.loadGrid();
        };
        _this.periodChange = function (period) {
            var self = _this;
            self.filterModel.periodStart = period.startDate;
            self.filterModel.periodEnd = period.endDate;
            self.filterModel.periodId = period.id;
            self.filterModel.variablePeriod = period.isVariable;
            self.pageGridScorecards.loadGrid();
        };
        _this.periodChangePersonal = function (period) {
            var self = _this;
            self.filterModelPersonal.periodStart = period.startDate;
            self.filterModelPersonal.periodEnd = period.endDate;
            self.filterModelPersonal.periodId = period.id;
            self.filterModelPersonal.variablePeriod = period.isVariable;
            self.pageGridPersonalScorecards.loadGrid();
        };
        _this.periodChangeTeam = function (period) {
            var self = _this;
            self.filterModelTeam.periodStart = period.startDate;
            self.filterModelTeam.periodEnd = period.endDate;
            self.filterModelTeam.periodId = period.id;
            self.filterModelTeam.variablePeriod = period.isVariable;
            self.pageGridTeamScorecards.loadGrid();
        };
        _this.periodChangeAdmin = function (period) {
            var self = _this;
            self.filterModelAdmin.periodStart = period.startDate;
            self.filterModelAdmin.periodEnd = period.endDate;
            self.filterModelAdmin.periodId = period.id;
            self.filterModelAdmin.variablePeriod = period.isVariable;
            self.pageGridAdminScorecards.loadGrid();
        };
        _this.yearChange = function (year) {
            var self = _this;
            if (year == "All") {
                year = self.filterOptions.allYears;
            }
            else {
                year = [year];
            }
            self.getPeriods(year, self.filterOptions, self.filterModel, function () { self.pageGridScorecards.loadGrid(); });
        };
        _this.yearChangePersonal = function (year) {
            var self = _this;
            if (year == "All") {
                year = self.filterOptions.allYears;
            }
            else {
                year = [year];
            }
            self.getPeriods(year, self.filterOptionsPersonal, self.filterModelPersonal, function () { self.pageGridPersonalScorecards.loadGrid(); });
        };
        _this.yearChangeTeam = function (year) {
            var self = _this;
            if (year == "All") {
                year = self.filterOptions.allYears;
            }
            else {
                year = [year];
            }
            self.getPeriods(year, self.filterOptionsTeam, self.filterModelTeam, function () { self.pageGridTeamScorecards.loadGrid(); });
        };
        _this.yearChangeAdmin = function (year) {
            var self = _this;
            if (year == "All") {
                year = self.filterOptions.allYears;
            }
            else {
                year = [year];
            }
            self.getPeriods(year, self.filterOptionsAdmin, self.filterModelAdmin, function () { self.pageGridAdminScorecards.loadGrid(); });
        };
        _this.deleteScoreCard = function (record) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to delete?", "You are about to delete this scoreCard")
                .then(function (action) {
                if (action) {
                    self.ScorecardService.scoreCardDelete(record)
                        .then(function (result) {
                        self.pageGridScorecards.loadGrid();
                        if (self.isAdmin == true) {
                            self.pageGridAdminScorecards.loadGrid();
                        }
                    }, function (error) {
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.toggleScoreCardLock = function (record) {
            var self = _this;
            self.ScorecardService.scoreCardLock(record)
                .then(function (result) {
                self.pageGridTeamScorecards.loadGrid();
                self.pageGridScorecards.loadGrid();
                self.pageGridPersonalScorecards.loadGrid();
                if (self.isAdmin == true) {
                    self.pageGridAdminScorecards.loadGrid();
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.unsubmitScoreCard = function (record) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to Unsubmit?", "You are about to Unsubmit this scorecard")
                .then(function (action) {
                if (action) {
                    self.ScorecardService.scoreCardUnsubmit(record)
                        .then(function (result) {
                        self.pageGridTeamScorecards.loadGrid();
                        self.pageGridScorecards.loadGrid();
                        self.pageGridPersonalScorecards.loadGrid();
                        if (self.isAdmin == true) {
                            self.pageGridAdminScorecards.loadGrid();
                        }
                    }, function (error) {
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.submitScoreCard = function (record) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Are you sure you want to Submit?", "You are about to Submit this scorecard")
                .then(function (action) {
                if (action) {
                    self.ScorecardService.scoreCardSubmit(record)
                        .then(function (result) {
                        self.pageGridTeamScorecards.loadGrid();
                        self.pageGridScorecards.loadGrid();
                        self.pageGridPersonalScorecards.loadGrid();
                        if (self.isAdmin == true) {
                            self.pageGridAdminScorecards.loadGrid();
                        }
                    }, function (error) {
                        self.Popups.showError(self.$scope, error, "Error");
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.formatDate = function (date) {
            return date.getFullYear() + "/" + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);
        };
        _this.reassignScorecard = function (scorecard) {
            var self = _this;
            self.Popups.scorecardReassignDailog(self.$scope, "Reassign Scorecard", "Ok", "Cancel", scorecard, self.reassignUsers)
                .then(function (action) {
                if (action) {
                    var scoreCard = { scorecardId: action.scorecard.scorecardId, evaluatorId: action.evaluator.id };
                    self.ScorecardService.scoreCardReassign(scoreCard)
                        .then(function (result) {
                        self.pageGridAdminScorecards.search();
                    }, function (error) {
                        self.Popups.showError(self.$scope, error, "Error");
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        // Setup initial filter options
        self.setupFilters();
        // Do security checks
        if (_this.SecurityService.isAllowed("PerformanceManagementCreateScoreCards"))
            _this.cscActive = "active";
        else if (self.isAllowed("PerformanceManagementViewMyScoreCards"))
            self.pscActive = "active";
        else if (self.isAllowed("PerformanceManagementViewMyTeamScoreCards"))
            self.yscActive = "active";
        else if (self.isAllowed("PerformanceManagementAdmin"))
            self.ascActive = "active";
        // Retrieve scorecard template period years
        _this.ScorecardTemplateService.scorecardTemplatePeriodDropdownYear()
            .then(function (result) {
            for (var i = 0; i < result.length; i++) {
                self.filterOptions.allYears.push(result[i]);
            }
            result.unshift("All");
            self.filterOptions.years = result;
            self.filterOptionsPersonal.years = result;
            self.filterOptionsTeam.years = result;
            self.filterOptionsAdmin.years = result;
        }, function (error) {
            self.handleError(error);
        });
        self.isAdmin = self.isAllowed("PerformanceManagementAdmin");
        // Retrieve scorecard template periods
        self.getPeriods([self.filterModel.year], self.filterOptions, self.filterModel, function () { self.loadGrids(); });
        self.getPeriods([self.filterModelPersonal.year], self.filterOptionsPersonal, self.filterModelPersonal, function () { });
        self.getPeriods([self.filterModelTeam.year], self.filterOptionsTeam, self.filterModelTeam, function () { });
        if (self.isAdmin) {
            self.getPeriods([self.filterModelAdmin.year], self.filterOptionsAdmin, self.filterModelAdmin, function () { });
            // Get users for reassigning
            _this.UserService.userDropdownList()
                .then(function (result) {
                self.reassignUsers = result;
            }, function (error) {
                self.handleError(error);
            });
        }
        return _this;
    }
    //#endregion
    ScorecardGridController.prototype.onLoadEventGridScorecards = function (event) {
        this.gridScorecards = event;
        if (this.gridScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    ScorecardGridController.prototype.onLoadEventGridPersonalScorecards = function (event) {
        this.gridPersonalScorecards = event;
        if (this.gridPersonalScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    ScorecardGridController.prototype.onLoadEventGridTeamScorecards = function (event) {
        this.gridTeamScorecards = event;
        if (this.gridTeamScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    ScorecardGridController.prototype.onLoadEventGridAdminScorecards = function (event) {
        this.gridAdminScorecards = event;
        if (this.gridAdminScorecards.totalItems > 0) {
            this.loadingIsDone = true;
        }
    };
    return ScorecardGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardGridController", [
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
//# sourceMappingURL=~ScorecardGridController.js.map