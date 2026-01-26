#region Usings

using System.ComponentModel.DataAnnotations;
using TCR.Lib.Utility;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class PermissionViewModel
    {
        public PrivilegeType Privilege { get; set; }

        public string Description
        {
            get { return NameSplitting.SplitCamelCase(Privilege); }
        }

        [Display(Name = "Selected")]
        public bool Selected { get; set; }
    }
}