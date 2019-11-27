namespace VisibilityPortal_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_AspRoleName_to_PortalUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PortalUserRole", "AspRoleName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PortalUserRole", "AspRoleName");
        }
    }
}
