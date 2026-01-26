#region Usings

using System;
using TCR.Lib.BL;

#endregion

namespace TRiZHub.BL.Entities.Logging
{
    public class SystemLog : DbEntity
    {
        public DateTime EventTime { get; set; }
        public string Sender { get; set; }
        public Guid? UserIdentityId { get; set; }
        public LogEventType EventType { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string InnerExceptionStackTrace { get; set; }
    }
}