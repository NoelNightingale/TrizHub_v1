class ScorecardReportController extends CHControllerBase {

    filterOptions: any;
    employeeSelectionInvalid: any = false;
    clientSelectionInvalid: any = false;
    lineManagerSelectionInvalid: any = false;
    evaluatorSelectionInvalid: any = false;
    scorecardSelectionInvalid: any = false;

    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $window: ng.IWindowService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private UserService: UserServiceModule.UserService,
        private ClientService: ClientServiceModule.ClientService,
        private ReportService: ReportServiceModule.ReportService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;

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
            .then(
                result => {

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
                },
                error => {
                    self.handleError(error);
            });

        // Retrieve evaluators
        self.UserService.userScorecardEvaluatorsDropdown()
            .then(
                result => {

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
                },
                error => {
                    self.handleError(error);
            });

        // Retrieve line managers
        self.UserService.userScorecardLineManagersDropdown()
            .then(
                result => {

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
                },
                error => {
                    self.handleError(error);
                });

        // Retrieve clients
        self.ClientService.clientDropdownList()
            .then(
            result => {
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
                },
                error => {
                    self.handleError(error);
                });

        self.yearChange();
    }

    getReviewYears = () => {
        const self = this;

        self.ScorecardTemplateService.scorecardTemplatePeriodDropdownYear()
            .then(
                result => {
                    self.filterOptions.years = [];
                    for (var i = 0; i < result.length; i++) {
                        self.filterOptions.years.push({ id: i, displayVal: result[i] });
                    }
                },
                error => {
                    self.handleError(error);
                });
    };

    getScoreCards = () => {
        const self = this;

        // Retrieve scorecards
        self.ScorecardTemplateService.scorecardTemplateDropdownListAll()
            .then(
                result => {
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
                },
                error => {
                    self.handleError(error);
                });
    };

    reportTypeChange = (): void => {
        const self = this;

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

    selectedScorecardChange = (): void => {
        const self = this;

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
                    self.filterOptions.periods = result;
                },
                error => {
                    self.handleError(error);
                });
    };

    allYearChange = (): void => {
        const self = this;
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

    yearChange = (): void => {
        const self = this;

        self.getScorecardsPerYear(self.filterOptions.selectedYears);
    };

    getScorecardsPerYear = (years: any): void => {
        const self = this;
        // Get Unique scorecards for years selected
        self.ScorecardTemplateService.scorecardTemplateDropdownListYearMultiple(years)
            .then(
                result => {

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
                },
                error => {
                    self.handleError(error);
                });
    };

    formatDate = (date): string => {
        return date.getFullYear() + "/" + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);
    };

    updateActiveEmployees = (): void => {
        const self = this;
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
                }
                );
            }
        }
    }

    updateActiveClients = (): void => {
        const self = this;
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
    }

    updateActiveLineManagers = (): void => {
        const self = this;
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
                }
                );
            }
        }
    }

    updateActiveEvaluators = (): void => {
        const self = this;
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
                }
                );
            }
        }
    }

    validateSelections = (employees: any[], clients: any[], lineMnagers: any[], evaluators: any[]): boolean => {
        const self = this;
        var valid: boolean = true;

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

    submitForm = (): any => {
        const self = this;

        var employees = [];
        var clients = [];
        var lineManagers = [];
        var evaluators = [];
        var scorecards = [];

        // Populate employees
        if (!self.filterOptions.employeesUnfiltered) {
            for (var i = 0; i < self.filterOptions.employees.length; i++) {
                if (self.filterOptions.employees[i].selected) {
                    employees.push(self.filterOptions.employees[i].id)
                }
            }
        }

        // Populate clients
        if (!self.filterOptions.clientsUnfiltered) {
            for (var i = 0; i < self.filterOptions.clients.length; i++) {
                if (self.filterOptions.clients[i].selected) {
                    clients.push(self.filterOptions.clients[i].id)
                }
            }
        }

        // Populate lineManagers
        if (!self.filterOptions.lineManagersUnfiltered) {
            for (var i = 0; i < self.filterOptions.lineManagers.length; i++) {
                if (self.filterOptions.lineManagers[i].selected) {
                    lineManagers.push(self.filterOptions.lineManagers[i].id)
                }
            }
        }

        // Populate evaluators
        if (!self.filterOptions.evaluatorsUnfiltered) {
            for (var i = 0; i < self.filterOptions.evaluators.length; i++) {
                if (self.filterOptions.evaluators[i].selected) {
                    evaluators.push(self.filterOptions.evaluators[i].id)
                }
            }
        }

        // Populate scorecards
        if (self.filterOptions.reportType == "ScorecardStatusSummary") {
            if (!self.filterOptions.searchAllScoreCards) {
                for (var i = 0; i < self.filterOptions.selectedScoreCards.length; i++) {
                    scorecards.push(self.filterOptions.selectedScoreCards[i].id)
                }
            }
        }
        else {
            scorecards.push(self.filterOptions.selectedScoreCard.id)
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

    open(verb, url, data, target) {
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
}

angular.module("AngularApp")
    .controller("ScorecardReportController",
        [
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