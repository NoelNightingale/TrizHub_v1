namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using TRiZHub.BL.Entities.ProjectData;

    internal sealed class Configuration : DbMigrationsConfiguration<TRiZHub.BL.Context.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TRiZHub.BL.Context.DataContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var messages = "";

            try
            {
                context.ProjectTypeSet.AddOrUpdate(p => p.Name,
                new ProjectType() { Name = "Admin", Description = "Admin", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 1 },
                new ProjectType() { Name = "Flex Engineering", Description = "Flex", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = true, SortOrder = 2 },
                new ProjectType() { Name = "Leave (Sick)", Description = "Leave (Sick)", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 3 },
                new ProjectType() { Name = "Leave (Study)", Description = "Leave (Study)", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 4 },
                new ProjectType() { Name = "Leave (Vacation)", Description = "Leave (Vacation)", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 5 },
                new ProjectType() { Name = "Leave (Other)", Description = "Leave (Other)", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 6 },
                new ProjectType() { Name = "Normal", Description = "Normal", AllowSubProjectAlternativeType = true, AllowSubProjectBillable = true, SortOrder = 7 },
                new ProjectType() { Name = "Non-Eligible", Description = "NE", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 8 },
                new ProjectType() { Name = "Non-Invoiceable Engineering", Description = "Non-Invoiceable Engineering", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 9 },
                new ProjectType() { Name = "System", Description = "System", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 10 },
                new ProjectType() { Name = "Training", Description = "Training", AllowSubProjectAlternativeType = false, AllowSubProjectBillable = false, SortOrder = 11 }
                );

                messages += "Added Project Types Types";

                // Set project types to default
                if (context.ProjectSet.Count(p => p.ProjectTypeId == null) > 0)
                {
                    context.ProjectSet.Where(p => p.ProjectTypeId == null).ToList().ForEach(p => p.ProjectTypeId = context.ProjectTypeSet.Where(pt => pt.Name == "Normal").FirstOrDefault().Id);
                }

                messages += ", Added Project Types To Projects";

                // Check That Sub Projects have names
                if (context.SubProjectSet.Count(p => p.ProjectName == null || p.ProjectName == "") > 0)
                {
                    context.SubProjectSet.Where(p => p.ProjectName == null || p.ProjectName == "").ToList().ForEach(p => p.ProjectName = "PROJECT NAME ADDED BY SYSTEM");
                }

                messages += "Performed Project Name Check";

                // Set sub project types to default
                if (context.SubProjectSet.Count(p => p.SubProjectTypeId == null) > 0)
                {
                    context.SubProjectSet.Where(p => p.SubProjectTypeId == null).ToList().ForEach(p => p.SubProjectTypeId = context.ProjectTypeSet.Where(pt => pt.Name == "Normal").FirstOrDefault().Id);
                }

                messages += ", Added Project Types To Sub Projects";

                if (context.EmployerSet.Count() == 0)
                {
                    context.EmployerSet.AddOrUpdate(p => p.Name,
                    new Entities.EmployerData.Employer() { Id = Guid.NewGuid(), Name = "Triz SA", IsActive = true, IsDeleted = false, DateCreated = DateTime.Now },
                    new Entities.EmployerData.Employer() { Id = Guid.NewGuid(), Name = "Triz USA", IsActive = true, IsDeleted = false, DateCreated = DateTime.Now }
                    );
                    context.SaveChanges();

                    // Migrate old location entities to employers
                    context.TeamJobDesignationSet.Where(p => p.Employer == null).ToList().ForEach(p => p.EmployerId = context.EmployerSet.Where(es => es.Name == p.Location).FirstOrDefault().Id);

                    messages += ", Added Employers";
                }

                context.SaveChanges();
            }
            catch (Exception e)
            {
                Exception ex = new Exception(messages, e);

                throw ex;
            }
        }
    }
}