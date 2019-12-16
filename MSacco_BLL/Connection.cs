using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Msacco_DAL;

namespace MSacco_BLL
{
  public static class Connection
  {
    public static string TestingConnectionString => new DBConnection("Test").ConnectionString;
    public static string ProductionConnectionString => new DBConnection().ConnectionString;
  }
}
