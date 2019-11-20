using System.Collections.Generic;
using VisibilityPortal_DAL;

namespace VisibilityPortal_BLL.Models
{
  public class CoretecClientWithUsers : CoreTecClient
  {
    public virtual IEnumerable<ClientUser> Users { get; set; }

  }
}
