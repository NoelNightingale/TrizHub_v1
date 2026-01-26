
module ProjectServiceModule {

    export interface IProjectService {
        projectGrid: (req: GridModel) => ng.IPromise<GridResultModel<ProjectGridModel>>;
        projectGet: (id: string) => ng.IPromise<ProjectModel>;
        projectSave: (viewModel: ProjectModel) => ng.IPromise<ProjectModel>;

        subProjectGrid: (req: GridModel) => ng.IPromise<GridResultModel<SubProjectGridModel>>;
        subProjectSave: (viewModel: SubProjectModel) => ng.IPromise<SubProjectModel>;
        subProjectGet: (id: string) => ng.IPromise<SubProjectModel>;

        projectAndSubProjectDropdownList: () => ng.IPromise<ProjectAndSubProjectDropdownModel>;
        projectDropdownList: () => ng.IPromise<ProjectDropdownModel>;
        allProjectDropdownList: () => ng.IPromise<ProjectDropdownModel>;
    
        projectDropdownListForClientReporter: () => ng.IPromise<ProjectDropdownModel>;
        userIdentityProjects: (id: string, includeInactive: boolean) => ng.IPromise<UserIdentityProjectModel>;
        saveUserIdentityProjects: (userId: string, projects: Array<any>) => ng.IPromise<any>;

        getUserAllocatedProjects: (id: string, includeInactive: boolean) => ng.IPromise<ProjectDropdownModel>;
    }

    export class ProjectService extends CHServiceBase implements IProjectService {

        urlRoot: string;

        //#region Ctor

        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private ENV: any) {
            super(ENV.serverLocation + "api/Project/");
        }

        //#endregion

        projectGrid = (req: GridResultModel<ProjectGridModel>): ng.IPromise<GridResultModel<ProjectGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ProjectsGrid", req)
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

        projectGet = (id: string): ng.IPromise<ProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProjectGet/" + id)
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

        projectSave = (viewModel: ProjectModel): ng.IPromise<ProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "ProjectSave", viewModel)
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

        subProjectGrid = (req: GridResultModel<SubProjectGridModel>): ng.IPromise<GridResultModel<SubProjectGridModel>> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SubProjectsGrid", req)
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

        subProjectSave = (viewModel: SubProjectModel): ng.IPromise<SubProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SubProjectSave", viewModel)
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

        subProjectGet = (id: string): ng.IPromise<SubProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "SubProjectGet/" + id)
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

        projectAndSubProjectDropdownList = (): ng.IPromise<ProjectAndSubProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProjectAndSubProjectDropdown/")
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

        getUserAllocatedProjects = (id: string, includeInactive: boolean): ng.IPromise<ProjectAndSubProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "GetUserAllocatedProjects/" + id + "?includeInactive=" + includeInactive)
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

        projectDropdownList = (): ng.IPromise<ProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProjectDropdown/")
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


        projectDropdownListForClientReporter = (): ng.IPromise<ProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProjectDropdownForClientReporter/")
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


        allProjectDropdownListForClientReporter = (): ng.IPromise<ProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "AllProjectDropdownForClientReporter/")
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


        allProjectDropdownList = (): ng.IPromise<ProjectDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "AllProjectDropdownForClientReporter/")
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

        projectTypeDropdownList = (): ng.IPromise<ProjectTypeDropdownModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "ProjectTypeDropdown/")
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

        deleteProject = (id: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "DeleteProject/" + id)
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

        deleteSubProject = (id: string): ng.IPromise<any> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "DeleteSubProject/" + id)
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

        userIdentityProjects = (id: string, includeInactive: boolean): ng.IPromise<UserIdentityProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.get(this.urlRoot + "UserIdentityProjectList/?id=" + id + '&includeInactive=' + includeInactive)
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

        saveUserIdentityProjects = (id: string, projects: Array<any>): ng.IPromise<UserIdentityProjectModel> => {
            const deferred = this.$q.defer();
            this.$http.post(this.urlRoot + "SaveUserIdentityProjectList", { userId: id, projects: projects })
                .then(
                    result => {
                        deferred.resolve(result);
                    },
                    error => {
                        deferred.reject(error.data.message);
                    }
                );
            return deferred.promise;
        };

    }

    function getInstance($http: angular.IHttpService, $q: angular.IQService, ENV: any) {
        return new ProjectService($http, $q, ENV);
    }

    angular.module("AngularApp")
        .factory("ProjectService",
        [
            "$http",
            "$q",
            "ENV",
            getInstance
        ]);
}