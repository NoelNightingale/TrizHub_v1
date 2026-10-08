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
var WorkTeamMaintenanceDetailController = /** @class */ (function (_super) {
    __extends(WorkTeamMaintenanceDetailController, _super);
    //#endregion
    //#region Ctor
    function WorkTeamMaintenanceDetailController($stateParams, $scope, $state, $timeout, WorkTeamService, TeamService, UserService, SecurityService, Popups, BillingRatesService) {
        var _this = _super.call(this, $scope, Popups, $state) || this;
        _this.$stateParams = $stateParams;
        _this.$scope = $scope;
        _this.$state = $state;
        _this.$timeout = $timeout;
        _this.WorkTeamService = WorkTeamService;
        _this.TeamService = TeamService;
        _this.UserService = UserService;
        _this.SecurityService = SecurityService;
        _this.Popups = Popups;
        _this.BillingRatesService = BillingRatesService;
        //#region members
        _this.saveSuccess = false;
        _this.successMessage = "Saved Successfully";
        _this.tab = "details";
        _this.includeEnded = false;
        _this.includeInactive = false;
        _this.roles = [
            { id: 0, name: "Manager" },
            { id: 1, name: "Lead" },
            { id: 2, name: "Member" }
        ];
        //#endregion
        //#region Details
        _this.loadTeam = function () {
            var self = _this;
            self.WorkTeamService.getWorkTeam(self.teamId)
                .then(function (result) {
                self.viewModel = result;
                self.permissions = result.permissions || {};
                var tab = self.$stateParams["tab"];
                if (tab && self.tab !== tab)
                    self.selectTab(tab);
                var ratesFor = self.$stateParams["ratesFor"];
                if (ratesFor && self.permissions.canRates && !self.memberRates)
                    self.loadMemberRates(ratesFor);
                if (self.permissions.canManagePeople && !self.users) {
                    self.UserService.userDropdownList()
                        .then(function (users) {
                        self.users = users;
                    }, function (error) {
                        self.handleError(error);
                    });
                }
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.selectTab = function (tab) {
            _this.tab = tab;
            if (tab === "people" && !_this.members)
                _this.loadMembers();
            if (tab === "allocations" && !_this.treeData)
                _this.loadTree();
        };
        _this.submitForm = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["EditForm"].$invalid)
                return;
            var isNew = !self.teamId;
            self.WorkTeamService.saveWorkTeam(self.viewModel)
                .then(function (result) {
                self.flashSuccess();
                if (isNew) {
                    self.$state.transitionTo("mainState.maintenance.workTeamMaintenance.detail", { "id": result.id });
                    return;
                }
                self.viewModel = result;
                self.permissions = result.permissions || {};
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.flashSuccess = function () {
            var self = _this;
            self.saveSuccess = true;
            self.$timeout(function () {
                self.saveSuccess = false;
            }, 3000);
        };
        //#endregion
        //#region People
        _this.loadMembers = function () {
            var self = _this;
            self.WorkTeamService.workTeamMembers(self.teamId, self.includeEnded)
                .then(function (result) {
                self.members = result;
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.toggleIncludeEnded = function () {
            _this.loadMembers();
        };
        _this.editableRoles = function () {
            var self = _this;
            return self.roles.filter(function (r) { return r.id === 0 ? self.permissions.canManageManagers : self.permissions.canManagePeople; });
        };
        _this.canEditMember = function (member) {
            if (!_this.permissions)
                return false;
            return member.role === 0 ? _this.permissions.canManageManagers : _this.permissions.canManagePeople;
        };
        _this.newMember = function () {
            var roles = _this.editableRoles();
            _this.memberEdit = {
                id: null,
                workTeamId: _this.teamId,
                userAccountId: null,
                role: roles.length ? roles[roles.length - 1].id : 2,
                startDate: new Date(),
                endDate: null
            };
        };
        _this.editMember = function (member) {
            _this.memberEdit = angular.copy(member);
            _this.memberEdit.startDate = member.startDate ? new Date(member.startDate) : null;
            _this.memberEdit.endDate = member.endDate ? new Date(member.endDate) : null;
        };
        _this.endMemberToday = function (member) {
            _this.editMember(member);
            _this.memberEdit.endDate = new Date();
        };
        _this.cancelMember = function () {
            _this.memberEdit = null;
        };
        _this.saveMember = function () {
            var self = _this;
            self.$scope.$broadcast("show-errors-check-validity");
            if (self.$scope["MemberForm"] && self.$scope["MemberForm"].$invalid)
                return;
            var model = angular.copy(self.memberEdit);
            model.startDate = self.getBasic(self.memberEdit.startDate);
            model.endDate = self.memberEdit.endDate ? self.getBasic(self.memberEdit.endDate) : null;
            self.WorkTeamService.saveWorkTeamMember(model)
                .then(function () {
                self.memberEdit = null;
                self.flashSuccess();
                self.loadMembers();
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.deleteMember = function (member) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Remove membership", "This deletes the membership history for " + member.userName + ". To keep history, set an end date instead. Continue?")
                .then(function (result) {
                if (!result)
                    return;
                self.WorkTeamService.deleteWorkTeamMember(member.id)
                    .then(function () {
                    self.loadMembers();
                }, function (error) {
                    self.handleError(error);
                });
            });
        };
        /**
         * Mirrors the server reach rule for row actions: managers reach leads and members, leads reach members,
         * nobody reaches themselves. The server re-checks dates and flags.
         */
        _this.canActOn = function (member, capability) {
            var p = _this.permissions;
            if (!p || !p[capability] || member.role === 0)
                return false;
            if (member.userAccountId === _this.SecurityService.getCurrentUserDetails().id)
                return false;
            return member.role === 2 || p.isManager || p.isAdmin;
        };
        _this.openTimesheets = function (member) {
            _this.$state.go("mainState.timesheet", { userId: member.userAccountId });
        };
        _this.openScorecards = function () {
            _this.$state.go("mainState.scorecard.grid");
        };
        _this.loadMemberRates = function (userId) {
            var self = _this;
            self.BillingRatesService.workTeamMemberRates(self.teamId, userId)
                .then(function (result) {
                self.memberRates = result;
            }, function (error) {
                self.memberRates = null;
                self.handleError(error);
            });
        };
        _this.closeMemberRates = function () {
            _this.memberRates = null;
        };
        _this.editRate = function (rate) {
            _this.$state.go("mainState.maintenance.userMaintenance.billingRatesDetail", { userid: _this.memberRates.userAccountId, id: rate.id, workTeamId: _this.teamId });
        };
        _this.addRate = function () {
            _this.$state.go("mainState.maintenance.userMaintenance.billingRatesDetail", { userid: _this.memberRates.userAccountId, id: "new", workTeamId: _this.teamId });
        };
        _this.deleteRate = function (rate) {
            var self = _this;
            self.Popups.confirmationDialog(self.$scope, "Delete rate", "Delete this " + rate.scope.toLowerCase() + " rate for " + self.memberRates.userName + "?")
                .then(function (result) {
                if (!result)
                    return;
                self.BillingRatesService.billingRatesDelete({ id: rate.id })
                    .then(function () {
                    self.loadMemberRates(self.memberRates.userAccountId);
                }, function (error) {
                    self.handleError(error);
                });
            });
        };
        //#endregion
        //#region Allocations
        _this.loadTree = function () {
            var self = _this;
            self.WorkTeamService.workTeamAllocationTree(self.teamId, self.includeInactive)
                .then(function (result) {
                self.treeData = result;
                self.setCollapsedValue(self.treeData, true);
            }, function (error) {
                self.handleError(error);
            });
        };
        _this.toggleInactiveProjectShow = function () {
            _this.loadTree();
        };
        _this.setCollapsedValue = function (nodes, value) {
            if (nodes != null && nodes.length > 0) {
                for (var i = 0; i < nodes.length; i++) {
                    nodes[i].collapsed = value;
                    _this.setCollapsedValue(nodes[i].listOfProjects, value);
                }
            }
        };
        _this.selectAllNone = function (node, selected) {
            _this.updateChildren(node.listOfProjects, selected);
        };
        _this.selectChange = function (node) {
            _this.updateChildren(node.listOfProjects, false);
        };
        _this.updateChildren = function (nodes, value) {
            if (nodes != null && nodes.length > 0) {
                for (var i = 0; i < nodes.length; i++) {
                    nodes[i].selected = value;
                    _this.updateChildren(nodes[i].listOfProjects, value);
                }
            }
        };
        _this.getClientCounts = function (node, type) {
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
                    if (proj.listOfProjects[j].selected || proj.selected)
                        selSubProjects++;
                }
            }
            return type === "Project"
                ? selProjects + "/" + totalProjects
                : selSubProjects + "/" + totalSubProjects;
        };
        _this.getProjectCounts = function (node) {
            if (node.selected)
                return "Entire Project Selected";
            var selSubProjects = 0;
            for (var i = 0; i < node.listOfProjects.length; i++) {
                if (node.listOfProjects[i].selected)
                    selSubProjects++;
            }
            return "Sub-Projects (" + selSubProjects + "/" + node.listOfProjects.length + ")";
        };
        _this.flatten = function (nodes) {
            var result = [];
            for (var i = 0; i < nodes.length; i++) {
                result.push(nodes[i]);
                if (nodes[i].listOfProjects && nodes[i].listOfProjects.length)
                    result = result.concat(_this.flatten(nodes[i].listOfProjects));
            }
            return result;
        };
        _this.saveAllocations = function () {
            var self = _this;
            var selection = self.flatten(self.treeData || [])
                .filter(function (p) { return p.selected; })
                .map(function (p) { return ({ clientId: p.clientId, projectId: p.projectId, subProjectId: p.subProjectId }); });
            self.WorkTeamService.saveWorkTeamAllocations(self.teamId, selection)
                .then(function () {
                self.flashSuccess();
            }, function (error) {
                self.handleError(error);
            });
        };
        //#endregion
        _this.getBasic = function (date) {
            var dateFormat = new Date(date);
            dateFormat.setMinutes(dateFormat.getMinutes() - dateFormat.getTimezoneOffset());
            return dateFormat.toUTCString();
        };
        var self = _this;
        self.teamId = self.$stateParams["id"];
        self.viewModel = { isActive: true };
        TeamService.teamDropdownList()
            .then(function (result) {
            self.teamTypes = result;
        }, function (error) {
            self.handleError(error);
        });
        if (self.teamId !== "new") {
            self.loadTeam();
        }
        else {
            self.teamId = null;
            var isAdmin = SecurityService.isAllowed("TeamMaintenance");
            self.permissions = {
                canView: isAdmin,
                isAdmin: isAdmin,
                canEditHeader: isAdmin,
                canEditAdminFlags: isAdmin,
                canEditLeadFlags: isAdmin
            };
        }
        return _this;
    }
    return WorkTeamMaintenanceDetailController;
}(CHControllerBase));
angular.module("AngularApp")
    .controller("WorkTeamMaintenanceDetailController", [
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
//# sourceMappingURL=~WorkTeamMaintenanceDetailController.js.map