#region Usings

using System;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using System.Collections.Generic;

#endregion

namespace TRiZHub.BL.Provider.ScorecardData
{
    public class ScorecardProvider : TRiZHubProvider, IScorecardProvider
    {
        #region Constructor
        IList<PrivilegeType> getTokens;

        public ScorecardProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
            getTokens = new List<PrivilegeType>();
            getTokens.Add(PrivilegeType.PerformanceManagementCreateScoreCards);
            getTokens.Add(PrivilegeType.PerformanceManagementViewMyScoreCards);
            getTokens.Add(PrivilegeType.PerformanceManagementViewMyTeamScoreCards);
            getTokens.Add(PrivilegeType.ReportGenerationScoreCard);
        }

        #endregion

        #region Scorecard

        public IQueryable<Scorecard> ScorecardList()
        {
            return DataContext.ScorecardSet;
        }

        public Scorecard GetScorecard(Guid id)
        {
            return DataContext.ScorecardSet
                .Include(a => a.ScorecardRecords)
                .Include(a => a.Employee)
                .Include(a => a.Evaluator)
                .Include(a => a.ScorecardTemplatePeriod)
                .FirstOrDefault(a => a.Id == id);
        }

        public Scorecard SaveEmployeeComment(Guid? id, string employeeMessage)
        {
            Authenticate(PrivilegeType.PerformanceManagementViewMyScoreCards);
            var record = DataContext.ScorecardSet.FirstOrDefault(a => a.Id == id);
            record.EmployeeMessage = employeeMessage;
            DataContextSaveChanges();

            return record;

        }

        public ScorecardRecord SaveScoreCardRecordEmployeeComment(Guid? id, string employeeMessage)
        {
            Authenticate(PrivilegeType.PerformanceManagementViewMyScoreCards);
            var record = DataContext.ScorecardRecordSet.FirstOrDefault(a => a.Id == id);
            record.EmployeeHtmlComment = employeeMessage;
            DataContextSaveChanges();

            return record;
        }

        public Scorecard SaveScorecard(Guid? id, Guid scorecardTemplateId, Guid evaluatorId, Guid employeeId, Guid scoreCardTemplatePeriodId, bool rated, bool completed, Guid createdBy, DateTime dateCreated, string evaluatorMessage, string employeeMessage, DateTime? variableStart, DateTime? variableEnd, int? variableYear)
        {
            Authenticate(PrivilegeType.PerformanceManagementCreateScoreCards);

            if (variableStart >= variableEnd)
                throw new ScorecardException("The Start Date cannot be on or after the End Date");


            var record = DataContext.ScorecardSet.FirstOrDefault(a => a.Id == id);

            if (record == null)
            {
                record = new Scorecard
                {
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = CurrentUser.Id
                };
                DataContext.ScorecardSet.Add(record);
            }

            record.ScorecardTemplateId = scorecardTemplateId;
            record.EvaluatorId = evaluatorId;
            record.EmployeeId = employeeId;
            record.ScorecardTemplatePeriodId = scoreCardTemplatePeriodId;
            record.Rated = rated;
            record.Completed = completed;
            record.CreatedBy = createdBy;
            record.DateCreated = dateCreated;
            record.EvaluatorMessage = evaluatorMessage;
            record.EmployeeMessage = employeeMessage;
            record.VariableStart = variableStart;
            record.VariableEnd = variableEnd;
            record.VariableYear = variableYear;

            DataContextSaveChanges();

            return record;
        }

        public void DeleteScoreCard(Guid id)
        {
            var scoreCard = GetScorecard(id);

            //delete records first
            foreach(var record in DataContext.ScorecardRecordSet.Where(a => a.ScorecardId == scoreCard.Id).ToList())
            {
                DataContext.ScorecardRecordSet.Remove(record);
            }

            DataContext.ScorecardSet.Remove(scoreCard);
            DataContextSaveChanges();
        }

        public void LockScoreCard(Guid id)
        {
            var scoreCard = GetScorecard(id);

            if (scoreCard.locked == true)
            {
                scoreCard.locked = false;
                DataContext.SaveChanges();
            }

            else
            {
                scoreCard.locked = true;
                DataContextSaveChanges();
            }
        }

        public void UnsubmitScoreCard(Guid id)
        {
            var scoreCard = GetScorecard(id);
            scoreCard.Completed = false;

            // delete records for that score card
     //       var record = DataContext.ScorecardRecordSet.FirstOrDefault(a => a.ScorecardId == scoreCard.Id);
     //       DataContext.ScorecardRecordSet.Remove(record);

            DataContext.SaveChanges();
        }

        public void SubmitScoreCard(Guid id)
        {
            var scoreCard = GetScorecard(id);
            scoreCard.Completed = true;
            DataContext.SaveChanges();
        }

        public void ReassignScorecard(Guid? id, Guid evaluatorId)
        {
            Authenticate(PrivilegeType.PerformanceManagementCreateScoreCards);

            var record = DataContext.ScorecardSet.FirstOrDefault(a => a.Id == id);

            if (record != null)
            {
                record.EvaluatorId = evaluatorId;
                DataContextSaveChanges();
            }
        }

        public IQueryable<Scorecard> GetAllScorecardEvaluators()
        {
            //Authenticate(PrivilegeType.PerformanceManagementCreateScoreCards);

            var records = DataContext.ScorecardSet.Include(s => s.Evaluator).GroupBy(s => s.EvaluatorId).Select(x => x.FirstOrDefault());
            return records;
        }

        #endregion

        #region Scorecard Record

        public IQueryable<ScorecardRecord> ScorecardRecordList(Guid scorecardId)
        {
            AuthenticateList(getTokens);
            return DataContext.ScorecardRecordSet.Where(a => a.ScorecardId == scorecardId);
        }

        public ScorecardRecord GetScorecardRecord(Guid id)
        {
            AuthenticateList(getTokens);
            return DataContext.ScorecardRecordSet
                .Include(a => a.ScorecardTemplateItem)
                .FirstOrDefault(a => a.Id == id);
        }

        public ScorecardRecord SaveScorecardRecord(Guid? id, Guid scorecardId, Guid scorecardTemplateItemId,
            ScorecardScoreType? rating, decimal? value, bool completed, string evaluatorHtmlComment, string employeeHtmlComment)
        {
            Authenticate(PrivilegeType.PerformanceManagementCreateScoreCards);

            var record = DataContext.ScorecardRecordSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new ScorecardRecord();
                DataContext.ScorecardRecordSet.Add(record);
            }

            record.ScorecardTemplateItemId = scorecardTemplateItemId;
            record.ScorecardId = scorecardId;
            record.Rating = rating;
            record.Completed = completed;
            record.LastUpdated = DateTime.UtcNow;
            record.Value = value;
            record.EvaluatorHtmlComment = evaluatorHtmlComment;
            record.EmployeeHtmlComment = employeeHtmlComment;

            DataContextSaveChanges();

            return record;
        }




        #endregion
    }
}