using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

namespace VisibilityPortal_BLL.Models.ViewModels
{
  public class ApplicationUserViewModel
  {
    public string ClientName {get; set; }
    public string ClientCorporateNo { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; private set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; private set; }
    public DateTime DateEmailConfirmed { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public bool IsDefaultPassword { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
  }
}
