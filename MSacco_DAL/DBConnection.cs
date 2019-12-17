using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msacco_DAL
{
  public class DBConnection
  {
    public DBConnection(string env = "Prod")
    {
      switch (env)
      {
        case "Prod":
          ConnectionString = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
          break;

        case "Test":
          // no test db for the moment
          ConnectionString = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
          break;
      }

    }

    public string ConnectionString { get; }
  }
}
