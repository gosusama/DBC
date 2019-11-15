using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BTS.SP.API.Api.AC
{
    [RoutePrefix("api/Ac/ImportExport")]
    [Route("{id?}")]
    [Authorize]
    public class ImportExportController : ApiController
    {
        private readonly IDclCloseoutService _service;
        private readonly IMdPeriodService _servicePeriod;
        public ImportExportController(IDclCloseoutService service, IMdPeriodService servicePeriod)
        {
            _service = service;
            _servicePeriod = servicePeriod;
        }

        [Route("ReportInventoryByDay")]
        public async Task<IHttpActionResult> ReportInventoryByDay(ParameterInventory para)
        {
            var result = new TransferObj<InventoryReport>();
            try
            {
                var reporter = new InventoryReport();
                var data = new List<InventoryExpImpLevel2>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                reporter.Year = para.ToDate.Year;
                var period = _service.UnitOfWork.Repository<MdPeriod>().DbSet.Where(x => x.ToDate == para.ToDate).FirstOrDefault();
                if (period != null)
                {
                    reporter.Period = period.Period;
                }
                var unitCode = _servicePeriod.GetCurrentUnitCode();
                reporter.CreateDateNow();
                reporter.FromDay = para.FromDate.Day;
                reporter.FromMonth = para.FromDate.Month;
                reporter.FromYear = para.FromDate.Year;
                reporter.ToDay = para.ToDate.Day;
                reporter.ToMonth = para.ToDate.Month;
                reporter.ToYear = para.ToDate.Year;
                reporter.TenDonVi = CurrentSetting.GetUnitName(unitCode);
                reporter.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var nhanVien = _service.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                            reporter.Username = nhanVien.TenNhanVien;
                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                switch (para.GroupBy)
                {
                    case TypeGroupInventory.MADONVI:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MADONVI.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        //data = _service.CreateReportInventoryByWareHouse( para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Đơn vị";
                        break;
                    case TypeGroupInventory.WAREHOUSE:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MAKHO.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        //data = _service.CreateReportInventoryByWareHouse( para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Kho hàng";
                        break;
                    case TypeGroupInventory.TYPE:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MALOAIVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Loại hàng hóa";
                        break;
                    case TypeGroupInventory.GROUP:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MANHOMVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhóm hàng hóa";
                        break;
                    case TypeGroupInventory.MERCHANDISE:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MAVATTU.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        //data = _service.CreateReportInventoryByMerchandise(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Hàng hóa";
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        data = _service.CreateReportInventoryTongHop(InventoryGroupBy.MAKHACHHANG.ToString(), para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        //data = _service.CreateReportInventoryByCustomer(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes, para.NhaCungCapCodes);
                        reporter.DetailData = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhà cung cấp";
                        break;
                    default:
                        //data = _service.CreateReportInventoryByDay(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                        break;
                }
                result.Data = reporter;
                result.Status = true;
                result.Message = "Xuất báo cáo thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
     /// <summary>
     /// Xuất nhập tồn Excel tổng hợp
     /// </summary>
     /// <param name="para"></param>
     /// <returns></returns>
        [Route("ExportInventoryByDay")]
        public HttpResponseMessage ExportInventoryByDay(InventoryReport para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelXNTTongHop(para);
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);

                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCaoXuatNhapTonTongHop.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;

            }

        }

        [Route("CreateReportCashierByStaff")]
        public async Task<IHttpActionResult> CreateReportCashierByStaff(ParameterCashier para)
        {
            var result = new TransferObj<ReportCashier>();
            try
            {
                var report = new ReportCashier();
                var data = _service.CreateReportCashierByStaff(para.FromDate, para.ToDate, para.UnitCode);
                report.Data = data;
                report.CreateDate();
                var userName = ControllerContext.RequestContext.Principal.Identity.Name;
                report.SetWhoCreateThis(userName);
                result.Data = report;
                result.Data.CreateDate();

                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {

                return InternalServerError();
            }

        }

        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var result = new ParameterInventory()
            {
                ToDate = currentDate,
                FromDate = currentDate,
                MinDate = currentDate,
                MaxDate = currentDate,
                UnitCode = unitCode,
                //IsOnlyInventory = 2,
                GroupBy = TypeGroupInventory.WAREHOUSE

            };
            var periodCollection = _servicePeriod.Repository.DbSet.Where(x => x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);

            if (periodCollection != null && periodCollection.Count() > 0)
            {
                var lastPeriod = periodCollection.OrderByDescending(x => x.Year).OrderByDescending(x => x.Period).FirstOrDefault();
                var originalPeriod = periodCollection.OrderBy(x => x.Period).FirstOrDefault();
                result.MaxDate = lastPeriod.ToDate;
                result.MinDate = originalPeriod.FromDate;
                result.ToDate = lastPeriod.ToDate;
                if (lastPeriod.ToDate.AddMonths(-1) >= originalPeriod.FromDate)
                {
                    result.FromDate = lastPeriod.ToDate.AddMonths(-1);
                }
                else
                {
                    result.FromDate = originalPeriod.FromDate;
                }
            }

            return Ok(result);
        }

        [Route("GetDateToAC_NhapMua")]
        public async Task<IHttpActionResult> GetDateToAC_NhapMua()
        {
            DateTime date = DateTime.Now;
            date = date.AddMonths(-1);
            return Ok(date);
        }

        [Route("GetLastPeriod")]
        public async Task<IHttpActionResult> GetLastPeriod()
        {
            var result = new MdPeriod();
            var unitCode = _servicePeriod.GetCurrentUnitCode();

            result = CurrentSetting.GetKhoaSo(unitCode);
            //if(result!=null)
            //{ }
            return Ok(result);
        }
        [Route("PostExportExcelXNT")]
        public HttpResponseMessage PostExportExcelXNT(ParameterInventory para)
        {
            var ininventoryExp = new TransferObj<ParameterInventory>();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelXNTDetail(para);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.GroupBy)
                {
                    case TypeGroupInventory.TYPE:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTheoLoaiHang.xlsx" };
                        break;
                    case TypeGroupInventory.GROUP:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTheoNhomHang.xlsx" };
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTheoNhaCungCap.xlsx" };
                        break;
                    case TypeGroupInventory.WAREHOUSE:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTheoKho.xlsx" };
                        break;
                    default:
                        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonChiTiet.xlsx" };
                        break;
                }
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;


            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

    }

}