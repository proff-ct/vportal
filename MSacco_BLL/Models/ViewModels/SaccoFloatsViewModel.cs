using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSacco_BLL.Models.ViewModels
{
  public class SaccoFloatsViewModel
  {
    public string CorporateNo { get; set; }
    public string SaccoName { get; set; }
    public decimal? MPesaFloat { get; set; }
    public DateTime? MpesaFloatDate { get; set; }
    public decimal? BulkSMSFloat { get; set; }
    public DateTime? BulkSMSFloatDate { get; set; }
  }
}
