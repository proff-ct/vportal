namespace VisibilityPortal_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename_IsActivated_to_IsEnabled_on_PortalModuleForClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PortalModuleForClient", "IsEnabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.PortalModuleForClient", "IsActivated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PortalModuleForClient", "IsActivated", c => c.Boolean(nullable: false));
            DropColumn("dbo.PortalModuleForClient", "IsEnabled");
        }
    }
}
