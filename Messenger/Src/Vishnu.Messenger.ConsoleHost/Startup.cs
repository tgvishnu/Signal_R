using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Vishnu.Messenger.Core.Modules;

[assembly: OwinStartup(typeof(Vishnu.Messenger.ConsoleHost.Startup))]

namespace Vishnu.Messenger.ConsoleHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //app.Map("/signalr", map => {
            //    HubConfiguration hubConfiguration = new HubConfiguration()
            //    {

            //    };

            //    hubConfiguration.EnableDetailedErrors = true;
            //    map.RunSignalR(hubConfiguration);
            //});
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            app.MapSignalR();
        }
    }
}
