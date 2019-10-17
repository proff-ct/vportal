using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Visibility_Portal.Startup))]
namespace Visibility_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
