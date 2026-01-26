#region Usings

using System;

#endregion

namespace TCR.Lib.BL
{
    public interface IAuditLog
    {
        Guid Id { get; set; }
        string UserName { get; set; }
        Guid? UserId { get; set; }
        DateTime EventDate { get; set; }
        AuditEventType EventType { get; set; }
        string TableName { get; set; }
        Guid? RecordId { get; set; }
        string ColumnName { get; set; }
        string NewValue { get; set; }
        string OriginalValue { get; set; }
    }
}