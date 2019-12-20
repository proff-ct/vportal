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
    public class SummaryBLL
    {
        private String table = SummaryModel.Table;

        /// <summary>
        /// Returns a summary of all the Transaction Types of a particular Sacco given 
        /// </summary>
        /// <param name="orgNo"> the Cooperate Number</param>
        /// <returns>Dictionary of Data</returns>

        public Dictionary<String, String> GetSummary(string orgNo)
        {
            string query =
                "select [Transaction Type] as \"TransactionType\", count(*) as \"Amount\" from [Agency Transactions]  where [bank code] = @code group by  [Transaction Type]";
            DynamicParameters par = new DynamicParameters();
            par.Add("code",orgNo);
            return DapperOrm.QueryGetList<SummaryModel>(query,par)
                .ToDictionary(row => row.TransactionType, row => row.Amount);
        }

        /// <summary>
        /// Returns a list of Transction for a particular Transaction type.
        /// </summary>
        /// <param name="type"> Transaction Type</param>
        /// <param name="bankno"></param>
        /// <returns></returns>
        public IEnumerable<TransactionModel> GetSummaryByName(string type, string bankno)
        {
            DynamicParameters par = new DynamicParameters();
            par.Add("code", bankno);
            string query =
                $"SELECT TOP(100) concat('Kshs ',[Amount]) as Amount, [Transaction By] as 'TransactionBy',[Transaction Date] as 'TransactionDate',[Transaction Time],[Transaction Type],[Transferred To Sacco], [Date Transferred To Sacco], [Time Transferred To Sacco], [Transferred To Sacco By], [Funds Source], [Receiver National ID No], [Receiver Name],[Receiver Telephone No], [Source Telephone No], [Bank Name], [Funds Received], [Date Funds Received], [Time Funds Received], [Confirmation Word], [Posted], [Date Posted], [Time Posted],[System Created Entry], [Transaction ID], [Description], [Agent Code], [Location], [OTTN], [OTTN Sent], [Date OTTN Sent], [Time OTTN Sent], [Balance SMS Sent], [Date SMS Sent], [Time SMS Sent], [Agent Name], [Depositer Telephone No], [Comments], [Account No 2], [ID No], [Society], [Society No], [Charge], [DeviceID]FROM[AGENCY BANKING].[dbo].[Agency Transactions] where [Transaction type]  = '{type}' and [bank code]=@code";
            return DapperOrm.QueryGetList<TransactionModel>(query,par);

        }
        /// <summary>
        ///  Gets shares and Deposit of a particular sacco
        /// </summary>
        /// <returns></returns>
        public string GetDeposit_vs_sharesDeposit(string OrgNo)
        {
            DynamicParameters par = new DynamicParameters();
            par.Add("code", OrgNo);
            return DapperOrm.QueryGetSingle<string>(
                "SELECT \r\nisnull(sum(Amount),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'\r\nFROM [AGENCY BANKING].[dbo].[Agency Transactions] \r\nwhere ([Transaction Type] = 'deposit' or [Transaction Type] = 'share deposit') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE()) and [bank code]= @code group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date])\r\n for json auto",par);
        }
        /// <summary>
        /// Returns the number of loans applications over a period of time
        /// </summary>
        /// <returns></returns>
        public string Loan_Application_Count(string OrgNo)
        {
            DynamicParameters par = new DynamicParameters();
            par.Add("code", OrgNo);

            return DapperOrm.QueryGetSingle<string>(
                "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Loan Application') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE())\r\n and [bank code] = @code group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto",par);
        }
        /// <summary>
        /// Returns membership count over a period of time
        /// </summary>
        /// <returns></returns>
        public string Member_Registration_Count(string OrgNo)
        {
            DynamicParameters par = new DynamicParameters();
            par.Add("code", OrgNo);
            return DapperOrm.QueryGetSingle<string>(
                "SELECT isnull(count(*),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Member Registration') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE())\r\n and [bank code] = @code group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto",par);

        }
        /// <summary>
        ///  Returns the number of loan repayment for the year
        /// </summary>
        /// <returns></returns>
        public string Loan_Repayment_Stats(string OrgNo)
        {
            DynamicParameters par = new DynamicParameters();
            par.Add("code", OrgNo);
            return DapperOrm.QueryGetSingle<string>("SELECT isnull(sum(amount),0) as 'total' ,DATEPART(year,[transaction date]) as 'year',DATeName(mm,[transaction date]) as 'month',[transaction type] as 'type'FROM [AGENCY BANKING].[dbo].[Agency Transactions] where ([Transaction Type] = 'Loan RePayment') and DATEPART(year,[transaction date]) = DATEPART(year, GETDATE()) and  [bank code] = @code \r\n group by DATEPART(year,[transaction date]),DATEPART(Month, [Transaction Date]),DATEname(mm,[transaction date]),[Transaction Type]  order by year,DATEPART(Month, [Transaction Date]) for json auto",par);
        }
    }
}

