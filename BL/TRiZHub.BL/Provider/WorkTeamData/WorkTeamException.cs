#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public class WorkTeamException : Exception
    {
        public WorkTeamException(string message)
            : base(message)
        {
        }
    }
}
