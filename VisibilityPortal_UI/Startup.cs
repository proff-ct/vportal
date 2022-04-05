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
            Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver = new Microsoft.AspNet.SignalR.DefaultDependencyResolver();
            app.MapSignalR();
        }
    }
}
