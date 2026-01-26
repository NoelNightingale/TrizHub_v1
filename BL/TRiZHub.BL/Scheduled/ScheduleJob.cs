using Quartz;
using TRiZHub.BL.Context;

namespace TRiZHub.BL.Scheduled
{
    public abstract class ScheduleJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (var dbContext = new DataContext())
            {
                RunJob(dbContext);
            }
        }

        protected abstract void RunJob(DataContext db);
    }
}
