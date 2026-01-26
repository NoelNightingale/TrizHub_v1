#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TeamData;
using TRiZHub.BL.Entities.TeamJobDesignationData;

#endregion

namespace TRiZHub.BL.Provider.TeamJobDesignationData
{
    public interface ITeamJobDesignationProvider
    {
        TeamJobDesignation SaveTeamJobDesignation(Guid? id, Guid userAccountId, string jobDesignation,
            DateTime startDate,
            DateTime? endDate,
            string location, Guid? lineLeaderId, Guid clientId, Guid? employerId);

        TeamJobDesignation GetTeamJobDesignation(Guid id);

        void DeleteTeamJobDesignation(Guid id);

        IQueryable<TeamJobDesignation> TeamJobDesignationtFilterList(Guid userAccountId);
        IQueryable<TeamJobDesignation> TeamJobDesignationtLineLeadFilterList(Guid userAccountId);
        IQueryable<TeamJobDesignation> TeamJobDesignationtLineLeadFilterListAll();
    }
}