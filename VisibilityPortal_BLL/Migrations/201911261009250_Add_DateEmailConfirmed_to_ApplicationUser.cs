namespace VisibilityPortal_BLL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_DateEmailConfirmed_to_ApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DateEmailConfirmed", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DateEmailConfirmed");
        }
    }
}
