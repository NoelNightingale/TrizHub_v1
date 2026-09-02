#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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

        public IQueryable<BillingRates> BillingRatesFilterList(Guid? userAccountId, Guid? clientId, Guid? projectId,
            string scope = null, DateTime? activeOn = null,
            IList<Guid> userAccountIds = null, IList<Guid> clientIds = null, IList<Guid> projectIds = null)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var query = DataContext.BillingRatesSet.AsQueryable();

            var userIdList = NormalizeIds(userAccountIds);
            if (userIdList.Count > 0)
                query = query.Where(a => userIdList.Contains(a.UserAccountId));
            else if (userAccountId.HasValue && userAccountId.Value != Guid.Empty)
                query = query.Where(a => a.UserAccountId == userAccountId.Value);

            var scopeKey = (scope ?? string.Empty).Trim();
            if (string.Equals(scopeKey, "Default", StringComparison.OrdinalIgnoreCase))
                query = query.Where(a => a.ClientId == null && a.ProjectId == null);
            else if (string.Equals(scopeKey, "Client", StringComparison.OrdinalIgnoreCase))
                query = query.Where(a => a.ClientId != null && a.ProjectId == null);
            else if (string.Equals(scopeKey, "Project", StringComparison.OrdinalIgnoreCase))
                query = query.Where(a => a.ProjectId != null);

            var clientIdList = NormalizeIds(clientIds);
            if (clientIdList.Count > 0)
            {
                if (string.Equals(scopeKey, "Client", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(a => a.ClientId != null && clientIdList.Contains(a.ClientId.Value) && a.ProjectId == null);
                }
                else
                {
                    query = query.Where(a =>
                        (a.ClientId != null && clientIdList.Contains(a.ClientId.Value) && a.ProjectId == null)
                        || (a.ProjectId != null && clientIdList.Contains(a.Project.ClientId)));
                }
            }
            else if (clientId.HasValue && clientId.Value != Guid.Empty)
            {
                var cid = clientId.Value;
                if (string.Equals(scopeKey, "Client", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(a => a.ClientId == cid && a.ProjectId == null);
                }
                else
                {
                    query = query.Where(a =>
                        (a.ClientId == cid && a.ProjectId == null)
                        || (a.ProjectId != null && a.Project.ClientId == cid));
                }
            }

            var projectIdList = NormalizeIds(projectIds);
            if (projectIdList.Count > 0)
                query = query.Where(a => a.ProjectId != null && projectIdList.Contains(a.ProjectId.Value) && a.ClientId == null);
            else if (projectId.HasValue && projectId.Value != Guid.Empty)
                query = query.Where(a => a.ProjectId == projectId.Value && a.ClientId == null);

            if (activeOn.HasValue)
            {
                var onDate = activeOn.Value.Date;
                query = query.Where(a => a.StartDate <= onDate && a.EndDate >= onDate);
            }

            return query;
        }

        public BillingRatesFilterOptionsResult GetFilterOptions(IList<Guid> userAccountIds, IList<Guid> clientIds,
            IList<Guid> projectIds, string userStatus = null, string clientStatus = null,
            string projectStatus = null)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var selectedUsers = NormalizeIds(userAccountIds);
            var selectedClients = NormalizeIds(clientIds);
            var selectedProjects = NormalizeIds(projectIds);
            var userMode = NormalizeActiveStatus(userStatus);
            var clientMode = NormalizeActiveStatus(clientStatus);
            var projectMode = NormalizeActiveStatus(projectStatus);

            HashSet<Guid> allowedUsers = null;
            HashSet<Guid> allowedClients = null;
            HashSet<Guid> allowedProjects = null;

            if (selectedUsers.Count > 0)
            {
                var clientFromAssign = DataContext.UserIdentityClientSet
                    .Where(a => selectedUsers.Contains(a.UserAccountId) && a.ClientId != null)
                    .Select(a => a.ClientId.Value)
                    .Distinct()
                    .ToList();

                var projectFromAssign = DataContext.UserIdentityProjectSet
                    .Where(a => selectedUsers.Contains(a.UserAccountId) && a.ProjectId != null)
                    .Select(a => a.ProjectId.Value)
                    .Distinct()
                    .ToList();

                var clientsFromProjects = DataContext.ProjectSet
                    .Where(p => projectFromAssign.Contains(p.Id) && !p.IsDeleted)
                    .Select(p => p.ClientId)
                    .Distinct()
                    .ToList();

                // Assignment cascade: include all non-deleted projects; status filter applied on final list.
                var projectsFromClientAssign = DataContext.ProjectSet
                    .Where(p => clientFromAssign.Contains(p.ClientId) && !p.IsDeleted)
                    .Select(p => p.Id)
                    .Distinct()
                    .ToList();

                IntersectIds(ref allowedClients, clientFromAssign.Union(clientsFromProjects));
                IntersectIds(ref allowedProjects, projectFromAssign.Union(projectsFromClientAssign));
            }

            if (selectedClients.Count > 0)
            {
                var clientProjectIds = DataContext.ProjectSet
                    .Where(p => selectedClients.Contains(p.ClientId) && !p.IsDeleted)
                    .Select(p => p.Id)
                    .Distinct()
                    .ToList();

                var usersFromClient = DataContext.UserIdentityClientSet
                    .Where(a => a.ClientId != null && selectedClients.Contains(a.ClientId.Value))
                    .Select(a => a.UserAccountId);

                var usersFromProjects = DataContext.UserIdentityProjectSet
                    .Where(a => a.ProjectId != null && clientProjectIds.Contains(a.ProjectId.Value))
                    .Select(a => a.UserAccountId);

                IntersectIds(ref allowedUsers, usersFromClient.Union(usersFromProjects));
                IntersectIds(ref allowedProjects, clientProjectIds);
            }

            if (selectedProjects.Count > 0)
            {
                var owningClientIds = DataContext.ProjectSet
                    .Where(p => selectedProjects.Contains(p.Id))
                    .Select(p => p.ClientId)
                    .Distinct()
                    .ToList();

                var usersFromProject = DataContext.UserIdentityProjectSet
                    .Where(a => a.ProjectId != null && selectedProjects.Contains(a.ProjectId.Value))
                    .Select(a => a.UserAccountId);

                var usersFromOwningClient = DataContext.UserIdentityClientSet
                    .Where(a => a.ClientId != null && owningClientIds.Contains(a.ClientId.Value))
                    .Select(a => a.UserAccountId);

                IntersectIds(ref allowedUsers, usersFromProject.Union(usersFromOwningClient));
                IntersectIds(ref allowedClients, owningClientIds);
            }

            var usersQuery = DataContext.UserAccountSet
                .Where(u => u.FirstName != "Importer");
            if (userMode == "active")
                usersQuery = usersQuery.Where(u => u.Active);
            else if (userMode == "inactive")
                usersQuery = usersQuery.Where(u => !u.Active);
            if (allowedUsers != null)
                usersQuery = usersQuery.Where(u => allowedUsers.Contains(u.Id));

            var clientsQuery = DataContext.ClientEntitySet
                .Where(c => !c.IsDeleted);
            if (clientMode == "active")
                clientsQuery = clientsQuery.Where(c => c.IsActive);
            else if (clientMode == "inactive")
                clientsQuery = clientsQuery.Where(c => !c.IsActive);
            if (allowedClients != null)
                clientsQuery = clientsQuery.Where(c => allowedClients.Contains(c.Id));

            var projectsQuery = DataContext.ProjectSet
                .Where(p => !p.IsDeleted);
            if (projectMode == "active")
                projectsQuery = projectsQuery.Where(p => p.IsActive);
            else if (projectMode == "inactive")
                projectsQuery = projectsQuery.Where(p => !p.IsActive);
            if (allowedProjects != null)
                projectsQuery = projectsQuery.Where(p => allowedProjects.Contains(p.Id));

            return new BillingRatesFilterOptionsResult
            {
                Users = usersQuery
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.Surname)
                    .Select(u => new BillingRatesFilterOption
                    {
                        Id = u.Id,
                        Name = (u.FirstName + " " + u.Surname).Trim()
                    })
                    .ToList(),
                Clients = clientsQuery
                    .OrderBy(c => c.EntityName)
                    .Select(c => new BillingRatesFilterOption
                    {
                        Id = c.Id,
                        Name = c.EntityName
                    })
                    .ToList(),
                Projects = projectsQuery
                    .OrderBy(p => p.ProjectName)
                    .Select(p => new BillingRatesFilterOption
                    {
                        Id = p.Id,
                        Name = (p.ProjectNumber == null || p.ProjectNumber == "")
                            ? p.ProjectName
                            : ("[" + p.ProjectNumber + "] " + p.ProjectName)
                    })
                    .ToList()
            };
        }

        private static string NormalizeActiveStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "active";
            status = status.Trim().ToLowerInvariant();
            if (status == "inactive" || status == "all")
                return status;
            return "active";
        }

        public List<BillingRatesEffectiveRow> GetEffectiveRates(IList<Guid> userAccountIds, IList<Guid> clientIds,
            IList<Guid> projectIds, DateTime asOf)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            asOf = asOf.Date;
            var selectedUsers = NormalizeIds(userAccountIds);
            var selectedClients = NormalizeIds(clientIds);
            var selectedProjects = NormalizeIds(projectIds);

            var contexts = new List<EffectiveContext>();

            if (selectedProjects.Count > 0)
            {
                var projects = DataContext.ProjectSet
                    .Where(p => selectedProjects.Contains(p.Id) && !p.IsDeleted)
                    .Select(p => new
                    {
                        p.Id,
                        p.ProjectName,
                        p.ProjectNumber,
                        p.ClientId,
                        ClientName = p.Client.EntityName
                    })
                    .OrderBy(p => p.ProjectName)
                    .ToList();

                foreach (var p in projects)
                {
                    contexts.Add(new EffectiveContext
                    {
                        ClientId = p.ClientId,
                        ClientName = p.ClientName,
                        ProjectId = p.Id,
                        ProjectName = string.IsNullOrEmpty(p.ProjectNumber)
                            ? p.ProjectName
                            : ("[" + p.ProjectNumber + "] " + p.ProjectName)
                    });
                }
            }
            else if (selectedClients.Count > 0)
            {
                var clients = DataContext.ClientEntitySet
                    .Where(c => selectedClients.Contains(c.Id) && !c.IsDeleted)
                    .Select(c => new { c.Id, c.EntityName })
                    .OrderBy(c => c.EntityName)
                    .ToList();

                foreach (var c in clients)
                {
                    contexts.Add(new EffectiveContext
                    {
                        ClientId = c.Id,
                        ClientName = c.EntityName
                    });
                }
            }
            else
            {
                // No client/project filter: one default-scope context per user.
                contexts.Add(new EffectiveContext());
            }

            List<Guid> userIds;
            if (selectedUsers.Count > 0)
            {
                userIds = selectedUsers;
            }
            else if (selectedProjects.Count > 0)
            {
                var owningClientIds = contexts
                    .Where(c => c.ClientId.HasValue)
                    .Select(c => c.ClientId.Value)
                    .Distinct()
                    .ToList();

                var fromProject = DataContext.UserIdentityProjectSet
                    .Where(a => a.ProjectId != null && selectedProjects.Contains(a.ProjectId.Value))
                    .Select(a => a.UserAccountId);

                var fromClient = DataContext.UserIdentityClientSet
                    .Where(a => a.ClientId != null && owningClientIds.Contains(a.ClientId.Value))
                    .Select(a => a.UserAccountId);

                userIds = fromProject.Union(fromClient).Distinct().ToList();
            }
            else if (selectedClients.Count > 0)
            {
                var clientProjectIds = DataContext.ProjectSet
                    .Where(p => selectedClients.Contains(p.ClientId) && !p.IsDeleted && p.IsActive)
                    .Select(p => p.Id)
                    .Distinct()
                    .ToList();

                var fromClient = DataContext.UserIdentityClientSet
                    .Where(a => a.ClientId != null && selectedClients.Contains(a.ClientId.Value))
                    .Select(a => a.UserAccountId);

                var fromProjects = DataContext.UserIdentityProjectSet
                    .Where(a => a.ProjectId != null && clientProjectIds.Contains(a.ProjectId.Value))
                    .Select(a => a.UserAccountId);

                userIds = fromClient.Union(fromProjects).Distinct().ToList();
            }
            else
            {
                userIds = DataContext.UserAccountSet
                    .Where(u => u.Active && u.FirstName != "Importer")
                    .Select(u => u.Id)
                    .ToList();
            }

            if (userIds.Count == 0 || contexts.Count == 0)
                return new List<BillingRatesEffectiveRow>();

            var users = DataContext.UserAccountSet
                .Where(u => userIds.Contains(u.Id) && u.Active && u.FirstName != "Importer")
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.Surname,
                    u.AccountName
                })
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.Surname)
                .ToList();

            userIds = users.Select(u => u.Id).ToList();
            if (userIds.Count == 0)
                return new List<BillingRatesEffectiveRow>();

            var neededClientIds = contexts
                .Where(c => c.ClientId.HasValue)
                .Select(c => c.ClientId.Value)
                .Distinct()
                .ToList();
            var neededProjectIds = contexts
                .Where(c => c.ProjectId.HasValue)
                .Select(c => c.ProjectId.Value)
                .Distinct()
                .ToList();

            List<BillingRates> rates;
            if (neededProjectIds.Count > 0)
            {
                rates = DataContext.BillingRatesSet
                    .Where(r => userIds.Contains(r.UserAccountId)
                                && r.StartDate <= asOf
                                && r.EndDate >= asOf
                                && (
                                    (r.ClientId == null && r.ProjectId == null)
                                    || (r.ClientId != null && r.ProjectId == null &&
                                        neededClientIds.Contains(r.ClientId.Value))
                                    || (r.ProjectId != null && r.ClientId == null &&
                                        neededProjectIds.Contains(r.ProjectId.Value))
                                ))
                    .ToList();
            }
            else if (neededClientIds.Count > 0)
            {
                rates = DataContext.BillingRatesSet
                    .Where(r => userIds.Contains(r.UserAccountId)
                                && r.StartDate <= asOf
                                && r.EndDate >= asOf
                                && (
                                    (r.ClientId == null && r.ProjectId == null)
                                    || (r.ClientId != null && r.ProjectId == null &&
                                        neededClientIds.Contains(r.ClientId.Value))
                                ))
                    .ToList();
            }
            else
            {
                rates = DataContext.BillingRatesSet
                    .Where(r => userIds.Contains(r.UserAccountId)
                                && r.StartDate <= asOf
                                && r.EndDate >= asOf
                                && r.ClientId == null
                                && r.ProjectId == null)
                    .ToList();
            }

            var rows = new List<BillingRatesEffectiveRow>();
            foreach (var user in users)
            {
                foreach (var ctx in contexts)
                {
                    BillingRates winning = null;
                    string scope = null;

                    if (ctx.ProjectId.HasValue)
                    {
                        winning = FindRateRecordForDate(rates, user.Id, asOf, null, ctx.ProjectId);
                        if (winning != null)
                        {
                            scope = "Project";
                        }
                        else
                        {
                            winning = FindRateRecordForDate(rates, user.Id, asOf, ctx.ClientId, null);
                            if (winning != null)
                            {
                                scope = "Client";
                            }
                            else
                            {
                                winning = FindRateRecordForDate(rates, user.Id, asOf, null, null);
                                if (winning != null)
                                    scope = "Default";
                            }
                        }
                    }
                    else if (ctx.ClientId.HasValue)
                    {
                        winning = FindRateRecordForDate(rates, user.Id, asOf, ctx.ClientId, null);
                        if (winning != null)
                        {
                            scope = "Client";
                        }
                        else
                        {
                            winning = FindRateRecordForDate(rates, user.Id, asOf, null, null);
                            if (winning != null)
                                scope = "Default";
                        }
                    }
                    else
                    {
                        winning = FindRateRecordForDate(rates, user.Id, asOf, null, null);
                        if (winning != null)
                            scope = "Default";
                    }

                    rows.Add(new BillingRatesEffectiveRow
                    {
                        UserAccountId = user.Id,
                        FirstName = user.FirstName,
                        Surname = user.Surname,
                        AccountName = user.AccountName,
                        UserName = (user.FirstName + " " + user.Surname).Trim(),
                        ClientId = ctx.ClientId,
                        ClientName = ctx.ClientName,
                        ProjectId = ctx.ProjectId,
                        ProjectName = ctx.ProjectName,
                        EffectiveRate = winning != null ? winning.Rate : (decimal?)null,
                        EffectiveScope = scope,
                        RateId = winning != null ? winning.Id : (Guid?)null
                    });
                }
            }

            return rows;
        }

        public byte[] ExportBillingRatesExcel(IList<Guid> userAccountIds, IList<Guid> clientIds,
            IList<Guid> projectIds, string scope, DateTime? activeOn, string resultMode)
        {
            Authenticate(PrivilegeType.UserBillingRatesMaintenance);

            var effectiveMode = string.Equals(resultMode, "effective", StringComparison.OrdinalIgnoreCase);
            if (effectiveMode && !activeOn.HasValue)
                throw new BillingRatesException("Effective Date is required for Effective export.");

            using (var pck = new ExcelPackage())
            {
                var sheet = pck.Workbook.Worksheets.Add(effectiveMode ? "Effective Rates" : "Rate Periods");
                const string excelDateFormat = "yyyy/mm/dd";

                if (effectiveMode)
                {
                    var asOf = activeOn.Value.Date;
                    var rows = GetEffectiveRates(userAccountIds, clientIds, projectIds, asOf)
                        .OrderBy(r => r.UserName)
                        .ThenBy(r => r.ClientName)
                        .ThenBy(r => r.ProjectName)
                        .ToList();

                    sheet.Cells[1, 1].Value = "Billing Rates — Effective";
                    sheet.Cells[1, 1].Style.Font.Bold = true;
                    sheet.Cells[2, 1].Value = "Effective Date";
                    sheet.Cells[2, 2].Value = asOf;
                    sheet.Cells[2, 2].Style.Numberformat.Format = excelDateFormat;

                    var headerRow = 4;
                    sheet.Cells[headerRow, 1].Value = "User";
                    sheet.Cells[headerRow, 2].Value = "Client";
                    sheet.Cells[headerRow, 3].Value = "Project";
                    sheet.Cells[headerRow, 4].Value = "Effective Rate";
                    sheet.Cells[headerRow, 5].Value = "Source";
                    using (var range = sheet.Cells[headerRow, 1, headerRow, 5])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(236, 239, 241));
                    }

                    var rowIndex = headerRow;
                    foreach (var r in rows)
                    {
                        rowIndex++;
                        sheet.Cells[rowIndex, 1].Value = r.UserName;
                        sheet.Cells[rowIndex, 2].Value = r.ClientName;
                        sheet.Cells[rowIndex, 3].Value = r.ProjectName;
                        if (r.EffectiveRate.HasValue)
                            sheet.Cells[rowIndex, 4].Value = r.EffectiveRate.Value;
                        sheet.Cells[rowIndex, 5].Value = r.EffectiveScope;
                    }
                }
                else
                {
                    DateTime? asOf = activeOn.HasValue ? activeOn.Value.Date : (DateTime?)null;
                    var query = BillingRatesFilterList(null, null, null, scope, asOf,
                        userAccountIds, clientIds, projectIds);

                    var rows = query
                        .Select(a => new
                        {
                            UserName = a.UserAccount.FirstName + " " + a.UserAccount.Surname,
                            Scope = a.ProjectId != null ? "Project" : (a.ClientId != null ? "Client" : "Default"),
                            ClientName = a.Client != null ? a.Client.EntityName : null,
                            ProjectName = a.Project != null ? a.Project.ProjectName : null,
                            a.Rate,
                            a.StartDate,
                            a.EndDate
                        })
                        .OrderBy(r => r.UserName)
                        .ThenBy(r => r.StartDate)
                        .ToList();

                    sheet.Cells[1, 1].Value = "Billing Rates — Periods";
                    sheet.Cells[1, 1].Style.Font.Bold = true;
                    var headerRow = 3;
                    if (asOf.HasValue)
                    {
                        sheet.Cells[2, 1].Value = "Effective Date";
                        sheet.Cells[2, 2].Value = asOf.Value;
                        sheet.Cells[2, 2].Style.Numberformat.Format = excelDateFormat;
                        headerRow = 4;
                    }

                    sheet.Cells[headerRow, 1].Value = "User";
                    sheet.Cells[headerRow, 2].Value = "Scope";
                    sheet.Cells[headerRow, 3].Value = "Client";
                    sheet.Cells[headerRow, 4].Value = "Project";
                    sheet.Cells[headerRow, 5].Value = "Rate";
                    sheet.Cells[headerRow, 6].Value = "Start Date";
                    sheet.Cells[headerRow, 7].Value = "End Date";
                    using (var range = sheet.Cells[headerRow, 1, headerRow, 7])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(236, 239, 241));
                    }

                    var rowIndex = headerRow;
                    foreach (var r in rows)
                    {
                        rowIndex++;
                        sheet.Cells[rowIndex, 1].Value = (r.UserName ?? "").Trim();
                        sheet.Cells[rowIndex, 2].Value = r.Scope;
                        sheet.Cells[rowIndex, 3].Value = r.ClientName;
                        sheet.Cells[rowIndex, 4].Value = r.ProjectName;
                        sheet.Cells[rowIndex, 5].Value = r.Rate;
                        sheet.Cells[rowIndex, 6].Value = r.StartDate;
                        sheet.Cells[rowIndex, 6].Style.Numberformat.Format = excelDateFormat;
                        sheet.Cells[rowIndex, 7].Value = r.EndDate;
                        sheet.Cells[rowIndex, 7].Style.Numberformat.Format = excelDateFormat;
                    }
                }

                if (sheet.Dimension != null)
                {
                    for (var i = 1; i <= sheet.Dimension.End.Column; i++)
                    {
                        sheet.Column(i).AutoFit();
                        sheet.Column(i).Width += 2;
                    }
                }

                return pck.GetAsByteArray();
            }
        }

        private class EffectiveContext
        {
            public Guid? ClientId { get; set; }
            public string ClientName { get; set; }
            public Guid? ProjectId { get; set; }
            public string ProjectName { get; set; }
        }

        private static List<Guid> NormalizeIds(IList<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<Guid>();
            return ids.Where(id => id != Guid.Empty).Distinct().ToList();
        }

        private static void IntersectIds(ref HashSet<Guid> allowed, IEnumerable<Guid> candidates)
        {
            var next = new HashSet<Guid>(candidates);
            if (allowed == null)
                allowed = next;
            else
                allowed.IntersectWith(next);
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
