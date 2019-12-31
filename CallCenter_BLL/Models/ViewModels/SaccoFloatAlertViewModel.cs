using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenter_BLL.ViewModels
{
  public class SaccoFloatAlertViewModel
  {
    public int Id { get; set; }
    [Required]
    public string ClientCorporateNo { get; set; }
    [Required]
    public string FloatResourceId { get; set; }
    [Required]
    public string AlertTypeId { get; set; }
    [Required]
    public string Threshold { get; set; }
    [Required]
    public string TriggerCondition { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
  }
}
