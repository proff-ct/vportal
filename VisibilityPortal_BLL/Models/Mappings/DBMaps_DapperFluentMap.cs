using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;
using Dapper.FluentMap.Mapping;

namespace VisibilityPortal_BLL.Models.Mappings
{
  class SaccoMap : EntityMap<Sacco>
  {
    internal SaccoMap()
    {
      Map(s => s.corporateNo).ToColumn("Corporate No");
      Map(s => s.corporateNo_2).ToColumn("Corporate No 2");
      Map(s => s.saccoName_1).ToColumn("Sacco Name 1");
    }
  }
}
