using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgencyBanking_DAL;

namespace AgencyBanking_BLL
{
    public class SummaryBLL
    {
        private String table = SummaryModel.Table;

        /// <summary>
        /// Returns a summary of all the Transaction Types of a particular Sacco given 
        /// </summary>
        /// <param name="orgNo"> the Cooperate Number</param>
        /// <returns>Dictionary of Data</returns>

        public Dictionary<String, String> GetSummary(string orgNo = null)
        {
            //TODO: Update this query to include where clause i.e where cooperatenumber = orgNo 
            var query =
                "select [Transaction Type] as \"TransactionType\", sum(amount) as \"Amount\" from [Agency Transactions] group by  [Transaction Type]";
            return DapperOrm.QueryGetList<SummaryModel>(query)
                .ToDictionary(row => row.TransactionType, row => row.Amount);
        }
        /// <summary>
        /// Returns a list of Transction for a particular Transaction type.
        /// </summary>
        /// <param name="type"> Transaction Type</param>
        /// <param name="bankno"></param>
        /// <returns></returns>
        public IEnumerable<TransactionModel> GetSummaryByName(string type, string bankno=null)
        {
            var query =
                $"SELECT TOP(100) concat('Kshs ',[Amount]) as Amount, [Transaction By] as 'TransactionBy',[Transaction Date] as 'TransactionDate',[Transaction Time],[Transaction Type],[Transferred To Sacco], [Date Transferred To Sacco], [Time Transferred To Sacco], [Transferred To Sacco By], [Funds Source], [Receiver National ID No], [Receiver Name],[Receiver Telephone No], [Source Telephone No], [Bank Name], [Funds Received], [Date Funds Received], [Time Funds Received], [Confirmation Word], [Posted], [Date Posted], [Time Posted],[System Created Entry], [Transaction ID], [Description], [Agent Code], [Location], [OTTN], [OTTN Sent], [Date OTTN Sent], [Time OTTN Sent], [Balance SMS Sent], [Date SMS Sent], [Time SMS Sent], [Agent Name], [Depositer Telephone No], [Comments], [Account No 2], [ID No], [Society], [Society No], [Charge], [DeviceID]FROM[AGENCY BANKING].[dbo].[Agency Transactions] where [Transaction type]  = '{type}'";
            return DapperOrm.QueryGetList<TransactionModel>(query);

        }
    }
}
