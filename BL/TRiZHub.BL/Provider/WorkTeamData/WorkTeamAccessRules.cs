#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public static class WorkTeamAccessRules
    {
        /// <summary>True when the two date ranges share a day. A null end is open-ended.</summary>
        public static bool Overlaps(DateTime start, DateTime? end, DateTime from, DateTime to)
        {
            return start.Date <= to.Date && (end == null || end.Value.Date >= from.Date);
        }
    }
}
