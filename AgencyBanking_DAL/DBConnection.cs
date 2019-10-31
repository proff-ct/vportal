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

#endif
        }

        public string ConnectionString
        {
            get { return _connstring; }
        }

    }
   
}
