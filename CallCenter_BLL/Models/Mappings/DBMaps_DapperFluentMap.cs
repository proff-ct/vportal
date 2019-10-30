using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;
using Dapper.FluentMap.Mapping;

namespace CallCenter_BLL.Models.Mappings
{
  class SaccoMap : EntityMap<Sacco>
  {
    internal SaccoMap()
    {
      Map(s => s.corporateNo).ToColumn("Corporate No");
      Map(s => s.corporateNo_2).ToColumn("Corporate No 2");
      Map(s => s.saccoName_1).ToColumn("Sacco Name 1");
      Map(s => s.mpesaFloat).ToColumn("Mpesa Float");
    }
  }
}
