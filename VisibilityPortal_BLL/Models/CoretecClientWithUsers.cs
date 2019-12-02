using System.Collections.Generic;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;
using VisibilityPortal_BLL.Models.ViewModels;

namespace VisibilityPortal_BLL.Models
{
  public class CoretecClientWithUsers : CoreTecClient
  {
    public virtual IEnumerable<ApplicationUserViewModel> Users { get; set; }

  }
}
