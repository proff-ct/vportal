namespace VisibilityPortal_DAL
{
  using System.Data.Entity;
  using System.Data.Entity.ModelConfiguration.Conventions;
  using VisibilityPortal_DAL.EF6_related;

  public partial class PortalDBContext : DbContext
  {
#if DEBUG
    public PortalDBContext()
        : base(new DBConnection("Test").ConnectionString)
    {
      Database.SetInitializer(new PortalDatabaseInitializer_DAL());
    }
#else
    public PortalDBContext()
        : base(new DBConnection().ConnectionString)
    {
     Database.SetInitializer(new PortalDatabaseInitializer_DAL());
    }
#endif
    public virtual DbSet<PortalModuleForClient> PortalModuleForClients { get; set; }
    public virtual DbSet<PortalModule> PortalModules { get; set; }
    public virtual DbSet<PortalUserRole> PortalUserRoles { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
    }
  }
}
