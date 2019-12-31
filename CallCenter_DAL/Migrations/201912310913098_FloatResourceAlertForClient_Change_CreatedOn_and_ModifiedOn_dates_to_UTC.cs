namespace CallCenter_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FloatResourceAlertForClient_Change_CreatedOn_and_ModifiedOn_dates_to_UTC : DbMigration
    {
        public override void Up()
        {
            AlterColumn(
              "dbo.FloatResourceAlertForClient",
              "CreatedOn",
              c => c.DateTime(
                nullable: false, 
                precision: 7,
                storeType: "datetime2",
                defaultValueSql: "SYSUTCDATETIME()"));
            AlterColumn(
              "dbo.FloatResourceAlertForClient", 
              "ModifiedOn",
              c => c.DateTime(
                nullable: false,
                precision: 7,
                storeType: "datetime2",
                defaultValueSql: "SYSUTCDATETIME()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FloatResourceAlertForClient", "ModifiedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.FloatResourceAlertForClient", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
