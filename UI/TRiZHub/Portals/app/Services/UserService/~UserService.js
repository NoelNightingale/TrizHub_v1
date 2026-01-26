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
var UserServiceModule;
(function (UserServiceModule) {
    var UserService = /** @class */ (function (_super) {
        __extends(UserService, _super);
        //#region Ctor
        function UserService($http, $q, ENV) {
            var _this = _super.call(this, ENV.serverLocation + "api/User/") || this;
            _this.$http = $http;
            _this.$q = $q;
            _this.ENV = ENV;
            //#endregion
            _this.signUp = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "SignUp", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            // #region user maintenance emergency contact
            _this.emergencyContactSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "EmergencyContactSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.emergencyContactGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "EmergencyContactGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.emergencyContactDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "emergencyContactDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.emergencyContactGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "emergencyContactGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            //#endregion
            //#region User Office Equipment
            _this.officeEquipmentSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "OfficeEquipmentSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.officeEquipmentGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "OfficeEquipmentGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.officeEquipmentDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "officeEquipmentDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.officeEquipmentGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "officeEquipmentGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
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
            _this.personalInformationGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "PersonalInformationGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.personalInformationSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "PersonalInformationSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            // endregion
            // #Region TeamJobDesignation
            _this.teamJobDesignationSave = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TeamJobDesignationSave", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.teamJobDesignationGet = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "TeamJobDesignationGet/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.teamJobDesignationDelete = function (viewModel) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "teamJobDesignationDelete", viewModel)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.teamJobDesignationUniqueClient = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "TeamJobDesignationUniqueClient/" + id)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.teamJobDesignationGrid = function (req) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "TeamJobDesignationGrid", req)
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            // end region
            _this.userUnlock = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "UserUnlock", { Id: id })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.activateUser = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "ActivateUser", { Id: id })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.deactivateUser = function (id) {
                var deferred = _this.$q.defer();
                _this.$http.post(_this.urlRoot + "DeactivateUser", { Id: id })
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.allUserDropdownList = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "AllUserDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userTimesheetFilterDropdown = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserTimesheetFilterDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userScorecardEmployeeFilterDropdown = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserScorecardEmployeeFilterDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userScorecardEvaluatorFilterDropdown = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserScorecardEvaluatorFilterDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userScorecardEvaluatorsDropdown = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserScorecardEvaluatorsDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            _this.userScorecardLineManagersDropdown = function () {
                var deferred = _this.$q.defer();
                _this.$http.get(_this.urlRoot + "UserScorecardLineManagersDropdown/")
                    .then(function (result) {
                    deferred.resolve(result.data);
                }, function (error) {
                    deferred.reject(error.data.message);
                });
                return deferred.promise;
            };
            return _this;
        }
        return UserService;
    }(CHServiceBase));
    UserServiceModule.UserService = UserService;
    function getInstance($http, $q, ENV) {
        return new UserService($http, $q, ENV);
    }
    angular.module("AngularApp")
        .factory("UserService", [
        "$http",
        "$q",
        "ENV",
        getInstance
    ]);
})(UserServiceModule || (UserServiceModule = {}));
//# sourceMappingURL=~UserService.js.map