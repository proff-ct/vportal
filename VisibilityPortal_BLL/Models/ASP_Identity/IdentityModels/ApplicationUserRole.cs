using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels
{
  public class ApplicationUserRole: IdentityUserRole
  {
    public string ClientModuleId { get; set; }
  }
}
