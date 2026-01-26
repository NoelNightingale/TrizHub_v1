using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRiZHub.BL.Models.Reports
{
    public class ProjectAllocationReportModel
    {
        public string FullName { get; set; }
        public bool UserActive { get; set; }

        public string ClientName { get; set; }
        public bool ClientActive { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public bool? ProjectActive { get; set; }

        public string SubProjectNumber { get; set; }
        public string SubProjectName { get; set; }
        public bool? SubProjectActive { get; set; }
    }
}
