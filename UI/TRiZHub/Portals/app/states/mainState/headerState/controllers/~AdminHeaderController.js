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
var AdminHeaderController = /** @class */ (function (_super) {
    __extends(AdminHeaderController, _super);
    //#region Ctor
    function AdminHeaderController($timeout, $http, $stateParams, $scope, $state, $window, SecurityService, AccountService, MasterDataService, ReportService, ProjectService, UserService, BillingCycleService, ScorecardTemplateService, ScorecardService, EnumService, Popups) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$timeout = $timeout;
        _this.$http = $http;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$window = $window;
        _this.SecurityService = SecurityService;
        _this.AccountService = AccountService;
        _this.MasterDataService = MasterDataService;
        _this.ReportService = ReportService;
        _this.ProjectService = ProjectService;
        _this.UserService = UserService;
        _this.BillingCycleService = BillingCycleService;
        _this.ScorecardTemplateService = ScorecardTemplateService;
        _this.ScorecardService = ScorecardService;
        _this.EnumService = EnumService;
        _this.Popups = Popups;
        _this.successMessage = "Imported Successfully";
        _this.saveSuccess = false;
        _this.errorMessage = "";
        _this.error = false;
        _this.showImpExp = false;
        //#endregion
        _this.isAllowed = function (privilegeType) {
            var self = _this;
            if (_this.SecurityService.getCurrentUserDetails().loggedIn)
                return _this.SecurityService.isAllowed(privilegeType);
            else
                self.$state.go("root.login");
        };
        _this.logout = function () {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Logout", "Are you sure you want to logout?")
                .then(function (result) {
                if (result) {
                    self.AccountService.logout();
                    self.$state.go("root.adminLogin");
                }
            });
        };
        _this.timesheetSummary = function () {
            var self = _this;
            self.UserService.userDropdownList()
                .then(function (result) {
                self.filterOptions.users = result;
                var allUsersEntry = {
                    description: 'All Users',
                    id: -1
                };
                self.filterOptions.users.splice(0, 0, allUsersEntry);
            }, function (error) {
                self.handleError(error);
            });
            self.Popups.showReportDateBetweenTimesheetViewDialog(self.$scope, "Export Timsheet Summary", "Supply details to download Excel Report", "Portals/app/states/mainState/headerState/views/TimesheetSummaryReportPopup.html")
                .then(function (result) {
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
                        result.showRates, "_blank");
                }
                self.BillingCycleService.billingCycleDropdownList()
                    .then(function (results) {
                    self.filterOptions.billingCycles = results;
                    self.filterOptions.billingCycles.splice(0, 0, {
                        id: 0,
                        description: "Manual Date"
                    });
                    self.filterOptions.billingCycleId = 0;
                }, function (error) {
                    self.handleError(error);
                });
            });
        };
        _this.userSummary = function () {
            var self = _this;
            self.UserService.userDropdownList()
                .then(function (result) {
                self.filterOptions.users = result;
                var allUsersEntry = {
                    description: 'All Users',
                    id: -1
                };
                self.filterOptions.users.splice(0, 0, allUsersEntry);
                self.Popups.showUserReportDailog(self.$scope, "Export Users", "Supply details to download User Summary Excel Report", "Portals/app/states/mainState/headerState/views/UserSummaryReportPopup.html")
                    .then(function (result) {
                    if ((result !== undefined) && (result.userId !== undefined)) {
                        if (result.userId == -1) {
                            result.userId = "00000000-0000-0000-0000-000000000000";
                        }
                        ;
                        self.$window.open(self.ReportService.reportApi() +
                            "UserSummaryExcel?userAccountId=" +
                            result.userId +
                            "&showInactive=" +
                            result.showInactive, "_blank");
                    }
                    ;
                });
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.scoreCardReport = function () {
            var self = _this;
            self.Popups.showScorecardReportDailog(self.$scope, "Export Score Cards", "Supply details to download Score Card Report in Excel", "Portals/app/states/mainState/headerState/views/ScorecardSummaryReportPopup.html")
                .then(function (result) {
                self.$window.open(self.ReportService.reportApi() +
                    "ScorecardEmployeeSummaryExcel?scorecardTemplateId=" +
                    result.scorecardTemplate +
                    "&scorecardTemplatePeriodsIds=" +
                    result.period +
                    "&employeeId" +
                    result.employee, "_blank");
            });
        };
        _this.UserAssetRegisterReport = function () {
            var self = _this;
            self.$window.open(self.ReportService.reportApi() +
                "UserAssetRegisterSummaryExcel?" +
                "_blank");
        };
        _this.userAssetRegisterImport = function () {
            var self = _this;
            self.exceptionList = [];
            self.errorMessage = "";
            self.Popups.showUserAssetRegisterImportDialog(self.$scope, "Import User Asset Regiser CSV File", "Select a csv file to Import", "Portals/app/states/mainState/headerState/views/ImportUserAssetRegister.html")
                .then(function (result) {
                {
                }
                ;
            });
        };
        _this.updateFilter = function (popupModel) {
            var self = _this;
            popupModel.disableFilter = false;
            if (self.filterOptions.billingCycleId == 0)
                return;
            for (var j = 0; j < self.filterOptions.billingCycles.length; j++) {
                if (self.filterOptions.billingCycles[j].id === self.filterOptions.billingCycleId) {
                    popupModel.startDate = self.filterOptions.billingCycles[j].startdate;
                    popupModel.endDate = self.filterOptions.billingCycles[j].enddate;
                    popupModel.disableFilter = true;
                    break;
                }
            }
        };
        _this.changedScorecardTemplate = function () {
            var self = _this;
            self.ScorecardTemplateService.scorecardTemplatePeriodDropdownList(self.viewModel.scorecardTemplateId)
                .then(function (result) {
                self.filterOptions.scorecardTemplatePeriods = result;
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.toggelPeriod = function (period) {
            var self = _this;
            var index = 1;
            var found = false;
            for (var j = 0; j < self.viewModel.scorecardTemplatePeriodsIds.length; j++) {
                if (self.viewModel.scorecardTemplatePeriodsIds[j] === period.id) {
                    found = true;
                    break;
                }
                index++;
            }
            if (found) {
                self.viewModel.scorecardTemplatePeriodsIds.splice(index, 1);
            }
            else {
                self.viewModel.scorecardTemplatePeriodsIds.push(period.id);
            }
        };
        _this.uploadedFile = function (element) {
            var self = _this;
            self.exceptionList = [];
            self.errorMessage = "";
            self.$scope.$apply(function ($scope) {
                self.files = element.files;
            });
        };
        _this.addFile = function () {
            var self = _this;
            self.exceptionList = [];
            self.errorMessage = "";
            self.Popups.confirmationDialog(self.$scope, "Asset Register Import", "You are about to overwrite your complete asset register! Are you sure?")
                .then(function (result) {
                if (result) {
                    self.uploadfile(self.files, function (msg) {
                        console.log('uploaded');
                    }, function (msg) {
                        console.log('error');
                    });
                }
            });
        };
        var self = _this;
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
        if (_this.SecurityService.getCurrentUserDetails() == undefined) {
            _this.AccountService.init()
                .then(function (result) { }, function (e) { this.$state.go("root.login"); });
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
            .then(function (result) {
            self.profile = result;
        }, function (error) {
            self.handleError(error);
        });
        return _this;
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
    // Arguments :
    //  verb : 'GET'|'POST'
    //  target : an optional opening target (a name, or "_blank"), defaults to "_self"
    AdminHeaderController.prototype.open = function (verb, url, data, target) {
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
    AdminHeaderController.prototype.uploadfile = function (files, success, error) {
        var self = this;
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
                self.successMessage = "Import Successfull";
                self.saveSuccess = true;
                self.exceptionList = data;
                self.$timeout(function () {
                    self.saveSuccess = false;
                }, 3000);
            })
                .error(function (data) {
                self.errorMessage = data.message;
                self.error = true;
                self.$timeout(function () {
                    self.error = false;
                }, 3000);
            });
        }
    };
    return AdminHeaderController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("AdminHeaderController", [
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
//# sourceMappingURL=~AdminHeaderController.js.map