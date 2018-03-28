using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Vishnu.Messenger.ApiManagement.ConsoleHost.Startup))]

namespace Vishnu.Messenger.ApiManagement.ConsoleHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            HttpConfiguration config = new HttpConfiguration();
            DevelopmentWepApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
