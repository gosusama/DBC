using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.SP.API.App_Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace BTS.SP.API
{
    public static class WebApiConfig
    {
        public static void ConfigureCamelCase(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.UseDataContractJsonSerializer = false;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.Formatting = Formatting.None;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
        public class CancelledTaskBugWorkaroundMessageHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                // Try to suppress response content when the cancellation token has fired; ASP.NET will log to the Application event log if there's content in this case.
                if (cancellationToken.IsCancellationRequested)
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
                return response;
            }
        }
        public static void Register(HttpConfiguration config)
        {
            UnityApiConfig.RegisterComponents(config);
            config.MessageHandlers.Add(new CancelledTaskBugWorkaroundMessageHandler());
            config.MapHttpAttributeRoutes();
            ConfigureCamelCase(config);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
             name: "actionApi",
             routeTemplate: "api/{controller}/{action}/{code}",
             defaults: new { code = RouteParameter.Optional }
            );
        }

       
    }
}
