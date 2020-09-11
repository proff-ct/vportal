using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_Dataspecs.Models
{
  public interface ISACCO
  {
    string corporateNo { get; set; }
    string corporateNo_2 { get; set; }
    string databaseName { get; }
    double mpesaFloat { get; set; }
    string saccoName_1 { get; set; }
    bool isActive { get; set; }
    string tableName { get; }
  }
}
