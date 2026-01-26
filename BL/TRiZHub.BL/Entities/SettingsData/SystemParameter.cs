#region Usings

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TCR.Lib.BL;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Settings;

#endregion

namespace TRiZHub.BL.Entities.SettingsData
{
    //There can only ever be 1 record 
    //Stores global parameters configurable by the UI here.
    [Table("SystemParameter")]
    public class SystemParameter : DbEntity, IAppSettings
    {
        [Required]
        [MaxLength(500)]
        public virtual string EmailFromAddress { get; set; }

        [Required]
        [MaxLength(500)]
        public string EmailFromName { get; set; }


        [Required]
        [MaxLength(500)]
        public string AboutApp { get; set; }


        internal static void LoadDefault(DataContext context)
        {
            var param = context.SystemParameterSet.SingleOrDefault();
            if (param == null)
            {
                param = new SystemParameter
                {
                    AboutApp =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    EmailFromName = "noreply",
                    EmailFromAddress = "noreply2@s7on.co.za"
                };
                context.SystemParameterSet.Add(param);
                context.SaveChanges();
            }
        }
    }
}