using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalrChat.Startup))]

namespace SignalrChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            app.MapSignalR();
        }
        public class ErrorHandlingPipelineModule : HubPipelineModule //handle exception
        {
            protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
            {
                Debug.WriteLine("=> Exception " + exceptionContext.Error.Message);
                if (exceptionContext.Error.InnerException != null)
                {
                    Debug.WriteLine("=> Inner Exception " + exceptionContext.Error.InnerException.Message);
                }
                base.OnIncomingError(exceptionContext, invokerContext);

            }
        }
    }
}
