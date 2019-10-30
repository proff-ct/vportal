using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
    /// <summary>
    /// This model is used to get and store the Summary of each Transaction type from the database
    /// </summary>
   public class SummaryModel
    {

        public string TransactionType { get; set; } 
        public string Amount { get; set; }


        public static string Table
        {
            get
            {
                return "[Agency Transactions]";
            }
        }
    }
}
