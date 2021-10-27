using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Activations;
using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Functions;
using MSacco_Dataspecs.Feature.MsaccoWhitelisting.Models;
using MSacco_Dataspecs.Functions;
using MSacco_Dataspecs.Models;
using MSacco_Dataspecs.MSSQLOperators;
using Newtonsoft.Json;
using Utilities;
using VisibilityPortal_Dataspecs.Models;

namespace MSacco_BLL
{
  public class MsaccoWhitelistingBLL : IBL_MSACCO_Whitelisting
  {
    private readonly int _minimumPhoneLength = 10;
    private ISACCO _activeSACCO;
    private IBL_SACCO _saccoBLL;
    private IBL_MsaccoRegistration _msaccoRegistrationsBLL;
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
    public MsaccoWhitelistingBLL()
    {
      _saccoBLL = new SaccoBLL();
      _msaccoRegistrationsBLL = new MsaccoRegistrationsBLL();
    }
    public bool WhitelistMember(
      string corporateNo, IMSACCO_WHITELISTING_ACTION_PARAMS whitelistingParams, out string operationMessage)
    {
      bool isWhitelisted = false;
      operationMessage = string.Empty;
      if (!SetActiveSACCO(corporateNo))
      {
        operationMessage = $"Sorry, an error occurred retrieving your SACCO's details. Kindly contact CoreTec Support urgently!";
        return isWhitelisted;
      }
      else if (!_activeSACCO.isActive)
      {
        operationMessage = string.Format(
          "Sorry, unable to process your request at this time.{0}Please contact CoreTec Support urgently to verify the active status of your SACCO.", Environment.NewLine);

        return isWhitelisted;
      }

      if (ParsePhoneNo(whitelistingParams.CustomerPhoneNo).Length < _minimumPhoneLength)
      {
        operationMessage = $"Member phone number: {whitelistingParams.CustomerPhoneNo} must have minimum {_minimumPhoneLength} digits.";
        return isWhitelisted;
      }
      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(
        corporateNo, ParsePhoneNo(whitelistingParams.CustomerPhoneNo));

      if (regRecord == null)
      {
        operationMessage = $"Phone number: {whitelistingParams.CustomerPhoneNo} not registered for MSACCO with {_activeSACCO.saccoName_1}";
        return isWhitelisted;
      }
      // TO_DO: log the entry about to be deleted

      whitelistingParams.CustomerPhoneNo = ParsePhoneNo(whitelistingParams.CustomerPhoneNo);
      try
      {
        Whitelist msaccoWhitelister = new Whitelist();
        WhitelistResult result = msaccoWhitelister.WhitelistPhone(
          whitelistingParams.CustomerPhoneNo, whitelistingParams.ActionUser, corporateNo);

        if (result == null)
        {
          isWhitelisted = false;
          operationMessage = string.Format(
            "Whitelisting NOT CONFIRMED.{0}Kindly try again.{0}Contact CoreTec support if this message persists."
            , Environment.NewLine);
        }
        else
        {
          isWhitelisted = result.IsSuccessful;
          operationMessage = result.Message;
        }
      }
      catch (Exception ex)
      {
        isWhitelisted = false;
        operationMessage = string.Format(
          "An error occurred processing your whitelisting request.{0}Kindly try again.{0}Contact CoreTec support if this message persists.",
           Environment.NewLine);

        AppLogger.LogOperationException(
            nameof(WhitelistMember),
            "Error during whitelisting process",
            new { corporateNo, whitelistingParams.CustomerPhoneNo, whitelistingParams.ActionUser },
            ex);
      }

      return isWhitelisted;
    }

    #region NestedClasses
    public class WhitelistingParams : IMSACCO_WHITELISTING_ACTION_PARAMS
    {
      public string CustomerPhoneNo { get; set; }
      public string KYCNarration { get; set; }
      public string ActionUser { get; set; }
    }

    #endregion
  }
}
