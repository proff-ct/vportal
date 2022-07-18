using CallCenter_Dataspecs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_Dataspecs.Functions
{
  public interface IBL_SACCO
  {
    ISACCO GetSaccoById(int id);
    ISACCO GetSaccoByUniqueParam(string corporateNo = null, string saccoName = null);
    IEnumerable<ISACCO> GetSaccoList();

    string GetMSACCOModuleID(string corporateNo);
  }
}
