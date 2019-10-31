using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
  public  class DeviceInfo:IDbModel
    {

        public long ID;
        public string Name;
       public string IMEI;
       public string Enabled;
       public string TypeCode;
       public string TypeName;
       public string Assigned;
       public string Organization;
       public string Region;
        public static string Table
        {
            get { return "DeviceInfo"; }
        }


        public string TableName
        {
            get { return Table; }
        }
    }
}
