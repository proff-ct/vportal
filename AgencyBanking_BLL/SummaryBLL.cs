using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgencyBanking_BLL.util;
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
            string query =
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
        public IEnumerable<TransactionModel> GetSummaryByName(string type, string bankno = null)
        {
            string query =
                $"SELECT TOP(100) concat('Kshs ',[Amount]) as Amount, [Transaction By] as 'TransactionBy',[Transaction Date] as 'TransactionDate',[Transaction Time],[Transaction Type],[Transferred To Sacco], [Date Transferred To Sacco], [Time Transferred To Sacco], [Transferred To Sacco By], [Funds Source], [Receiver National ID No], [Receiver Name],[Receiver Telephone No], [Source Telephone No], [Bank Name], [Funds Received], [Date Funds Received], [Time Funds Received], [Confirmation Word], [Posted], [Date Posted], [Time Posted],[System Created Entry], [Transaction ID], [Description], [Agent Code], [Location], [OTTN], [OTTN Sent], [Date OTTN Sent], [Time OTTN Sent], [Balance SMS Sent], [Date SMS Sent], [Time SMS Sent], [Agent Name], [Depositer Telephone No], [Comments], [Account No 2], [ID No], [Society], [Society No], [Charge], [DeviceID]FROM[AGENCY BANKING].[dbo].[Agency Transactions] where [Transaction type]  = '{type}'";
            return DapperOrm.QueryGetList<TransactionModel>(query);

        }
        /// <summary>
        ///  TODO: In future we need to use sacco id
        /// </summary>
        /// <returns></returns>
        public string GetDeposit_vs_sharesDeposit()
        {
            return DapperOrm.QueryGetSingle<string>(
                "SELECT \r\nisnull(sum(Amount),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'\r\nFROM [AGENCY BANKING].[dbo].[Agency Transactions] \r\nwhere ([Transaction Type] = 'deposit' or [Transaction Type] = 'share deposit') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE()) group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date])\r\n for json auto");
        }
        /// <summary>
        /// TODO: In future we need to use sacco id
        /// </summary>
        /// <returns></returns>
        public string Loan_Application_Count()
        {
            return DapperOrm.QueryGetSingle<string>(
                "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Loan Application') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE())\r\n group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto");
        }
        /// <summary>
        ///  TODO: In future we need to use sacco id
        /// </summary>
        /// <returns></returns>
        public string Member_Registration_Count()
        {
            return DapperOrm.QueryGetSingle<string>(
                "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Member Registration') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE())\r\n group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto");

        }
        /// <summary>
        ///  TODO: In future we need to use sacco id
        /// </summary>
        /// <returns></returns>
        public string Loan_Repayment_Stats()
        {
            return DapperOrm.QueryGetSingle<string>("SELECT isnull(sum(amount),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Loan RePayment') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE())\r\n group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto");

        }
    }
}

