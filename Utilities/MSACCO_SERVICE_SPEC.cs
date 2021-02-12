using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSacco_Dataspecs.Feature.MSACCOBankTransfer.Models;

namespace Utilities.MSACCO_SERVICE_SPEC
{
  public class BankTransferServiceSpec : IBankTransferServiceSpec
  {
    public BankTransferServiceSpec(bool usesCoretecFloat)
    {
      SubscribedToCoretecFloat = usesCoretecFloat;
    }
    public bool SubscribedToCoretecFloat { get; }
  }
}
