namespace VisibilityPortal_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PortalUserRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortalUserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientModuleId = c.String(maxLength: 50),
                        UserId = c.String(maxLength: 50),
                        AspRoleId = c.String(maxLength: 50),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
                        ModifiedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
                    })
                .PrimaryKey(t => t.Id);

    }
    
        
        public override void Down()
        {
            DropTable("dbo.PortalUserRole");
        }
    }
}
