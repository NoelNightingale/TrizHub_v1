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
var ScorecardReportController = /** @class */ (function (_super) {
    __extends(ScorecardReportController, _super);
    function ScorecardReportController($stateParams, $scope, $state, $window, ScorecardTemplateService, UserService, ClientService, ReportService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$window = $window;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.UserService = UserService;
        _this.ClientService = ClientService;
        _this.ReportService = ReportService;
        _this.Popups = Popups;
        _this.employeeSelectionInvalid = false;
        _this.clientSelectionInvalid = false;
        _this.lineManagerSelectionInvalid = false;
        _this.evaluatorSelectionInvalid = false;
        _this.scorecardSelectionInvalid = false;
        _this.getReviewYears = function () {
            var self = _this;
            self.ScorecardTemplateService.scorecardTemplatePeriodDropdownYear()
                .then(function (result) {
                self.filterOptions.years = [];
                for (var i = 0; i < result.length; i++) {
                    self.filterOptions.years.push({ id: i, displayVal: result[i] });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.getScoreCards = function () {
            var self = _this;
            // Retrieve scorecards
            self.ScorecardTemplateService.scorecardTemplateDropdownListAll()
                .then(function (result) {
                for (var i = 0; i < result.length; i++) {
                    self.filterOptions.allScorecards.push({
                        "id": result[i].id,
                        "description": result[i].entityName,
                        "scorecardCode": result[i].scorecardCode,
                        "scorecardName": result[i].scorecardName,
                        "isActive": result[i].active,
                        "selected": false
                    });
                    self.filterOptions.scorecards.push({
                        "id": result[i].id,
                        "description": result[i].entityName,
                        "scorecardCode": result[i].scorecardCode,
                        "scorecardName": result[i].scorecardName,
                        "isActive": result[i].active,
                        "selected": false
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.reportTypeChange = function () {
            var self = _this;
            // Reset selection validation
            self.employeeSelectionInvalid = false;
            self.clientSelectionInvalid = false;
            self.lineManagerSelectionInvalid = false;
            self.evaluatorSelectionInvalid = false;
            self.scorecardSelectionInvalid = false;
            self.yearChange();
            //if (self.filterOptions.reportType == "ScorecardStatusSummary") {
            //    self.getScoreCards();
            //    self.getPeriods(self.filterOptions.selectedYears);
            //}
            //else {
            //    self.yearChange();
            //}
        };
        _this.selectedScorecardChange = function () {
            var self = _this;
            var years = [];
            if (self.filterOptions.searchAllYears) {
                for (var i = 0; i < self.filterOptions.years.length; i++) {
                    years.push(self.filterOptions.years[i].displayVal);
                }
            }
            else {
                years = self.filterOptions.selectedYears;
            }
            var ids = [];
            if (self.filterOptions.reportType == "ScorecardStatusSummary") {
                if (!self.filterOptions.searchAllScoreCards && self.filterOptions.selectedScoreCards != undefined) {
                    for (var i = 0; i < self.filterOptions.selectedScoreCards.length; i++) {
                        ids.push(self.filterOptions.selectedScoreCards[i].id);
                    }
                }
            }
            else {
                ids = [self.filterOptions.selectedScoreCard.id];
            }
            // Get periods for new scorecard
            var params = {
                ScorecardTemplateItemIds: ids,
                ReviewYears: years
            };
            self.ScorecardTemplateService.scorecardTemplatePeriodSearchDropdownList(params)
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
                self.filterOptions.periods = result;
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.allYearChange = function () {
            var self = _this;
            if (self.filterOptions.searchAllYears) {
                var years = [];
                for (var i = 0; i < self.filterOptions.years.length; i++) {
                    years.push(self.filterOptions.years[i].displayVal);
                }
                self.getScorecardsPerYear(years);
            }
            else {
                self.getScorecardsPerYear(self.filterOptions.selectedYears);
            }
        };
        _this.yearChange = function () {
            var self = _this;
            self.getScorecardsPerYear(self.filterOptions.selectedYears);
        };
        _this.getScorecardsPerYear = function (years) {
            var self = _this;
            // Get Unique scorecards for years selected
            self.ScorecardTemplateService.scorecardTemplateDropdownListYearMultiple(years)
                .then(function (result) {
                self.filterOptions.allScorecards = [];
                self.filterOptions.scorecards = [];
                for (var i = 0; i < result.length; i++) {
                    self.filterOptions.allScorecards.push({
                        "id": result[i].id,
                        "description": result[i].entityName,
                        "scorecardCode": result[i].scorecardCode,
                        "scorecardName": result[i].scorecardName,
                        "isActive": result[i].active,
                        "selected": false
                    });
                    self.filterOptions.scorecards.push({
                        "id": result[i].id,
                        "description": result[i].entityName,
                        "scorecardCode": result[i].scorecardCode,
                        "scorecardName": result[i].scorecardName,
                        "isActive": result[i].active,
                        "selected": false
                    });
                }
                self.filterOptions.selectedScoreCard = self.filterOptions.scorecards[0];
                self.filterOptions.selectedScoreCards = [self.filterOptions.scorecards[0]];
                self.selectedScorecardChange();
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.formatDate = function (date) {
            return date.getFullYear() + "/" + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);
        };
        _this.updateActiveEmployees = function () {
            var self = _this;
            self.filterOptions.employees = [];
            for (var i = 0; i < self.filterOptions.allEmployees.length; i++) {
                if (self.filterOptions.allEmployees[i].accountName == "Yes" || self.filterOptions.showInactiveUsers) {
                    self.filterOptions.employees.push({
                        "id": self.filterOptions.allEmployees[i].id,
                        "description": self.filterOptions.allEmployees[i].description,
                        "firstname": self.filterOptions.allEmployees[i].firstname,
                        "surname": self.filterOptions.allEmployees[i].surname,
                        "accountName": self.filterOptions.allEmployees[i].accountName,
                        "selected": false
                    });
                }
            }
        };
        _this.updateActiveClients = function () {
            var self = _this;
            self.filterOptions.clients = [];
            for (var i = 0; i < self.filterOptions.allClients.length; i++) {
                if (self.filterOptions.allClients[i].isActive || self.filterOptions.showInactiveClients) {
                    self.filterOptions.clients.push({
                        "id": self.filterOptions.allClients[i].id,
                        "entityName": self.filterOptions.allClients[i].entityName,
                        "isActive": self.filterOptions.allClients[i].isActive,
                        "selected": false
                    });
                }
            }
        };
        _this.updateActiveLineManagers = function () {
            var self = _this;
            self.filterOptions.lineManagers = [];
            for (var i = 0; i < self.filterOptions.allLineManagers.length; i++) {
                if (self.filterOptions.allLineManagers[i].accountName == "True" || self.filterOptions.showInactiveLineManagers) {
                    self.filterOptions.lineManagers.push({
                        "id": self.filterOptions.allLineManagers[i].id,
                        "description": self.filterOptions.allLineManagers[i].description,
                        "firstname": self.filterOptions.allLineManagers[i].firstname,
                        "surname": self.filterOptions.allLineManagers[i].surname,
                        "accountName": self.filterOptions.allLineManagers[i].accountName,
                        "selected": false
                    });
                }
            }
        };
        _this.updateActiveEvaluators = function () {
            var self = _this;
            self.filterOptions.evaluators = [];
            for (var i = 0; i < self.filterOptions.allEvaluators.length; i++) {
                if (self.filterOptions.allEvaluators[i].accountName == "True" || self.filterOptions.showInactiveEvaluators) {
                    self.filterOptions.evaluators.push({
                        "id": self.filterOptions.allEvaluators[i].id,
                        "description": self.filterOptions.allEvaluators[i].description,
                        "firstname": self.filterOptions.allEvaluators[i].firstname,
                        "surname": self.filterOptions.allEvaluators[i].surname,
                        "accountName": self.filterOptions.allEvaluators[i].accountName,
                        "selected": false
                    });
                }
            }
        };
        _this.validateSelections = function (employees, clients, lineMnagers, evaluators) {
            var self = _this;
            var valid = true;
            self.employeeSelectionInvalid = false;
            self.clientSelectionInvalid = false;
            self.lineManagerSelectionInvalid = false;
            self.evaluatorSelectionInvalid = false;
            //self.scorecardSelectionInvalid = false;
            if (!self.filterOptions.employeesUnfiltered && employees.length < 1) {
                valid = false;
                self.employeeSelectionInvalid = true;
            }
            if (!self.filterOptions.clientsUnfiltered && clients.length < 1) {
                valid = false;
                self.clientSelectionInvalid = true;
            }
            if (!self.filterOptions.lineManagersUnfiltered && lineMnagers.length < 1) {
                valid = false;
                self.lineManagerSelectionInvalid = true;
            }
            if (!self.filterOptions.evaluatorsUnfiltered && evaluators.length < 1) {
                valid = false;
                self.evaluatorSelectionInvalid = true;
            }
            //if (self.filterOptions.reportType == 'ScorecardStatusSummary' && !self.filterOptions.scorecardsUnfiltered && scorecards.length < 1) {
            //    valid = false;
            //    self.scorecardSelectionInvalid = true;
            //}
            return valid;
        };
        _this.submitForm = function () {
            var self = _this;
            var employees = [];
            var clients = [];
            var lineManagers = [];
            var evaluators = [];
            var scorecards = [];
            // Populate employees
            if (!self.filterOptions.employeesUnfiltered) {
                for (var i = 0; i < self.filterOptions.employees.length; i++) {
                    if (self.filterOptions.employees[i].selected) {
                        employees.push(self.filterOptions.employees[i].id);
                    }
                }
            }
            // Populate clients
            if (!self.filterOptions.clientsUnfiltered) {
                for (var i = 0; i < self.filterOptions.clients.length; i++) {
                    if (self.filterOptions.clients[i].selected) {
                        clients.push(self.filterOptions.clients[i].id);
                    }
                }
            }
            // Populate lineManagers
            if (!self.filterOptions.lineManagersUnfiltered) {
                for (var i = 0; i < self.filterOptions.lineManagers.length; i++) {
                    if (self.filterOptions.lineManagers[i].selected) {
                        lineManagers.push(self.filterOptions.lineManagers[i].id);
                    }
                }
            }
            // Populate evaluators
            if (!self.filterOptions.evaluatorsUnfiltered) {
                for (var i = 0; i < self.filterOptions.evaluators.length; i++) {
                    if (self.filterOptions.evaluators[i].selected) {
                        evaluators.push(self.filterOptions.evaluators[i].id);
                    }
                }
            }
            // Populate scorecards
            if (self.filterOptions.reportType == "ScorecardStatusSummary") {
                if (!self.filterOptions.searchAllScoreCards) {
                    for (var i = 0; i < self.filterOptions.selectedScoreCards.length; i++) {
                        scorecards.push(self.filterOptions.selectedScoreCards[i].id);
                    }
                }
            }
            else {
                scorecards.push(self.filterOptions.selectedScoreCard.id);
            }
            // Validate selections
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.validateSelections(employees, clients, lineManagers, evaluators) == false || self.$scope["ReportForm"].$invalid) {
                return;
            }
            var params = {
                "searchAllYears": self.filterOptions.searchAllYears,
                "reviewYearsString": self.filterOptions.selectedYears,
                "searchAllPeriods": self.filterOptions.searchAllPeriods,
                "reviewPeriodIds": self.filterOptions.selectedPeriods,
                "detailLevel": self.filterOptions.detailLevel,
                "submitted": self.filterOptions.submitted,
                "locked": self.filterOptions.locked,
                "employeeHasScorecard": self.filterOptions.employeeHasScorecard,
                "employeeIds": employees,
                "clientIds": clients,
                "lineManagerIds": lineManagers,
                "evaluatorIds": evaluators,
                "scorecardIds": scorecards
            };
            self.open("POST", self.ReportService.reportApi() + self.filterOptions.reportType, params, "_blank");
        };
        var self = _this;
        self.filterOptions = {
            reportType: "ScorecardStatusSummary",
            selectedScoreCard: {},
            selectedScoreCards: [],
            years: [],
            selectedYears: [new Date().getFullYear()],
            searchAllYears: false,
            searchAllScoreCards: true,
            periods: [],
            searchAllPeriods: false,
            selectedPeriods: [],
            detailLevel: 0,
            submitted: 0,
            submittedOptions: [{ displayVal: "Yes", val: 0 }, { displayVal: "No", val: 1 }, { displayVal: "All", val: 2 }],
            detailLevels: [{ displayVal: "Final Combined", val: 0 }, { displayVal: "Detailed", val: 1 }, { displayVal: "Final Combined And Detailed", val: 2 }],
            locked: 0,
            employeeHasScorecard: 1,
            allEmployees: [],
            employees: [],
            allLineManagers: [],
            lineManagers: [],
            allEvaluators: [],
            evaluators: [],
            allClients: [],
            clients: [],
            allScorecards: [],
            scorecards: [],
            showInactiveUsers: false,
            showInactiveClients: false,
            showInactiveLineManagers: false,
            showInactiveEvaluators: false,
            showInactiveScorecards: false,
            employeesUnfiltered: true,
            clientsUnfiltered: true,
            lineManagersUnfiltered: true,
            evaluatorsUnfiltered: true,
            scorecardsUnfiltered: true
        };
        // Retrieve scorecard template period years
        self.getReviewYears();
        // Retrieve employees
        self.UserService.allUserDropdownList()
            .then(function (result) {
            for (var i = 0; i < result.length; i++) {
                self.filterOptions.allEmployees.push({
                    "id": result[i].id,
                    "description": result[i].description,
                    "firstname": result[i].firstname,
                    "surname": result[i].surname,
                    "accountName": result[i].accountName,
                    "selected": false
                });
                if (result[i].accountName == "Yes") {
                    self.filterOptions.employees.push({
                        "id": result[i].id,
                        "description": result[i].description,
                        "firstname": result[i].firstname,
                        "surname": result[i].surname,
                        "accountName": result[i].accountName,
                        "selected": false
                    });
                }
            }
        }, function (error) {
            self.handleError(error);
        });
        // Retrieve evaluators
        self.UserService.userScorecardEvaluatorsDropdown()
            .then(function (result) {
            for (var i = 0; i < result.length; i++) {
                self.filterOptions.allEvaluators.push({
                    "id": result[i].id,
                    "description": result[i].description,
                    "firstname": result[i].firstname,
                    "surname": result[i].surname,
                    "accountName": result[i].accountName,
                    "selected": false
                });
                if (result[i].accountName == "True") {
                    self.filterOptions.evaluators.push({
                        "id": result[i].id,
                        "description": result[i].description,
                        "firstname": result[i].firstname,
                        "surname": result[i].surname,
                        "accountName": result[i].accountName,
                        "selected": false
                    });
                }
            }
        }, function (error) {
            self.handleError(error);
        });
        // Retrieve line managers
        self.UserService.userScorecardLineManagersDropdown()
            .then(function (result) {
            for (var i = 0; i < result.length; i++) {
                self.filterOptions.allLineManagers.push({
                    "id": result[i].id,
                    "description": result[i].description,
                    "firstname": result[i].firstname,
                    "surname": result[i].surname,
                    "accountName": result[i].accountName,
                    "selected": false
                });
                if (result[i].accountName == "True") {
                    self.filterOptions.lineManagers.push({
                        "id": result[i].id,
                        "description": result[i].description,
                        "firstname": result[i].firstname,
                        "surname": result[i].surname,
                        "accountName": result[i].accountName,
                        "selected": false
                    });
                }
            }
        }, function (error) {
            self.handleError(error);
        });
        // Retrieve clients
        self.ClientService.clientDropdownList()
            .then(function (result) {
            for (var i = 0; i < result.length; i++) {
                self.filterOptions.allClients.push({
                    "id": result[i].id,
                    "entityName": result[i].entityName,
                    "isActive": result[i].isActive,
                    "selected": false
                });
                if (result[i].isActive) {
                    self.filterOptions.clients.push({
                        "id": result[i].id,
                        "entityName": result[i].entityName,
                        "isActive": result[i].isActive,
                        "selected": false
                    });
                }
            }
        }, function (error) {
            self.handleError(error);
        });
        self.yearChange();
        return _this;
    }
    ScorecardReportController.prototype.open = function (verb, url, data, target) {
        var form = document.createElement("form");
        form.action = url;
        form.method = verb;
        form.target = target || "_self";
        if (data) {
            for (var key in data) {
                var input = document.createElement("textarea");
                input.name = key;
                input.value = typeof data[key] === "object" ? JSON.stringify(data[key]) : data[key];
                form.appendChild(input);
            }
        }
        form.style.display = "none";
        document.body.appendChild(form);
        form.submit();
    };
    ;
    return ScorecardReportController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ScorecardReportController", [
    "$stateParams",
    "$scope",
    "$state",
    "$window",
    "ScorecardTemplateService",
    "UserService",
    "ClientService",
    "ReportService",
    "Popups",
    ScorecardReportController
]);
//# sourceMappingURL=~ScorecardReportController.js.map