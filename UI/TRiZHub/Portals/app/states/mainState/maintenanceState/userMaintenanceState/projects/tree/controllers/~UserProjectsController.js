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
var UserProjectsController = /** @class */ (function (_super) {
    __extends(UserProjectsController, _super);
    //#endregion
    //#region Ctor
    function UserProjectsController($scope, $stateParams, $timeout, $window, $state, Popups, UserService, ProjectService) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$stateParams = $stateParams;
        _this.$timeout = $timeout;
        _this.$window = $window;
        _this.$state = $state;
        _this.Popups = Popups;
        _this.UserService = UserService;
        _this.ProjectService = ProjectService;
        _this.saveSuccess = false;
        _this.setCollapsedValue = function (listOfProjects, value) {
            if (listOfProjects != null && listOfProjects.length > 0) {
                for (var i = 0; i < listOfProjects.length; i++) {
                    listOfProjects[i].collapsed = value;
                    _this.setCollapsedValue(listOfProjects[i].children, value);
                }
            }
        };
        _this.setSelectedProject = function (node) {
            _this.selectedProject = node;
        };
        _this.setFirstClientProject = function () {
            _this.selectedProject = _this.selectedClient.listOfProjects[0];
        };
        _this.unassignItem = function (node) {
            var id = node.projectId;
            // If it was a node that was added now ie before saved to DB
            _this.assignedData.forEach(function (item, index) {
                if ((item.subProjectId == node.subProjectId) && (item.projectId === node.projectId)) {
                    if (node.isNew)
                        _this.assignedData.splice(index, 1);
                    else
                        node.isDeleted = true;
                }
            });
        };
        _this.assignProject = function (node) {
            for (var i = 0; i < _this.assignedData.length; i++) {
                if ((_this.assignedData[i].projectId == node.projectId) && (_this.assignedData[i].subProjectId == null)) {
                    _this.assignedData[i].isDeleted = false;
                    return;
                }
            }
            var newAssignment = {
                "id": null,
                "description": "[" + node.code + "] " + node.name,
                "projectId": node.projectId,
                "subProjectId": null,
                "projectName": node.name,
                "subProjectName": null,
                "clientName": _this.selectedClient.name,
                "clientId": _this.selectedClient.clientId,
                "isNew": true,
                "isActive": node.isActive
            };
            _this.assignedData.push(newAssignment);
        };
        _this.assignSubProject = function (node) {
            for (var i = 0; i < _this.assignedData.length; i++) {
                if (_this.assignedData[i].subProjectId == node.subProjectId) {
                    _this.assignedData[i].isDeleted = false;
                    return;
                }
            }
            var newAssignment = {
                "description": "[" + _this.selectedProject.code + "-" + node.code + "] " + _this.selectedProject.name + " [" + node.name + "]",
                "projectId": node.projectId,
                "subProjectId": node.subProjectId,
                "projectName": node.name,
                "subProjectName": null,
                "clientName": _this.selectedClient.name,
                "clientId": _this.selectedClient.clientId,
                "isNew": true,
                "isActive": node.isActive
            };
            _this.assignedData.push(newAssignment);
        };
        _this.mustShowProject = function (id) {
            for (var i = 0; i < _this.assignedData.length; i++) {
                if (((_this.assignedData[i].projectId == id) && (_this.assignedData[i].subProjectId == null))
                    || (_this.assignedData[i].subProjectId == id)) {
                    if (_this.assignedData[i].isDeleted)
                        return true;
                    else
                        return false;
                }
            }
            return true;
        };
        _this.getClientCounts = function (node, type) {
            //        alert(node.selected);
            var totalProjects = node.listOfProjects.length;
            var totalSubProjects = 0;
            var selProjects = 0;
            var selSubProjects = 0;
            for (var i = 0; i < node.listOfProjects.length; i++) {
                var proj = node.listOfProjects[i];
                if (proj.selected)
                    selProjects++;
                totalSubProjects += proj.listOfProjects.length;
                for (var j = 0; j < proj.listOfProjects.length; j++) {
                    if ((proj.listOfProjects[j].selected) || (proj.selected))
                        selSubProjects++;
                }
            }
            if (type == "Project")
                return selProjects + "/" + totalProjects;
            if (type == "SubProject")
                return selSubProjects + "/" + totalSubProjects;
        };
        _this.getProjectCounts = function (node) {
            if (node.selected)
                return "Entire Project Selected";
            var totalSubProjects = node.listOfProjects.length;
            var selSubProjects = 0;
            for (var i = 0; i < node.listOfProjects.length; i++) {
                if (node.listOfProjects[i].selected)
                    selSubProjects++;
            }
            return "Sub-Projects (" + selSubProjects + "/" + totalSubProjects + ")";
        };
        _this.toggleInactiveProjectShow = function () {
            _this.viewModel.includeInactive = !_this.viewModel.includeInactive;
            _this.getUserIdentityProjects();
            _this.getUserAssignedProjects();
        };
        _this.selectAllNone = function (node, selected) {
            _this.updateChildren(node.listOfProjects, selected);
        };
        _this.selectChange = function (node) {
            _this.updateChildren(node.listOfProjects, false);
        };
        _this.updateChildren = function (listOfProjects, value) {
            if (listOfProjects != null && listOfProjects.length > 0) {
                for (var i = 0; i < listOfProjects.length; i++) {
                    listOfProjects[i].selected = value;
                    _this.updateChildren(listOfProjects[i].listOfProjects, value);
                }
            }
        };
        _this.getMembers = function (members) {
            var listOfProjects = [];
            var flattenMembers = members.map(function (m) {
                if (m.listOfProjects && m.listOfProjects.length) {
                    listOfProjects = listOfProjects.concat(m.listOfProjects);
                }
                return m;
            });
            return flattenMembers.concat(listOfProjects.length ? this.getMembers(listOfProjects) : listOfProjects);
        };
        _this.submitForm = function () {
            var self = _this;
            var flattened = _this.getMembers(_this.treeData);
            var selected = flattened.filter(function (p) { return p.selected; });
            var toSubmit = [];
            // Remove project list
            for (var i = 0; i < selected.length; i++) {
                toSubmit.push({
                    projectId: selected[i].projectId,
                    subprojectId: selected[i].subProjectId,
                    clientId: selected[i].clientId
                });
                //            selected[i].listOfProjects = [];
            }
            _this.ProjectService.saveUserIdentityProjects(_this.viewModel.id, toSubmit)
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
        _this.submitForm1 = function () {
            var selected = [];
            _this.assignedData.forEach(function (item, index) {
                if ((item.isNew) || (item.isDeleted)) {
                    selected.push({
                        "id": item.id,
                        "clientId": item.clientId,
                        "projectId": item.projectId,
                        "subProjectId": item.subProjectId,
                        "action": (item.isNew ? 'Insert' : 'Delete')
                    });
                }
            });
            var self = _this;
            _this.ProjectService.saveUserIdentityProjects(_this.viewModel.id, selected)
                .then(function (result) {
                self.saveSuccess = true;
                self.$timeout(function () {
                    self.saveSuccess = false;
                }, 4000);
                _this.getUserAssignedProjects();
            }, function (error) {
                self.handleError(error);
            });
        };
        var self = _this;
        _this.viewModel = {};
        _this.viewModel.id = _this.$stateParams["id"];
        _this.viewModel.includeInactive = false;
        _this.UserService.userGet(_this.viewModel.id)
            .then(function (result) {
            self.user = result;
        }, function (error) {
            self.handleError(error);
        });
        _this.getUserIdentityProjects();
        return _this;
        //        this.getUserAssignedProjects();
    }
    UserProjectsController.prototype.getUserAssignedProjects = function () {
        var self = this;
        this.ProjectService.getUserAllocatedProjects(this.viewModel.id, this.viewModel.includeInactive)
            .then(function (result) {
            self.assignedData = result;
            //                    let JsonData = JSON.stringify(result);
            //                    alert(JsonData);
        }, function (error) {
            self.handleError(error);
        });
    };
    UserProjectsController.prototype.getUserIdentityProjects = function () {
        var self = this;
        this.ProjectService.userIdentityProjects(this.viewModel.id, this.viewModel.includeInactive)
            .then(function (result) {
            var JsonData = JSON.stringify(result);
            var next = JsonData;
            self.treeData = result;
            //                    self.selectedClient = result[0];
            //                    self.selectedProject = self.selectedClient.listOfProjects[0];
            self.setCollapsedValue(self.treeData, true);
        }, function (error) {
            self.handleError(error);
        });
    };
    return UserProjectsController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("UserProjectsController", [
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
//# sourceMappingURL=~UserProjectsController.js.map