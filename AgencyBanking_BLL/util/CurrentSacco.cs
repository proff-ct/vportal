using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using Utilities.PortalApplicationParams;

namespace AgencyBanking_BLL.util
{
    public class CurrentSacco
    {
        public static string CorporateNo
        {

            get
            {
                try
                {
                    ActiveUserParams pActiveUserParams =
                        HttpContext.Current.Session[ActiveUserParams.SessionVaribleName()] as ActiveUserParams;
                    return pActiveUserParams.ClientCorporateNo ?? "";
                }
                catch (Exception)
                {
                    return "";
                }

            }
        }
        /// <summary>
        /// Returns the Sacco Name
        /// </summary>
        public static string SaccoName
        {
            get
            {
                //Get the session name
                String sacconame = HttpContext.Current.Session["SaccoName"] as string;
                //if Key SaccoName is null query it from the Database
                if (string.IsNullOrEmpty(sacconame))
                {
                    sacconame = GetSaccoNameFromDB();
                }
                return sacconame;
            }
        }

        private static string GetSaccoNameFromDB()
        {
            string connectionstring = "";
#if DEBUG
            connectionstring = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
#else
connectionstring = @ConfigurationManager.ConnectionStrings["saccoDB_prod"].ConnectionString;
#endif
            var sql =
                "select [Sacco Name 1] from [source information] where [Corporate No] = @number or [Corporate No 2] = @number";
            var par = new DynamicParameters();
            par.Add("number", CorporateNo);
            return DapperOrm.QueryGetSingle<string>(connectionstring, sql, par);
        }
    }
}
