#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TRiZHub.Models.TimesheetTemplateModels
{
    public class TimesheetTemplateItemModel
    {
        public int DayOffset { get; set; }
        public Guid ProjectGridId { get; set; }
        public string ProjectDescription { get; set; }
        public string ClientEntityName { get; set; }
        public bool? Billable { get; set; }
        public Guid? SubProjectId { get; set; }
        public Guid TeamId { get; set; }
        public Guid ActivityId { get; set; }
        public decimal Hours { get; set; }
        public string Comments { get; set; }
    }

    public class TimesheetTemplateModel
    {
        public Guid? Id { get; set; }
        public Guid UserAccountId { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string CopiedAt { get; set; }
        public int RowCount { get; set; }
        public List<TimesheetTemplateItemModel> Rows { get; set; }
    }

    public class TimesheetTemplateSaveModel
    {
        public Guid UserAccountId { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public List<TimesheetTemplateItemModel> Rows { get; set; }
    }

    public class TimesheetTemplateRenameModel
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
    }

    public class TimesheetTemplateListRequest
    {
        public Guid UserAccountId { get; set; }
    }
}
