namespace VisibilityPortal_BLL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Set_DBGenerated_Option_to_ApplicationUser_CreatedOn_ModifiedOn_fields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CreatedOn", c => c.DateTime(
              nullable: false, defaultValueSql: "SYSDATETIME()"));
            AlterColumn("dbo.AspNetUsers", "ModifiedOn", c => c.DateTime(
              nullable: false, defaultValueSql: "SYSDATETIME()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "ModifiedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
