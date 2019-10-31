using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using VisibilityPortal_BLL.Models.ASP_Identity.IdentityModels;

namespace VisibilityPortal_BLL.Models.ASP_Identity.IdentityConfig
{
  /// <summary>
  /// The role manager used by the application to store roles and their connections to users
  /// </summary>
  public class ApplicationRoleManager : RoleManager<ApplicationRole>
  {
    public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
        : base(roleStore)
    {
    }

    public static ApplicationRoleManager Create(
      IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
    {
      ///It is based on the same context as the ApplicationUserManager
      return new ApplicationRoleManager(
        new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
    }
  }
}
