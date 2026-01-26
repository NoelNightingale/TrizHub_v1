#region Usings

using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using Quartz.Impl;
using TRiZHub.BL.Context;
using TRiZHub.BL.Scheduled;

#endregion

namespace TRiZHub
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DataContext.Setup();
            ConfigureScheduledJobs();
        }

        private void ConfigureScheduledJobs()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            var sched = schedFact.GetScheduler();
            sched.Start();

            //ScheduleJob<EmailQueueJob>(sched, "EmailQueueJob", EmailQueueJob.ScheduledMinutes);
            ScheduleJob<EmailQueueJob>(sched, "EmailQueueJob", Convert.ToInt32(5));
        }

        private static void ScheduleJob<T>(IScheduler sched, string jobId, int seconds = 5) where T : IJob
        {
            var theJob = JobBuilder.Create<T>()
                .WithIdentity(jobId)
                .WithDescription(jobId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .ForJob(theJob)
                .WithCronSchedule("0/" + seconds + " * * * * ?")
                .WithIdentity(jobId + "trigger")
                .StartNow()
                .Build();

            sched.ScheduleJob(theJob, trigger);
            sched.Start();
        }
    }
}