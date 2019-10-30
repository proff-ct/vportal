using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL
{
  public  class SummaryBLL
    {
        private String table = SummaryModel.Table;
        /// <summary>
        /// Returns a summary of all the Transaction Types of a particular Sacco given 
        /// </summary>
        /// <param name="orgNo"> the Cooperate Number</param>
        /// <returns>Dictionary of Data</returns>
        
        public Dictionary<String, String> GetSummary(string orgNo=null)
        {
            //TODO: Update this query to include where clause i.e where cooperatenumber = orgNo 
          var  query = "select [Transaction Type] as \"TransactionType\", sum(amount) as \"Amount\" from [Agency Transactions] group by  [Transaction Type]";
            return DapperOrm.QueryGetList<SummaryModel>(query).ToDictionary(row=>row.TransactionType,row=>row.Amount);
        }
    }
}
