namespace VisibilityPortal_BLL.Migrations
{
  using System.Data.Entity.Migrations;
  using VisibilityPortal_BLL.InitialSetup.SetupDefaults;

  public partial class Add_SetupPassPhrase_To_Role : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.AspNetRoles", "SetupPassPhrase", c => c.String(
        defaultValue: ApplicationRoleDefaults.PASSPHRASE_DEFAULT));
    }

    public override void Down()
    {
      DropColumn("dbo.AspNetRoles", "SetupPassPhrase");
    }
  }
}
