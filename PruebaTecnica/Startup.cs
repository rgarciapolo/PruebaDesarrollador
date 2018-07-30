using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PruebaTecnica.Startup))]
namespace PruebaTecnica
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
 
