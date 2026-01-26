#region Usings

using System;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.ProjectAllocationModel
{
    public class ProjectAllocationModel
    {   
        public string Fullname { get; set; }
        public Guid? ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid? SubProjectId { get; set; }
        public string SubProjectName { get; set; }
        public string ProjectNumber { get; set; }
        public string SubProjectNumber { get; set; }
    }
}