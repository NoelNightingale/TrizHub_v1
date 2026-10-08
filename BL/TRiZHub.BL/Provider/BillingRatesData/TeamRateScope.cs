#region Usings

using System;
using System.Collections.Generic;
using TRiZHub.BL.Provider.WorkTeamData;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    /// <summary>Whether a team manager/lead's Rates reach covers one rate.</summary>
    public static class TeamRateScope
    {
        /// <summary>
        /// A Client rate needs the client on a reaching team; a Project rate needs the project (whole or through a
        /// subproject) or its client. Only teams whose reach overlaps the rate period count. Default rates never pass.
        /// </summary>
        /// <param name="periods">The target's periods the actor reaches for Rates.</param>
        /// <param name="scopes">Allocations of each team in <paramref name="periods"/>.</param>
        public static bool Allows(IEnumerable<WorkTeamTypePeriod> periods, IDictionary<Guid, WorkTeamScope> scopes,
            Guid? clientId, Guid? projectId, Guid? projectClientId, DateTime start, DateTime end)
        {
            if (!clientId.HasValue && !projectId.HasValue)
                return false;

            foreach (var period in periods)
            {
                if (!WorkTeamAccessRules.Overlaps(period.StartDate, period.EndDate, start, end))
                    continue;

                WorkTeamScope scope;
                if (!scopes.TryGetValue(period.WorkTeamId, out scope))
                    continue;

                if (clientId.HasValue && scope.ClientIds.Contains(clientId.Value))
                    return true;
                if (projectId.HasValue
                    && (scope.ProjectIds.Contains(projectId.Value)
                        || (projectClientId.HasValue && scope.ClientIds.Contains(projectClientId.Value))))
                    return true;
            }

            return false;
        }
    }
}
