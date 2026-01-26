class ProjectMaintenanceGridController extends CHControllerBase {

    //#region members

    pageGrid: any;
    loadingIsDone = false;
    gridModel: TcrGridModel;
    onDataLoaded = (event) => { this.onLoadEvent(event); };
    selectedProjectId: any;
    subProjectPageGrid: any;
    subProjectGridModel: TcrGridModel;
    onDetailDataLoaded = (event) => { this.onDetailLoadEvent(event); };
    loopedElement: any;
    show: boolean;

    compareLoopedIds = (element) => {
        return this.compareLoopedElement(element);
    };

    //#endregion

    //#region Ctor

    constructor(private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private ProjectService: ProjectServiceModule.ProjectService,
        private Popups: any,
        private tcrGrid: TcrGridServiceModule.TcrGridService) {
        super($scope, Popups, $state);
        const self = this;
        self.show = false;
        self.pageGrid = new TcrGridServiceModule
            .TcrGridService("projectnumber", self.ProjectService.projectGrid, self.onDataLoaded, null, null, $state);
        self.pageGrid.loadGrid();
    }

    //#endregion

    private onLoadEvent(event: TcrGridModel): void {
        this.gridModel = event;
        if (this.gridModel.totalItems > 0) {
            this.setupDetailOnRows();
            this.loadingIsDone = true;
        }
    }

    private onDetailLoadEvent(event: TcrGridModel): void {
        this.subProjectGridModel = event;
    }

    newProject = () => {
        this.$state.transitionTo("mainState.maintenance.projectMaintenance.detail", { "id": "new" });
    };

    newSubProject = (projectId: string) => {
        this.$state.transitionTo("mainState.maintenance.projectMaintenance.subProjectDetail",
            { "id": projectId, "subProjectId": "new" });
    };

    toggleSubProjects = (project: any) => {
        const self = this;
        if (self.selectedProjectId == project.id) {
            self.selectedProjectId = null;
        } else {
            self.selectedProjectId = project.id;
            self.subProjectPageGrid = new TcrGridServiceModule.TcrGridService("projectname",
                self.ProjectService.subProjectGrid,
                self.onDetailDataLoaded,
                model => {
                    model.parentId = self.selectedProjectId;
                },
                null,
                self.$state);
            self.subProjectPageGrid.loadGrid();
        }
    };

    setupDetailOnRows = () => {
        var copyOfData = angular.copy(this.gridModel.data);
        for (let i = 0; i < copyOfData.length; i++) {
            this.loopedElement = copyOfData[i];
            let index = 1;
            for (let j = 0; j < this.gridModel.data.length; j++) {
                if (this.gridModel.data[j].id === this.loopedElement.id) {
                    break;
                }
                index++;
            }
            const object = { id: copyOfData[i].id, isDetail: true };
            this.gridModel.data.splice(index, 0, object);
        }
    };

    deleteProject = (project: any, index: number) => {
        this.Popups.confirmationDialog(this.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this project")
            .then(
                action => {
                    if (action) {
                        this.ProjectService.deleteProject(project.id)
                            .then(
                                result => {
                                    if (result == 0) {
                                        this.Popups.showError(this.$scope, "The Project could not be deleted as it has Timesheet entries assigned to it.", null, null);
                                    }
                                    else {
                                        this.gridModel.data.splice(index, 1);
                                    }
                                },
                                error => {
                                    this.handleError(error);
                                });
                    }

                },
                error => {
                    this.handleError(error);
                });

    };

    deleteSubProject = (subProject: any, index:number) => {
        this.Popups.confirmationDialog(this.$scope,
            "Are you sure you want to delete?",
            "You are about to delete this sub project")
            .then(
                action => {
                    if (action) {
                        this.ProjectService.deleteSubProject(subProject.id)
                            .then(
                                result => {
                                    if (result == 0) {
                                        this.Popups.showError(this.$scope, "The Sub Project could not be deleted as it has Timesheet entries assigned to it.", null, null);
                                    }
                                    else {
                                        this.subProjectGridModel.data.splice(index, 1);
                                    }
                                },
                                error => {
                                    this.handleError(error);
                                });
                    }

                },
                error => {
                    this.handleError(error);
                });
    };

    private compareLoopedElement(element) {
        return this.loopedElement.id === element.id;
    }

    toggleInactiveProjectShow = () => {
        const self = this;
        this.pageGrid.loadGrid();
    };



}

angular.module("AngularApp")
    .controller("ProjectMaintenanceGridController",
        [
            "$scope",
            "$state",
            "ProjectService",
            "Popups",
            ProjectMaintenanceGridController
        ]);