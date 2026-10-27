class WorkGroupMaintenanceGridController extends CHControllerBase {

    tree: any = { isAdmin: false, groups: [] };
    clients: any[] = [];
    users: any[] = [];
    teamTypes: any[] = [];
    loading = false;
    loaded = false;
    searchText = "";
    statusFilter = "active";
    summary: any = { groups: 0, teams: 0, people: 0 };
    drawer: any = { open: false, kind: null, isNew: false, saving: false, submitted: false };

    constructor(
        private $scope: ng.IScope,
        private $state: ng.ui.IStateService,
        private WorkGroupService: WorkGroupServiceModule.WorkGroupService,
        private ClientService: ClientServiceModule.ClientService,
        private UserService: UserServiceModule.UserService,
        private TeamService: TeamServiceModule.TeamService,
        private Popups: any) {
        super($scope, Popups, $state);
        this.loadLookups();
        this.loadTree();
    }

    loadLookups = (): void => {
        const me = this;
        me.ClientService.clientDropdownList().then(result => { me.clients = result || []; }, error => me.handleError(error));
        me.UserService.userDropdownList().then(result => { me.users = result || []; }, error => me.handleError(error));
        me.TeamService.teamDropdownList().then(result => { me.teamTypes = result || []; }, error => me.handleError(error));
    };

    loadTree = (reopenTeamId?: string): void => {
        const me = this;
        me.loading = true;
        me.WorkGroupService.tree().then(
            result => {
                me.tree = result || { isAdmin: false, groups: [] };
                if (!me.tree.groups) me.tree.groups = [];
                me.recount();
                me.loading = false;
                me.loaded = true;
                if (reopenTeamId)
                    me.reopenTeam(reopenTeamId);
            },
            error => {
                me.loading = false;
                me.loaded = true;
                me.handleError(error);
            });
    };

    recount = (): void => {
        const groups = this.tree.groups || [];
        let teams = 0;
        let people = 0;
        groups.forEach(g => {
            (g.managers || []).forEach(() => { people += 1; });
            (g.teams || []).forEach(t => {
                teams += 1;
                people += (t.leads || []).length + (t.members || []).length;
            });
        });
        this.summary = { groups: groups.length, teams: teams, people: people };
    };

    clientNames = (group: any): string => {
        return ((group && group.clients) || []).map(c => c.clientName).filter(n => !!n).join(", ");
    };

    filteredGroups = (): any[] => {
        const q = (this.searchText || "").trim().toLowerCase();
        return (this.tree.groups || []).filter(g => {
            if (this.statusFilter === "active" && !g.isActive)
                return false;
            if (!q)
                return true;
            const haystack = [g.name, this.clientNames(g), g.description]
                .concat((g.managers || []).map(p => p.name))
                .concat((g.teams || []).map(t => t.teamTypeName + " " + t.label))
                .concat(this.peopleNames(g));
            return haystack.join(" ").toLowerCase().indexOf(q) >= 0;
        });
    };

    peopleNames = (group: any): string[] => {
        const names = [];
        (group.teams || []).forEach(t => {
            (t.leads || []).concat(t.members || []).forEach(p => names.push(p.name));
        });
        return names;
    };

    availableClients = (): any[] => {
        const used = (this.drawer.clients || []).map(c => c.clientId);
        return (this.clients || []).filter(c => used.indexOf(c.id) < 0);
    };

    emptyPersonFlags = (): any => {
        return {
            isHomeTeam: false,
            canView: true
        };
    };

    newGroup = (): void => {
        this.drawer = {
            open: true,
            kind: "group",
            isNew: true,
            saving: false,
            submitted: false,
            kicker: "New work group",
            title: "Work group",
            group: { name: "", description: "", isActive: true, canEdit: true, canDelete: false },
            clients: [],
            clientId: null,
            managers: [],
            managerUserId: null,
            managerFlags: this.emptyPersonFlags()
        };
    };

    openGroup = (group: any): void => {
        this.drawer = {
            open: true,
            kind: "group",
            isNew: false,
            saving: false,
            submitted: false,
            kicker: this.clientNames(group) || "Work group",
            title: group.name,
            group: {
                id: group.id,
                name: group.name,
                description: group.description,
                isActive: group.isActive,
                canEdit: group.canEdit,
                canDelete: group.canDelete
            },
            clients: (group.clients || []).map(c => ({
                clientId: c.clientId,
                clientName: c.clientName
            })),
            clientId: null,
            managers: (group.managers || []).map(p => ({
                userAccountId: p.userAccountId,
                name: p.name,
                canView: p.canView !== false
            })),
            managerUserId: null,
            managerFlags: this.emptyPersonFlags()
        };
    };

    newTeam = (group: any): void => {
        this.drawer = {
            open: true,
            kind: "team",
            isNew: true,
            saving: false,
            submitted: false,
            kicker: group.name,
            title: "New team",
            groupId: group.id,
            usedTypeIds: (group.teams || []).map(t => t.teamTypeId),
            team: { teamTypeId: null, isActive: true, canEdit: true, canDelete: false }
        };
    };

    openTeam = (group: any, team: any): void => {
        this.drawer = {
            open: true,
            kind: "team",
            isNew: false,
            saving: false,
            submitted: false,
            kicker: group.name,
            title: team.teamTypeName,
            groupId: group.id,
            team: angular.copy(team),
            leadUserId: null,
            memberUserId: null,
            leadFlags: this.emptyPersonFlags(),
            memberFlags: this.emptyPersonFlags()
        };
    };

    reopenTeam = (teamId: string): void => {
        const groups = this.tree.groups || [];
        for (let i = 0; i < groups.length; i++) {
            const team = (groups[i].teams || []).filter(t => t.id === teamId)[0];
            if (team) {
                this.openTeam(groups[i], team);
                return;
            }
        }
    };

    availableTeamTypes = (): any[] => {
        const used = this.drawer.usedTypeIds || [];
        return (this.teamTypes || []).filter(t => used.indexOf(t.id) < 0);
    };

    addClient = (): void => {
        const id = this.drawer.clientId;
        if (!id)
            return;
        if ((this.drawer.clients || []).some(c => c.clientId === id))
            return;
        const client = (this.clients || []).filter(c => c.id === id)[0];
        if (!client)
            return;
        this.drawer.clients.push({
            clientId: id,
            clientName: client.entityName
        });
        this.drawer.clientId = null;
    };

    removeClient = (client: any): void => {
        this.drawer.clients = (this.drawer.clients || []).filter(c => c.clientId !== client.clientId);
    };

    addManager = (): void => {
        const id = this.drawer.managerUserId;
        if (!id)
            return;
        if ((this.drawer.managers || []).some(p => p.userAccountId === id))
            return;
        const user = (this.users || []).filter(u => u.id === id)[0];
        if (!user)
            return;
        const first = (user.firstname || "").trim();
        const last = (user.surname || "").trim();
        const flags = this.drawer.managerFlags || this.emptyPersonFlags();
        this.drawer.managers.push({
            userAccountId: id,
            name: user.description || (first + " " + last).trim(),
            canView: flags.canView !== false
        });
        this.drawer.managerUserId = null;
        this.drawer.managerFlags = this.emptyPersonFlags();
    };

    removeManager = (person: any): void => {
        this.drawer.managers = (this.drawer.managers || []).filter(p => p.userAccountId !== person.userAccountId);
    };

    canSaveDrawer = (): boolean => {
        if (!this.drawer.open)
            return false;
        if (this.drawer.kind === "group")
            return !!this.drawer.group && this.drawer.group.canEdit;
        return this.drawer.kind === "team" && !!this.drawer.isNew;
    };

    saveButtonLabel = (): string => {
        if (this.drawer.kind === "team" && this.drawer.isNew)
            return "Create team";
        return "Save";
    };

    canDeleteDrawer = (): boolean => {
        if (this.drawer.kind === "group")
            return !!this.drawer.group && this.drawer.group.canDelete;
        return this.drawer.kind === "team" && !this.drawer.isNew && !!this.drawer.team && this.drawer.team.canDelete;
    };

    saveDrawer = (): void => {
        const me = this;
        me.drawer.submitted = true;
        if (me.drawer.kind === "group")
            me.saveGroup();
        else if (me.drawer.kind === "team" && me.drawer.isNew)
            me.saveTeam();
    };

    saveGroup = (): void => {
        const me = this;
        const group = me.drawer.group;
        if (!group.name || !(me.drawer.clients || []).length) {
            me.handleError("A work group needs a name and at least one client.");
            return;
        }
        me.drawer.saving = true;
        me.WorkGroupService.saveGroup({
            id: group.id || null,
            name: group.name,
            description: group.description,
            clientIds: (me.drawer.clients || []).map(c => c.clientId),
            isActive: group.isActive,
            managers: (me.drawer.managers || []).map(p => ({
                userAccountId: p.userAccountId,
                canView: p.canView !== false
            }))
        }).then(
            () => {
                me.drawer.saving = false;
                me.closeDrawer();
                me.loadTree();
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    saveTeam = (): void => {
        const me = this;
        const team = me.drawer.team;
        if (!team.teamTypeId) {
            me.handleError("Choose a team type.");
            return;
        }
        me.drawer.saving = true;
        me.WorkGroupService.saveTeam({
            id: null,
            workGroupId: me.drawer.groupId,
            teamTypeId: team.teamTypeId,
            isActive: team.isActive
        }).then(
            id => {
                me.drawer.saving = false;
                me.loadTree(id);
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    setTeamActive = (isActive: boolean): void => {
        const me = this;
        const team = me.drawer.team;
        if (!team || !team.id || !team.canEdit || team.isActive === isActive)
            return;
        const previous = team.isActive;
        team.isActive = isActive;
        me.drawer.saving = true;
        me.WorkGroupService.saveTeam({
            id: team.id,
            workGroupId: me.drawer.groupId,
            teamTypeId: team.teamTypeId,
            isActive: isActive
        }).then(
            () => {
                me.drawer.saving = false;
                me.loadTree(team.id);
            },
            error => {
                team.isActive = previous;
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    addPerson = (role: number): void => {
        const me = this;
        const userId = role === 0 ? me.drawer.leadUserId : me.drawer.memberUserId;
        const flags = role === 0
            ? (me.drawer.leadFlags || me.emptyPersonFlags())
            : (me.drawer.memberFlags || me.emptyPersonFlags());
        if (!me.drawer.team || !me.drawer.team.id) {
            me.handleError("Create the team first, then add people.");
            return;
        }
        if (!userId) {
            me.handleError(role === 0 ? "Choose a lead to add." : "Choose a member to add.");
            return;
        }
        me.drawer.saving = true;
        me.WorkGroupService.savePerson({
            workTeamId: me.drawer.team.id,
            userAccountId: userId,
            role: role,
            isHomeTeam: !!flags.isHomeTeam,
            canView: flags.canView !== false
        }).then(
            () => {
                me.drawer.saving = false;
                if (role === 0) {
                    me.drawer.leadUserId = null;
                    me.drawer.leadFlags = me.emptyPersonFlags();
                } else {
                    me.drawer.memberUserId = null;
                    me.drawer.memberFlags = me.emptyPersonFlags();
                }
                me.loadTree(me.drawer.team.id);
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    updatePersonFlags = (person: any, role: number): void => {
        const me = this;
        if (!person || !me.drawer.team)
            return;
        me.drawer.saving = true;
        me.WorkGroupService.savePerson({
            workTeamId: me.drawer.team.id,
            userAccountId: person.userAccountId,
            role: role,
            isHomeTeam: !!person.isHomeTeam,
            canView: person.canView !== false
        }).then(
            () => {
                me.drawer.saving = false;
                me.loadTree(me.drawer.team.id);
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    removePerson = (person: any): void => {
        const me = this;
        me.drawer.saving = true;
        me.WorkGroupService.deletePerson(person.id).then(
            () => {
                me.drawer.saving = false;
                me.loadTree(me.drawer.team.id);
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    deleteDrawer = (): void => {
        const me = this;
        const removingGroup = me.drawer.kind === "group";
        const call = removingGroup
            ? me.WorkGroupService.deleteGroup(me.drawer.group.id)
            : me.WorkGroupService.deleteTeam(me.drawer.team.id);
        me.drawer.saving = true;
        call.then(
            () => {
                me.drawer.saving = false;
                me.closeDrawer();
                me.loadTree();
            },
            error => {
                me.drawer.saving = false;
                me.handleError(error);
            });
    };

    closeDrawer = (): void => {
        this.drawer.open = false;
    };
}

angular.module("AngularApp")
    .controller("WorkGroupMaintenanceGridController", [
        "$scope",
        "$state",
        "WorkGroupService",
        "ClientService",
        "UserService",
        "TeamService",
        "Popups",
        WorkGroupMaintenanceGridController
    ]);
