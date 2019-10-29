namespace VisibilityPortal_BLL.Migrations
{
  using System.Data.Entity.Migrations;
  using VisibilityPortal_BLL.InitialSetup.SetupDefaults;

  public partial class Add_SetupKey_To_Role : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.AspNetRoles", "SetupKey", c => c.String(
        defaultValue: ApplicationRoleDefaults.PASSKEY_DEFAULT));
    }

    public override void Down()
    {
      DropColumn("dbo.AspNetRoles", "SetupKey");
    }
  }
}
