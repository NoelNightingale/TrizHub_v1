#region Usings

using System;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.SecurityData;

#endregion

namespace TRiZHub.BL.Provider.ReportData.ReportModels.ScorecardSummary
{
    public class ScorecardSummaryEntryModel
    {
        public virtual Guid ScorecardTemplateId
        {
            get { return ScorecardTemplate.Id; }
        }

        public virtual Guid ScorecardTemplatePeriodId
        {
            get { return ScorecardTemplatePeriod.Id; }
        }

        public virtual Guid ScorecardTemplateItemId
        {
            get { return ScorecardTemplateItem.Id; }
        }

        //public virtual Guid ScorecardTemplateItemScoreId
        //{
        //    get { return ScorecardTemplateItemScore.Id; }
        //}

        public virtual Guid ScorecardId
        {
            get { return Scorecard.Id; }
        }

        //public virtual Guid ScorecardPeriodId
        //{
        //    get { return ScorecardPeriod.Id; }
        //}

        public virtual Guid ScorecardRecordId
        {
            get { return ScorecardRecord.Id; }
        }

        public virtual Guid EvaluatorId
        {
            get { return Evaluator.Id; }
        }

        public virtual Guid EmployeeId
        {
            get { return Employee.Id; }
        }

        public virtual ScorecardTemplate ScorecardTemplate { get; set; }
        public virtual ScorecardTemplatePeriod ScorecardTemplatePeriod { get; set; }
        public virtual ScorecardTemplateItem ScorecardTemplateItem { get; set; }
        //public virtual ScorecardTemplateItemScore ScorecardTemplateItemScore { get; set; }
        public virtual Scorecard Scorecard { get; set; }
        //public virtual ScorecardPeriod ScorecardPeriod { get; set; }
        public virtual ScorecardRecord ScorecardRecord { get; set; }
        public virtual UserAccount Evaluator { get; set; }
        public virtual UserAccount Employee { get; set; }
    }
}