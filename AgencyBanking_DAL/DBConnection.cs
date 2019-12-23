using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
  public  class DBConnection
    {
        private string _connstring;
        public DBConnection()
        {
#if DEBUG
 _connstring = @ConfigurationManager.ConnectionStrings["testagency"].ConnectionString;
#else
/**
 * MatAdo edit:
 * Added this content to address the error: 
 * 'The ConnectionString property has not been initialized.'
**/
_connstring = @ConfigurationManager.ConnectionStrings["testagency"].ConnectionString;
#endif
}

public string ConnectionString
  {
      get { return _connstring; }
  }

}

}
