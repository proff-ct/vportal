using System.Collections.Generic;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

namespace VisibilityPortal_BLL.Models
{
  public class CoretecClientWithUsers : CoreTecClient
  {
    public virtual IEnumerable<ApplicationUser> Users { get; set; }

  }
}
