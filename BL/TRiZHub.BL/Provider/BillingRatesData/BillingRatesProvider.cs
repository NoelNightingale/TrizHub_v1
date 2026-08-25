#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using TRiZHub.BL.Context;
using TRiZHub.BL.Entities.BillingRatesData;
using TRiZHub.BL.Entities.Types;
using TRiZHub.BL.Extensions;
using TRiZHub.BL.Provider.Security;

#endregion

namespace TRiZHub.BL.Provider.BillingRatesData
{
    public class BillingRatesProvider : TRiZHubProvider, IBillingRatesProvider
    {
        #region Constructor

        public BillingRatesProvider(DataContext context, ICurrentUser currentUser)
            : base(context, currentUser)
        {
        }

        #endregion

        #region Billing Rates

        public IQueryable<BillingRates> BillingRatesFilterList(Guid? userAccountId, Guid? clientId, Guid? projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var query = DataContext.BillingRatesSet.AsQueryable();

            if (userAccountId.HasValue && userAccountId.Value != Guid.Empty)
                query = query.Where(a => a.UserAccountId == userAccountId.Value);

            if (clientId.HasValue && clientId.Value != Guid.Empty)
                query = query.Where(a => a.ClientId == clientId.Value && a.ProjectId == null);

            if (projectId.HasValue && projectId.Value != Guid.Empty)
                query = query.Where(a => a.ProjectId == projectId.Value && a.ClientId == null);

            return query;
        }

        public void DeleteBillingRatesEntry(Guid id)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var record = GetBillingRates(id);

            if (record != null)
            {
                DataContext.BillingRatesSet.Remove(record);
                DataContext.SaveChanges();
            }
        }

        public BillingRates GetBillingRates(Guid id)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);
            return DataContext.BillingRatesSet.FirstOrDefault(a => a.Id == id);
        }

        public BillingRates SaveBillingRates(Guid? id, Guid userAccountId, decimal rate, DateTime startDate,
            DateTime endDate, Guid? clientId, Guid? projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            if (id == Guid.Empty)
                id = null;

            if (userAccountId == Guid.Empty)
                throw new BillingRatesException("User is required!");

            if (clientId == Guid.Empty)
                clientId = null;

            if (projectId == Guid.Empty)
                projectId = null;

            if (clientId.HasValue && projectId.HasValue)
                throw new BillingRatesException("A billing rate cannot be scoped to both a Client and a Project!");

            if (clientId.HasValue)
            {
                var clientExists = DataContext.ClientEntitySet.Any(c => c.Id == clientId.Value);
                if (!clientExists)
                    throw new BillingRatesException("Selected Client was not found!");
            }

            if (projectId.HasValue)
            {
                var projectExists = DataContext.ProjectSet.Any(p => p.Id == projectId.Value);
                if (!projectExists)
                    throw new BillingRatesException("Selected Project was not found!");
            }

            if (startDate.Date >= endDate.Date)
                throw new BillingRatesException("Selected End Date is bofore or on selected Start Date!");

            // Overlap only within the same (User + scope)
            var billingRates = DataContext.BillingRatesSet
                .Where(a => a.UserAccountId == userAccountId);

            if (clientId.HasValue)
                billingRates = billingRates.Where(a => a.ClientId == clientId && a.ProjectId == null);
            else if (projectId.HasValue)
                billingRates = billingRates.Where(a => a.ProjectId == projectId && a.ClientId == null);
            else
                billingRates = billingRates.Where(a => a.ClientId == null && a.ProjectId == null);

            foreach (var a in billingRates)
            {
                if (a.Id == id)
                    continue;

                if (startDate >= a.StartDate.Date && startDate.Date <= a.EndDate.Date ||
                    endDate.Date >= a.StartDate.Date && endDate.Date <= a.EndDate.Date ||
                    a.StartDate.Date >= startDate.Date && a.StartDate.Date <= endDate.Date ||
                    a.EndDate.Date >= startDate.Date && a.EndDate.Date <= endDate.Date)
                {
                    throw new BillingRatesException(
                        "Billing Rates period will overlap with another period!");
                }
            }

            var record = DataContext.BillingRatesSet.FirstOrDefault(a => a.Id == id);
            if (record == null)
            {
                record = new BillingRates
                {
                    UserAccountId = userAccountId
                };
                DataContext.BillingRatesSet.Add(record);
            }

            record.UserAccountId = userAccountId;
            record.ClientId = clientId;
            record.ProjectId = projectId;
            record.Rate = rate;
            record.StartDate = DateExtensions.ChangeTime(startDate, 0, 0, 0, 0);
            record.EndDate = DateExtensions.ChangeTime(endDate, 0, 0, 0, 0);

            DataContextSaveChanges();

            return record;
        }

        public ProjectTeamRatesResult GetProjectTeamRates(Guid projectId, DateTime asOfDate)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var project = DataContext.ProjectSet.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                throw new BillingRatesException("Selected Project was not found!");

            var asOf = asOfDate.Date;
            var clientId = project.ClientId;
            var clientName = project.Client != null
                ? project.Client.EntityName
                : DataContext.ClientEntitySet.Where(c => c.Id == clientId).Select(c => c.EntityName).FirstOrDefault();

            var projectUserIds = DataContext.UserIdentityProjectSet
                .Where(a => a.ProjectId == projectId)
                .Select(a => a.UserAccountId);

            var clientUserIds = DataContext.UserIdentityClientSet
                .Where(a => a.ClientId == clientId)
                .Select(a => a.UserAccountId);

            var teamUserIds = projectUserIds.Union(clientUserIds).Distinct().ToList();

            var users = DataContext.UserAccountSet
                .Where(u => teamUserIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.AccountName
                })
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .ToList();

            var userIds = users.Select(u => u.Id).ToList();

            var rates = DataContext.BillingRatesSet
                .Where(r => userIds.Contains(r.UserAccountId)
                            && r.StartDate <= asOf
                            && r.EndDate >= asOf
                            && (
                                (r.ProjectId == projectId && r.ClientId == null)
                                || (r.ClientId == clientId && r.ProjectId == null)
                                || (r.ClientId == null && r.ProjectId == null)
                            ))
                .ToList();

            var team = new List<ProjectTeamRateRow>();
            foreach (var user in users)
            {
                var projectRate = FindRateForDate(rates, user.Id, asOf, null, projectId);
                var clientRate = FindRateForDate(rates, user.Id, asOf, clientId, null);
                var defaultRate = FindRateForDate(rates, user.Id, asOf, null, null);

                decimal? effectiveRate = null;
                string effectiveScope = null;
                if (projectRate.HasValue)
                {
                    effectiveRate = projectRate;
                    effectiveScope = "Project";
                }
                else if (clientRate.HasValue)
                {
                    effectiveRate = clientRate;
                    effectiveScope = "Client";
                }
                else if (defaultRate.HasValue)
                {
                    effectiveRate = defaultRate;
                    effectiveScope = "Default";
                }

                team.Add(new ProjectTeamRateRow
                {
                    UserAccountId = user.Id,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    AccountName = user.AccountName,
                    ProjectRate = projectRate,
                    ClientRate = clientRate,
                    DefaultRate = defaultRate,
                    EffectiveRate = effectiveRate,
                    EffectiveScope = effectiveScope
                });
            }

            return new ProjectTeamRatesResult
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                ClientId = clientId,
                ClientName = clientName,
                AsOfDate = asOf,
                Team = team
            };
        }

        public UserRatesForProjectContextResult GetUserRatesForProjectContext(Guid userAccountId, Guid projectId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var project = DataContext.ProjectSet.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                throw new BillingRatesException("Selected Project was not found!");

            var user = DataContext.UserAccountSet.FirstOrDefault(u => u.Id == userAccountId);
            if (user == null)
                throw new BillingRatesException("Selected User was not found!");

            var clientId = project.ClientId;
            var clientName = project.Client != null
                ? project.Client.EntityName
                : DataContext.ClientEntitySet.Where(c => c.Id == clientId).Select(c => c.EntityName).FirstOrDefault();

            var projectRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ProjectId == projectId && r.ClientId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var clientRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == clientId && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var defaultRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == null && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            return new UserRatesForProjectContextResult
            {
                UserAccountId = user.Id,
                UserName = (user.FirstName + " " + user.Surname).Trim(),
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                ClientId = clientId,
                ClientName = clientName,
                ProjectRates = projectRates,
                ClientRates = clientRates,
                DefaultRates = defaultRates
            };
        }

        public ClientTeamRatesResult GetClientTeamRates(Guid clientId, DateTime asOfDate)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var client = DataContext.ClientEntitySet.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
                throw new BillingRatesException("Selected Client was not found!");

            var asOf = asOfDate.Date;

            var clientProjectIds = DataContext.ProjectSet
                .Where(p => p.ClientId == clientId && !p.IsDeleted)
                .Select(p => p.Id)
                .ToList();

            var clientUserIds = DataContext.UserIdentityClientSet
                .Where(a => a.ClientId == clientId)
                .Select(a => a.UserAccountId);

            var projectUserIds = DataContext.UserIdentityProjectSet
                .Where(a => a.ProjectId != null && clientProjectIds.Contains(a.ProjectId.Value))
                .Select(a => a.UserAccountId);

            var teamUserIds = clientUserIds.Union(projectUserIds).Distinct().ToList();

            var users = DataContext.UserAccountSet
                .Where(u => teamUserIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.AccountName
                })
                .OrderBy(u => u.Surname)
                .ThenBy(u => u.FirstName)
                .ToList();

            var userIds = users.Select(u => u.Id).ToList();

            var asOfRates = DataContext.BillingRatesSet
                .Where(r => userIds.Contains(r.UserAccountId)
                            && r.StartDate <= asOf
                            && r.EndDate >= asOf
                            && (
                                (r.ClientId == clientId && r.ProjectId == null)
                                || (r.ClientId == null && r.ProjectId == null)
                            ))
                .ToList();

            var projectOverrideCounts = DataContext.BillingRatesSet
                .Where(r => userIds.Contains(r.UserAccountId)
                            && r.ProjectId != null
                            && r.ClientId == null
                            && clientProjectIds.Contains(r.ProjectId.Value))
                .GroupBy(r => r.UserAccountId)
                .Select(g => new
                {
                    UserAccountId = g.Key,
                    Count = g.Select(x => x.ProjectId.Value).Distinct().Count()
                })
                .ToList()
                .ToDictionary(x => x.UserAccountId, x => x.Count);

            var team = new List<ClientTeamRateRow>();
            foreach (var user in users)
            {
                var clientRate = FindRateForDate(asOfRates, user.Id, asOf, clientId, null);
                var defaultRate = FindRateForDate(asOfRates, user.Id, asOf, null, null);

                decimal? effectiveRate = null;
                string effectiveScope = null;
                if (clientRate.HasValue)
                {
                    effectiveRate = clientRate;
                    effectiveScope = "Client";
                }
                else if (defaultRate.HasValue)
                {
                    effectiveRate = defaultRate;
                    effectiveScope = "Default";
                }

                int overrideCount;
                projectOverrideCounts.TryGetValue(user.Id, out overrideCount);

                team.Add(new ClientTeamRateRow
                {
                    UserAccountId = user.Id,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    AccountName = user.AccountName,
                    ClientRate = clientRate,
                    DefaultRate = defaultRate,
                    EffectiveRate = effectiveRate,
                    EffectiveScope = effectiveScope,
                    ProjectOverrideCount = overrideCount
                });
            }

            return new ClientTeamRatesResult
            {
                ClientId = client.Id,
                ClientName = client.EntityName,
                AsOfDate = asOf,
                Team = team
            };
        }

        public UserRatesForClientContextResult GetUserRatesForClientContext(Guid userAccountId, Guid clientId)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var client = DataContext.ClientEntitySet.FirstOrDefault(c => c.Id == clientId);
            if (client == null)
                throw new BillingRatesException("Selected Client was not found!");

            var user = DataContext.UserAccountSet.FirstOrDefault(u => u.Id == userAccountId);
            if (user == null)
                throw new BillingRatesException("Selected User was not found!");

            var clientProjects = DataContext.ProjectSet
                .Where(p => p.ClientId == clientId && !p.IsDeleted)
                .OrderBy(p => p.ProjectName)
                .Select(p => new ClientProjectOption
                {
                    ProjectId = p.Id,
                    ProjectName = p.ProjectName
                })
                .ToList();

            var clientProjectIds = clientProjects.Select(p => p.ProjectId).ToList();

            var clientRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == clientId && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var defaultRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId && r.ClientId == null && r.ProjectId == null)
                .OrderBy(r => r.StartDate)
                .ToList();

            var projectRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId
                            && r.ProjectId != null
                            && r.ClientId == null
                            && clientProjectIds.Contains(r.ProjectId.Value))
                .OrderBy(r => r.StartDate)
                .ToList();

            var projectNameLookup = clientProjects.ToDictionary(p => p.ProjectId, p => p.ProjectName);

            var projectRateGroups = projectRates
                .GroupBy(r => r.ProjectId.Value)
                .Select(g => new ClientProjectRateGroup
                {
                    ProjectId = g.Key,
                    ProjectName = projectNameLookup.ContainsKey(g.Key) ? projectNameLookup[g.Key] : g.Key.ToString(),
                    Rates = g.OrderBy(r => r.StartDate).ToList()
                })
                .OrderBy(g => g.ProjectName)
                .ToList();

            return new UserRatesForClientContextResult
            {
                UserAccountId = user.Id,
                UserName = (user.FirstName + " " + user.Surname).Trim(),
                ClientId = client.Id,
                ClientName = client.EntityName,
                ClientRates = clientRates,
                DefaultRates = defaultRates,
                ProjectRateGroups = projectRateGroups,
                ClientProjects = clientProjects
            };
        }

        public UserRatesAsOfResult GetUserRatesAsOf(Guid userAccountId, DateTime asOfDate)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var user = DataContext.UserAccountSet.FirstOrDefault(u => u.Id == userAccountId);
            if (user == null)
                throw new BillingRatesException("Selected User was not found!");

            var asOf = asOfDate.Date;

            var allRates = DataContext.BillingRatesSet
                .Where(r => r.UserAccountId == userAccountId)
                .ToList();

            var defaultRecord = FindRateRecordForDate(allRates, userAccountId, asOf, null, null);
            decimal? defaultRate = defaultRecord != null ? defaultRecord.Rate : (decimal?)null;

            var clientIdsFromRates = allRates
                .Where(r => r.ClientId != null && r.ProjectId == null)
                .Select(r => r.ClientId.Value)
                .Distinct()
                .ToList();

            var projectIdsFromRates = allRates
                .Where(r => r.ProjectId != null && r.ClientId == null)
                .Select(r => r.ProjectId.Value)
                .Distinct()
                .ToList();

            // Also include clients/projects the user is assigned to (As-of clarity only).
            var clientIdsFromAssignment = DataContext.UserIdentityClientSet
                .Where(a => a.UserAccountId == userAccountId && a.ClientId != null)
                .Select(a => a.ClientId.Value)
                .Distinct()
                .ToList();

            var projectIdsFromAssignment = DataContext.UserIdentityProjectSet
                .Where(a => a.UserAccountId == userAccountId && a.ProjectId != null)
                .Select(a => a.ProjectId.Value)
                .Distinct()
                .ToList();

            var allProjectIds = projectIdsFromRates.Union(projectIdsFromAssignment).Distinct().ToList();

            var projects = DataContext.ProjectSet
                .Where(p => allProjectIds.Contains(p.Id) && !p.IsDeleted)
                .Select(p => new
                {
                    p.Id,
                    p.ProjectName,
                    p.ClientId
                })
                .ToList();

            var clientIdsFromProjects = projects.Select(p => p.ClientId).Distinct().ToList();
            var allClientIds = clientIdsFromRates
                .Union(clientIdsFromAssignment)
                .Union(clientIdsFromProjects)
                .Distinct()
                .ToList();

            var clients = DataContext.ClientEntitySet
                .Where(c => allClientIds.Contains(c.Id))
                .Select(c => new { c.Id, c.EntityName })
                .OrderBy(c => c.EntityName)
                .ToList();

            var clientRows = new List<UserRatesAsOfClientRow>();
            foreach (var client in clients)
            {
                var clientRecord = FindRateRecordForDate(allRates, userAccountId, asOf, client.Id, null);
                decimal? clientRate = clientRecord != null ? clientRecord.Rate : (decimal?)null;

                decimal? clientEffective;
                string clientEffectiveScope;
                if (clientRate.HasValue)
                {
                    clientEffective = clientRate;
                    clientEffectiveScope = "Client";
                }
                else if (defaultRate.HasValue)
                {
                    clientEffective = defaultRate;
                    clientEffectiveScope = "Default";
                }
                else
                {
                    clientEffective = null;
                    clientEffectiveScope = null;
                }

                var clientProjects = projects
                    .Where(p => p.ClientId == client.Id)
                    .OrderBy(p => p.ProjectName)
                    .ToList();

                var projectRows = new List<UserRatesAsOfProjectRow>();
                foreach (var project in clientProjects)
                {
                    var projectRecord = FindRateRecordForDate(allRates, userAccountId, asOf, null, project.Id);
                    decimal? projectRate = projectRecord != null ? projectRecord.Rate : (decimal?)null;

                    decimal? projectEffective;
                    string projectEffectiveScope;
                    if (projectRate.HasValue)
                    {
                        projectEffective = projectRate;
                        projectEffectiveScope = "Project";
                    }
                    else if (clientRate.HasValue)
                    {
                        projectEffective = clientRate;
                        projectEffectiveScope = "Client";
                    }
                    else if (defaultRate.HasValue)
                    {
                        projectEffective = defaultRate;
                        projectEffectiveScope = "Default";
                    }
                    else
                    {
                        projectEffective = null;
                        projectEffectiveScope = null;
                    }

                    projectRows.Add(new UserRatesAsOfProjectRow
                    {
                        ProjectId = project.Id,
                        ProjectName = project.ProjectName,
                        ProjectRate = projectRate,
                        ProjectRateId = projectRecord != null ? projectRecord.Id : (Guid?)null,
                        EffectiveRate = projectEffective,
                        EffectiveScope = projectEffectiveScope
                    });
                }

                // Include from rate history, client assignment, or nested projects (rates/assignment)
                clientRows.Add(new UserRatesAsOfClientRow
                {
                    ClientId = client.Id,
                    ClientName = client.EntityName,
                    ClientRate = clientRate,
                    ClientRateId = clientRecord != null ? clientRecord.Id : (Guid?)null,
                    EffectiveRate = clientEffective,
                    EffectiveScope = clientEffectiveScope,
                    Projects = projectRows
                });
            }

            return new UserRatesAsOfResult
            {
                UserAccountId = user.Id,
                UserName = (user.FirstName + " " + user.Surname).Trim(),
                AsOfDate = asOf,
                DefaultRate = defaultRate,
                DefaultRateId = defaultRecord != null ? defaultRecord.Id : (Guid?)null,
                Clients = clientRows
            };
        }

        /// <summary>
        /// Finds the rate for a user on a date within a specific scope bucket.
        /// Pass clientId for client scope, projectId for project scope, or neither for default.
        /// </summary>
        private static decimal? FindRateForDate(IEnumerable<BillingRates> rates, Guid userAccountId, DateTime asOf,
            Guid? clientId, Guid? projectId)
        {
            var match = FindRateRecordForDate(rates, userAccountId, asOf, clientId, projectId);
            return match != null ? match.Rate : (decimal?)null;
        }

        private static BillingRates FindRateRecordForDate(IEnumerable<BillingRates> rates, Guid userAccountId,
            DateTime asOf, Guid? clientId, Guid? projectId)
        {
            if (projectId.HasValue)
            {
                return rates.FirstOrDefault(r =>
                    r.UserAccountId == userAccountId
                    && r.ProjectId == projectId
                    && r.ClientId == null
                    && r.StartDate.Date <= asOf
                    && r.EndDate.Date >= asOf);
            }

            if (clientId.HasValue)
            {
                return rates.FirstOrDefault(r =>
                    r.UserAccountId == userAccountId
                    && r.ClientId == clientId
                    && r.ProjectId == null
                    && r.StartDate.Date <= asOf
                    && r.EndDate.Date >= asOf);
            }

            return rates.FirstOrDefault(r =>
                r.UserAccountId == userAccountId
                && r.ClientId == null
                && r.ProjectId == null
                && r.StartDate.Date <= asOf
                && r.EndDate.Date >= asOf);
        }

        #endregion
    }
}
