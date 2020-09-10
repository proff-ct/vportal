using CallCenter_DAL;
using CallCenter_Dataspecs.Functions;
using CallCenter_Dataspecs.Models;
using CallCenter_Dataspecs.MSACCOCustomer.Functions;
using CallCenter_Dataspecs.MSACCOCustomer.Models;
using CallCenter_Dataspecs.MSSQLOperators;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisibilityPortal_Dataspecs.Models;

namespace CallCenter_BLL
{
  public class MSaccoTIMSIBLL : IBL_MSACCO_TIMSI_NumberChecker
  {
    private string _query;
    private readonly string _tblMsaccoTimsiNumberChecker = MsaccoTimsiNumberChecker.DBTableName;
    private readonly int _minimumPhoneLength = 10;
    private ISACCO _activeSACCO;
    private IBL_SACCO _saccoBLL;
    private IBL_MsaccoRegistration _msaccoRegistrationsBLL;
    private IBL_TIMSI_RESET_LOGGER _resetLogger;
    private List<IRouting_Table> _listMSACCORegistrationRecords;
    private List<string> _registeredPhoneNumbers;
    #region Private Methods
    private bool SetActiveSACCO(string corporateNo)
    {
      bool success = false;
      try
      {
        _activeSACCO = _saccoBLL.GetSaccoByUniqueParam(corporateNo);
        success = _activeSACCO == null ? false:true;
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
      string unwantedChar = Regex.Escape(@"-_");
      string pattern = string.Format("[{0}]", unwantedChar);

      TelephoneNo = Regex.Replace(TelephoneNo, pattern, "");
      int phoneLength = TelephoneNo.Length;
      return string.Format("+254{0}", TelephoneNo.Substring(phoneLength - requiredLength));
    }
    #endregion
    public MSaccoTIMSIBLL()
    {
      _saccoBLL = new SaccoBLL();
      _msaccoRegistrationsBLL = new MSaccoRegistrationsBLL();
      _resetLogger = new TIMSIResetLogger();
      _listMSACCORegistrationRecords = new List<IRouting_Table>();
      _registeredPhoneNumbers = new List<string>();
    }
    public IEnumerable<IMSACCO_TIMSI_NumberChecker> GetTIMSIBlockedMembersForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null)
    {
      lastPage = 0;

      _listMSACCORegistrationRecords = _msaccoRegistrationsBLL.GetMsaccoRegistrationListForClient(corporateNo, out lastPage).ToList();

      if ((_listMSACCORegistrationRecords == null) || !_listMSACCORegistrationRecords.Any())
      {
        return null;
      }

      _listMSACCORegistrationRecords.ForEach(rec => _registeredPhoneNumbers.Add(FormatRegistrationPhoneNumberForSQLLookup(rec)));

      if (paginate)
      {
        _query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker} 
          WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
          ORDER BY [Id] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoTimsiNumberChecker}
          WHERE [Corporate No]='{corporateNo}'
          ";

        DynamicParameters dp = new DynamicParameters();
        dp.Add("PageSize", pagingParams.PageSize);
        dp.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(_query, dp, commandType: CommandType.Text))
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
        _query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker}
                  WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
                  ORDER BY [Id] DESC";
        return new DapperORM().QueryGetList<MsaccoTimsiNumberChecker>(_query);
      }

    }

    public IMSACCO_TIMSI_NumberChecker GetTIMSIMemberRecordForClient(string corporateNo, string memberPhoneNo)
    {
      if (memberPhoneNo.Length < _minimumPhoneLength) return null;

      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(corporateNo, ParsePhoneNo(memberPhoneNo));

      if (regRecord == null)
      {
        return null;
      }

      _query = $@"SELECT * FROM {_tblMsaccoTimsiNumberChecker} 
                WHERE [PhoneNumber] = '{regRecord.Telephone_No}' 
                AND ([Comments] IS NOT NULL OR [Comments] <> '')";
      return new DapperORM().QueryGetSingle<MsaccoTimsiNumberChecker>(_query);
    }

    public bool ResetTIMSIMemberRecordForClient(
      string corporateNo, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams, out string resetMessage)
    {
      bool isReset = false;
      if (!SetActiveSACCO(corporateNo))
      {
        resetMessage = $"SACCO of Corporate No {corporateNo} NOT FOUND.{Environment.NewLine}Kindly contact Mobility urgently!";
        return isReset;
      }
      else if (!_activeSACCO.isActive)
      {
        resetMessage = string.Format(
          "SACCO {1} is not currently active.{0}Kindly contact Mobility urgently!",Environment.NewLine,_activeSACCO.saccoName_1);

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
          "Unable to retrieve the TIMSI record for {0} in {1}. Kindly try again.",
          resetParams.CustomerPhoneNo, _activeSACCO.saccoName_1);
        return isReset;
      }
      else if(!_resetLogger.LogNewRecordToReset(timsiRecord, _activeSACCO, resetParams))
      {
        resetMessage = "Unable to log the TIMSI record. Kindly wait 5 minutes and try again.";
        return isReset;
      }

      // delete
      // update log
      // return

      string partialPhoneNo = regRecord.Telephone_No.Substring(regRecord.Telephone_No.Length - 9);
      _query = $@"DELETE FROM {_tblMsaccoTimsiNumberChecker}
                WHERE ([PhoneNumber] = '{regRecord.Telephone_No}' AND ([Comments] IS NOT NULL OR [Comments] <> ''))
                OR [Comments] LIKE '%{partialPhoneNo}%' ";
      try
      {
        new DapperORM().ExecuteQuery(_query);
        resetMessage = "0";
        isReset = true;

        _resetLogger.LogResetOperationStatus(
          _activeSACCO, timsiRecord.Id.ToString(), TIMSI_RESET_STATUS.Success, null, out resetMessage);
      }
      catch (Exception ex)
      {
        _resetLogger.LogResetOperationStatus(
          _activeSACCO, timsiRecord.Id.ToString(), TIMSI_RESET_STATUS.Failed, ex.Message, out string logResponse);

        resetMessage = string.Format("Server error: {0}", ex.Message);
      }

      return isReset;
    }

    #region NestedClasses
    public class TIMSIResetLogger : IBL_TIMSI_RESET_LOGGER
    {
      private string _query;
      private string _tblTIMSIRESETLOG = "[MSACCO_TIMSI_RESET_LOG]";
      public IEnumerable<IMSACCO_TIMSI_RESET_LOG> GetCustomerLogEntries(string memberPhoneNo, TIMSI_RESET_STATUS? resetStatus = null)
      {
        throw new NotImplementedException();
      }

      public bool LogNewRecordToReset(IMSACCO_TIMSI_NumberChecker timsiRecord, ISACCO sacco, IMSACCO_TIMSI_RESET_LOG_PARAMS resetParams)
      {
        _query = $@"INSERT INTO {_tblTIMSIRESETLOG}
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
           ,'{DateTime.Now}')";

        try
        {
          new DapperORM().ExecuteQuery(_query);
          return true;
        }
        catch (Exception ex)
        {
          // TO-DO: log exception
          return false;
        }
      }

      public bool LogResetOperationStatus(ISACCO sacco, string timsiRecordID, TIMSI_RESET_STATUS resetStatus, string remarks, out string logResult)
      {
        _query = $@"UPDATE {_tblTIMSIRESETLOG}
           SET 
             [ResetStatus] = '{nameof(resetStatus)}'
            ,[OperationRemarks] = '{remarks}'
            ,[ResetStatusDate] = '{DateTime.Now}'
            ,[DateLastModified] = '{DateTime.Now}'
          where [TIMSINumberCheckerID] = '{timsiRecordID}'";
        try
        {
          new DapperORM().ExecuteQuery(_query);
          logResult = string.Empty;
          return true;
        }
        catch (Exception ex)
        {
          // TO-DO: log exception
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
    #endregion
  }
}
