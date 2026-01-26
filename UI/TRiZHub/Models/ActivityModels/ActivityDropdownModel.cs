#region Usings

using System;

#endregion

namespace TRiZHub.Models.ActivityModels
{
    public class ActivityDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("{0}", ActivityName); }
        }

        public string ActivityName { get; set; }
    }
}