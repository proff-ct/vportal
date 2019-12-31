namespace CallCenter_DAL.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class Add_FloatResource_AlertType_FloatResourceAlertForClient : DbMigration
  {
    public override void Up()
    {
      CreateTable(
          "dbo.AlertType",
          c => new
          {
            Id = c.Int(nullable: false, identity: true),
            AlertName = c.String(maxLength: 256),
            Description = c.String(maxLength: 256),
            CreatedBy = c.String(),
            CreatedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
            ModifiedBy = c.String(),
            ModifiedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
          })
          .PrimaryKey(t => t.Id);

      CreateTable(
          "dbo.FloatResource",
          c => new
          {
            Id = c.String(nullable: false, maxLength: 256),
            ResourceName = c.String(maxLength: 256),
            CreatedBy = c.String(),
            CreatedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
            ModifiedBy = c.String(),
            ModifiedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
          })
          .PrimaryKey(t => t.Id);

      CreateTable(
          "dbo.FloatResourceAlertForClient",
          c => new
          {
            Id = c.Int(nullable: false, identity: true),
            ClientCorporateNo = c.String(nullable: false, maxLength: 256),
            FloatResourceId = c.String(nullable: false, maxLength: 256),
            AlertTypeId = c.Int(nullable: false),
            Threshold = c.String(nullable: false),
            TriggerCondition = c.String(nullable: false),
            CreatedBy = c.String(),
            CreatedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
            ModifiedBy = c.String(),
            ModifiedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
          })
          .PrimaryKey(t => t.Id);

      

    }

    public override void Down()
    {
      DropTable("dbo.FloatResourceAlertForClient");
      DropTable("dbo.FloatResource");
      DropTable("dbo.AlertType");
    }
  }
}
