namespace VisibilityPortal_BLL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Annotate_ClientModuleId_as_Key_and_Add_AppRoles_isDefaultPassword_to_ApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsDefaultPassword", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsDefaultPassword");
        }
    }
}
