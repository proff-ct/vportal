using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL.ViewModels
{
  public class SaccoFloatAlertListViewModel:SaccoFloatAlertViewModel
  {
    public string SaccoName { get; set; }
    public string FloatResource { get; set; }
    public string AlertType { get; set; }
  }
}
