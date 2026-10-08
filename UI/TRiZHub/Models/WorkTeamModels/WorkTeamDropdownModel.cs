#region Usings

using System;

#endregion

namespace TRiZHub.Models.WorkTeamModels
{
    public class WorkTeamDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0}", Name); }
        }

        public string Name { get; set; }
    }
}
