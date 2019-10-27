using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels
{
  public class ApplicationRole : IdentityRole
  {
    public string SetupPassPhrase { get; set; }
    public string SetupKey { get; set; }
    public ApplicationRole() : base() { }
    public ApplicationRole(string roleName) : base(roleName) { }
  }
}
