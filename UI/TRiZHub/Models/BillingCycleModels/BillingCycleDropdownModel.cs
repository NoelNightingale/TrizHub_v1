using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRiZHub.Models.BillingCycleModels
{
    public class BillingCycleDropdownModel
    {
        public Guid Id { get; set; }

        public string Description
        {
            get { return string.Format("({0}) - {1}", Cycle, Year); }
        }

        public DateTime Startdate { get; set; }

        public DateTime Enddate { get; set; }

        public short Cycle { get; set; }

        public short Year { get; set; }
    }
}