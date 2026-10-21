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
    roleDefaults: any[] = [];
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
        if (tab === "people") {
            if (!this.roleDefaults.length)
                this.loadRoleDefaults();
            if (!this.members)
                this.loadMembers();
        }
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
        const p = this.permissions;
        if (!p)
            return false;
        if (member.role === 0)
            return p.canManageManagers;
        if (!p.canManagePeople)
            return false;
        return member.role === 2 || p.isAdmin || p.isManager;
    };

    loadRoleDefaults = (): void => {
        const self = this;
        self.WorkTeamService.rolePrivilegeDefaults()
            .then(
                result => {
                    self.roleDefaults = result || [];
                    if (self.memberEdit)
                        self.refreshPrivilegeDefaults();
                },
                error => {
                    self.handleError(error);
                });
    };

    privilegeRows = (role: number, existing: any[]): any[] => {
        const names = [
            { privilege: 0, name: "Capture time" },
            { privilege: 1, name: "View/Create my team's timesheets" },
            { privilege: 2, name: "View/Create my team's scorecards" },
            { privilege: 3, name: "Maintain client" },
            { privilege: 4, name: "Manage team rates" },
            { privilege: 5, name: "Manage team members" }
        ];
        return names.map(item => {
            const roleDefault = (this.roleDefaults || []).filter(d => d.role === role && d.privilege === item.privilege)[0];
            const current = (existing || []).filter(row => row.privilege === item.privilege)[0];
            let choice = "default";
            if (current && current.override === true)
                choice = "allow";
            else if (current && current.override === false)
                choice = "deny";
            else if (current && current.choice)
                choice = current.choice;
            return {
                privilege: item.privilege,
                name: item.name,
                roleDefault: !!(roleDefault && roleDefault.isGranted),
                choice: choice
            };
        });
    };

    refreshPrivilegeDefaults = (): void => {
        if (!this.memberEdit)
            return;
        this.memberEdit.privileges = this.privilegeRows(this.memberEdit.role, this.memberEdit.privileges);
    };

    newMember = (): void => {
        const roles = this.editableRoles();
        const role = roles.length ? roles[roles.length - 1].id : 2;
        this.memberEdit = {
            id: null,
            workTeamId: this.teamId,
            userAccountId: null,
            role: role,
            startDate: new Date(),
            endDate: null,
            privileges: this.privilegeRows(role, [])
        };
        if (!this.roleDefaults.length)
            this.loadRoleDefaults();
    };

    editMember = (member: any): void => {
        this.memberEdit = angular.copy(member);
        this.memberEdit.startDate = member.startDate ? new Date(member.startDate) : null;
        this.memberEdit.endDate = member.endDate ? new Date(member.endDate) : null;
        this.memberEdit.privileges = this.privilegeRows(member.role, member.privileges || []);
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
        model.privileges = (self.memberEdit.privileges || []).map(row => ({
            privilege: row.privilege,
            override: row.choice === "allow" ? true : row.choice === "deny" ? false : null
        }));

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
     * Team reach: a manager reaches leads and members, a lead or member reaches members, and nobody
     * reaches themselves. Administrators use their global privileges and can open a manager's row too.
     */
    canActOn = (member: any, capability: string): boolean => {
        const p = this.permissions;
        if (!p || !p[capability])
            return false;
        if (member.userAccountId === this.SecurityService.getCurrentUserDetails().id)
            return false;
        if (p.isAdmin)
            return true;
        if (member.role === 0)
            return false;
        return member.role === 2 || p.isManager;
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
