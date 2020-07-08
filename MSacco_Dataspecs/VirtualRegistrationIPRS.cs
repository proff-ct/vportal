using MSacco_Dataspecs.MSSQLOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.VirtualRegistration
{
  namespace Functions
  {
    public interface IBL_VirtualRegistration
    {
      IEnumerable<Models.IVirtualRegistrationIPRS> GetIPRSVirtualRegistrationListForClient(
        string corporateNo, out int lastPage, bool paginate = false, IPaginationParameters pagingParams = null);

    }
  }
  namespace Models
  {
    public interface IVirtualRegistrationIPRS
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
      string Status { get; set; }
      bool Complete { get; set; }
      string Comment { get; set; }
      string Mpesa_Names { get; set; }
      DateTime UssdDateOfBirth { get; set; }
      DateTime Registration_Date { get; set; }
    }
  }
}
