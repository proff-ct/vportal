using System.Configuration;

namespace VisibilityPortal_DAL
{
  public class DBConnection
  {
    public DBConnection(string env = "Prod")
    {
      switch (env)
      {
        case "Prod":
          /*_conString = @"Data Source=MATT-HP-PAV-450;Initial Catalog=VisibilityPortal;Integrated Security=True";*/
          ConnectionString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
          break;

        case "Test":
          //_conString = @"Data Source=MATT-HP-PAV-450;Initial Catalog=Test_VisibilityPortal;Integrated Security=True";
          ConnectionString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
          break;
      }

    }

    public string ConnectionString { get; }

  }
}
