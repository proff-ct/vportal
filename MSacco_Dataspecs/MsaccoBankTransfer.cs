using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.MSACCOBankTransfer
{
  namespace Models
  {
    public interface IBankTransfer
    {
      int Entry_No { get; set; }

      string Status { get; set; }

      string Account_No { get; set; }

      string Telephone_No { get; set; }

      string Recipient_Account_No { get; set; }

      string Corporate_No { get; set; }

      string Bank_Name { get; set; }

      string Bank_Code { get; set; }

      bool Request_Confirmed { get; set; }

      bool Sent_To_Journal { get; set; }

      decimal Amount { get; set; }

      string Narration { get; set; }

      string Session_Id { get; set; }

      string Comments { get; set; }

      string Transaction_Reference { get; set; }

      DateTime TransactionDate { get; set; }

      decimal AccountBalance { get; set; }
    }

    public interface IBankTransferViewModel
    {
      int Entry_No { get; set; }

      string Status { get; set; }

      string Account_No { get; set; }

      string Telephone_No { get; set; }

      string Recipient_Account_No { get; set; }

      string Corporate_No { get; set; }

      string Bank_Name { get; set; }

      //string Bank_Code { get; set; }

      bool Request_Confirmed { get; set; }

      bool Sent_To_Journal { get; set; }

      decimal Amount { get; set; }

      string Narration { get; set; }

      //string Session_Id { get; set; }

      string Comments { get; set; }

      //string Transaction_Reference { get; set; }

      DateTime TransactionDate { get; set; }

      decimal AccountBalance { get; set; }
    }
  }
  namespace Functions
  {
    public interface IBL_BankTransfer
    {
      IEnumerable<Models.IBankTransfer> GetBankTransferRecordsForClient(string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

      bool IsSaccoRegisteredForBankTransfer(string corporateNo);
    }
  }
}
