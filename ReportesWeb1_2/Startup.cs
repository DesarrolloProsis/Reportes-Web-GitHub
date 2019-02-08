using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReportesWeb1_2.Startup))]
namespace ReportesWeb1_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
