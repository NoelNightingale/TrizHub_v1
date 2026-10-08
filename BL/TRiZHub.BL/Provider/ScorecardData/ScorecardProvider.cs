#region Usings

using System;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ScorecardData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;
using TRiZHub.BL.Provider.WorkTeamData;
using System.Collections.Generic;
using TCR.Lib.BL;

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

        #region Ownership

        private Guid CurrentUserId
        {
            get
            {
                if (CurrentUser == null)
                    throw new GenericSecurityException("Not Allowed!");
                return CurrentUser.Id;
            }
        }

        /// <summary>The dates a scorecard covers: the variable dates when set, else the template period's.</summary>
        private void ScorecardRange(Guid scoreCardTemplatePeriodId, DateTime? variableStart, DateTime? variableEnd,
            out DateTime from, out DateTime to)
        {
            var period = DataContext.ScorecardTemplatePeriodSet
                .Where(p => p.Id == scoreCardTemplatePeriodId)
                .Select(p => new { p.StartDate, p.EndDate, p.IsVariable })
                .FirstOrDefault();
            if (period == null)
                throw new ScorecardException("The scorecard period could not be found.");

            if (period.IsVariable && variableStart.HasValue)
            {
                from = variableStart.Value.Date;
                to = (variableEnd ?? variableStart).Value.Date;
            }
            else
            {
                from = period.StartDate.Date;
                to = period.EndDate.Date;
            }
        }

        /// <summary>A current team lead/manager with the scorecards flag whose reach covers part of the period.</summary>
        private bool TeamReaches(Guid employeeId, Guid scoreCardTemplatePeriodId, DateTime? variableStart, DateTime? variableEnd)
        {
            DateTime from, to;
            ScorecardRange(scoreCardTemplatePeriodId, variableStart, variableEnd, out from, out to);
            return new WorkTeamAccessProvider(DataContext, CurrentUser)
                .CanActOn(CurrentUserId, employeeId, WorkTeamCapability.Scorecards, from, to);
        }

        private bool TeamReaches(Scorecard scorecard)
        {
            return TeamReaches(scorecard.EmployeeId, scorecard.ScorecardTemplatePeriodId, scorecard.VariableStart, scorecard.VariableEnd);
        }

        private bool IsLineLeaderOf(Guid employeeId)
        {
            var now = DateTime.Now;
            var userId = CurrentUserId;
            return UserIsAllowed(PrivilegeType.PerformanceManagementViewMyTeamScoreCards)
                   && DataContext.TeamJobDesignationSet.Any(a => a.LineLeaderId == userId
                                                                 && a.UserAccountId == employeeId
                                                                 && a.StartDate < now
                                                                 && (a.EndDate == null || a.EndDate > now));
        }

        /// <summary>Evaluator, creator, performance admin, or team reach over the scorecard period.</summary>
        private void EnsureCanManage(Scorecard scorecard)
        {
            var userId = CurrentUserId;
            if (scorecard.EvaluatorId == userId
                || scorecard.CreatedBy == userId
                || UserIsAllowed(PrivilegeType.PerformanceManagementAdmin)
                || TeamReaches(scorecard))
                return;
            throw new ScorecardException("You are not allowed to change this scorecard.");
        }

        /// <summary>Anyone who may manage it, the employee, or their line leader.</summary>
        private void EnsureCanView(Scorecard scorecard)
        {
            var userId = CurrentUserId;
            if (scorecard.EmployeeId == userId || IsLineLeaderOf(scorecard.EmployeeId))
                return;
            EnsureCanManage(scorecard);
        }

        private Scorecard LoadScorecard(Guid id)
        {
            var scorecard = DataContext.ScorecardSet
                .Include(a => a.ScorecardRecords)
                .Include(a => a.Employee)
                .Include(a => a.Evaluator)
                .Include(a => a.ScorecardTemplatePeriod)
                .FirstOrDefault(a => a.Id == id);
            if (scorecard == null)
                throw new ScorecardException("The scorecard could not be found.");
            return scorecard;
        }

        #endregion

        #region Scorecard

        public IQueryable<Scorecard> ScorecardList()
        {
            return DataContext.ScorecardSet;
        }

        public IQueryable<Guid> TeamScorecardEmployeeIds()
        {
            return new WorkTeamAccessProvider(DataContext, CurrentUser)
                .ManagedUserIds(CurrentUserId, WorkTeamCapability.Scorecards, new DateTime(1753, 1, 1), DateTime.MaxValue.Date);
        }

        public bool CanManageScorecardFor(Guid employeeId, Guid scoreCardTemplatePeriodId, DateTime? variableStart, DateTime? variableEnd)
        {
            return UserIsAllowed(PrivilegeType.PerformanceManagementCreateScoreCards)
                   || TeamReaches(employeeId, scoreCardTemplatePeriodId, variableStart, variableEnd);
        }

        public Scorecard GetScorecard(Guid id)
        {
            var scorecard = LoadScorecard(id);
            EnsureCanView(scorecard);
            return scorecard;
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
            if (variableStart >= variableEnd)
                throw new ScorecardException("The Start Date cannot be on or after the End Date");

            if (!CanManageScorecardFor(employeeId, scoreCardTemplatePeriodId, variableStart, variableEnd))
                throw new GenericSecurityException("Not Allowed!");

            var record = DataContext.ScorecardSet.FirstOrDefault(a => a.Id == id);
            if (record != null && !UserIsAllowed(PrivilegeType.PerformanceManagementCreateScoreCards))
                EnsureCanManage(record);

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
            var scoreCard = LoadScorecard(id);
            EnsureCanManage(scoreCard);

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
            var scoreCard = LoadScorecard(id);
            EnsureCanManage(scoreCard);

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
            var scoreCard = LoadScorecard(id);
            EnsureCanManage(scoreCard);
            scoreCard.Completed = false;

            // delete records for that score card
     //       var record = DataContext.ScorecardRecordSet.FirstOrDefault(a => a.ScorecardId == scoreCard.Id);
     //       DataContext.ScorecardRecordSet.Remove(record);

            DataContext.SaveChanges();
        }

        public void SubmitScoreCard(Guid id)
        {
            var scoreCard = LoadScorecard(id);
            EnsureCanManage(scoreCard);
            scoreCard.Completed = true;
            DataContext.SaveChanges();
        }

        public void ReassignScorecard(Guid? id, Guid evaluatorId)
        {
            var record = DataContext.ScorecardSet.FirstOrDefault(a => a.Id == id);

            if (record != null)
            {
                if (!UserIsAllowed(PrivilegeType.PerformanceManagementCreateScoreCards))
                {
                    if (!TeamReaches(record))
                        throw new GenericSecurityException("Not Allowed!");
                }

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
            if (!UserIsAllowed(PrivilegeType.PerformanceManagementCreateScoreCards))
                EnsureCanManage(LoadScorecard(scorecardId));

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