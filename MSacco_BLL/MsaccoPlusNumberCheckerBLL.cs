﻿using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Functions;
using MSacco_Dataspecs.Feature.MsaccoPlusNumberChecker.Models;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Functions;
using MSacco_Dataspecs.Feature.MsaccoRegistration.Models;
using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSacco_BLL
{
  public class MsaccoPlusNumberCheckerBLL : IBL_MsaccoPlusNumberChecker
  {
    private string _query;
    private readonly string _tblMsaccoPlusNumberChecker = MsaccoPlusNumberChecker.DBTableName;
    private readonly int _minimumPhoneLength = 10;
    private IBL_MsaccoRegistration _msaccoRegistrationsBLL;
    private List<IRouting_Table> _listMSACCORegistrationRecords;
    private List<string> _registeredPhoneNumbers;
    #region Private Methods
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
    public MsaccoPlusNumberCheckerBLL()
    {
      _msaccoRegistrationsBLL = new MsaccoRegistrationsBLL();
      _listMSACCORegistrationRecords = new List<IRouting_Table>();
      _registeredPhoneNumbers = new List<string>();
    }

    public IEnumerable<IMsaccoPlusNumberChecker> GetMsaccoPlusRegisteredMemberDevicesListForClient(
      string corporateNo,
      out int lastPage,
      bool paginate = false,
      IPaginationParameters pagingParams = null)
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
        _query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker} 
          WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoPlusNumberChecker}
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
            IEnumerable<IMsaccoPlusNumberChecker> records = results.Read<MsaccoPlusNumberChecker>();
            int totalLoanRecords = results.Read<int>().First();

            lastPage = (int)Math.Ceiling(
              totalLoanRecords / (decimal)pagingParams.PageSize);
            return records;
          }
        }
      }
      else
      {
        _query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker}
                  WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
                  ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<MsaccoPlusNumberChecker>(_query);
      }

    }

    public IMsaccoPlusNumberChecker GetMsaccoPlusRegisteredMemberDeviceRecordForClient(string corporateNo, string MemberPhoneNo)
    {
      if (MemberPhoneNo.Length < _minimumPhoneLength) return null;

      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(corporateNo, ParsePhoneNo(MemberPhoneNo));

      if (regRecord == null)
      {
        return null;
      }

      _query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker} 
                WHERE [PhoneNumber] = '{regRecord.Telephone_No}' 
                AND ([Comments] IS NOT NULL OR [Comments] <> '')";
      return new DapperORM().QueryGetSingle<MsaccoPlusNumberChecker>(_query);
    }

    public bool ResetMsaccoPlusMemberDeviceForClient(string corporateNo, string memberPhoneNo, out string resetMessage)
    {

      bool isReset = false;
      if (ParsePhoneNo(memberPhoneNo).Length < _minimumPhoneLength)
      {
        resetMessage = $"Member phone number: {memberPhoneNo} must have minimum {_minimumPhoneLength} digits.";
        return isReset;
      }
      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(
        corporateNo, ParsePhoneNo(memberPhoneNo));

      if (regRecord == null)
      {
        resetMessage = $"Member phone number: {memberPhoneNo} not registered for MSACCO with this sacco";
        return isReset;
      }
      string partialPhoneNo = regRecord.Telephone_No.Substring(regRecord.Telephone_No.Length - 9);
      _query = $@"DELETE FROM {_tblMsaccoPlusNumberChecker}
                WHERE ([PhoneNumber] = '{regRecord.Telephone_No}' AND ([Comments] IS NOT NULL OR [Comments] <> ''))
                OR [Comments] LIKE '%{partialPhoneNo}%' ";
      try
      {
        new DapperORM().ExecuteQuery(_query);
        resetMessage = "0";
        isReset = true;
      }
      catch (Exception ex)
      {
        resetMessage = string.Format("Server error: {0}", ex.Message);
      }

      return isReset;
    }
  }
}