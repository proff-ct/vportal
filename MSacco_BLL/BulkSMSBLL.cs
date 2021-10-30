using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MSacco_DAL;
using MSacco_DAL.SMSFloat.Credit;
using MSacco_DAL.SMSFloat.Debit.Archived;
using MSacco_DAL.SMSFloat.Debit.Unarchived;

namespace MSacco_BLL
{
  public class BulkSMSBLL
  {
    private string _conString;
    private readonly string _tblBulkSMSBalances = BulkSMSBalance.DBTableName;
    private readonly string _tblBulkSMSCreditTrx = BulkSMSCredit.DBTableName;
    private readonly string _tblUnarchivedBulkSMSDebitTrx = UnarchivedBulkSMSDebit.DBTableName;
    private readonly string _tblArchivedBulkSMSDebitTrx = ArchivedBulkSMSDebit.DBTableName;
    //#if DEBUG
    //    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_testing"].ConnectionString;
    //#else
    //    string _connString = @ConfigurationManager.ConnectionStrings["visibilityPortalDBConnectionString_prod"].ConnectionString;
    //#endif
    public BulkSMSCredit GetLatestCreditTrxForClient(string corporateNo)
    {
      string query = string.Empty;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      query = $@"SELECT TOP 1 * 
                FROM {_tblBulkSMSCreditTrx} 
                WHERE Sacco = @CorporateNo
                ORDER BY[Datetime] DESC";

      return new DapperORM().QueryGetSingle<BulkSMSCredit>(query, qryParams);
    }

    public IEnumerable<BulkSMSCredit> GetCreditTrxListForClient(string corporateNo)
    {
      string query = string.Empty;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      query = $@"SELECT * 
                FROM {_tblBulkSMSCreditTrx} 
                WHERE Sacco = @CorporateNo
                ORDER BY [Datetime] DESC";

      return new DapperORM().QueryGetList<BulkSMSCredit>(query, qryParams);
    }

    public IEnumerable<UnarchivedBulkSMSDebit> GetUnarchivedDebitTrxListForClient(string corporateNo)
    {
      _conString = @ConfigurationManager.ConnectionStrings["messagesDB_prod"].ConnectionString;
      string query = string.Empty;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      query = $@"SELECT * 
                FROM {_tblUnarchivedBulkSMSDebitTrx} 
                WHERE [Corporate No] = @CorporateNo
                ORDER BY [Datetime] DESC";

      return new DapperORM(_conString).QueryGetList<UnarchivedBulkSMSDebit>(query, qryParams);
    }
    public IEnumerable<ArchivedBulkSMSDebit> GetArchivedDebitTrxListForClient(string corporateNo)
    {

      _conString = @ConfigurationManager.ConnectionStrings["archiveDB_prod"].ConnectionString;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(_conString)
      {
        ConnectTimeout = 600 // timeout value is in sec
      };
      _conString = connectionStringBuilder.ConnectionString;

      string query = string.Empty;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      query = $@"SELECT * 
                FROM {_tblArchivedBulkSMSDebitTrx} 
                WHERE [Corporate No] = @CorporateNo
                ORDER BY [Datetime] DESC";

      return new DapperORM(_conString).QueryGetList<ArchivedBulkSMSDebit>(query, qryParams);
    }

    public BulkSMSBalance GetBulkSMSBalanceForClient(string corporateNo)
    {
      string query = string.Empty;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);

      query = $@"SELECT * 
                FROM {_tblBulkSMSBalances} 
                WHERE [Corporate No] = @CorporateNo";

      return new DapperORM().QueryGetSingle<BulkSMSBalance>(query, qryParams);
    }
  }
}
