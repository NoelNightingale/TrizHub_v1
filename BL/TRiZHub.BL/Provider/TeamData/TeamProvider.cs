#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.TeamData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.TeamData
{
    public class TeamProvider : TRiZHubProvider, ITeamProvider
    {
        #region Constructor

        public TeamProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Team

        public IQueryable<Team> TeamList()
        {
            return DataContext.TeamSet;
        }

        public Team GetTeam(Guid id)
        {
            return DataContext.TeamSet.FirstOrDefault(a => a.Id == id);
        }

        public Team SaveTeam(Guid? id, string teamName, bool isActive)
        {
            Authenticate(PrivilegeType.TeamMaintenance);

            var existing = DataContext.TeamSet.FirstOrDefault(a => a.TeamName == teamName && a.Id != id);
            if (existing != null)
                throw new TeamException("A team with the name: " + teamName + " already exists.");

            var record = DataContext.TeamSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new Team();
                DataContext.TeamSet.Add(record);
            }

            record.TeamName = teamName;
            record.IsActive = isActive;

            DataContextSaveChanges();

            return record;
        }

        #endregion
    }
}