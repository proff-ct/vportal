using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL
{
  namespace MSSQLOperators
  {
    public class PaginationParameters
    {
      public PaginationParameters(int pageToLoad, int pageSize, string orderByColumn)
      {
        PageToLoad = pageToLoad;
        PageSize = pageSize;
        ColumnToOrderBy = orderByColumn;
      }

      public int PageToLoad { get; }
      public int PageSize { get; }
      public string ColumnToOrderBy { get; }
    }
  }
}
