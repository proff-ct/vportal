using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL.ViewModels
{
  public class AlertTypeViewModel
  {
    public int Id { get; set; }
    public string AlertName { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
  }
}
