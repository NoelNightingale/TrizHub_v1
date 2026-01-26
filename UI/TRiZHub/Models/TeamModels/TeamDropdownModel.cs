#region Usings

using System;

#endregion

namespace TRiZHub.Models.TeamModels
{
    public class TeamDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0}", TeamName); }
        }

        public string TeamName { get; set; }
    }
}