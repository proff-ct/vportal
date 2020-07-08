using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.IPRS
{
  public interface IBL_IPRS
  {
    IIPRS_Record PerformIPRSLookup(string corporateNo, string idNumber, string phoneNumber);
    List<IIPRS_Record> GetIPRSRecords(string corporateNo, int entryNo = 0);
  }

  public interface IIPRS_Record
  {
    int Entry_No { get; set; }
    string Corporate_No { get; set; }
    string MSISDN { get; set; }
    string Surname { get; set; }
    string Othernames { get; set; }
    string Citizenship { get; set; }
    string Gender { get; set; }
    DateTime Date_Of_Birth { get; set; }
    string IDNum { get; set; }
    string Serial_Number { get; set; }
    bool IDisValid { get; set; }
    bool Sent_To_IPRS { get; set; }
    bool Complete { get; set; }
    string Comment { get; set; }
    string Status { get; set; }
    DateTime UssdDateOfBirth { get; set; }
  }

  public interface IWaumini_IPRSLookupViewModel
  {
    string MSISDN { get; set; }
    string IDNum { get; set; }
    string Surname { get; set; }
    string Othernames { get; set; }
    string Citizenship { get; set; }
    string Gender { get; set; }
    DateTime Date_Of_Birth { get; set; }
  }

  namespace VirtualRegistrations
  {
    public enum RegistrationChannels
    {
      USSD = 1,
      IPRSOnly = 2
    }
  }
}
