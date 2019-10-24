using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FluentMap.Mapping;

namespace VisibilityPortal_DAL
{
  class CoreTecClientMap : EntityMap<CoreTecClient>
  {
    internal CoreTecClientMap()
    {
      Map(c => c.corporate_no).ToColumn("[Corporate No]");
      Map(c => c.corporate_no_2).ToColumn("[Corporate No 2]");
      Map(c => c.sacco_name_1).ToColumn("[Sacco Name 1]");
    }
  }
}
