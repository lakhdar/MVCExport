using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCExport.Startup))]
namespace MVCExport
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
