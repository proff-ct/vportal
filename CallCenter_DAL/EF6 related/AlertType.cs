namespace CallCenter_DAL
{
  public partial class AlertType: IDBModel
  {
    public static string DBName => "VisibilityPortal";
    public static string DBTableName => "AlertType";

    //public string databaseName => DBName;
    //public string tableName => DBTableName;

    public enum AlertTypes
    {
      RED_ALERT,
      ALMOST_RED_ALERT,
      WARNING_ALERT
    };

    public enum TriggerConditions
    {
      GREATER_THAN,
      LESS_THAN,
      EQUAL_TO,
      EQUAL_OR_GREATER,
      EQUAL_OR_LESS
    }
  }
}
