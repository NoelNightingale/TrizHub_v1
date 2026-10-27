namespace TRiZHub.BL.Provider.WorkTeamData
{
    /// <summary>
    /// Who may see or change a work group. WorkGroupAdmin edits every level. A group manager edits the
    /// teams, leads, and members under the groups they manage. A team lead edits members on their own
    /// active teams. A team member can view the group and the team they belong to.
    /// </summary>
    public static class WorkGroupAccess
    {
        public static bool CanEditGroup(bool isAdmin)
        {
            return isAdmin;
        }

        public static bool CanManageStructure(bool isAdmin, bool isManager)
        {
            return isAdmin || isManager;
        }

        public static bool CanEditMembers(bool isAdmin, bool isManager, bool isLead, bool groupActive, bool teamActive)
        {
            if (isAdmin || isManager)
                return true;
            return isLead && groupActive && teamActive;
        }

        public static bool CanSeeGroup(bool isAdmin, bool isManager, bool hasATeamInGroup)
        {
            return isAdmin || isManager || hasATeamInGroup;
        }

        public static bool CanSeeTeam(bool isAdmin, bool isManager, bool isOnTeam)
        {
            return isAdmin || isManager || isOnTeam;
        }

        public static bool CanDeleteGroup(bool isAdmin, int teamCount, int managerCount)
        {
            return isAdmin && teamCount == 0 && managerCount == 0;
        }

        public static bool CanDeleteTeam(bool canManageStructure, int personCount)
        {
            return canManageStructure && personCount == 0;
        }
    }
}
