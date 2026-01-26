#region Usings

using System;

#endregion

namespace TRiZHub.Models.ReportModels
{
    public class DateBetweenGridModel : GridModel
    {
        public Guid? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}