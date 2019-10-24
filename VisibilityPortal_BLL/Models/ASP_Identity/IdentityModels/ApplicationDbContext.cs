using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels
{
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
#if DEBUG
    public ApplicationDbContext()
        : base(Connection.TestingConnectionString, throwIfV1Schema: false)
    {
    }
#else
    public AppDBContext()
        : base(Connection.ProductionConnectionString, throwIfV1Schema: false)
    {
    }
#endif

    public static ApplicationDbContext Create()
    {
      return new ApplicationDbContext();
    }
  }
}
