namespace CallCenter_DAL
{
  public partial class FloatResource: IDBModel
  {
    public static string DBName => "VisibilityPortal";
    public static string DBTableName => "FloatResource";

    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public enum FloatResources
    {
      B2C_MPESA,
      BULK_SMS
    };
  }
}
