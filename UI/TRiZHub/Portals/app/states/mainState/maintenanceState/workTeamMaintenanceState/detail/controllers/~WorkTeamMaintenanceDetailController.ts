class WorkTeamMaintenanceDetailController extends CHControllerBase {

    //#region members

    saveSuccess = false;
    successMessage = "Saved Successfully";
    tab = "details";
    teamId: string;
    viewModel: any;
    permissions: any;
    teamTypes: any;
    users: any;
    members: any;
    includeEnded = false;
    memberEdit: any;
    treeData: any;
    includeInactive = false;
    memberRates: any;
    roles = [
        { id: 0, name: "Manager" },
        { id: 1, name: "Lead" },
        { id: 2, name: "Member" }
    ];

    //#endregion

    //#region Ctor
    constructor(
        private $stateParams: ng.ui.IStateParamsService,
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private $timeout: ng.ITimeoutService,
        private WorkTeamService: WorkTeamServiceModule.WorkTeamService,
        private TeamService: TeamServiceModule.TeamService,
        private UserService: UserServiceModule.UserService,
        private SecurityService: SecurityServiceModule.SecurityService,
        private Popups: any,
        private BillingRatesService: BillingRatesServiceModule.BillingRatesService) {
        super($scope, Popups, $state);
        const self = this;
        self.teamId = self.$stateParams["id"];
        self.viewModel = { isActive: true };

        TeamService.teamDropdownList()
            .then(
                result => {
                    self.teamTypes = result;
                },
                error => {
                    self.handleError(error);
                });

        if (self.teamId !== "new") {
            self.loadTeam();
        } else {
            self.teamId = null;
            const isAdmin = SecurityService.isAllowed("TeamMaintenance");
            self.permissions = {
                canView: isAdmin,
                isAdmin: isAdmin,
                canEditHeader: isAdmin,
                canEditAdminFlags: isAdmin,
                canEditLeadFlags: isAdmin
            };
        }
    }

    //#endregion

    //#region Details

    loadTeam = (): void => {
        const self = this;
        self.WorkTeamService.getWorkTeam(self.teamId)
            .then(
                result => {
                    self.viewModel = result;
                    self.permissions = result.permissions || {};
                    const tab = self.$stateParams["tab"];
                    if (tab && self.tab !== tab)
                        self.selectTab(tab);
                    const ratesFor = self.$stateParams["ratesFor"];
                    if (ratesFor && self.permissions.canRates && !self.memberRates)
                        self.loadMemberRates(ratesFor);
                    if (self.permissions.canManagePeople && !self.users) {
                        self.UserService.userDropdownList()
                            .then(
                                users => {
                                    self.users = users;
                                },
                                error => {
                                    self.handleError(error);
                                });
                    }
                },
                error => {
                    self.handleError(error);
                });
    };

    selectTab = (tab: string): void => {
        this.tab = tab;
        if (tab === "people" && !this.members)
            this.loadMembers();
        if (tab === "allocations" && !this.treeData)
            this.loadTree();
    };

    submitForm = (): void => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["EditForm"].$invalid)
            return;

        const isNew = !self.teamId;
        self.WorkTeamService.saveWorkTeam(self.viewModel)
            .then(
                result => {
                    self.flashSuccess();
                    if (isNew) {
                        self.$state.transitionTo("mainState.maintenance.workTeamMaintenance.detail", { "id": result.id });
                        return;
                    }
                    self.viewModel = result;
                    self.permissions = result.permissions || {};
                },
                error => {
                    self.handleError(error);
                });
    };

    flashSuccess = (): void => {
        const self = this;
        self.saveSuccess = true;
        self.$timeout(() => {
            self.saveSuccess = false;
        }, 3000);
    };

    //#endregion

    //#region People

    loadMembers = (): void => {
        const self = this;
        self.WorkTeamService.workTeamMembers(self.teamId, self.includeEnded)
            .then(
                result => {
                    self.members = result;
                },
                error => {
                    self.handleError(error);
                });
    };

    toggleIncludeEnded = (): void => {
        this.loadMembers();
    };

    editableRoles = (): any[] => {
        const self = this;
        return self.roles.filter(r => r.id === 0 ? self.permissions.canManageManagers : self.permissions.canManagePeople);
    };

    canEditMember = (member: any): boolean => {
        if (!this.permissions)
            return false;
        return member.role === 0 ? this.permissions.canManageManagers : this.permissions.canManagePeople;
    };

    newMember = (): void => {
        const roles = this.editableRoles();
        this.memberEdit = {
            id: null,
            workTeamId: this.teamId,
            userAccountId: null,
            role: roles.length ? roles[roles.length - 1].id : 2,
            startDate: new Date(),
            endDate: null
        };
    };

    editMember = (member: any): void => {
        this.memberEdit = angular.copy(member);
        this.memberEdit.startDate = member.startDate ? new Date(member.startDate) : null;
        this.memberEdit.endDate = member.endDate ? new Date(member.endDate) : null;
    };

    endMemberToday = (member: any): void => {
        this.editMember(member);
        this.memberEdit.endDate = new Date();
    };

    cancelMember = (): void => {
        this.memberEdit = null;
    };

    saveMember = (): void => {
        const self = this;
        self.$scope.$broadcast("show-errors-check-validity");
        if (self.$scope["MemberForm"] && self.$scope["MemberForm"].$invalid)
            return;

        const model = angular.copy(self.memberEdit);
        model.startDate = self.getBasic(self.memberEdit.startDate);
        model.endDate = self.memberEdit.endDate ? self.getBasic(self.memberEdit.endDate) : null;

        self.WorkTeamService.saveWorkTeamMember(model)
            .then(
                () => {
                    self.memberEdit = null;
                    self.flashSuccess();
                    self.loadMembers();
                },
                error => {
                    self.handleError(error);
                });
    };

    deleteMember = (member: any): void => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope, "Remove membership",
            "This deletes the membership history for " + member.userName + ". To keep history, set an end date instead. Continue?")
            .then(result => {
                if (!result)
                    return;
                self.WorkTeamService.deleteWorkTeamMember(member.id)
                    .then(
                        () => {
                            self.loadMembers();
                        },
                        error => {
                            self.handleError(error);
                        });
            });
    };

    /**
     * Mirrors the server reach rule for row actions: managers reach leads and members, leads reach members,
     * nobody reaches themselves. The server re-checks dates and flags.
     */
    canActOn = (member: any, capability: string): boolean => {
        const p = this.permissions;
        if (!p || !p[capability] || member.role === 0)
            return false;
        if (member.userAccountId === this.SecurityService.getCurrentUserDetails().id)
            return false;
        return member.role === 2 || p.isManager || p.isAdmin;
    };

    openTimesheets = (member: any): void => {
        this.$state.go("mainState.timesheet", { userId: member.userAccountId });
    };

    openScorecards = (): void => {
        this.$state.go("mainState.scorecard.grid");
    };

    loadMemberRates = (userId: string): void => {
        const self = this;
        self.BillingRatesService.workTeamMemberRates(self.teamId, userId)
            .then(
                result => {
                    self.memberRates = result;
                },
                error => {
                    self.memberRates = null;
                    self.handleError(error);
                });
    };

    closeMemberRates = (): void => {
        this.memberRates = null;
    };

    editRate = (rate: any): void => {
        this.$state.go("mainState.maintenance.userMaintenance.billingRatesDetail",
            { userid: this.memberRates.userAccountId, id: rate.id, workTeamId: this.teamId });
    };

    addRate = (): void => {
        this.$state.go("mainState.maintenance.userMaintenance.billingRatesDetail",
            { userid: this.memberRates.userAccountId, id: "new", workTeamId: this.teamId });
    };

    deleteRate = (rate: any): void => {
        const self = this;
        self.Popups.confirmationDialog(self.$scope, "Delete rate",
            "Delete this " + rate.scope.toLowerCase() + " rate for " + self.memberRates.userName + "?")
            .then(result => {
                if (!result)
                    return;
                self.BillingRatesService.billingRatesDelete({ id: rate.id })
                    .then(
                        () => {
                            self.loadMemberRates(self.memberRates.userAccountId);
                        },
                        error => {
                            self.handleError(error);
                        });
            });
    };

    //#endregion

    //#region Allocations

    loadTree = (): void => {
        const self = this;
        self.WorkTeamService.workTeamAllocationTree(self.teamId, self.includeInactive)
            .then(
                result => {
                    self.treeData = result;
                    self.setCollapsedValue(self.treeData, true);
                },
                error => {
                    self.handleError(error);
                });
    };

    toggleInactiveProjectShow = (): void => {
        this.loadTree();
    };

    setCollapsedValue = (nodes: any[], value: boolean): void => {
        if (nodes != null && nodes.length > 0) {
            for (let i = 0; i < nodes.length; i++) {
                nodes[i].collapsed = value;
                this.setCollapsedValue(nodes[i].listOfProjects, value);
            }
        }
    };

    selectAllNone = (node: any, selected: boolean): void => {
        this.updateChildren(node.listOfProjects, selected);
    };

    selectChange = (node: any): void => {
        this.updateChildren(node.listOfProjects, false);
    };

    updateChildren = (nodes: any[], value: boolean): void => {
        if (nodes != null && nodes.length > 0) {
            for (let i = 0; i < nodes.length; i++) {
                nodes[i].selected = value;
                this.updateChildren(nodes[i].listOfProjects, value);
            }
        }
    };

    getClientCounts = (node: any, type: string): string => {
        const totalProjects = node.listOfProjects.length;
        let totalSubProjects = 0;
        let selProjects = 0;
        let selSubProjects = 0;

        for (let i = 0; i < node.listOfProjects.length; i++) {
            const proj = node.listOfProjects[i];
            if (proj.selected) selProjects++;
            totalSubProjects += proj.listOfProjects.length;
            for (let j = 0; j < proj.listOfProjects.length; j++) {
                if (proj.listOfProjects[j].selected || proj.selected) selSubProjects++;
            }
        }

        return type === "Project"
            ? selProjects + "/" + totalProjects
            : selSubProjects + "/" + totalSubProjects;
    };

    getProjectCounts = (node: any): string => {
        if (node.selected) return "Entire Project Selected";

        let selSubProjects = 0;
        for (let i = 0; i < node.listOfProjects.length; i++) {
            if (node.listOfProjects[i].selected) selSubProjects++;
        }
        return "Sub-Projects (" + selSubProjects + "/" + node.listOfProjects.length + ")";
    };

    flatten = (nodes: any[]): any[] => {
        let result = [];
        for (let i = 0; i < nodes.length; i++) {
            result.push(nodes[i]);
            if (nodes[i].listOfProjects && nodes[i].listOfProjects.length)
                result = result.concat(this.flatten(nodes[i].listOfProjects));
        }
        return result;
    };

    saveAllocations = (): void => {
        const self = this;
        const selection = self.flatten(self.treeData || [])
            .filter(p => p.selected)
            .map(p => ({ clientId: p.clientId, projectId: p.projectId, subProjectId: p.subProjectId }));

        self.WorkTeamService.saveWorkTeamAllocations(self.teamId, selection)
            .then(
                () => {
                    self.flashSuccess();
                },
                error => {
                    self.handleError(error);
                });
    };

    //#endregion

    getBasic = (date) => {
        let dateFormat = new Date(date);
        dateFormat.setMinutes(dateFormat.getMinutes() - dateFormat.getTimezoneOffset());
        return dateFormat.toUTCString();
    };
}

angular.module("AngularApp")
    .controller("WorkTeamMaintenanceDetailController",
    [
        "$stateParams",
        "$scope",
        "$state",
        "$timeout",
        "WorkTeamService",
        "TeamService",
        "UserService",
        "SecurityService",
        "Popups",
        "BillingRatesService",
        WorkTeamMaintenanceDetailController
    ]);
