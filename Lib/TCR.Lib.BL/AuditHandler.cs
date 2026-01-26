#region Usings

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

#endregion

namespace TCR.Lib.BL
{
    public class AuditHandler
    {
        public static int SaveChanges<T>(DbContext context, DbSet<T> auditLogSet, string userName = "system",
            Guid? userId = null) where T : class
        {
            var changeTime = DateTime.UtcNow;

            if (userId == new Guid())
                userId = null;

            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            foreach (var x in from ent in context.ChangeTracker.Entries()
                .Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted
                            || p.State == EntityState.Modified)
                where (ent.Entity is IAuditedEntity)
                from x in GetAuditRecordsForChange<T>(ent, userId, userName, changeTime)
                select x)
            {
                auditLogSet.Add(x);
            }


            return context.SaveChanges();
        }

        private static IEnumerable<T> GetAuditRecordsForChange<T>(DbEntityEntry dbEntry, Guid? userId, string userName,
            DateTime changeTime) where T : class
        {
            var result = new List<T>();

            var tableAttr =
                dbEntry.Entity.GetType().GetCustomAttributes(typeof (TableAttribute), true).SingleOrDefault() as
                    TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            var tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var keyNames =
                dbEntry.Entity.GetType()
                    .GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof (KeyAttribute), false).Any())
                    .ToList();

            var keyName = string.Empty;
            if (keyNames != null && keyNames.Count > 0)
                keyName = keyNames[0].Name;

            Guid? recordId = null;
            if (!string.IsNullOrWhiteSpace(keyName))
            {
                try
                {
                    recordId = dbEntry.CurrentValues.GetValue<Guid>(keyName);
                }
                catch
                {
                    recordId = null;
                }
            }

            var record = Activator.CreateInstance<T>();
            var auditRecord = record as IAuditLog;
            auditRecord.Id = Guid.NewGuid();
            auditRecord.UserName = userName;
            auditRecord.UserId = userId;
            auditRecord.EventDate = changeTime;
            auditRecord.RecordId = recordId;
            auditRecord.TableName = tableName;

            switch (dbEntry.State)
            {
                case EntityState.Added:
                    auditRecord.EventType = AuditEventType.Insert; // Added
                    auditRecord.ColumnName = "*ALL";
                    auditRecord.NewValue = (dbEntry.Entity as IAuditedEntity).Describe();
                    result.Add(record);
                    break;
                case EntityState.Deleted:
                    auditRecord.EventType = AuditEventType.Delete; // Added
                    auditRecord.ColumnName = "*ALL";
                    auditRecord.NewValue = (dbEntry.OriginalValues.ToObject() as IAuditedEntity).Describe();
                    result.Add(record);

                    break;
                case EntityState.Modified:
                    var q = (from propertyName in dbEntry.OriginalValues.PropertyNames
                        where
                            !Equals(dbEntry.OriginalValues.GetValue<object>(propertyName),
                                dbEntry.CurrentValues.GetValue<object>(propertyName))
                        select new
                        {
                            Id = Guid.NewGuid(),
                            UserName = userName,
                            UserId = userId,
                            EventDate = changeTime,
                            EventType = AuditEventType.Modify,
                            TableName = tableName,
                            RecordId = recordId,
                            ColumnName = propertyName,
                            OriginalValue =
                                dbEntry.OriginalValues.GetValue<object>(propertyName) == null
                                    ? null
                                    : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue =
                                dbEntry.CurrentValues.GetValue<object>(propertyName) == null
                                    ? null
                                    : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                        }
                        ).Where(a => a.OriginalValue != a.NewValue);
                    foreach (var itm in q.ToList())
                    {
                        record = Activator.CreateInstance<T>();
                        auditRecord = record as IAuditLog;

                        auditRecord.Id = itm.Id;
                        auditRecord.UserName = itm.UserName;
                        auditRecord.UserId = itm.UserId;
                        auditRecord.EventDate = itm.EventDate;
                        auditRecord.EventType = itm.EventType;
                        auditRecord.TableName = itm.TableName;
                        auditRecord.RecordId = itm.RecordId;
                        auditRecord.ColumnName = itm.ColumnName;
                        auditRecord.OriginalValue = itm.OriginalValue;
                        auditRecord.NewValue = itm.NewValue;
                        result.Add(record);
                    }
                    break;
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

            return result;
        }
    }
}