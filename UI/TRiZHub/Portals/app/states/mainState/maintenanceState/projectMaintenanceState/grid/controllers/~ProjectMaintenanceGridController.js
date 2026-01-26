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
var ProjectMaintenanceGridController = /** @class */ (function (_super) {
    __extends(ProjectMaintenanceGridController, _super);
    //#endregion
    //#region Ctor
    function ProjectMaintenanceGridController($scope, $state, ProjectService, Popups, tcrGrid) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.ProjectService = ProjectService;
        _this.Popups = Popups;
        _this.tcrGrid = tcrGrid;
        _this.loadingIsDone = false;
        _this.onDataLoaded = function (event) { _this.onLoadEvent(event); };
        _this.onDetailDataLoaded = function (event) { _this.onDetailLoadEvent(event); };
        _this.compareLoopedIds = function (element) {
            return _this.compareLoopedElement(element);
        };
        _this.newProject = function () {
            _this.$state.transitionTo("mainState.maintenance.projectMaintenance.detail", { "id": "new" });
        };
        _this.newSubProject = function (projectId) {
            _this.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectDetail", { "id": projectId, "subProjectId": "new" });
        };
        _this.toggleSubProjects = function (project) {
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
        _this.setupDetailOnRows = function () {
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
        _this.deleteProject = function (project, index) {
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
        _this.deleteSubProject = function (subProject, index) {
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
        _this.toggleInactiveProjectShow = function () {
            var self = _this;
            _this.pageGrid.loadGrid();
        };
        var self = _this;
        self.show = false;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("projectnumber", self.ProjectService.projectGrid, self.onDataLoaded, null, null, $state);
        self.pageGrid.loadGrid();
        return _this;
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