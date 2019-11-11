using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_DAL.SMSFloat.Debit.Unarchived
{
  public class UnarchivedTransactionSMSDebit : IDBModel
  {
    public int ID { get; set; }

    public int? Direction { get; set; }

    public int? Type { get; set; }

    public int? Status { get; set; }

    public int? StatusDetails { get; set; }

    public long? ChannelID { get; set; }

    public string BillingID { get; set; }

    public string MessageReference { get; set; }

    public int? ScheduledTimeSecs { get; set; }

    public int? SentTimeSecs { get; set; }

    public int? ReceivedTimeSecs { get; set; }

    public int? LastUpdateSecs { get; set; }

    public string FromAddress { get; set; }

    public int? Priority { get; set; }

    public bool? ReadReceipt { get; set; }

    public string Subject { get; set; }

    public int? BodyFormat { get; set; }

    public int? CustomField1 { get; set; }

    public string CustomField2 { get; set; }

    public bool? sysLock { get; set; }

    public string sysHash { get; set; }

    public bool? sysForwarded { get; set; }

    public string sysGwReference { get; set; }

    public int? sysCreator { get; set; }

    public bool? sysArchive { get; set; }

    public string ToAddress { get; set; }

    public string Header { get; set; }

    public string Body { get; set; }

    public string Trace { get; set; }

    public string Attachments { get; set; }

    public DateTime? SMS_Date { get; set; }

    public string Corporate_No { get; set; }

    public long? Bulk_SMS_ID { get; set; }

    public long? Bulk_SMS { get; set; }

    public DateTime? Bulk_SMS_Date { get; set; }

    public DateTime? Bulk_SMS_Time { get; set; }

    public string Source { get; set; }

    public int? Payment_Processed { get; set; }

    public DateTime? Date_Payment_Processed { get; set; }

    public DateTime? Time_Payment_Processed { get; set; }

    public string Processed_By { get; set; }

    public string Bulk_SMS_No { get; set; }

    public string SMS_Sent { get; set; }

    public string Back_Up_SMS { get; set; }

    public int? Message_Type_ID { get; set; }

    public int? EMail_Sent { get; set; }

  public int? Marked_For_Email { get; set; }

public DateTime? Datetime { get; set; }

public int? Reply_Sent { get; set; }

public int? TotalSmsTrace { get; set; }

public DateTime? CDateTime { get; set; }

public string Sender { get; set; }

public long? spID { get; set; }

public long? serviceID { get; set; }

public int? correlator { get; set; }

public int isProcessed { get; set; }

public int isSynchronized { get; set; }

public bool? FreeSms { get; set; }

public string Reference_id { get; set; }

public int? Sent_Detailed_Analysis { get; set; }

public int? Sent_Summarised_Analysis { get; set; }

public string ReceivedIMEI { get; set; }

public string CheckedMwalimu { get; set; }

public string Checked_Police { get; set; }
public string databaseName => DBName;

public string tableName => DBTableName;

public static string DBName = "Messages";
public static string DBTableName = "Messages2";
  }
}
