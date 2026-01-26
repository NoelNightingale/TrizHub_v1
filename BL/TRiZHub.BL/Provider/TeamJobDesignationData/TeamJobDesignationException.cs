#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.TeamJobDesignationData
{
    public class TeamJobDesignationException : Exception
    {
        public TeamJobDesignationException(string error) : base(error)
        {
        }
    }
}