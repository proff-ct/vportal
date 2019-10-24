using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL
{
  public static class Connection
  {
    public static string TestingConnectionString
    {
      get
      {
        return new DBConnection("Test").ConnectionString;
      }

    }
    public static string ProductionConnectionString
    {
      get
      {
        return new DBConnection().ConnectionString;
      }
    }

  }
}
