namespace MSacco_Dataspecs.MSSQLOperators
{
  public interface IPaginationParameters
  {
    string ColumnToOrderBy { get; }
    int PageSize { get; }
    int PageToLoad { get; }
  }
}