using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL
{
  public interface IDBModel
  {
    //string databaseName { get; }
    //string tableName { get; }

  }

  public enum ModelOperation
  {
    AddNew,
    Update,
    Delete
  }
}
