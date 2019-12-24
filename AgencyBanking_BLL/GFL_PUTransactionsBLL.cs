using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgencyBanking_BLL.util;
using AgencyBanking_DAL;
using Dapper;

namespace AgencyBanking_BLL
{
    public class GFL_PUTransactionsBLL
  {
        private String table = PUTransactionModel.Table;

        /// <summary>
        /// Returns a list of GFL specific transactions from the PUTransactions
        /// </summary>
        /// <param name="orgNo"> the Cooperate Number</param>
        /// <returns>Dictionary of Data</returns>

        public IEnumerable<PUTransactionModel> GetPUTransactions()
        {

      string query =
                  "select [Entry No] as \"Entry_No\", [Account No] as \"Account_No\", " +
                  "[Transaction By] as \"Transaction_By\", [Transaction Date] as \"Transaction_Date\"" +
                  ", [Transaction Type] as \"Transaction_Type\", [Agent Code] as \"Agent_Code\", Comments, DeviceID" +
                  " from " + table;
            DynamicParameters par = new DynamicParameters();
      return DapperOrm.QueryGetList<PUTransactionModel>(query);
        }
    }
}

