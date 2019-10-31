namespace VisibilityPortal_BLL.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class Extend_User_UserRole : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.AspNetRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
      AddColumn("dbo.AspNetUserRoles", "ClientModuleId", c => c.String());
      AddColumn("dbo.AspNetUserRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
      AddColumn("dbo.AspNetUsers", "ClientCorporateNo", c => c.String());
      AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
      AddColumn("dbo.AspNetUsers", "MiddleName", c => c.String());
      AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
      AddColumn("dbo.AspNetUsers", "CreatedBy", c => c.String());
      AddColumn("dbo.AspNetUsers", "CreatedOn", c => c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"));
      AddColumn("dbo.AspNetUsers", "ModifiedBy", c => c.String());
      AddColumn("dbo.AspNetUsers", "ModifiedOn", c => c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"));

    }

    public override void Down()
    {
      DropColumn("dbo.AspNetUsers", "ModifiedOn");
      DropColumn("dbo.AspNetUsers", "ModifiedBy");
      DropColumn("dbo.AspNetUsers", "CreatedOn");
      DropColumn("dbo.AspNetUsers", "CreatedBy");
      DropColumn("dbo.AspNetUsers", "LastName");
      DropColumn("dbo.AspNetUsers", "MiddleName");
      DropColumn("dbo.AspNetUsers", "FirstName");
      DropColumn("dbo.AspNetUsers", "ClientCorporateNo");
      DropColumn("dbo.AspNetUserRoles", "Discriminator");
      DropColumn("dbo.AspNetUserRoles", "ClientModuleId");
      DropColumn("dbo.AspNetRoles", "Discriminator");
    }
  }
}
