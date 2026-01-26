#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.TeamData
{
    public class TeamException : Exception
    {
        public TeamException(string error) : base(error)
        {
        }
    }
}