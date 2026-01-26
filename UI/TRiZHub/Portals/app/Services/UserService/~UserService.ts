module UserServiceModule {
    export interface IUserService {
        userSave: (viewModel: UserEditModel) => ng.IPromise<UserEditModel>;
        userGrid: (req: GridModel) => ng.IPromise<GridResultModel<UserGridModel>>;
        userGet: (id: string) => ng.IPromise<UserEditModel>;
        userUnlock: (id: string) => ng.IPromise<UserEditModel>;
        activateUser: (id: string) => ng.IPromise<UserEditModel>;
        deactivateUser: (id: string) => ng.IPromise<UserEditModel>;
        userDropdownList: () => ng.IPromise<UserDropdownModel>;
        userTimesheetFilterDropdown: () => ng.IPromise<UserDropdownModel>;

        userScorecardEmployeeFilterDropdown: () => ng.IPromise<UserDropdownModel>;
        userScorecardEvaluatorFilterDropdown: () => ng.IPromise<UserDropdownModel>;
        userScorecardEvaluatorsDropdown: () => ng.IPromise<UserDropdownModel>;
        userScorecardLineManagersDropdown: () => ng.IPromise<UserDropdownModel>;

        emergencyContactSave: (viewModel: EmergencyContactEditModel) => ng.IPromise<EmergencyContactEditModel>;
        emergencyContactGet: (id: string) => ng.IPromise<EmergencyContactEditModel>;

        personalInformationSave: (viewModel: PersonalInformationModel) => ng.IPromise<PersonalInformationModel>;
        personalInformationGet: (id: string) => ng.IPromise<PersonalInformationModel>;

        teamJobDesignationSave: (viewModel: TeamJobDesignationEditModel) => ng.IPromise<TeamJobDesignationEditModel>;
        teamJobDesignationGet: (id: string) => ng.IPromise<TeamJobDesignationEditModel>;

        teamJobDesignationUniqueClient: (id: string) => ng.IPromise<any>;

        //uploadImportOfficeEquiment: (req: FormData) => ng.IPromise<FormData>;
    }

    export class UserService extends CHServiceBase implements IUserService {
        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/User/");
        }

        //#endregion

        signUp = (viewModel: UserEditModel): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SignUp", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userSave = (viewModel: UserEditModel): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "UserSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userGrid = (req: GridModel): ng.IPromise<GridResultModel<UserGridModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "UserGrid", req)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userGet = (id: string): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        // #region user maintenance emergency contact

        emergencyContactSave = (viewModel: EmergencyContactEditModel): ng.IPromise<EmergencyContactEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "EmergencyContactSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        emergencyContactGet = (id: string): ng.IPromise<EmergencyContactEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "EmergencyContactGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        emergencyContactDelete = (viewModel: EmergencyContactEditModel): ng.IPromise<EmergencyContactEditModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "emergencyContactDelete", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        emergencyContactGrid = (req: GridResultModel<EmergencyContactGridModel>): ng.
            IPromise<GridResultModel<EmergencyContactGridModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "emergencyContactGrid", req)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        //#endregion

        //#region User Office Equipment

        officeEquipmentSave = (viewModel: OfficeEquipmentEditModel): ng.IPromise<OfficeEquipmentEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "OfficeEquipmentSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        officeEquipmentGet = (id: string): ng.IPromise<OfficeEquipmentEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "OfficeEquipmentGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        officeEquipmentDelete = (viewModel: OfficeEquipmentEditModel): ng.IPromise<OfficeEquipmentEditModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "officeEquipmentDelete", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        officeEquipmentGrid = (req: GridResultModel<OfficeEquipmentGridModel>): ng.
            IPromise<GridResultModel<OfficeEquipmentGridModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "officeEquipmentGrid", req)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        //uploadImportOfficeEquiment = (req: FormData): ng.IPromise<FormData> => {
        //    var deferred = this.$q.defer();
        //    this.$http.post(this.urlRoot + "uploadImportOfficeEquiment", req)
        //        .then(
        //            result => {
        //                deferred.resolve(result.data);
        //            },
        //            error => {
        //                deferred.reject(error.data.message);
        //            }
        //        );
        //    return deferred.promise;
        //};

        //#endregion

        //#region Personal Information

        personalInformationGet = (id: string): ng.IPromise<PersonalInformationModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "PersonalInformationGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        personalInformationSave = (viewModel: PersonalInformationModel): ng.IPromise<PersonalInformationModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "PersonalInformationSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        // endregion

        // #Region TeamJobDesignation

        teamJobDesignationSave = (viewModel: TeamJobDesignationEditModel): ng.IPromise<TeamJobDesignationEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TeamJobDesignationSave", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        teamJobDesignationGet = (id: string): ng.IPromise<TeamJobDesignationEditModel> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "TeamJobDesignationGet/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        teamJobDesignationDelete = (viewModel: TeamJobDesignationEditModel): ng.IPromise<TeamJobDesignationEditModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "teamJobDesignationDelete", viewModel)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        teamJobDesignationUniqueClient = (id: string): ng.IPromise<any> => {
            var deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "TeamJobDesignationUniqueClient/" + id)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        teamJobDesignationGrid = (req: GridResultModel<TeamJobDesignationEditModel>): ng.
            IPromise<GridResultModel<TeamJobDesignationEditModel>> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "TeamJobDesignationGrid", req)
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        // end region

        userUnlock = (id: string): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "UserUnlock", { Id: id })
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        activateUser = (id: string): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ActivateUser", { Id: id })
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        deactivateUser = (id: string): ng.IPromise<UserEditModel> => {
            var deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "DeactivateUser", { Id: id })
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userDropdownList = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        allUserDropdownList = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "AllUserDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userTimesheetFilterDropdown = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserTimesheetFilterDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userScorecardEmployeeFilterDropdown = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserScorecardEmployeeFilterDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userScorecardEvaluatorFilterDropdown = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserScorecardEvaluatorFilterDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userScorecardEvaluatorsDropdown = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserScorecardEvaluatorsDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

        userScorecardLineManagersDropdown = (): ng.IPromise<UserDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserScorecardLineManagersDropdown/")
                .then(
                    result => {
                        deferred.resolve(result.data);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };
    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new UserService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("UserService",
            [
                "$http",
                "$q",
                "ENV",
                getInstance
            ]);
}