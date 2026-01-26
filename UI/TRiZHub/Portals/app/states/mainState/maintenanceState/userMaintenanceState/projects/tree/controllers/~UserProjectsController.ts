class UserProjectsController extends CHControllerBase {
    //#region members

    //successMessage = "Saved Successfully";
    //saveSuccess = false;
    viewModel: any;
    treeData: any;
    user: any;
    saveSuccess = false;
    assignedData: any;
    selectedClient: any;
    selectedProject: any;
    //#endregion

    //#region Ctor

    constructor(
        private $scope: ng.IScope,
        private $stateParams: ng.ui.IStateParamsService,
        private $timeout: ng.ITimeoutService,
        private $window: ng.IWindowService,
        private $state: ng.ui.IStateService,
        private Popups: any,
        private UserService: UserServiceModule.UserService,
        private ProjectService: ProjectServiceModule.ProjectService) {
        super($scope, Popups, $state);

        const self = this;
        this.viewModel = {};

        this.viewModel.id = this.$stateParams["id"];
        this.viewModel.includeInactive = false;
        this.UserService.userGet(this.viewModel.id)
            .then(
                result => {
                    self.user = result;
                },
                error => {
                    self.handleError(error);
                });

        this.getUserIdentityProjects();
//        this.getUserAssignedProjects();

    }

    setCollapsedValue = (listOfProjects: any[], value: boolean): void => {
        if (listOfProjects != null && listOfProjects.length > 0) {
            for (var i = 0; i < listOfProjects.length; i++) {
                listOfProjects[i].collapsed = value;
                this.setCollapsedValue(listOfProjects[i].children, value);
            }
        }
    }


    setSelectedProject = (node: any): void => {
        this.selectedProject = node;
    };

    setFirstClientProject = (): void => {
        this.selectedProject = this.selectedClient.listOfProjects[0];
    };


    unassignItem = (node: any): void => {

        let id = node.projectId

        // If it was a node that was added now ie before saved to DB
        this.assignedData.forEach((item, index) => {
            if ((item.subProjectId == node.subProjectId) && (item.projectId === node.projectId)) {
                if (node.isNew) this.assignedData.splice(index, 1);
                else node.isDeleted = true;
            }
        });

    }

    assignProject = (node: any): void => {

        for (var i = 0; i < this.assignedData.length; i++) {
            if ((this.assignedData[i].projectId == node.projectId) && (this.assignedData[i].subProjectId == null)) {
                this.assignedData[i].isDeleted = false;
                return;
            }
        }

        let newAssignment = {
            "id": null,
            "description": "[" + node.code + "] " + node.name,
            "projectId": node.projectId,
            "subProjectId": null,
            "projectName": node.name,
            "subProjectName": null,
            "clientName": this.selectedClient.name,
            "clientId": this.selectedClient.clientId,
            "isNew": true,
            "isActive": node.isActive
        };

        this.assignedData.push(newAssignment);


    };

    assignSubProject = (node: any): void => {

        for (var i = 0; i < this.assignedData.length; i++) {
            if (this.assignedData[i].subProjectId == node.subProjectId) {
                this.assignedData[i].isDeleted = false;
                return;
            }
        }


        let newAssignment = {
            "description": "[" + this.selectedProject.code + "-" + node.code + "] " + this.selectedProject.name + " [" + node.name + "]",
            "projectId": node.projectId,
            "subProjectId": node.subProjectId,
            "projectName": node.name,
            "subProjectName": null,
            "clientName": this.selectedClient.name,
            "clientId": this.selectedClient.clientId,
            "isNew": true,
            "isActive" : node.isActive

        };

        this.assignedData.push(newAssignment);
    };

    mustShowProject = (id: any): boolean => {

        for (var i = 0; i < this.assignedData.length; i++) {
            if (((this.assignedData[i].projectId == id) && (this.assignedData[i].subProjectId == null))
                || (this.assignedData[i].subProjectId == id)) {
                if (this.assignedData[i].isDeleted) return true;
                else return false;
            }
        }
         return true;
        
    }


    getUserAssignedProjects() {
        const self = this;
        this.ProjectService.getUserAllocatedProjects(this.viewModel.id, this.viewModel.includeInactive)
            .then(
                result => {
                    self.assignedData = result;
//                    let JsonData = JSON.stringify(result);
//                    alert(JsonData);
                    
                },
                error => {
                    self.handleError(error);
                });        

    }


    getUserIdentityProjects() {
        const self = this;
        this.ProjectService.userIdentityProjects(this.viewModel.id, this.viewModel.includeInactive)
            .then(
                result => {
                    let JsonData = JSON.stringify(result);
                    let next = JsonData;
                    self.treeData = result;
//                    self.selectedClient = result[0];
//                    self.selectedProject = self.selectedClient.listOfProjects[0];
                    self.setCollapsedValue(self.treeData, true);
                },
                error => {
                    self.handleError(error);
                });
    }

    getClientCounts = (node: any, type:any) => {
//        alert(node.selected);

        var totalProjects = node.listOfProjects.length;
        var totalSubProjects = 0;
        var selProjects = 0;
        var selSubProjects = 0;

        for (var i = 0; i < node.listOfProjects.length; i++) {
            var proj = node.listOfProjects[i];
            if (proj.selected) selProjects++;
            totalSubProjects += proj.listOfProjects.length;
            for (var j = 0;  j < proj.listOfProjects.length; j++) {
                if ((proj.listOfProjects[j].selected) || (proj.selected)) selSubProjects++;
            }
        }

        if (type == "Project")
            return selProjects + "/" + totalProjects;
        if (type == "SubProject")
            return selSubProjects + "/" + totalSubProjects;


    }

    getProjectCounts = (node: any) => {


        if (node.selected) return "Entire Project Selected";


        var totalSubProjects = node.listOfProjects.length;
        var selSubProjects = 0;

        for (var i = 0; i < node.listOfProjects.length; i++) {
            if (node.listOfProjects[i].selected) selSubProjects++;
        }

        return "Sub-Projects (" + selSubProjects + "/" + totalSubProjects + ")";

    }


    toggleInactiveProjectShow = () => {
        this.viewModel.includeInactive = !this.viewModel.includeInactive;
        this.getUserIdentityProjects();
        this.getUserAssignedProjects();
    };

    selectAllNone = (node: any, selected: boolean): void => {
        this.updateChildren(node.listOfProjects, selected)
    }

    selectChange = (node: any): void => {
        this.updateChildren(node.listOfProjects, false)
    };

    updateChildren = (listOfProjects: any[], value: boolean): void => {
        if (listOfProjects != null && listOfProjects.length > 0) {
            for (var i = 0; i < listOfProjects.length; i++) {
                listOfProjects[i].selected = value;
                this.updateChildren(listOfProjects[i].listOfProjects, value);
            }
        }
    }

    getMembers = function (members) {
        var listOfProjects = [];

        var flattenMembers = members.map(function (m) {
            if (m.listOfProjects && m.listOfProjects.length) {
                listOfProjects = listOfProjects.concat(m.listOfProjects);
            }
            return m;
        });
        return flattenMembers.concat(listOfProjects.length ? this.getMembers(listOfProjects) : listOfProjects);
    };

    submitForm = () => {
        var self = this;
        var flattened = this.getMembers(this.treeData);
        var selected = flattened.filter(function (p) { return p.selected; });
        var toSubmit = [];
        // Remove project list
        for (var i = 0; i < selected.length; i++) {
            toSubmit.push({
                projectId : selected[i].projectId,
                subprojectId : selected[i].subProjectId,
                clientId : selected[i].clientId
              }
            )
//            selected[i].listOfProjects = [];
        }
        this.ProjectService.saveUserIdentityProjects(this.viewModel.id, toSubmit)
            .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.saveSuccess = false;
                }, 4000);
//                this.getUserIdentityProjects();
            }, function (error) {
               self.handleError(error);
            });
    };


    submitForm1 = () => {

        let selected = [];

        this.assignedData.forEach((item, index) => {
            if ((item.isNew) || (item.isDeleted)) {

                selected.push(
                    {
                        "id": item.id,
                        "clientId": item.clientId,
                        "projectId": item.projectId,
                        "subProjectId": item.subProjectId,
                        "action": (item.isNew ? 'Insert' : 'Delete')
                    }
                )                                
            }
        });

        const self = this;

        this.ProjectService.saveUserIdentityProjects(this.viewModel.id, selected)
            .then(
                result => {
                    self.saveSuccess = true;
                    self.$timeout(function () {
                        self.saveSuccess = false;
                    }, 4000);

                    this.getUserAssignedProjects();

                },
                error => {
                    self.handleError(error);
                });
    };


    ////#endregion

}

angular.module("AngularApp")
    .controller("UserProjectsController",
        [
            "$scope",
            "$stateParams",
            "$timeout",
            "$window",
            "$state",
            "Popups",
            "UserService",
            "ProjectService",
            UserProjectsController
        ]);