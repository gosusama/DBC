using System;
using System.Threading;
using System.Web.Http;
using BTS.API.SERVICE;
using Telerik.Reporting.Services.WebApi;
using BTS.SP.API.Utils;

namespace BTS.SP.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoMapperConfig.Config();
            log4net.Config.XmlConfigurator.Configure();
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Set("Server", "ERPServer");
        }
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is ThreadAbortException) return;
            WriteLogs.LogError(ex);
        }
    }
}
