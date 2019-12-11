using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AgencyBanking_DAL;
using Dapper;
using Microsoft.AspNet.Identity;

namespace AgencyBanking_BLL
{
 public   class RepairBLL
    {
        public void NewRepair(DeviceRepairModel model)
        {
            var sql = "insert into devicerepair ( [imei],[description],[saccoid] ) values (@imei,@desc,@saccoid)";
            DynamicParameters par1  = new DynamicParameters();
            par1.Add("imei",model.Imei);
            par1.Add("desc",model.Description);
            par1.Add("saccoid",model.Saccoid);

            DynamicParameters par2 = new DynamicParameters();
            par2.Add("userid",HttpContext.Current.User.Identity.GetUserId());
            par2.Add("imei",model.Imei);
            var sql2 = "insert into devicestatus ( imei,userid ) values(@imei,@userid)";

            if (DapperOrm.ExecuteInsert(sql, par1) > 0)
            {
                DapperOrm.ExecuteInsert(sql2, par2);
            }
            else
            {
                throw new Exception("There was a problem Executing your request");
            }

            
        }
    }
}
