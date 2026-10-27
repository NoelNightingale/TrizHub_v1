#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public interface IWorkGroupProvider : ITRiZHubProvider
    {
        WorkGroupTree Tree();

        Guid SaveGroup(Guid? id, string name, string description, IList<Guid> clientIds, bool isActive, IList<WorkGroupManagerSave> managers);

        void DeleteGroup(Guid id);

        Guid SaveTeam(Guid? id, Guid workGroupId, Guid teamTypeId, bool isActive);

        void DeleteTeam(Guid id);

        Guid SavePerson(Guid workTeamId, Guid userAccountId, WorkTeamRoleType role, bool isHomeTeam, bool canView);

        void DeletePerson(Guid id);
    }
}
