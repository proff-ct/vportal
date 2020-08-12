using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.MsaccoRegistration
{
  namespace Models
  {
    public interface IRouting_Table
    {
      string Account_No { get; set; }
      string Comments { get; set; }
      string Corporate_No { get; set; }
      DateTime? Date_Blocked { get; set; }
      DateTime? DateRegistered { get; set; }
      int Entry_No { get; set; }
      bool? GFEDHATest { get; set; }
      string IMEI { get; set; }
      bool? Is_Agent { get; set; }
      string Language_Code { get; set; }
      string Language_Code_2 { get; set; }
      DateTime? Last_Login_Date { get; set; }
      DateTime? Last_Pin_Change_Date { get; set; }
      int? No_Pin_Attempt { get; set; }
      string PIN_No_Changed { get; set; }
      bool? PinChanged { get; set; }
      string SMS_Code { get; set; }
      bool? SMS_Code_Verified { get; set; }
      int? Status { get; set; }
      bool Subscribed { get; set; }
      string Telephone_No { get; set; }
      double? Withdrawal_Limit_daily { get; set; }
      double? Withdrawal_Limit_Transaction { get; set; }
    }
  }
  namespace Functions
  {
    public interface IBL_MsaccoRegistration
    {
      IEnumerable<Models.IRouting_Table> GetMsaccoRegistrationListForClient(
        string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      Models.IRouting_Table GetMsaccoRegistrationRecordForClient(string corporateNo, string MemberPhoneNo);
    }
  }
}
