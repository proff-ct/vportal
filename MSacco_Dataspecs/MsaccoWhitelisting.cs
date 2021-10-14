using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_Dataspecs.Feature.MsaccoWhitelisting
{
  namespace Models
  {
    public interface IMSACCO_WHITELISTING_ACTION_PARAMS
    {
      string CustomerPhoneNo { get; set; }
      string KYCNarration { get; set; }
      string ActionUser { get; set; }
    }

    public interface IRegistrationRecordToWhitelistViewModel
    {
      string PhoneNumber { get; set; }
      DateTime DateRegistered { get; set; }
    }
  }

  namespace Functions
  {
    public interface IBL_MSACCO_Whitelisting
    {
      bool WhitelistMember(
        string corporateNo, Models.IMSACCO_WHITELISTING_ACTION_PARAMS whitelistingParams, out string operationMessage);
    }
  }
}
