namespace CallCenter_DAL
{
  using System.Configuration;
  using System.Data.Entity;
  using System.Data.Entity.ModelConfiguration.Conventions;
  using CallCenter_DAL.EF6_related;

  public partial class CallCenterDBContext : DbContext
  {
#if DEBUG
    public CallCenterDBContext()
        : base(@ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString)
    {
      Database.SetInitializer(new CallCenterDatabaseInitializer_DAL());
    }
#else
    public CallCenterDBContext()
        : base(@ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString)
    {
     Database.SetInitializer(new CallCenterDatabaseInitializer_DAL());
    }
#endif
    public virtual DbSet<AlertType> AlertTypes { get; set; }
    public virtual DbSet<FloatResource> FloatResources { get; set; }
    public virtual DbSet<FloatResourceAlertForClient> FloatResourceAlertsForClients { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
  }
}
