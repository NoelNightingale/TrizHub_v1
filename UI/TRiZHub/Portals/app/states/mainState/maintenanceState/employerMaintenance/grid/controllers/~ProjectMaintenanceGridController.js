var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var ProjectMaintenanceGridController = (function (_super) {
    __extends(ProjectMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function ProjectMaintenanceGridController($scope, $state, ProjectService, Popups, tcrGrid) {
        var _this = this;
        _super.call(this, $scope, Popups, $state);
        this.$scope = $scope;
        this.$state = $state;
        this.ProjectService = ProjectService;
        this.Popups = Popups;
        this.tcrGrid = tcrGrid;
        this.loadingIsDone = false;
        this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        this.onDetailDataLoaded = function (event) { _this.onDetailLoadEvent(event); };
        this.compareLoopedIds = function (element) {
            return _this.compareLoopedElement(element);
        };
        this.newProject = function () {
            _this.$state.transitionTo("mainState.maintenance.projectMaintenance.detail", { "id": "new" });
        };
        this.newSubProject = function (projectId) {
            _this.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectDetail", { "id": projectId, "subProjectId": "new" });
        };
        this.toggleSubProjects = function (project) {
            var self = _this;
            if (self.selectedProjectId == project.id) {
                self.selectedProjectId = null;
            }
            else {
                self.selectedProjectId = project.id;
                self.subProjectPageGrid = new TcrGridServiceModule.TcrGridService("projectname", self.ProjectService.subProjectGrid, self.onDetailDataLoaded, function (model) {
                    model.parentId = self.selectedProjectId;
                }, null, self.$state);
                self.subProjectPageGrid.loadGrid();
            }
        };
        this.setupDetailOnRows = function () {
            var copyOfData = angular.copy(_this.gridModel.data);
            for (var i = 0; i < copyOfData.length; i++) {
                _this.loopedElement = copyOfData[i];
                var index = 1;
                for (var j = 0; j < _this.gridModel.data.length; j++) {
                    if (_this.gridModel.data[j].id === _this.loopedElement.id) {
                        break;
                    }
                    index++;
                }
                var object = { id: copyOfData[i].id, isDetail: true };
                _this.gridModel.data.splice(index, 0, object);
            }
        };
        this.deleteProject = function (project, index) {
            _this.Popups.confirmationDialog(_this.$scope, "Are you sure you want to delete?", "You are about to delete this project")
                .then(function (action) {
                if (action) {
                    _this.ProjectService.deleteProject(project.id)
                        .then(function (result) {
                        if (result == 0) {
                            _this.Popups.showError(_this.$scope, "The Project could not be deleted as it has Timesheet entries assigned to it.", null, null);
                        }
                        else {
                            _this.gridModel.data.splice(index, 1);
                        }
                    }, function (error) {
                        _this.handleError(error);
                    });
                }
            }, function (error) {
                _this.handleError(error);
            });
        };
        this.deleteSubProject = function (subProject, index) {
            _this.Popups.confirmationDialog(_this.$scope, "Are you sure you want to delete?", "You are about to delete this sub project")
                .then(function (action) {
                if (action) {
                    _this.ProjectService.deleteSubProject(subProject.id)
                        .then(function (result) {
                        if (result == 0) {
                            _this.Popups.showError(_this.$scope, "The Sub Project could not be deleted as it has Timesheet entries assigned to it.", null, null);
                        }
                        else {
                            _this.subProjectGridModel.data.splice(index, 1);
                        }
                    }, function (error) {
                        _this.handleError(error);
                    });
                }
            }, function (error) {
                _this.handleError(error);
            });
        };
        this.toggleInactiveProjectShow = function () {
            var self = _this;
            _this.pageGrid.loadGrid();
        };
        var self = this;
        self.show = false;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("projectnumber", self.ProjectService.projectGrid, self.onDataLoaded, null, null, $state);
        self.pageGrid.loadGrid();
    }
    //#endregion
    ProjectMaintenanceGridController.prototype.onLoadEvent = function (event) {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.setupDetailOnRows();
            this.loadingIsDone = true;
        }
    };
    ProjectMaintenanceGridController.prototype.onDetailLoadEvent = function (event) {
        this.subProjectGridModel = event;
    };
    ProjectMaintenanceGridController.prototype.compareLoopedElement = function (element) {
        return this.loopedElement.id === element.id;
    };
    return ProjectMaintenanceGridController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("ProjectMaintenanceGridController", [
    "$scope",
    "$state",
    "ProjectService",
    "Popups",
    ProjectMaintenanceGridController
]);
//# sourceMappingURL=~ProjectMaintenanceGridController.js.map