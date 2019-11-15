using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.WebApi;

namespace BTS.SP.API.Reports
{
    public class ReportsController : ReportsControllerBase
    {
        static ReportServiceConfiguration configurationInstance;

        static ReportsController()
        {
            var appPath = HttpContext.Current.Server.MapPath("~/");
            var reportsPath = HttpContext.Current.Server.MapPath("~/Reports");

            var resolver = new ReportFileResolver(reportsPath)
                .AddFallbackResolver(new ReportTypeResolver());

            //Setup the ReportServiceConfiguration
            configurationInstance = new ReportServiceConfiguration
            {
                HostAppId = "Html5App",
                Storage = new Telerik.Reporting.Cache.File.FileStorage(),
                ReportResolver = resolver,
            };
        }
        public ReportsController()
        {
            this.ReportServiceConfiguration = configurationInstance;
        }
    }
    //static Telerik.Reporting.Services.ReportServiceConfiguration configurationInstance =
    //    new Telerik.Reporting.Services.ReportServiceConfiguration
    //    {
    //        HostAppId = "TBNETERP",
    //        ReportResolver = new ReportFileResolver(HttpContext.Current.Server.MapPath("~/Reports"))
    //            .AddFallbackResolver(new ReportTypeResolver()),
    //        Storage = new Telerik.Reporting.Cache.File.FileStorage(),
    //    };


}
