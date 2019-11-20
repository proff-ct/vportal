namespace VisibilityPortal_DAL
{
  public interface IDBModel
  {
    string databaseName { get; }
    string tableName { get; }

  }

  public enum ModelOperation
  {
    AddNew,
    Update,
    Delete
  }
}
