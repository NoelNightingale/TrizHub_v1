using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRiZHub.Models
{
    public class TimesheetSearchModel : GridModel
    {
        public Guid? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProjectId { get; set; }
        public int BillingOption { get; set; }

    }
}