#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TCR.Lib.Utility;
using TRiZHub.BL.Entities.Types;
using TRiZHub.Controllers.Filters;
using TRiZHub.Models.Enums;

#endregion

namespace TRiZHub.Controllers
{
    [AllowAnonymous]
    [NoCache]
    /// <summary>
    /// General Purpose call to retrieve various enumarated lists
    /// </summary>
    public class EnumsController : TCRControllerBase
    {
        /// <summary>
        /// Retrieve list of Privileges
        /// </summary>
        [HttpGet]
        public List<SecurityTypeModel> SecurityEnum()
        {
            return
                (from PrivilegeType t in Enum.GetValues(typeof (PrivilegeType)) select new SecurityTypeModel(t)).ToList();
        }

        /// <summary>
        /// Retrieve list of Statuses. Active/Inactive/Archive
        /// </summary>
        [HttpGet]
        public List<EnumModel> StatusTypeEnum()
        {
            return EnumToModel<StatusType>();
        }

        /// <summary>
        /// Retrieve list of Client Types. 
        /// </summary>
        [HttpGet]
        public List<ClientTypeModel> ClientTypeEnum()
        {
            return
                (from ClientEntityType t in Enum.GetValues(typeof (ClientEntityType)) select new ClientTypeModel(t))
                    .ToList();
        }

        /// <summary>
        /// Retrieve list of Scorecard ScoreTypes
        /// </summary>
        [HttpGet]
        public List<ScorecardScoreTypeModel> ScorecardScoreTypeEnum()
        {
            return
                (from ScorecardScoreType t in Enum.GetValues(typeof (ScorecardScoreType))
                    select new ScorecardScoreTypeModel(t))
                    .ToList();
        }

        private List<EnumModel> EnumToModel<T>(bool sort = true)
        {
            var result = new List<EnumModel>();

            foreach (T eValue in Enum.GetValues(typeof (T)))
            {
                result.Add(new EnumModel
                {
                    Description = NameSplitting.SplitCamelCase(eValue),
                    Value = Convert.ToInt32(eValue)
                });
            }

            if (sort)
            {
                result = result.OrderBy(a => a.Description).ToList();
            }
            return result;
        }
    }
}