#region Usings

using System;
using System.Data.Entity;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.SecurityData;
using TRiZHub.BL.Entities.TeamData;
using TRiZHub.BL.Entities.TeamJobDesignationData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.TeamJobDesignationData
{
    public class TeamJobDesignationProvider : TRiZHubProvider, ITeamJobDesignationProvider
    {
        #region Constructor

        public TeamJobDesignationProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        public TeamJobDesignation SaveTeamJobDesignation(Guid? id, Guid userAccountId, string jobDesignation,
            DateTime startDate, DateTime? endDate,
            string location, Guid? lineLeaderId, Guid clientId, Guid? employerId)
        {
            Authenticate(PrivilegeType.UserTeamJobDesignationMaintenance);


            if (startDate > endDate)
            {
                throw new TeamJobDesignationException("Selected End Date is before Selected Start Date!");
            }
            bool duplicatePeriod = false;

            var teamDesignations = DataContext.TeamJobDesignationSet.Where(a => a.UserAccountId == userAccountId && a.Id != id).ToList();
            foreach (var teamDesignation in teamDesignations)
            {
                if (startDate >= teamDesignation.StartDate && startDate <= teamDesignation.EndDate)
                    duplicatePeriod = true;
                else if (endDate >= teamDesignation.StartDate && endDate <= teamDesignation.EndDate)
                    duplicatePeriod = true;
            }


            if (duplicatePeriod)
                throw new TeamJobDesignationException("A Team and Job Designation already exists");


            var record = DataContext.TeamJobDesignationSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new TeamJobDesignation
                {

                    UserAccountId = userAccountId
                };

                DataContext.TeamJobDesignationSet.Add(record);
            }

            record.JobDesignation = jobDesignation;
            record.StartDate = startDate;
            record.EndDate = endDate;
            record.Location = location;
            record.LineLeaderId = lineLeaderId;
            record.ClientId = clientId;
            record.EmployerId = employerId;

            DataContextSaveChanges();

            return record;
        }

        public TeamJobDesignation GetTeamJobDesignation(Guid id)
        {
            return DataContext.TeamJobDesignationSet.Include(a => a.Client).Include(a => a.LineLeader).Include(a => a.UserAccount).FirstOrDefault(a => a.Id == id);
        }

        public void DeleteTeamJobDesignation(Guid id)
        {
            var record = GetTeamJobDesignation(id);

            if (record != null)
            {
                DataContext.TeamJobDesignationSet.Remove(record);
                DataContext.SaveChanges();
            }
        }

        public IQueryable<TeamJobDesignation> TeamJobDesignationtFilterList(Guid userAccountId)
        {
            return DataContext.TeamJobDesignationSet.Where(a => a.UserAccountId == userAccountId);
        }

        public IQueryable<TeamJobDesignation> TeamJobDesignationtLineLeadFilterList(Guid userAccountId)
        {
          return DataContext.TeamJobDesignationSet.Where(a => a.LineLeaderId == userAccountId);
        }

        public IQueryable<TeamJobDesignation> TeamJobDesignationtLineLeadFilterListAll()
        {
            return DataContext.TeamJobDesignationSet.Where(a => a.LineLeaderId != null).Include(a => a.LineLeader).GroupBy(a => a.LineLeaderId).Select(a => a.FirstOrDefault());
        }
    }
}