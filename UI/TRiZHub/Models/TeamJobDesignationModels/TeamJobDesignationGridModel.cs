#region Usings

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.UI;
using TRiZHub.BL.Entities.TeamData;

#endregion

namespace TRiZHub.Models.TeamJobDesignationModels
{
    public class TeamJobDesignationGridModel
    {
        public Guid? Id { get; set; }

        public Guid UserAccountId { get; set; }

        public string ClientName { get; set; }
        public Guid ClientId { get; set; }

        public string LineLeader { get; set; }

        public string JobDesignation { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Location { get; set; }
    }
}