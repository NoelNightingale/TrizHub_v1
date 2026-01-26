namespace TRiZHub.BL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeScoreCardSchema : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScorecardPeriod", "ScorecardId", "dbo.Scorecard");
            DropForeignKey("dbo.ScorecardRecord", "ScorecardPeriodId", "dbo.ScorecardPeriod");
            DropForeignKey("dbo.ScorecardTemplateItemScore", "ScorecardTemplateItemId", "dbo.ScorecardTemplateItem");
            DropForeignKey("dbo.ScorecardPeriod", "ScorecardTemplatePeriodId", "dbo.ScorecardTemplatePeriod");
            DropIndex("dbo.ScorecardPeriod", new[] { "ScorecardId" });
            DropIndex("dbo.ScorecardPeriod", new[] { "ScorecardTemplatePeriodId" });
            DropIndex("dbo.ScorecardRecord", new[] { "ScorecardPeriodId" });
            DropIndex("dbo.ScorecardTemplateItemScore", new[] { "ScorecardTemplateItemId" });
            AddColumn("dbo.Scorecard", "ScorecardTemplatePeriodId", c => c.Guid(nullable: false));
            AddColumn("dbo.Scorecard", "Rated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Scorecard", "Completed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Scorecard", "EvaluatorMessage", c => c.String());
            AddColumn("dbo.Scorecard", "EmployeeMessage", c => c.String());
            AddColumn("dbo.ScorecardRecord", "ScorecardId", c => c.Guid(nullable: false));
            AddColumn("dbo.ScorecardTemplateItem", "Order", c => c.String());
            AddColumn("dbo.ScorecardTemplateItem", "ExcellentDefinition", c => c.String());
            AddColumn("dbo.ScorecardTemplateItem", "AdequateDefinition", c => c.String());
            AddColumn("dbo.ScorecardTemplateItem", "InadequateDefinition", c => c.String());
            AddColumn("dbo.ScorecardTemplate", "ExcellentWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ScorecardTemplate", "AdequateWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ScorecardTemplate", "InadequateWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("dbo.Scorecard", "ScorecardTemplatePeriodId");
            CreateIndex("dbo.ScorecardRecord", "Id");
            AddForeignKey("dbo.Scorecard", "ScorecardTemplatePeriodId", "dbo.ScorecardTemplatePeriod", "Id");
            AddForeignKey("dbo.ScorecardRecord", "Id", "dbo.Scorecard", "Id");
            DropColumn("dbo.ScorecardRecord", "ScorecardPeriodId");
            DropColumn("dbo.ScorecardTemplateItem", "Code");
            DropColumn("dbo.ScorecardTemplateItem", "ScorecardScoring");
            DropTable("dbo.ScorecardPeriod");
            DropTable("dbo.ScorecardTemplateItemScore");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ScorecardTemplateItemScore",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardTemplateItemId = c.Guid(nullable: false),
                        ScoreType = c.Int(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Definition = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScorecardPeriod",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ScorecardId = c.Guid(nullable: false),
                        ScorecardTemplatePeriodId = c.Guid(nullable: false),
                        Rated = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Completed = c.Boolean(nullable: false),
                        EvaluatorMessage = c.String(),
                        EmployeeMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ScorecardTemplateItem", "ScorecardScoring", c => c.Int(nullable: false));
            AddColumn("dbo.ScorecardTemplateItem", "Code", c => c.String());
            AddColumn("dbo.ScorecardRecord", "ScorecardPeriodId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.ScorecardRecord", "Id", "dbo.Scorecard");
            DropForeignKey("dbo.Scorecard", "ScorecardTemplatePeriodId", "dbo.ScorecardTemplatePeriod");
            DropIndex("dbo.ScorecardRecord", new[] { "Id" });
            DropIndex("dbo.Scorecard", new[] { "ScorecardTemplatePeriodId" });
            DropColumn("dbo.ScorecardTemplate", "InadequateWeight");
            DropColumn("dbo.ScorecardTemplate", "AdequateWeight");
            DropColumn("dbo.ScorecardTemplate", "ExcellentWeight");
            DropColumn("dbo.ScorecardTemplateItem", "InadequateDefinition");
            DropColumn("dbo.ScorecardTemplateItem", "AdequateDefinition");
            DropColumn("dbo.ScorecardTemplateItem", "ExcellentDefinition");
            DropColumn("dbo.ScorecardTemplateItem", "Order");
            DropColumn("dbo.ScorecardRecord", "ScorecardId");
            DropColumn("dbo.Scorecard", "EmployeeMessage");
            DropColumn("dbo.Scorecard", "EvaluatorMessage");
            DropColumn("dbo.Scorecard", "Completed");
            DropColumn("dbo.Scorecard", "Rated");
            DropColumn("dbo.Scorecard", "ScorecardTemplatePeriodId");
            CreateIndex("dbo.ScorecardTemplateItemScore", "ScorecardTemplateItemId");
            CreateIndex("dbo.ScorecardRecord", "ScorecardPeriodId");
            CreateIndex("dbo.ScorecardPeriod", "ScorecardTemplatePeriodId");
            CreateIndex("dbo.ScorecardPeriod", "ScorecardId");
            AddForeignKey("dbo.ScorecardPeriod", "ScorecardTemplatePeriodId", "dbo.ScorecardTemplatePeriod", "Id");
            AddForeignKey("dbo.ScorecardTemplateItemScore", "ScorecardTemplateItemId", "dbo.ScorecardTemplateItem", "Id");
            AddForeignKey("dbo.ScorecardRecord", "ScorecardPeriodId", "dbo.ScorecardPeriod", "Id");
            AddForeignKey("dbo.ScorecardPeriod", "ScorecardId", "dbo.Scorecard", "Id");
        }
    }
}
