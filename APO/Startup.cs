using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APO.Startup))]
namespace APO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
