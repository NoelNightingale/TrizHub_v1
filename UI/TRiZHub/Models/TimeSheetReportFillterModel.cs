using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRiZHub.Models
{
    public class TimeSheetReportFillterModel
    {
        public string UserAccountId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProjectId { get; set; }
        public string Clients { get; set; }
        public string Projects { get; set; }
        public bool ShowBillingPeriod { get; set; }
        public bool ShowRates { get; set; }
        public bool ShowPhases { get; set; }
    }
}