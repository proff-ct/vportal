using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AgencyBanking_BLL.util
{
    public class StatusMessage
    {
        public string Code { get; set; }
        public  string Message { get; set; }


        public string toJson()
        {
            return JObject.FromObject(this).ToString();
        }
    }
}
