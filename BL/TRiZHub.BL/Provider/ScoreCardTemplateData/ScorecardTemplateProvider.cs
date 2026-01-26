#region Usings

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.ScorecardTemplateData
{
    public class ScorecardTemplateProvider : TRiZHubProvider, IScorecardTemplateProvider
    {
        #region Constructor

        public ScorecardTemplateProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Scorecard Template

        public IQueryable<ScorecardTemplate> ScorecardTemplateList()
        {
            return DataContext.ScorecardTemplateSet;
        }

        public ScorecardTemplate GetScorecardTemplate(Guid id)
        {
            return DataContext.ScorecardTemplateSet
                .Include(a => a.Scorecards)
                .Include(a => a.ScorecardTemplateItems)
                .FirstOrDefault(a => a.Id == id);
        }

        public ScorecardTemplate SaveScorecardTemplate(Guid? id, string scoreCardName, string scoreCardCode, decimal excellentWeight,
            decimal adequateWeight, decimal inadequateWeight, bool isActive)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            var existing =
                DataContext.ScorecardTemplateSet.FirstOrDefault(
                    a => a.ScorecardName == scoreCardName && a.Id != id);
            if (existing != null)
                throw new ScorecardTemplateException("A scorecard with the Name: " + scoreCardName +
                                                     " already exists.");

            var record = DataContext.ScorecardTemplateSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new ScorecardTemplate
                {

                };
                DataContext.ScorecardTemplateSet.Add(record);
            }

            record.ScorecardCode = scoreCardCode;
            record.ScorecardName = scoreCardName;
            record.ExcellentWeight = excellentWeight;
            record.AdequateWeight = adequateWeight;
            record.InadequateWeight = inadequateWeight;
            record.IsActive = isActive;

            DataContextSaveChanges();

            return record;
        }

        public void DeleteScorecardTemplate(Guid? id)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            // Check if scorecards exist for the template
            var scorecards = DataContext.ScorecardSet.Where(sc => sc.ScorecardTemplateId == id);

            if (scorecards.Count() > 0)
            {
                // If the exist don't allow to delete
                throw new ScorecardTemplateException("There are scorecards associated with this template, could not perform delete.");
            }

            // Remove periods
            DataContext.ScorecardTemplatePeriodSet.RemoveRange(DataContext.ScorecardTemplatePeriodSet.Where(st => st.ScorecardTemplateId == id));

            // Remove items
            DataContext.ScorecardTemplateItemSet.RemoveRange(DataContext.ScorecardTemplateItemSet.Where(st => st.ScorecardTemplateId == id));

            var record = DataContext.ScorecardTemplateSet.FirstOrDefault(a => a.Id == id);
            if (record != null)
            {
                DataContext.ScorecardTemplateSet.Remove(record);
            }

            DataContextSaveChanges();
        }

        #endregion

        #region Scorecard Template Period

        public IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodList(List<Guid> scoreCardIds)
        {
            if (scoreCardIds.Count() < 1)
            {
                return DataContext.ScorecardTemplatePeriodSet;
            }
            else
            {
                return DataContext.ScorecardTemplatePeriodSet.Where(a => scoreCardIds.Contains(a.ScorecardTemplateId));
            }
        }

        public IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodList(int year)
        {
            return DataContext.ScorecardTemplatePeriodSet.Where(a => a.ReviewYear == year);
        }

        public IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodListMultiple(int[] years)
        {
            return from t in DataContext.ScorecardTemplatePeriodSet
                   where years.Contains(t.ReviewYear)
                   select t;
        }

        public IQueryable<ScorecardTemplate> ScorecardTemplateDropdownListYearMultiple(int[] years)
        {
            var periods = from t in DataContext.ScorecardTemplatePeriodSet
                          where years.Contains(t.ReviewYear)
                          select t.ScorecardTemplateId;

            return from t in DataContext.ScorecardTemplateSet
                   where periods.Contains(t.Id)
                   select t;
        }

        public IQueryable<int> ScorecardTemplatePeriodYearList()
        {
            return DataContext.ScorecardTemplatePeriodSet.Select(a => a.ReviewYear).Distinct();
        }

        public ScorecardTemplatePeriod GetScorecardTemplatePeriod(Guid id)
        {
            return DataContext.ScorecardTemplatePeriodSet
                .Include(a => a.ScorecardTemplate)
                .FirstOrDefault(a => a.Id == id);
        }

        public ScorecardTemplatePeriod SaveScorecardTemplatePeriod(Guid? id, Guid scoreCardId, DateTime startDate,
            DateTime endDate,
            string description, int reviewYear, bool isVariable, int reportSortOrder)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            var scorecardTemplatePeriodList = DataContext.ScorecardTemplatePeriodSet.Where(s => s.ScorecardTemplateId == scoreCardId && s.Id != id && !s.IsVariable);

            if (scorecardTemplatePeriodList != null && !isVariable)
            {
                foreach (ScorecardTemplatePeriod a in scorecardTemplatePeriodList)
                {
                    if (a.Id == id)
                    {
                        continue;
                    }

                    if (startDate.Date >= a.StartDate.Date && startDate.Date <= a.EndDate.Date
                        || endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date
                        || a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date
                        || a.EndDate.Date >= startDate.Date && a.EndDate.Date <= startDate.Date)
                    {
                        throw new ScorecardTemplateException("Scorecard period will overlap with another period!");

                    }

                }


            }

            var record = DataContext.ScorecardTemplatePeriodSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new ScorecardTemplatePeriod();
                DataContext.ScorecardTemplatePeriodSet.Add(record);
            }

            record.ScorecardTemplateId = scoreCardId;
            record.StartDate = startDate;
            record.EndDate = endDate;
            record.Description = description;
            record.ReviewYear = reviewYear;
            record.IsVariable = isVariable;
            record.ReportSortOrder = reportSortOrder;

            DataContextSaveChanges();

            return record;
        }

        public void DeleteScorecardTemplatePeriod(Guid? id)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            // Check if scorecards exist for the template
            var scorecards = DataContext.ScorecardSet.Where(sc => sc.ScorecardTemplatePeriodId == id);

            if (scorecards.Count() > 0)
            {
                // If the exist don't allow to delete
                throw new ScorecardTemplateException("There are scorecards associated with this period, could not perform delete.");
            }

            var record = DataContext.ScorecardTemplatePeriodSet.FirstOrDefault(p => p.Id == id);
            if (record != null)
            {
                DataContext.ScorecardTemplatePeriodSet.Remove(record);
            }

            DataContextSaveChanges();
        }

        #endregion

        #region Scorecard Template Item

        public IQueryable<ScorecardTemplateItem> ScorecardTemplateItemList(Guid scorecardTemplateId)
        {
            return DataContext.ScorecardTemplateItemSet.Where(a => a.ScorecardTemplateId == scorecardTemplateId);
        }

        public ScorecardTemplateItem GetScorecardTemplateItem(Guid id)
        {
            return DataContext.ScorecardTemplateItemSet
                .Include(a => a.ScorecardTemplate)
                .FirstOrDefault(a => a.Id == id);
        }

        public ScorecardTemplateItem SaveScorecardTemplateItem(Guid? id, Guid scoreCardId,
            string description, string definition, decimal weight, int scorecardScoring, decimal? minimum,
            decimal? maximum, string manualScoreDefinition, string excellentDefinition, string adequateDefinition, string inadequateDefinition, string order)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            var existing =
               DataContext.ScorecardTemplateItemSet.FirstOrDefault(
                   a => a.ScorecardTemplateId == scoreCardId && a.Description == description && a.Id != id);
            if (existing != null)
                throw new ScorecardTemplateException("Scorecard Item with the description" + description + "already exists!");


            var record = DataContext.ScorecardTemplateItemSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new ScorecardTemplateItem();
                DataContext.ScorecardTemplateItemSet.Add(record);
            }

            record.ScorecardScoring = scorecardScoring;
            record.Maximum = maximum;
            record.Minimum = minimum;
            record.ManualDefinition = manualScoreDefinition;
            record.ScorecardTemplateId = scoreCardId;
            //record.Code = code;
            record.Description = description;
            record.Definition = definition;
            record.Weight = weight;
            record.ExcellentDefinition = excellentDefinition;
            record.AdequateDefinition = adequateDefinition;
            record.InadequateDefinition = inadequateDefinition;
            record.Order = order;
            DataContextSaveChanges();

            return record;
        }

        public void DeleteScorecardTemplateItem(Guid? id)
        {
            Authenticate(PrivilegeType.ScorecardTemplateMaintenance);

            // Check if scorecards exist for the template
            var scorecards = DataContext.ScorecardRecordSet.Where(sc => sc.ScorecardTemplateItemId == id);

            if (scorecards.Count() > 0)
            {
                // If the exist don't allow to delete
                throw new ScorecardTemplateException("There are scorecards associated with this item, could not perform delete.");
            }

            var record = DataContext.ScorecardTemplateItemSet.Where(i => i.Id == id).FirstOrDefault();
            if (record != null)
            {
                DataContext.ScorecardTemplateItemSet.Remove(record);
            }

            DataContextSaveChanges();
        }

        #endregion

    }
}