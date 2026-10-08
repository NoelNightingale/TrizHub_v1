#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ClientEntityData;
using TRiZHub.BL.Entities.ProjectData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Entities.WorkTeamData;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public interface IWorkTeamProvider : ITRiZHubProvider
    {
        #region Team

        IQueryable<WorkTeam> WorkTeamList();

        WorkTeam GetWorkTeam(Guid id);

        WorkTeamPermissions PermissionsFor(Guid workTeamId);

        WorkTeam SaveWorkTeam(Guid? id, string name, string description, bool isActive, Guid teamTypeId,
            bool allowTeamTimesheets, bool allowTeamScorecards, bool leadsManageAllocations, bool leadsManageRates);

        #endregion

        #region People

        IQueryable<WorkTeamMember> MemberList(Guid workTeamId);

        WorkTeamMember SaveMember(Guid? id, Guid workTeamId, Guid userAccountId, WorkTeamRoleType role,
            DateTime startDate, DateTime? endDate);

        void DeleteMember(Guid id);

        #endregion

        #region Allocations

        List<WorkTeamClient> ClientAllocations(Guid workTeamId);

        List<WorkTeamProject> ProjectAllocations(Guid workTeamId);

        void SaveAllocations(Guid workTeamId, IEnumerable<Guid> clientIds,
            IEnumerable<KeyValuePair<Guid, Guid?>> projectAndSubProjectIds);

        IQueryable<ClientEntity> ClientCatalog();

        IQueryable<Project> ProjectCatalog();

        IQueryable<SubProject> SubProjectCatalog();

        #endregion
    }
}
