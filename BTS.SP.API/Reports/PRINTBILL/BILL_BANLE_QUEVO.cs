using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;

namespace BTS.SP.API.Reports.PRINTBILL
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BILL_BANLE_QUEVO.
    /// </summary>
    public partial class BILL_BANLE_QUEVO : Telerik.Reporting.Report
    {
        public BILL_BANLE_QUEVO()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            LoadDataSource();
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public void LoadDataSource()
        {
            this.DataSource = null;
            NvGiaoDichQuayVm.DataDto obj = new NvGiaoDichQuayVm.DataDto();
            string a = this.ReportParameters[0].Text.ToString();
            var jsonObj = JObject.Parse(a);
            var filtered = ((JObject)jsonObj).ToObject<NvGiaoDichQuayVm.DataDto>();
        }
    }
}