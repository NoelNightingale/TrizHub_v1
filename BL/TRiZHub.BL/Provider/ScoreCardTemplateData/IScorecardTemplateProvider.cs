#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Entities.ScorecardTemplateData;
using TRiZHub.BL.Entities.Types;

#endregion

namespace TRiZHub.BL.Provider.ScorecardTemplateData
{
    public interface IScorecardTemplateProvider : ITRiZHubProvider
    {
        #region Scorecard Template

        IQueryable<ScorecardTemplate> ScorecardTemplateList();

        ScorecardTemplate GetScorecardTemplate(Guid id);

        ScorecardTemplate SaveScorecardTemplate(Guid? id, string scoreCardName, string scoreCardCode, decimal excellentWeight, decimal adequateWeight, decimal inadequateWeight, bool isActive);

        void DeleteScorecardTemplate(Guid? id);

        #endregion

        #region Scorecard Template Period

        IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodList(List<Guid> scoreCardIds);

        IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodList(int year);

        IQueryable<ScorecardTemplatePeriod> ScorecardTemplatePeriodListMultiple(int[] years);

        IQueryable<int> ScorecardTemplatePeriodYearList();

        ScorecardTemplatePeriod GetScorecardTemplatePeriod(Guid id);

        ScorecardTemplatePeriod SaveScorecardTemplatePeriod(Guid? id, Guid scoreCardId, DateTime startDate,
            DateTime endDate, string description, int reviewYear, bool isVariable, int reportSortOrder);

        void DeleteScorecardTemplatePeriod(Guid? id);

        #endregion

        #region Scorecard Template Item

        IQueryable<ScorecardTemplateItem> ScorecardTemplateItemList(Guid scorecardTemplateId);

        IQueryable<ScorecardTemplate> ScorecardTemplateDropdownListYearMultiple(int[] years);

        ScorecardTemplateItem GetScorecardTemplateItem(Guid id);

        ScorecardTemplateItem SaveScorecardTemplateItem(Guid? id, Guid scoreCardId,
            string description, string definition, decimal weight, int scorecardScoring, decimal? minimum,
            decimal? maximum, string manualScoreDefinition, string excellentDefinition, string adequateDefinition, string inadequateDefinition, string order);

        void DeleteScorecardTemplateItem(Guid? id);

        #endregion

        #region Scorecard Template Item Score

        // NOT USED ANYMORE

        //IQueryable<ScorecardTemplateItemScore> ScorecardTemplateItemScoreList(Guid scoreCardItemId);

        //ScorecardTemplateItemScore GetScorecardTemplateItemScoreItem(Guid id);

        //ScorecardTemplateItemScore SaveScorecardTemplateItemScore(Guid? id, Guid scoreCardItemId,
        //    ScorecardScoreType type,
        //    decimal score, string definition);

        #endregion
    }
}