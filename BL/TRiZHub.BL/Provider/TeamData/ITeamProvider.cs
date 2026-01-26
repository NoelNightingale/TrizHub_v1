#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.BL.Provider.TeamData
{
    public interface ITeamProvider : ITRiZHubProvider
    {
        #region Team

        Team SaveTeam(Guid? id, string activityName, bool isActive);

        Team GetTeam(Guid id);

        IQueryable<Team> TeamList();

        #endregion
    }
}