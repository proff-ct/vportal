using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Functions;
using MSacco_Dataspecs.Feature.MsaccoTIMSINumberChecker.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_BLL
{
  public class MSaccoTIMSIBLL : IBL_MSACCO_TIMSI_NumberChecker
  {
    private readonly string _tblMsaccoTimsiNumberChecker = MsaccoTimsiNumberChecker.DBTableName;
    private readonly int _minimumPhoneLength = 10;
    private ISACCO _activeSACCO;
    private IBL_SACCO _saccoBLL;
    private IBL_MsaccoRegistration _msaccoRegistrationsBLL;
    private IBL_TIMSI_RESET_LOGGER _resetLogger;
    private IBL_TIMSI_RESET_LOG _timsiResetDBLog;
    private List<IRouting_Table> _listMSACCORegistrationRecords;
    private List<string> _registeredPhoneNumbers;
    #region Private Methods
    private bool SetActiveSACCO(string corporateNo)
    {
      bool success = false;
      try
      {
        _activeSACCO = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
        success = _activeSACCO == null ? false : true;
      }
      catch { }

      return success;
    }
    private string FormatRegistrationPhoneNumberForSQLLookup(IRouting_Table regRecord)
    {
      return string.Format("'{0}'", regRecord.Telephone_No);
    }
    private string ParsePhoneNo(string TelephoneNo)
    {
      int requiredLength = 9;
      string unwantedChar = Regex.Escape(@"-_()");
      string pattern = string.Format("[{0}]", unwantedChar);

      TelephoneNo = Regex.Replace(TelephoneNo, pattern, "");
      //int phoneLength = TelephoneNo.Length;
      //return string.Format("+254{0}", TelephoneNo.Substring(phoneLength - requiredLength));
      return TelephoneNo;
    }
    #endregion
    public MSaccoTIMSIBLL()
    {
      _saccoBLL = new SaccoBLL();
      _msaccoRegistrationsBLL = new MsaccoRegistrationsBLL();
      _resetLogger = new TIMSIResetLogger();
      _timsiResetDBLog = new TIMSIResetDBLog();
      _listMSACCORegistrationRecords = new List<IRouting_Table>();
      _registeredPhoneNumbers = new List<string>();
    }
    public IEnumerable<IMSACCO_TIMSI_NumberChecker> GetTIMSIBlockedMembersForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      _listMSACCORegistrationRecords = _msaccoRegistrationsBLL.GetMsaccoRegistrationListForClient(corporateNo, out lastPage).ToList();

      if ((_listMSACCORegistrationRecords == null) || !_listMSACCORegistrationRecords.Any())
      {
        return null;
      }

      _listMSACCORegistrationRecords.ForEach(rec => _registeredPhoneNumbers.Add(FormatRegistrationPhoneNumberForSQLLookup(rec)));

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker} 
          WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
          ORDER BY [Id] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoTimsiNumberChecker}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
          {
            IEnumerable<IMSACCO_TIMSI_NumberChecker> records = results.Read<MsaccoTimsiNumberChecker>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker}
                  WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
                  ORDER BY [Id] DESC";
        return new DapperORM().QueryGetList<MsaccoTimsiNumberChecker>(query);
      }

    }

    public IMSACCO_TIMSI_NumberChecker GetTIMSIMemberRecordForClient(string corporateNo, string memberPhoneNo)
    {
      if (ParsePhoneNo(memberPhoneNo).Length < _minimumPhoneLength) return null;

      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(corporateNo, ParsePhoneNo(memberPhoneNo));

      if (regRecord == null)
      {
        return null;
      }

     string query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker} 
                WHERE [PhoneNumber] = '{regRecord.Telephone_No}'"; 
                //AND ([Comments] IS NOT NULL OR [Comments] <> '')";
      return new DapperORM().QueryGetSingle<MsaccoTimsiNumberChecker>(query);
    }

    public bool ResetTIMSIMemberRecordForClient(
      string corporateNo, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams, out string resetMessage)
    {
      bool isReset = false;
      string resetLogNo = string.Empty;
      if (!SetActiveSACCO(corporateNo))
      {
        resetMessage = $"Sorry, an error occurred retrieving your SACCO's details. Kindly contact CoreTec Support urgently!";
        return isReset;
      }
      else if (!_activeSACCO.isActive)
      {
        resetMessage = string.Format(
          "Sorry, unable to process your request at this time.{0}Please contact CoreTec Support urgently to verify the active status of your SACCO.", Environment.NewLine);

        return isReset;
      }

      if (ParsePhoneNo(resetParams.CustomerPhoneNo).Length < _minimumPhoneLength)
      {
        resetMessage = $"Member phone number: {resetParams.CustomerPhoneNo} must have minimum {_minimumPhoneLength} digits.";
        return isReset;
      }
      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(
        corporateNo, ParsePhoneNo(resetParams.CustomerPhoneNo));

      if (regRecord == null)
      {
        resetMessage = $"Phone number: {resetParams.CustomerPhoneNo} not registered for MSACCO with {_activeSACCO.saccoName_1}";
        return isReset;
      }
      // log the entry about to be deleted
      // get the timsi record
      resetParams.CustomerPhoneNo = ParsePhoneNo(resetParams.CustomerPhoneNo);
      IMSACCO_TIMSI_NumberChecker timsiRecord = GetTIMSIMemberRecordForClient(corporateNo, resetParams.CustomerPhoneNo);
      if (timsiRecord == null)
      {
        resetMessage = string.Format(
          "Unable to retrieve the IMSI record for {0} in {1}. Kindly try again.",
          resetParams.CustomerPhoneNo, _activeSACCO.saccoName_1);
        return isReset;
      }
      else if (!_resetLogger.LogNewRecordToReset(timsiRecord, _activeSACCO, resetParams, out resetLogNo))
      {
        resetMessage = "Unable to log the IMSI record. Kindly wait 5 minutes and try again.";
        return isReset;
      }

      // delete
      // update log
      // return

      string partialPhoneNo = regRecord.Telephone_No.Substring(regRecord.Telephone_No.Length - 9);
      string query = $@"DELETE FROM {_tblMsaccoTimsiNumberChecker}
                WHERE [PhoneNumber] = '{regRecord.Telephone_No}' ";
                //WHERE ([PhoneNumber] = '{regRecord.Telephone_No}' AND ([Comments] IS NOT NULL OR [Comments] <> ''))
                //OR [Comments] LIKE '%{partialPhoneNo}%' ";
      try
      {
        new DapperORM().ExecuteQuery(query);
        resetMessage = "0";
        isReset = true;

        _resetLogger.LogResetOperationStatus(
          _activeSACCO, resetLogNo, TIMSI_RESET_STATUS.Success, null, out resetMessage);
      }
      catch (Exception ex)
      {
        _resetLogger.LogResetOperationStatus(
          _activeSACCO, resetLogNo, TIMSI_RESET_STATUS.Failed, ex.Message, out string logResponse);

        resetMessage = string.Format("Server error: {0}", ex.Message);
      }

      return isReset;
    }

    #region NestedClasses
    public class TIMSIResetLogger : IBL_TIMSI_RESET_LOGGER
    {
      private string query;
      private string _tblTIMSIRESETLOG = "[MSACCO_TIMSI_RESET_LOG]";
      public IEnumerable<IMSACCO_TIMSI_RESET_LOG> GetCustomerLogEntries(string memberPhoneNo, TIMSI_RESET_STATUS? resetStatus = null)
      {
        throw new NotImplementedException();
      }

      public bool LogNewRecordToReset(IMSACCO_TIMSI_NumberChecker timsiRecord, ISACCO sacco, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams, out string logNo)
      {
        query = $@"INSERT INTO {_tblTIMSIRESETLOG}
           ([CorporateNo]
           ,[CustomerPhoneNo]
           ,[TIMSINumberCheckerID]
           ,[SaccoInfo]
           ,[TIMSIRecord]
           ,[ActionUser]
           ,[ResetNarration]
           ,[ResetStatus]
           ,[ResetStatusDate]
           ,[DateCreated])
     VALUES 
           ('{sacco.corporateNo}'
           ,'{resetParams.CustomerPhoneNo}'
           ,'{timsiRecord.Id}'
           ,'{JsonConvert.SerializeObject(new { SaccoInfo = sacco }, Formatting.Indented)}'
           ,'{JsonConvert.SerializeObject(new { TIMSIRecord = timsiRecord }, Formatting.Indented)}'
           ,'{resetParams.ActionUser}'
           ,'{resetParams.ResetNarration}'
           ,'{nameof(TIMSI_RESET_STATUS.Pending)}'
           ,'{DateTime.Now}'
           ,'{DateTime.Now}');

      SELECT SCOPE_IDENTITY() as ResetLogNo;";

        try
        {
          logNo = new DapperORM().QueryGetSingle<string>(query);
          return true;
        }
        catch (Exception ex)
        {
          // TO-DO: log exception
          AppLogger.LogOperationException(
            "TIMSIResetLogger.LogNewRecordToReset",
            $"Exception while creating new record in timsiResetLog table({_tblTIMSIRESETLOG})",
            new { query },
            ex);
          logNo = null;
          return false;
        }
      }

      public bool LogResetOperationStatus(ISACCO sacco, string logNo, TIMSI_RESET_STATUS resetStatus, string remarks, out string logResult)
      {
        query = $@"UPDATE {_tblTIMSIRESETLOG}
           SET 
             [ResetStatus] = '{resetStatus}'
            ,[OperationRemarks] = '{remarks}'
            ,[ResetStatusDate] = '{DateTime.Now}'
            ,[DateLastModified] = '{DateTime.Now}'
          where [LogNo] = '{logNo}'";
        try
        {
          new DapperORM().ExecuteQuery(query);
          logResult = string.Empty;
          return true;
        }
        catch (Exception ex)
        {
          // TO-DO: log exception
          AppLogger.LogOperationException(
            "TIMSIResetLogger.LogNewRecordToReset",
            $"Exception while updating record in timsiResetLog table({_tblTIMSIRESETLOG})",
            new { query },
            ex);
          logResult = $"Error: {ex.Message}";
          return false;
        }
      }

    }
    public class TIMSIResetParams : IMSACCO_TIMSI_RESET_LOG_PARAMS
    {
      public string CustomerPhoneNo { get; set; }
      public string ResetNarration { get; set; }
      public string ActionUser { get; set; }
    }

    public class TIMSIResetDBLog : IBL_TIMSI_RESET_LOG
    {
      private readonly string _tblMsaccoTimsiResetDBLog = MsaccoTIMSIResetLog.DBTableName;
      public IEnumerable<IMSACCO_TIMSI_RESET_DB_LOG> GetResetRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
      {
        lastPage = 0;
        DynamicParameters qryParams = new DynamicParameters();
        qryParams.Add("CorporateNo", corporateNo);
        string query;

        if (paginate)
        {
          query = $@"SELECT * FROM {_tblMsaccoTimsiResetDBLog} 
          WHERE [CorporateNo] = @CorporateNo
          AND [ResetStatus] = '{nameof(TIMSI_RESET_STATUS.Success)}'
          ORDER BY [LogNo] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);
         
          Select count([LogNo]) as TotalRecords  
          FROM {_tblMsaccoTimsiResetDBLog}
          WHERE [CorporateNo]=@CorporateNo 
          AND [ResetStatus] = '{nameof(TIMSI_RESET_STATUS.Success)}'
          ";

          qryParams.Add("PageSize", pagingParams.PageSize);
          qryParams.Add("PageNumber", pagingParams.PageToLoad);

          using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
          {
            sqlCon.Open();
            using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
            {
              IEnumerable<IMSACCO_TIMSI_RESET_DB_LOG> records = results.Read<MsaccoTIMSIResetLog>();
              int totalLoanRecords = results.Read<int>().First();

              lastPage = (int)Math.Ceiling(
                totalLoanRecords / (decimal)pagingParams.PageSize);
              return records;
            }
          }
        }
        else
        {
          query = $@"SELECT * FROM {_tblMsaccoTimsiResetDBLog}
                  WHERE [CorporateNo]=@CorporateNo
                  ORDER BY [LogNo] DESC";
          return new DapperORM().QueryGetList<MsaccoTIMSIResetLog>(query, qryParams);
        }
      }

      public IEnumerable<IMSACCO_TIMSI_RESET_DB_LOG> GetMemberTIMSIResetRecordsForClient(string corporateNo, string memberPhoneNo)
      {
        DynamicParameters qryParams = new DynamicParameters();
        qryParams.Add("CorporateNo", corporateNo);
        qryParams.Add("PhoneNo", corporateNo);

        string query = $@"SELECT * FROM {_tblMsaccoTimsiResetDBLog}
                  WHERE [CorporateNo]=@CorporateNo AND [CustomerPhoneNo]=@PhoneNo
                  ORDER BY [LogNo] DESC";

        return new DapperORM().QueryGetList<MsaccoTIMSIResetLog>(query, qryParams);
      }
    }

    #endregion
  }
}
