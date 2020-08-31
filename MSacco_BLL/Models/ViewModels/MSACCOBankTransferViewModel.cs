using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class MSACCOBankTransferViewModel : IBankTransferViewModel
  {
    public int Entry_No { get; set; }
    public string Status { get; set; }
    public string Account_No { get; set; }
    public string Telephone_No { get; set; }
    public string Recipient_Account_No { get; set; }
    public string Corporate_No { get; set; }
    public string Bank_Name { get; set; }
    public bool Request_Confirmed { get; set; }
    public bool Sent_To_Journal { get; set; }
    public decimal Amount { get; set; }
    public string Narration { get; set; }
    public string Comments { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal AccountBalance { get; set; }
  }
}
