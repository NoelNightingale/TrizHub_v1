#region Usings

using System;
using TCR.Lib.Utility;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.Models.SecurityData
{
    public class RoleGridModel
    {
        public Guid? Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public StatusType StatusTypes { get; set; }

        public string Status
        {
            get { return NameSplitting.SplitCamelCase(StatusTypes); }

            set { NameSplitting.SplitCamelCase(StatusTypes); }
        }

        public bool IsActive { get; set; }
    }
}