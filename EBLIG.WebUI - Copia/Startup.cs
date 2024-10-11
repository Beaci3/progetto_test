using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EBLIG.WebUI.Startup))]
namespace EBLIG.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
