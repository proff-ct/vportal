namespace VisibilityPortal_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsEnabled_boolean_property_to_PortalUserRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PortalUserRole", "IsEnabled", c => c.Boolean(nullable: false, defaultValueSql:"1"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PortalUserRole", "IsEnabled");
        }
    }
}
