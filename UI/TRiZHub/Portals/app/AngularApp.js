angular.module("AngularApp",
    [
        "ngRoute",
        "ui.bootstrap",
        "ui.router",
        "ui.bootstrap.dropdown",
        "ui.bootstrap.showErrors",
        "ngAnimate",
        "ngStorage",
        "ngSanitize",
        "validation.match",
        "angular-loading-bar",
        "oc.lazyLoad",
        "toggle-switch",
        "config",
        "mgcrea.ngStrap",
        "mgcrea.ngStrap.helpers.dateParser",
        "vr.directives.slider",
        "ui.select",
        "summernote",
        //"moment"
        //"ui.tinymce"
    ])
    .config(
        [
            "$stateProvider", "$httpProvider", "$locationProvider",
            function ($stateProvider, $httpProvider, $locationProvider) {
                $stateProvider
                    //
                    // Root
                    //
                    .state("root",
                        {
                            url: "",
                            abstract: true,
                            views: {
                                'main': {
                                    templateUrl: "Portals/app/states/rootState/stateView.html"
                                }
                            }
                            // Remove for reason that it causes blank screen onsite - was ment to solve refresh redirect
                            //,
                            //resolve: {
                            //    AccountServiceInit: ['AccountService',
                            //        function (AccountService) {
                            //            return AccountService.init();

                            //        }
                            //    ]
                            //}
                        })
                    /// Login
                    .state("root.login",
                        {
                            url: "/login",
                            views: {
                                'login': {
                                    templateUrl: "Portals/app/states/rootState/login/views/mainView.html",
                                    controller: "LoginController as vm"
                                }
                            },
                            resolve: {
                                loadCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/rootState/login/controllers/~LoginController.js");
                                    }
                                ]
                            }
                        })
                    .state("mainState",
                        {
                            url: "/main",
                            abstract: true,
                            views: {
                                'header': {
                                    templateUrl: "Portals/app/states/mainState/headerState/views/mainView.html",
                                    controller: "AdminHeaderController as vm"
                                },
                                'footer': {
                                    templateUrl: "Portals/app/states/mainState/footerState/views/mainView.html",
                                    controller: "AdminFooterController as vm"
                                },
                                'main': {
                                    templateUrl: "Portals/app/states/mainState/stateView.html"
                                }
                            },
                            resolve: {
                                loadHeader: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/headerState/controllers/~AdminHeaderController.js");
                                    }
                                ],
                                loadFooter: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/footerState/controllers/~AdminFooterController.js");
                                    }
                                ]
                            }
                        })
                    /// Admin Home
                    .state("mainState.home",
                        {
                            url: "/home",
                            views: {
                                'home': {
                                    templateUrl: "Portals/app/states/mainState/homeState/views/mainView.html",
                                    controller: "HomeController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/homeState/controllers/~HomeController.js");
                                    }
                                ]
                            }
                        })
                    /// Profile
                    .state("mainState.profile",
                        {
                            url: "/profile",
                            views: {
                                'profile': {
                                    templateUrl: "Portals/app/states/mainState/profileState/views/mainView.html",
                                    controller: "ProfileController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/profileState/controllers/~ProfileController.js");
                                    }
                                ]
                            }
                        })
                    /// Timesheet
                    .state("mainState.timesheet",
                        {
                            url: "/timesheet",
                            views: {
                                'timesheet': {
                                    templateUrl: "Portals/app/states/mainState/timesheetState/views/mainView.html",
                                    controller: "TimesheetController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/timesheetState/controllers/~TimesheetController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Maintenance Report
                    .state("mainState.timesheetreport",
                        {
                            url: "/report",
                            views: {
                                'timesheet': {
                                    templateUrl: "Portals/app/states/mainState/reportState/timereports/timesheetSummary.html",
                                    controller: "TimeSheetReportController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/reportState/controllers/~TimeSheetReportController.js");
                                    }
                                ]
                            }
                        })

                    // Scorecard Maintenance Report
                    .state("mainState.customertimesheetreport",
                        {
                            url: "/customertimereport",
                            views: {
                                'timesheet': {
                                    templateUrl: "Portals/app/states/mainState/reportState/timereports/clientTimesheetSummary.html",
                                    controller: "ClientTimeSheetReportController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/reportState/controllers/~ClientTimeSheetReportController.js");
                                    }
                                ]
                            }
                        })
                    // User project summary
                    .state("mainState.userprojects",
                        {
                            url: "/userprojects",
                            views: {
                                'timesheet': {
                                    templateUrl: "Portals/app/states/mainState/reportState/timereports/userProjectSummary.html",
                                    controller: "UserProjectSummaryController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/reportState/controllers/~UserProjectSummaryController.js");
                                    }
                                ]
                            }
                        })
                    // User project summary
                    .state("mainState.userroles",
                        {
                            url: "/userroles",
                            views: {
                                'timesheet': {
                                    templateUrl: "Portals/app/states/mainState/reportState/timereports/userRoleSummary.html",
                                    controller: "UserRoleSummaryController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/reportState/controllers/~UserRoleSummaryController.js");
                                    }
                                ]
                            }
                        })
                    /// Maintenance
                    .state("mainState.maintenance",
                        {
                            url: "/maintenance",
                            abstract: true,
                            views: {
                                'maintenance': {
                                    templateUrl: "Portals/app/states/mainState/maintenanceState/stateView.html"
                                }
                            }
                        })
                    // Activity Maintenance
                    .state("mainState.maintenance.activityMaintenance",
                        {
                            url: "/activity",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/activityMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // Activity Maintenance Grid
                    .state("mainState.maintenance.activityMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/activityMaintenanceState/grid/views/mainView.html",
                                    controller: "ActivityMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/activityMaintenanceState/grid/controllers/~ActivityMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Activity Maintenance Detail
                    .state("mainState.maintenance.activityMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/activityMaintenanceState/detail/views/mainView.html",
                                    controller: "ActivityMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/activityMaintenanceState/detail/controllers/~ActivityMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })

                    // Team Maintenance
                    .state("mainState.maintenance.teamMaintenance",
                        {
                            url: "/team",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/teamMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // Team Maintenance Grid
                    .state("mainState.maintenance.teamMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/teamMaintenanceState/grid/views/mainView.html",
                                    controller: "TeamMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/teamMaintenanceState/grid/controllers/~TeamMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Team Maintenance Detail
                    .state("mainState.maintenance.teamMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/teamMaintenanceState/detail/views/mainView.html",
                                    controller: "TeamMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/teamMaintenanceState/detail/controllers/~TeamMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Role Maintenance
                    .state("mainState.maintenance.roleMaintenance",
                        {
                            url: "/roles",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/roleMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // Role Maintenance Grid
                    .state("mainState.maintenance.roleMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/roleMaintenanceState/grid/views/mainView.html",
                                    controller: "RoleMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/roleMaintenanceState/grid/controllers/~RoleMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Role Maintenance Detail
                    .state("mainState.maintenance.roleMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/roleMaintenanceState/detail/views/mainView.html",
                                    controller: "RoleMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/roleMaintenanceState/detail/controllers/~RoleMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Employer Maintenance
                    .state("mainState.maintenance.employerMaintenance",
                        {
                            url: "/employers",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/employerMaintenance/stateView.html"
                                }
                            }
                        })
                    .state("mainState.maintenance.employerMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/employerMaintenance/grid/views/mainView.html",
                                    controller: "EmployerMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/employerMaintenance/grid/controllers/~EmployerMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Employer Maintenance Detail
                    .state("mainState.maintenance.employerMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/employerMaintenance/detail/views/mainView.html",
                                    controller: "EmployerMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/employerMaintenance/detail/controllers/~EmployerMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })

                    // User Maintenance
                    .state("mainState.maintenance.userMaintenance",
                        {
                            url: "/users",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // User Maintenance Grid
                    .state("mainState.maintenance.userMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/grid/views/mainView.html",
                                    controller: "UserMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/grid/controllers/~UserMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Detail
                    .state("mainState.maintenance.userMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/detail/views/mainView.html",
                                    controller: "UserMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/detail/controllers/~UserMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Emergency Contact Grid
                    .state("mainState.maintenance.userMaintenance.emergencyContactGrid",
                        {
                            url: "/emergencyContactGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/emergancy/grid/views/mainView.html",
                                    controller: "EmergencyContactGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/emergancy/grid/controllers/~EmergencyContactGridController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Emergency Contact Detail
                    .state("mainState.maintenance.userMaintenance.emergencyContactDetail",
                        {
                            url: "/emergencyContactDetail/:userid/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/emergancy/detail/views/mainView.html",
                                    controller: "EmergencyContactDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/emergancy/detail/controllers/~EmergencyContactDetailController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Billing Rates Grid
                    .state("mainState.maintenance.userMaintenance.billingRatesGrid",
                        {
                            url: "/billingRatesGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/billingRates/grid/views/mainView.html",
                                    controller: "BillingRatesGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/billingRates/grid/controllers/~BillingRatesGridController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Billing Rates Detail
                    .state("mainState.maintenance.userMaintenance.billingRatesDetail",
                        {
                            url: "/billingRatesDetail/:userid/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/billingRates/detail/views/mainView.html",
                                    controller: "BillingRatesDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/billingRates/detail/controllers/~BillingRatesDetailController.js");
                                    }
                                ]
                            }
                        })
                    // User maintnenance Travel Information Grid
                    .state("mainState.maintenance.userMaintenance.travelInformtionGrid",
                        {
                            url: "/travelInformationGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/travelInformation/grid/views/mainView.html",
                                    controller: "TravelInformationGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/travelInformation/grid/controllers/~TravelInformationGridController.js");
                                    }
                                ]
                            }
                        })
                    // User maintenance Travel Information Detail
                    .state("mainState.maintenance.userMaintenance.travelInformationDetail",
                        {
                            url: "/travelInformationDetail/:userid/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/travelInformation/detail/views/mainView.html",
                                    controller: "TravelInformationDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/travelInformation/detail/controllers/~TravelInformationDetailController.js");
                                    }
                                ]
                            }
                        })
                    // User maintnenance Office Equipment Grid
                    .state("mainState.maintenance.userMaintenance.officeEquipmentGrid",
                        {
                            url: "/officeEquipmentGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/officeEquipment/grid/views/mainView.html",
                                    controller: "OfficeEquipmentGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/officeEquipment/grid/controllers/~OfficeEquipmentGridController.js");
                                    }
                                ]
                            }
                        })
                    // User maintenance Office Equipemnt Detail
                    .state("mainState.maintenance.userMaintenance.officeEquipmentDetail",
                        {
                            url: "/officeEquipmentDetail/:userid/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/officeEquipment/detail/views/mainView.html",
                                    controller: "OfficeEquipmentDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/officeEquipment/detail/controllers/~OfficeEquipmentDetailController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance TeamJobDesignation Grid
                    .state("mainState.maintenance.userMaintenance.teamJobDesignationGrid",
                        {
                            url: "/teamJobDesignationGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/teamJobDesignation/grid/views/mainView.html",
                                    controller: "TeamJobDesignationGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/teamJobDesignation/grid/controllers/~TeamJobDesignationGridController.js");
                                    }
                                ]
                            }
                        })
                    // User maintenance TeamJobDesignation Detail
                    .state("mainState.maintenance.userMaintenance.teamJobDesignationtDetail",
                        {
                            url: "/teamJobDesignationDetail/:userid/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/teamJobDesignation/detail/views/mainView.html",
                                    controller: "TeamJobDesignationDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/teamJobDesignation/detail/controllers/~TeamJobDesignationDetailController.js");
                                    }
                                ]
                            }
                        })
                    .state("mainState.maintenance.userMaintenance.projectMaintenance",
                        {
                            url: "/projectMaintenance/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/projects/tree/views/mainView.html",
                                    controller: "UserProjectsController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/projects/tree/controllers/~UserProjectsController.js");
                                    }
                                ]
                            }
                        })
                    // User Maintenance Personal Information
                    .state("mainState.maintenance.userMaintenance.personalInformationDetail",
                        {
                            url: "/personalInformationDetail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/userMaintenanceState/personalInformation/views/mainView.html",
                                    controller: "PersonalInformationController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/userMaintenanceState/personalInformation/controllers/~PersonalInformationController.js");
                                    }
                                ]
                            }
                        })
                    /// Settings
                    .state("mainState.maintenance.settings",
                        {
                            url: "/settings",
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/settingsMaintenanceState/views/mainView.html",
                                    controller: "AdminMaintenanceSettingsController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/settingsMaintenanceState/controllers/~AdminMaintenanceSettingsController.js");
                                    }
                                ]
                            }
                        })
                    // Client Maintenance
                    .state("mainState.maintenance.clientMaintenance",
                        {
                            url: "/clients",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/clientMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // Client Maintenance Grid
                    .state("mainState.maintenance.clientMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/clientMaintenanceState/grid/views/mainView.html",
                                    controller: "ClientMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/clientMaintenanceState/grid/controllers/~ClientMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Client Maintenance Detail
                    .state("mainState.maintenance.clientMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/clientMaintenanceState/detail/views/mainView.html",
                                    controller: "ClientMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/clientMaintenanceState/detail/controllers/~ClientMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Project Maintenance
                    .state("mainState.maintenance.projectMaintenance",
                        {
                            url: "/projects",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/projectMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // Project Maintenance Grid
                    .state("mainState.maintenance.projectMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/projectMaintenanceState/grid/views/mainView.html",
                                    controller: "ProjectMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/projectMaintenanceState/grid/controllers/~ProjectMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Project Maintenance Detail
                    .state("mainState.maintenance.projectMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/projectMaintenanceState/detail/views/mainView.html",
                                    controller: "ProjectMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/projectMaintenanceState/detail/controllers/~ProjectMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    .state("mainState.maintenance.projectMaintenance.subProjectGrid",
                        {
                            url: "/subProjectGrid/:id",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/projectMaintenanceState/subProjectDetail/views/gridView.html",
                                    controller: "SubProjectGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/projectMaintenanceState/subProjectDetail/controllers/~SubProjectGridController.js");
                                    }
                                ]
                            }
                        })
                    // Sub Project Maintenance Detail
                    .state("mainState.maintenance.projectMaintenance.subProjectDetail",
                        {
                            url: "/detail/:id/subProject/:subProjectId",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/projectMaintenanceState/subProjectDetail/views/mainView.html",
                                    controller: "SubProjectMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/projectMaintenanceState/subProjectDetail/controllers/~SubProjectMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Template Maintenance
                    .state("mainState.maintenance.scorecardTemplateMaintenance",
                        {
                            url: "/scorecardTemplates",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // ScorecardTemplate Maintenance Grid
                    .state("mainState.maintenance.scorecardTemplateMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateMaintenanceState/grid/views/mainView.html",
                                    controller: "ScorecardTemplateMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplateMaintenanceState/grid/controllers/~ScorecardTemplateMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // ScorecardTemplate Maintenance Detail
                    .state("mainState.maintenance.scorecardTemplateMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateMaintenanceState/detail/views/mainView.html",
                                    controller: "ScorecardTemplateMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplateMaintenanceState/detail/controllers/~ScorecardTemplateMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Template Item Maintenance
                    .state("mainState.maintenance.scorecardTemplateItemMaintenance",
                        {
                            url: "/scorecardTemplateItems",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateItemMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // ScorecardTemplate Item Maintenance Grid
                    .state("mainState.maintenance.scorecardTemplateItemMaintenance.grid",
                        {
                            url: "/scorecardTemplate/:scorecardTemplateId/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateItemMaintenanceState/grid/views/mainView.html",
                                    controller: "ScorecardTemplateItemMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplateItemMaintenanceState/grid/controllers/~ScorecardTemplateItemMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // ScorecardTemplate Item Maintenance Detail
                    .state("mainState.maintenance.scorecardTemplateItemMaintenance.detail",
                        {
                            url: "/scorecardTemplateItem/:scorecardTemplateId/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplateItemMaintenanceState/detail/views/mainView.html",
                                    controller: "ScorecardTemplateItemMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplateItemMaintenanceState/detail/controllers/~ScorecardTemplateItemMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Template Period Maintenance
                    .state("mainState.maintenance.scorecardTemplatePeriodMaintenance",
                        {
                            url: "/scorecardTemplatePeriods",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplatePeriodMaintenanceState/stateView.html"
                                }
                            }
                        })
                    // ScorecardTemplate Period Maintenance Grid
                    .state("mainState.maintenance.scorecardTemplatePeriodMaintenance.grid",
                        {
                            url: "/scorecardTemplate/:scorecardTemplateId/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplatePeriodMaintenanceState/grid/views/mainView.html",
                                    controller: "ScorecardTemplatePeriodMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplatePeriodMaintenanceState/grid/controllers/~ScorecardTemplatePeriodMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // ScorecardTemplate Period Maintenance Detail
                    .state("mainState.maintenance.scorecardTemplatePeriodMaintenance.detail",
                        {
                            url: "/scorecardTemplatePeriod/:scorecardTemplateId/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/scorecardTemplatePeriodMaintenanceState/detail/views/mainView.html",
                                    controller: "ScorecardTemplatePeriodMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/scorecardTemplatePeriodMaintenanceState/detail/controllers/~ScorecardTemplatePeriodMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Maintenance
                    .state("mainState.scorecard",
                        {
                            url: "/scorecards",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl: "Portals/app/states/mainState/scorecardState/stateView.html"
                                }
                            }
                        })
                    // Scorecard Maintenance Grid
                    .state("mainState.scorecard.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl: "Portals/app/states/mainState/scorecardState/grid/views/mainView.html",
                                    controller: "ScorecardGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/scorecardState/grid/controllers/~ScorecardGridController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Maintenance Detail
                    .state("mainState.scorecard.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl: "Portals/app/states/mainState/scorecardState/detail/views/mainView.html",
                                    controller: "ScorecardDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/scorecardState/detail/controllers/~ScorecardDetailController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Maintenance Report
                    .state("mainState.scorecard.report",
                        {
                            url: "/report",
                            views: {
                                'detail': {
                                    templateUrl: "Portals/app/states/mainState/scorecardState/report/views/mainView.html",
                                    controller: "ScorecardReportController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/scorecardState/report/controllers/~ScorecardReportController.js");
                                    }
                                ]
                            }
                        })
                    // Scorecard Submit Scorecard Detail
                    .state("mainState.scorecard.submitScorecard",
                        {
                            url: "/submitScorecard/:id/:readOnly",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/scorecardState/submitScorecard/views/mainView.html",
                                    controller: "ScorecardSubmitScorecardController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/scorecardState/submitScorecard/controllers/~ScorecardSubmitScorecardController.js");
                                    }
                                ]
                            }
                        })
                    // Billing Cycle Maintenance
                    .state("mainState.maintenance.billingCycleMaintenance",
                        {
                            url: "/billingCycle",
                            abstract: true,
                            views: {
                                'view': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/billingCycleMaintenanceState/stateView.html"
                                }
                            }
                        })

                    // Billing Cycle Grid
                    .state("mainState.maintenance.billingCycleMaintenance.grid",
                        {
                            url: "/grid",
                            views: {
                                'grid': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/billingCycleMaintenanceState/grid/views/mainView.html",
                                    controller: "BillingCycleMaintenanceGridController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/billingCycleMaintenanceState/grid/controllers/~BillingCycleMaintenanceGridController.js");
                                    }
                                ]
                            }
                        })
                    // Billing Cycle Maintenance Detail
                    .state("mainState.maintenance.billingCycleMaintenance.detail",
                        {
                            url: "/detail/:id",
                            views: {
                                'detail': {
                                    templateUrl:
                                        "Portals/app/states/mainState/maintenanceState/billingCycleMaintenanceState/detail/views/mainView.html",
                                    controller: "BillingCycleMaintenanceDetailController as vm"
                                }
                            },
                            resolve: {
                                loadMainCtrl: [
                                    "$ocLazyLoad", function ($ocLazyLoad) {
                                        return $ocLazyLoad
                                            .load("Portals/app/states/mainState/maintenanceState/billingCycleMaintenanceState/detail/controllers/~BillingCycleMaintenanceDetailController.js");
                                    }
                                ]
                            }
                        });

                $locationProvider.html5Mode(false);

                $httpProvider.interceptors.push("AuthHttpResponseInterceptor");

                //initialize get if not there
                if (!$httpProvider.defaults.headers.get) {
                    $httpProvider.defaults.headers.get = {};
                }

                $httpProvider.defaults.headers.get["If-Modified-Since"] = "Mon, 26 Jul 1997 05:00:00 GMT";
                $httpProvider.defaults.headers.get["Cache-Control"] = "no-cache";
                $httpProvider.defaults.headers.get["Pragma"] = "no-cache";
            }
        ])
    .run([
        "$location", "$state", "$rootScope", function ($location, $state, $rootScope) {
            $rootScope.$on('$stateChangeStart', function (e, to) {
                //if (to.name == 'root.login') {
                //}
                //else {
                //    //e.preventDefault();
                //    alert("Check your tokens");
                //}
            });
            const portalType = angular.lowercase($location.$$path);
            switch (portalType) {
                case "/admin":
                case "/admin/":
                case "/subscriber":
                case "/subscriber/":
                case "/":
                default:
                    $state.transitionTo("root.login");
                    break;
            }
        }
    ]);

angular.module("AngularApp")
    .filter("trusted",
        [
            "$sce", function ($sce) {
                return function (url) {
                    return $sce.trustAsResourceUrl(url);
                };
            }
        ]);