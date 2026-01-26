#region Usings

using System;
using System.Linq;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.ScorecardData
{
    public interface IScorecardProvider : ITRiZHubProvider
    {
        #region Scorecard

        IQueryable<Scorecard> ScorecardList();

        Scorecard GetScorecard(Guid id);

        Scorecard SaveScorecard(Guid? id, Guid scorecardTemplateId, Guid evaluatorId, Guid employeeId, Guid scoreCardTemplatePeriodId, bool rated, bool completed, Guid createdBy, DateTime dateCreated, string evaluatorMessage, string employeeMessage, DateTime? variableStart, DateTime? variableEnd, int? variableYear);
        Scorecard SaveEmployeeComment(Guid? id, string employeeMessage);
        void ReassignScorecard(Guid? id, Guid evaluatorId);

        #endregion

        #region Scorecard Period

        //IQueryable<ScorecardPeriod> ScorecardPeriodList();
        //IQueryable<ScorecardPeriod> ScorecardPeriodList(Guid scorecardId);

        //ScorecardPeriod GetScorecardPeriod(Guid id);

        //ScorecardPeriod SaveScorecardPeriod(Guid? id, Guid scorecardId, Guid scorecardTemplatePeriodId,
        //    bool rated, bool completed, string evaluatorMessage, string employeeMessage);

        #endregion

        #region Scorecard Record

        IQueryable<ScorecardRecord> ScorecardRecordList(Guid scorecardPeriodId);

        ScorecardRecord GetScorecardRecord(Guid id);

        ScorecardRecord SaveScorecardRecord(Guid? id, Guid scorecardPeriodId, Guid scorecardTemplateItemId,
            ScorecardScoreType? rating, decimal? value, bool completed, string evaluatorHtmlComment, string employeeHtmlComment);

        ScorecardRecord SaveScoreCardRecordEmployeeComment(Guid? id, string employeeMessage);

        void DeleteScoreCard(Guid id);

        void LockScoreCard(Guid id);

        void UnsubmitScoreCard(Guid id);

        void SubmitScoreCard(Guid id);

        IQueryable<Scorecard> GetAllScorecardEvaluators();
        #endregion
    }
}