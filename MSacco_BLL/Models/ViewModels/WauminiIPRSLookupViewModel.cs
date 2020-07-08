using MSacco_Dataspecs.Feature.IPRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.ViewModels
{
  public class WauminiIPRSLookupViewModel: IWaumini_IPRSLookupViewModel
  {
    public string Status { get; set; }
    public string MSISDN { get; set; }
    public string IDNum { get; set; }
    public string Surname { get; set; }
    public string Othernames { get; set; }
    public string Citizenship { get; set; }
    public string Gender { get; set; }
    public DateTime Date_Of_Birth { get; set; }
  }
}
