namespace VisibilityPortal_BLL.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class Add_Id_to_ApplicationUserRole : DbMigration
  {
    public override void Up()
    {
      DropPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
      AddColumn("dbo.AspNetUserRoles", "Id", c => c.Int(identity: true));
      AddPrimaryKey("dbo.AspNetUserRoles", new[] { "Id", "UserId", "RoleId" });
    }

    public override void Down()
    {
      DropPrimaryKey("dbo.AspNetUserRoles", new[] { "Id", "UserId", "RoleId" });
      DropColumn("dbo.AspNetUserRoles", "Id");
      AddPrimaryKey("dbo.AspNetUserRoles", new[] { "UserId", "RoleId" });
    }
  }
}
