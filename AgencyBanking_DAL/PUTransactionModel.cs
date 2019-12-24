using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgencyBanking_DAL
{
  /// <summary>
  /// This represents Greenland Fedha (GFL / G Fedha Ltd) transactions
  /// </summary>
  public class PUTransactionModel
  {
    public int Entry_No { get; set; }

    public string Document_No { get; set; }

    public string Bank_No { get; set; }

    public string Bank_Code { get; set; }

    public string Account_No { get; set; }

    public string Account_Name { get; set; }

    public string Account_Type { get; set; }

    public decimal? Amount { get; set; }

    public string Transaction_By { get; set; }

    public DateTime? Transaction_Date { get; set; }

    public DateTime? Transaction_Time { get; set; }

    public int? Status { get; set; }

    public DateTime? Status_Date { get; set; }

    public DateTime? Status_Time { get; set; }

    public string Status_By { get; set; }

    public string Confirmation_Code { get; set; }

    public string Transaction_Type { get; set; }

    public bool? Transferred_To_Sacco { get; set; }

    public DateTime? Date_Transferred_To_Sacco { get; set; }

    public DateTime? Time_Transferred_To_Sacco { get; set; }

    public string Transferred_To_Sacco_By { get; set; }

    public string Funds_Source { get; set; }

    public string Receiver_National_ID_No { get; set; }

    public string Receiver_Name { get; set; }

    public string Receiver_Telephone_No { get; set; }

    public string Source_Telephone_No { get; set; }

    public string Bank_Name { get; set; }

    public bool? Funds_Received { get; set; }

    public DateTime? Date_Funds_Received { get; set; }

    public DateTime? Time_Funds_Received { get; set; }

    public string Confirmation_Word { get; set; }

    public bool? Posted { get; set; }

    public DateTime? Date_Posted { get; set; }

    public DateTime? Time_Posted { get; set; }

    public bool? System_Created_Entry { get; set; }

    public int? Transaction_ID { get; set; }

    public string Description { get; set; }

    public string Agent_Code { get; set; }

    public string Location { get; set; }

    public int? OTTN { get; set; }

    public int? OTTN_Sent { get; set; }

    public DateTime? Date_OTTN_Sent { get; set; }

    public DateTime? Time_OTTN_Sent { get; set; }

    public bool? Balance_SMS_Sent { get; set; }

    public DateTime? Date_SMS_Sent { get; set; }

    public DateTime? Time_SMS_Sent { get; set; }

    public string OTTN_Telephone_No { get; set; }

    public string Agent_Name { get; set; }

    public string Depositer_Telephone_No { get; set; }

    public string Comments { get; set; }

    public string Account_No_2 { get; set; }

    public string ID_No { get; set; }

    public string Society { get; set; }

    public string Society_No { get; set; }

    public decimal? Charge { get; set; }

    public string DeviceID { get; set; }

    public string longitude { get; set; }

    public string latitude { get; set; }

    public static string Table
    {
      get
      {
        return "PUTransactions";
      }
    }
  }
}
