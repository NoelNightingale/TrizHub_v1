using Quartz;
using TRiZHub.BL.Context;
using TRiZHub.BL.Provider.Email;
using TRiZHub.BL.Provider.Settings;

namespace TRiZHub.BL.Scheduled
{
    [DisallowConcurrentExecution]
    public class EmailQueueJob : ScheduleJob
    {
        public static int ScheduledMinutes { get { return 1; } }

        protected override void RunJob(DataContext db)
        {
            var provider = new EmailProvider(db, new AppSettings(db));
            provider.ProcessQueue();
        }
    }
}
