#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.WorkTeamData
{
    public class WorkGroupException : Exception
    {
        public WorkGroupException(string message)
            : base(message)
        {
        }
    }
}
