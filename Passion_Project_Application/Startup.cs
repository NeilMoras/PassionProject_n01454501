using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Passion_Project_Application.Startup))]
namespace Passion_Project_Application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
