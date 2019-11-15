using BTS.API.SERVICE.Dashboard;
using BTS.API.SERVICE.Helper;
using BTS.SP.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static BTS.API.SERVICE.Dashboard.DashboardVm;

namespace BTS.SP.API.Api
{
    [RoutePrefix("api/Dashboard")]
    [Route("{id?}")]
    [Authorize]
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }
        [Route("GetRetailRevenue")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRetailRevenue()
        {
            var result = new TransferObj<List<RetailRevenue>>();
            try
            {
                var data = _service.GetRevenue();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetBestOfFiveMerchandiseSelled")]
        [HttpGet]
        public async Task<IHttpActionResult> GetBestOfFiveMerchandiseSelled()
        {
            var result = new TransferObj<List<DashboardVm.BestMerchandise>>();
            try
            {
                var data = _service.GetBestOfFiveMerchandiseSelled();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetDoanhThuLoaiHang")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDoanhThuLoaiHang()
        {
            //DashboardVm.Parameter para
            var result = new TransferObj<List<DashboardVm.BestMerchandise>>();
            try
            {
                var data = _service.GetDoanhThuLoaiHang();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetDoanhThuNhomHang")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDoanhThuNhomHang()
        {
            //DashboardVm.Parameter para
            var result = new TransferObj<List<DashboardVm.BestMerchandise>>();
            try
            {
                var data = _service.GetDoanhThuNhomHang();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetAmmountImportToDay")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAmmountImportToDay()
        {
            var result = new TransferObj<decimal>();
            try
            {
                var data = _service.GetAmmountImportToDay();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetAmmountExportToDay")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAmmountExportToDay()
        {
            var result = new TransferObj<decimal>();
            try
            {
                var data = _service.GetAmmountExportToDay();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetCountImportTransactionNotApproved")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCountImportTransactionNotApproved()
        {
            var result = new TransferObj<decimal>();
            try
            {
                var data = _service.GetCountImportTransactionNotApproved();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetCountExportTransactionNotApproved")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCountExportTransactionNotApproved()
        {
            var result = new TransferObj<decimal>();
            try
            {
                var data = _service.GetCountExportTransactionNotApproved();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetTransactionSummary")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionSummary()
        {
            var result = new TransferObj<List<TransactionAmmount>>();
            try
            {
                var data = _service.GetTransactionSummary();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetMatHangBanCham")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMatHangBanCham()
        {
            var result = new TransferObj<List<DashboardVm.BestMerchandise>>();
            try
            {
                var data = _service.GetMatHangBanCham();
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("PostKiemTraTonHang")]
        [HttpPost]
        public async Task<IHttpActionResult> PostKiemTraTonHang(DashboardVm.Parameter code)
        {
            var result = new TransferObj<List<DashboardVm.InventoryMerchandise>>();
            try
            {
                var data = _service.GetTonMatHang(code.MaVatTu);
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
    }
}
