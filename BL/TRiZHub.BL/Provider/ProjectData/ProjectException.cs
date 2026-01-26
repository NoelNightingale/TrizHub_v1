#region Usings

using System;

#endregion

namespace TRiZHub.BL.Provider.ProjectData
{
    public class ProjectException : Exception
    {
        public ProjectException(string error) : base(error)
        {
        }
    }
}