namespace VisibilityPortal_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Route_Prefix_to_PortalModule_and_CreatedBy_to_PortalModuleForClient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortalModuleForClient",
                c => new
                    {
                        ClientModuleId = c.String(nullable: false, maxLength: 50),
                        ClientCorporateNo = c.String(nullable: false, maxLength: 50),
                        PortalModuleName = c.String(nullable: false, maxLength: 256),
                        IsActivated = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
                        ModifiedBy = c.String(),
                        ModifiedOn = c.DateTime(nullable: false, defaultValueSql: "SYSDATETIME()"),
                    })
                .PrimaryKey(t => t.ClientModuleId);
            
            CreateTable(
                "dbo.PortalModule",
                c => new
                    {
                        ModuleName = c.String(nullable: false, maxLength: 256),
                        CoreTecProductName = c.String(maxLength: 256),
                        RoutePrefix = c.String(maxLength: 256),
                    })
                .PrimaryKey(t => t.ModuleName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PortalModule");
            DropTable("dbo.PortalModuleForClient");
        }
    }
}
