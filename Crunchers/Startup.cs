using Crunchers.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Crunchers.Startup))]
namespace Crunchers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
