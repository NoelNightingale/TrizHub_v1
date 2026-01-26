using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRiZHub.Models.TimesheetModels
{
    public class Project
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public Guid ProjectId { get; set; }
        public String ProjectName { get; set; }
        public String Description { get; set; }
        public Guid? SubProjectId { get; set; }
        public String SubProjectName { get; set; }
    }
}