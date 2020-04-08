using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallCenter_DAL;
using CallCenter_DAL.SMSFloat.Credit;
using CallCenter_DAL.SMSFloat.Debit.Archived;
using CallCenter_DAL.SMSFloat.Debit.Unarchived;

namespace CallCenter_BLL
{
  public class BulkSMSBLL
  {
    string _conString;
    string _query;
    string _tblBulkSMSCreditTrx = BulkSMSCredit.DBTableName;
    string _tblBulkSMSBalances = BulkSMSBalance.DBTableName;
    string _tblUnarchivedBulkSMSDebitTrx = UnarchivedBulkSMSDebit.DBTableName;
    string _tblArchivedBulkSMSDebitTrx = ArchivedBulkSMSDebit.DBTableName;
    //#if DEBUG
    //    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
    //#else
    //    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
    //#endif
    public BulkSMSCredit GetLatestCreditTrxForClient(string corporateNo)
    {
      _query = $@"SELECT TOP 1 * 
                FROM {_tblBulkSMSCreditTrx} 
                WHERE Sacco = '{corporateNo}' 
                ORDER BY[Datetime] DESC";

      return new DapperORM().QueryGetSingle<BulkSMSCredit>(_query);
    }

    public IEnumerable<BulkSMSCredit> GetCreditTrxListForClient(string corporateNo)
    {
      _query = $@"SELECT * 
                FROM {_tblBulkSMSCreditTrx} 
                WHERE Sacco = '{corporateNo}' 
                ORDER BY [Datetime] DESC";

      return new DapperORM().QueryGetList<BulkSMSCredit>(_query);
    }

    public IEnumerable<UnarchivedBulkSMSDebit> GetUnarchivedDebitTrxListForClient(string corporateNo)
    {
      _conString = @ConfigurationManager.ConnectionStrings["messagesDB_prod"].ConnectionString;
      _query = $@"SELECT * 
                FROM {_tblUnarchivedBulkSMSDebitTrx} 
                WHERE [Corporate No] = '{corporateNo}' 
                ORDER BY [Datetime] DESC";

      return new DapperORM(_conString).QueryGetList<UnarchivedBulkSMSDebit>(_query);
    }
    public IEnumerable<ArchivedBulkSMSDebit> GetArchivedDebitTrxListForClient(string corporateNo)
    {

      _conString = @ConfigurationManager.ConnectionStrings["archiveDB_prod"].ConnectionString;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(_conString);
      connectionStringBuilder.ConnectTimeout = 600; // timeout value is in sec
      _conString = connectionStringBuilder.ConnectionString;

      _query = $@"SELECT * 
                FROM {_tblArchivedBulkSMSDebitTrx} 
                WHERE [Corporate No] = '{corporateNo}' 
                ORDER BY [Datetime] DESC";

      return new DapperORM(_conString).QueryGetList<ArchivedBulkSMSDebit>(_query);
    }

    public BulkSMSBalance GetBulkSMSBalanceForClient(string corporateNo)
    {
      _query = $@"SELECT * 
                FROM {_tblBulkSMSBalances} 
                WHERE [Corporate No] = '{corporateNo}'";

      return new DapperORM().QueryGetSingle<BulkSMSBalance>(_query);
    }
    public BulkSMSBalance GetEarliestBulkSMSBalanceRecordForClient(string corporateNo)
    {
      List<BulkSMSBalance> records = new List<BulkSMSBalance>();
      _query = $@"SELECT * 
                FROM {_tblBulkSMSBalances} 
                WHERE [Corporate No] = '{corporateNo}'";

      records = new DapperORM().QueryGetList<BulkSMSBalance>(_query).ToList();

      // TO-DO: Execute an async delete of the other records, 08Apr2020_2001
      return records.OrderBy(r => r.Entry_No).First();
    }
  }
}
