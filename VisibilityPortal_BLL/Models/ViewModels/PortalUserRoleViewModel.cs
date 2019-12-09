using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Models.ViewModels
{
  public class PortalUserRoleViewModel : PortalUserRole
  {
    public string Module { get; set; }
  }
}
