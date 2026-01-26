class AdminHeaderController extends CHControllerBase {

    successMessage = "Imported Successfully";
    saveSuccess = false;
    errorMessage = "";
    error = false;
    showImpExp = false;

    exceptionList: any;
    currentUser: any;
    profile: any;
    filterOptions: any;
    scorecardPeriodId: string;
    clientDropdown: any;
    userDropdown: any;
    viewModel: any;
    csvFile: any;
    apiUrl: string;
    files: any;


    //#region Ctor
    constructor(

        private $timeout: ng.ITimeoutService,
        private $http: ng.IHttpService,
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $window: ng.IWindowService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private AccountService: AccountServiceModule.AccountService,
        private MasterDataService: MasterDataServiceModule.MasterDataService,
        private ReportService: ReportServiceModule.ReportService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private UserService: UserServiceModule.UserService,
        private BillingCycleService: BillingCycleServiceModule.BillingCycleService,
        private ScorecardTemplateService: ScorecardTemplateServiceModule.ScorecardTemplateService,
        private ScorecardService: ScorecardServiceModule.ScorecardService,
        private EnumService: EnumServiceModule.EnumService,
        private Popups: any) {
        super($scope, Popups, $state);
        const self = this;
        self.scorecardPeriodId = self.$stateParams["id"];
        self.viewModel = {
            isActive: true,
            scorecardTemplatePeriodsIds: []
        };
        self.filterOptions = {
            projects: [],
            users: [],
            billingCycles: [],
            evaluators: [],
            employees: [],
        };

        self.apiUrl = location.protocol + '//' + location.host + '//' + 'api/User/UploadImportOfficeEquiment';
        self.files = File;
        if (this.SecurityService.getCurrentUserDetails() == undefined) {
            this.AccountService.init()
                .then(
                    function (result) { },
                    function (e) { this.$state.go("root.login"); });
        }

        //ProjectService.projectAndSubProjectDropdownList()
        //    .then(
        //    result => {
        //        self.filterOptions.projects = result;
        //    },
        //    error => {
        //        self.handleError(error);
        //    });
        //UserService.userDropdownList()
        //    .then(
        //    result => {
        //        self.filterOptions.users = result;
        //        var allUsersEntry = {
        //            description: 'All Users',
        //            id: -1
        //        };
        //        self.filterOptions.users.splice(0, 0, allUsersEntry);
        //    },
        //    error => {
        //        self.handleError(error);
        //    });
        //BillingCycleService.billingCycleDropdownList()
        //    .then(
        //    result => {
        //        self.filterOptions.billingCycles = result;
        //        self.filterOptions.billingCycles.splice(0, 0, {
        //            id: 0,
        //            description: "Manual Date"
        //        });
        //        self.filterOptions.billingCycleId = 0;
        //    },
        //    error => {
        //        self.handleError(error);
        //    });
        self.currentUser = self.SecurityService.getCurrentUserDetails();
        MasterDataService.profileGet()
            .then(
                result => {
                    self.profile = result;
                },
                error => {
                    self.handleError(error);
                });
        //UserService.userScorecardEmployeeFilterDropdown()
        //    .then(
        //    result => {
        //        self.filterOptions.employees = result;
        //    },
        //    error => {
        //        self.handleError(error);
        //    });
        //ScorecardTemplateService.scorecardTemplateDropdownList()
        //    .then(
        //    result => {
        //        self.filterOptions.scorecardTemplates = result;
        //    },
        //    error => {
        //        self.handleError(error);
        //    });
    }

    //#endregion

    isAllowed = (privilegeType: string): boolean => {
        const self = this;
        if (this.SecurityService.getCurrentUserDetails().loggedIn)
            return this.SecurityService.isAllowed(privilegeType);
        else
            self.$state.go("root.login");
    };

    logout = (): void => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope, "Logout", "Are you sure you want to logout?")
            .then(
                function (result) {
                    if (result) {
                        self.AccountService.logout();
                        self.$state.go("root.adminLogin");
                    }
                });
    };

    timesheetSummary = (): void => {
        const self = this;

        self.UserService.userDropdownList()
            .then(
                result => {
                    self.filterOptions.users = result;
                    var allUsersEntry = {
                        description: 'All Users',
                        id: -1
                    };
                    self.filterOptions.users.splice(0, 0, allUsersEntry);
                },
                error => {
                    self.handleError(error);
                });

        self.Popups.showReportDateBetweenTimesheetViewDialog(self.$scope,
            "Export Timsheet Summary",
            "Supply details to download Excel Report",
            "Portals/app/states/mainState/headerState/views/TimesheetSummaryReportPopup.html")
            .then(
                function (result) {
                    if (result.result) {

                        var startDate = null;
                        var endDate = null;
                        try {
                            startDate = result.startDate.toJSON();
                        }
                        catch (e) {
                            startDate = result.startDate;
                        }
                        try {
                            endDate = result.endDate.toJSON();
                        }
                        catch (e) {
                            endDate = result.endDate;
                        }

                        if (!startDate || !endDate) {
                            self.handleError("Both Start and End Dates should be supplied...");
                            return;
                        }

                        self.$window.open(self.ReportService.reportApi() +
                            "TimesheetSummaryExcel?startDate=" +
                            startDate +
                            "&endDate=" +
                            endDate +
                            "&projectId=" +
                            result.projectId +
                            "&userAccountId=" +
                            result.userId +
                            "&showBillingPeriod=" +
                            result.showBillingPeriod +
                            "&showRates=" +
                            result.showRates,
                            "_blank");
                    }
                    self.BillingCycleService.billingCycleDropdownList()
                        .then(
                            results => {
                                self.filterOptions.billingCycles = results;
                                self.filterOptions.billingCycles.splice(0, 0, {
                                    id: 0,
                                    description: "Manual Date"
                                });
                                self.filterOptions.billingCycleId = 0;
                            },
                            error => {
                                self.handleError(error);
                            });
                });
    };

    userSummary = (): void => {
        const self = this;

        self.UserService.userDropdownList()
            .then(
                result => {
                    self.filterOptions.users = result;
                    var allUsersEntry = {
                        description: 'All Users',
                        id: -1
                    };
                    self.filterOptions.users.splice(0, 0, allUsersEntry);

                    self.Popups.showUserReportDailog(self.$scope,
                        "Export Users",
                        "Supply details to download User Summary Excel Report",
                        "Portals/app/states/mainState/headerState/views/UserSummaryReportPopup.html")
                        .then(
                            function (result) {
                                if ((result !== undefined) && (result.userId !== undefined)) {
                                    if (result.userId == -1) {
                                        result.userId = "00000000-0000-0000-0000-000000000000";
                                    };
                                    self.$window.open(self.ReportService.reportApi() +
                                        "UserSummaryExcel?userAccountId=" +
                                        result.userId +
                                        "&showInactive=" +
                                        result.showInactive,
                                        "_blank");
                                };
                            });
                },
                error => {
                    self.handleError(error);
                });
    };


    scoreCardReport = (): void => {
        const self = this;
        self.Popups.showScorecardReportDailog(self.$scope,
            "Export Score Cards",
            "Supply details to download Score Card Report in Excel",
            "Portals/app/states/mainState/headerState/views/ScorecardSummaryReportPopup.html")
            .then(
                function (result) {
                    self.$window.open(self.ReportService.reportApi() +
                        "ScorecardEmployeeSummaryExcel?scorecardTemplateId=" +
                        result.scorecardTemplate +
                        "&scorecardTemplatePeriodsIds=" +
                        result.period +
                        "&employeeId" +
                        result.employee,
                        "_blank");
                });
    };

    UserAssetRegisterReport = (): void => {
        const self = this;

        self.$window.open(self.ReportService.reportApi() +
            "UserAssetRegisterSummaryExcel?" +
            "_blank");
    };

    userAssetRegisterImport = (): void => {
        const self = this;
        self.exceptionList = [];
        self.errorMessage = "";
        self.Popups.showUserAssetRegisterImportDialog(self.$scope,
            "Import User Asset Regiser CSV File",
            "Select a csv file to Import",
            "Portals/app/states/mainState/headerState/views/ImportUserAssetRegister.html")
            .then(
                function (result) {
                    {
                    };
                });
    };

    updateFilter = (popupModel): void => {
        const self = this;
        popupModel.disableFilter = false;
        if (self.filterOptions.billingCycleId == 0) return;
        for (let j = 0; j < self.filterOptions.billingCycles.length; j++) {
            if (self.filterOptions.billingCycles[j].id === self.filterOptions.billingCycleId) {
                popupModel.startDate = self.filterOptions.billingCycles[j].startdate;
                popupModel.endDate = self.filterOptions.billingCycles[j].enddate;
                popupModel.disableFilter = true;
                break;
            }
        }
    };

    changedScorecardTemplate = (): void => {
        const self = this;
        self.ScorecardTemplateService.scorecardTemplatePeriodDropdownList(self.viewModel.scorecardTemplateId)
            .then(
                result => {
                    self.filterOptions.scorecardTemplatePeriods = result;
                },
                error => {
                    self.handleError(error);
                });
    };

    toggelPeriod = (period): void => {
        const self = this;
        let index = 1;
        let found = false;
        for (let j = 0; j < self.viewModel.scorecardTemplatePeriodsIds.length; j++) {
            if (self.viewModel.scorecardTemplatePeriodsIds[j] === period.id) {
                found = true;
                break;
            }
            index++;
        }
        if (found) {
            self.viewModel.scorecardTemplatePeriodsIds.splice(index, 1);
        } else {
            self.viewModel.scorecardTemplatePeriodsIds.push(period.id);
        }
    };

    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
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
    }


    uploadedFile = (element): void => {
        const self = this;
        self.exceptionList = [];
        self.errorMessage = "";
        self.$scope.$apply(function ($scope) {
            self.files = element.files;
        });

    }


    addFile = (): void => {
        const self = this;
        self.exceptionList = [];
        self.errorMessage = "";
        self.Popups.confirmationDialog(self.$scope, "Asset Register Import", "You are about to overwrite your complete asset register! Are you sure?")
            .then(
                function (result) {
                    if (result) {
                        self.uploadfile(self.files,
                            function (msg) // success
                            {
                                console.log('uploaded');
                            },
                            function (msg) // error
                            {
                                console.log('error');
                            });
                    }
                });

    }


    uploadfile(files, success, error): void {
        const self = this;

        var url = self.apiUrl;

        for (var i = 0; i < files.length; i++) {
            var fd = new FormData();

            fd.append("file", files[i]);

            self.$http.post(url, fd, {

                withCredentials: false,

                headers: {
                    'Content-Type': undefined
                },
                transformRequest: angular.identity
            })
                .success(function (data) {
                    self.successMessage = "Import Successfull"
                    self.saveSuccess = true;
                    self.exceptionList = data;
                    self.$timeout(function () {
                        self.saveSuccess = false;
                    },
                        3000);

                })
                .error(function (data) {
                    self.errorMessage = data.message;
                    self.error = true;
                    self.$timeout(function () {
                        self.error = false;
                    },
                        3000);
                });
        }
    }
}

angular.module("AngularApp")
    .controller("AdminHeaderController",
        [
            "$timeout",
            "$http",
            "$stateParams",
            "$scope",
            "$state",
            "$window",
            "SecurityService",
            "AccountService",
            "MasterDataService",
            "ReportService",
            "ProjectService",
            "UserService",
            "BillingCycleService",
            "ScorecardTemplateService",
            "ScorecardService",
            "EnumService",
            "Popups",
            AdminHeaderController
        ]);