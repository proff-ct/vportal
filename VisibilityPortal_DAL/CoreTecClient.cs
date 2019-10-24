using System.Collections.Generic;

namespace VisibilityPortal_DAL
{
  public class CoreTecClient : IDBModel
  {
    public static string DBName => "SACCO";
    public static string DBTableName => "Source Information";
    public string corporate_no { get; set; }
    public string corporate_no_2 { get; set; }
    public string sacco_name_1 { get; set; }
    public bool active { get; set; }

    public virtual IEnumerable<ClientUser> users { get; set; }

    public string databaseName => DBName;
    public string tableName => DBTableName;
  }
}
