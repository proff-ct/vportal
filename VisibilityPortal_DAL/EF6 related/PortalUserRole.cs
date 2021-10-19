using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisibilityPortal_DAL
{
  public partial class PortalUserRole : IDBModel
  {
    public static string DBName => "VisibilityPortal";
    public static string DBTableName => "PortalUserRole";
    //public string databaseName => DBName;

    //public string tableName => DBTableName;
  }
}
