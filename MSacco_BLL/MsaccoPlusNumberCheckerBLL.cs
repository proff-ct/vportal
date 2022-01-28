using Dapper;
using MSacco_DAL;
using MSacco_Dataspecs;
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
    private readonly string _tblMsaccoPlusNumberChecker = MsaccoPlusNumberChecker.DBTableName;
    private readonly int _minimumPhoneLength = 10;
    private IBL_MsaccoRegistration _msaccoRegistrationsBLL;
    private List<IRouting_Table> _listMSACCORegistrationRecords;
    private List<string> _registeredPhoneNumbers;
   
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
      DynamicParameters qryParams = new DynamicParameters();
      qryParams.Add("CorporateNo", corporateNo);
      string query;

      _listMSACCORegistrationRecords = _msaccoRegistrationsBLL.GetMsaccoRegistrationListForClient(corporateNo, out lastPage).ToList();

      if ((_listMSACCORegistrationRecords == null) || !_listMSACCORegistrationRecords.Any())
      {
        return null;
      }

      _listMSACCORegistrationRecords.ForEach(rec => _registeredPhoneNumbers.Add(MSACCOToolbox.FormatPhoneNumberForSQLLookup(rec.Telephone_No)));

      if (paginate)
      {
        query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker} 
          WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
          ORDER BY [Entry No] DESC
          OFFSET @PageSize * (@PageNumber - 1) ROWS
          FETCH NEXT @PageSize ROWS ONLY OPTION (RECOMPILE);

          Select count([Entry No]) as TotalRecords  
          FROM {_tblMsaccoPlusNumberChecker}
          WHERE [Corporate No]=@CorporateNo
          ";

        qryParams.Add("PageSize", pagingParams.PageSize);
        qryParams.Add("PageNumber", pagingParams.PageToLoad);

        using (SqlConnection sqlCon = new SqlConnection(new DapperORM().ConnectionString))
        {
          sqlCon.Open();
          using (SqlMapper.GridReader results = sqlCon.QueryMultiple(query, qryParams, commandType: CommandType.Text))
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
        query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker}
                  WHERE [PhoneNumber] IN ({string.Format("{0}", string.Join(",", _registeredPhoneNumbers))})
                  ORDER BY [Entry No] DESC";
        return new DapperORM().QueryGetList<MsaccoPlusNumberChecker>(query, qryParams);
      }

    }

    public IMsaccoPlusNumberChecker GetMsaccoPlusRegisteredMemberDeviceRecordForClient(string corporateNo, string MemberPhoneNo)
    {
      if (MemberPhoneNo.Length < _minimumPhoneLength) return null;

      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(
        corporateNo, MSACCOToolbox.ParsePhoneNo(MemberPhoneNo));

      if (regRecord == null)
      {
        return null;
      }

      string query = $@"SELECT * FROM {_tblMsaccoPlusNumberChecker} 
                WHERE [PhoneNumber] = '{regRecord.Telephone_No}' 
                AND ([Comments] IS NOT NULL OR [Comments] <> '')";
      return new DapperORM().QueryGetSingle<MsaccoPlusNumberChecker>(query);
    }

    public bool ResetMsaccoPlusMemberDeviceForClient(string corporateNo, string memberPhoneNo, out string resetMessage)
    {

      bool isReset = false;
      if (MSACCOToolbox.ParsePhoneNo(memberPhoneNo).Length < _minimumPhoneLength)
      {
        resetMessage = $"Member phone number: {memberPhoneNo} must have minimum {_minimumPhoneLength} digits.";
        return isReset;
      }
      IRouting_Table regRecord = _msaccoRegistrationsBLL.GetMsaccoRegistrationRecordForClient(
        corporateNo, MSACCOToolbox.ParsePhoneNo(memberPhoneNo));

      if (regRecord == null)
      {
        resetMessage = $"Member phone number: {memberPhoneNo} not registered for MSACCO with this sacco";
        return isReset;
      }
      string partialPhoneNo = regRecord.Telephone_No.Substring(regRecord.Telephone_No.Length - 9);
      string query = $@"DELETE FROM {_tblMsaccoPlusNumberChecker}
                WHERE ([PhoneNumber] = '{regRecord.Telephone_No}' AND ([Comments] IS NOT NULL OR [Comments] <> ''))
                OR [Comments] LIKE '%{partialPhoneNo}%' ";
      try
      {
        new DapperORM().ExecuteQuery(query);
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
