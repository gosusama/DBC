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
    [RoutePrefix("api/Ac/BaoCaoXuatNhapTonChiTiet")]
    [Route("{id?}")]
    [Authorize]
    public class XuatNhapTonChiTietController : ApiController
    {
        private readonly IXuatNhapTonChiTietService _service;
        private readonly IMdPeriodService _servicePeriod;
        public XuatNhapTonChiTietController(IXuatNhapTonChiTietService service, IMdPeriodService servicePeriod)
        {
            _service = service;
            _servicePeriod = servicePeriod;
        }
        
        [Route("ReportXNTNewTongHop")]
        public async Task<IHttpActionResult> ReportXNTNewTongHop(ParameterInventory para)
        {
            var result = new TransferObj<InventoryDetailReport>();
            try
            {
                var reporter = new InventoryDetailReport();
                var data = new List<InventoryDetailItem>();
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
                    case TypeGroupInventory.WAREHOUSE:
                        data = _service.CreateReportXNTNewTongHop(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Kho hàng";
                        break;
                    case TypeGroupInventory.TYPE:
                        data = _service.CreateReportXNTNewTongHop(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Loại hàng hóa";
                        break;
                    case TypeGroupInventory.GROUP:
                        data = _service.CreateReportXNTNewTongHop(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhóm hàng hóa";
                        break;
                    case TypeGroupInventory.MERCHANDISE:
                        data = _service.CreateReportXNTNewTongHop(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Hàng hóa";
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        data = _service.CreateReportXNTNewTongHop(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhà cung cấp";
                        break;
                    default:
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


        [Route("ExportExcelXNTNewTongHop")]
        public HttpResponseMessage ExportExcelXNTNewTongHop(ParameterInventory para)
        {
            var ininventoryExp = new TransferObj<ParameterInventory>();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelXNTNewTongHop(para);

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
        [Route("ReportXNTNewChiTiet")]
        public async Task<IHttpActionResult> ReportXNTNewChiTiet(ParameterInventory para)
        {
            var result = new TransferObj<InventoryDetailNewReport>();
            try
            {
                var reporter = new InventoryDetailNewReport();
                var data = new List<InventoryDetailItemCha>();
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
                    case TypeGroupInventory.WAREHOUSE:
                        data = _service.CreateReportXNTNewChiTiet(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Kho hàng";
                        break;
                    case TypeGroupInventory.TYPE:
                        data = _service.CreateReportXNTNewChiTiet(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Loại hàng hóa";
                        break;
                    case TypeGroupInventory.GROUP:
                        data = _service.CreateReportXNTNewChiTiet(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhóm hàng hóa";
                        break;
                    case TypeGroupInventory.MERCHANDISE:
                        data = _service.CreateReportXNTNewChiTiet(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Hàng hóa";
                        break;
                    case TypeGroupInventory.NHACUNGCAP:
                        data = _service.CreateReportXNTNewChiTiet(para);
                        reporter.DataDetails = data.ToList();
                        reporter.MapUnitUserName(_service.UnitOfWork);
                        reporter.GroupType = "Nhà cung cấp";
                        break;
                    default:
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


        [Route("ExportExcelXNTNewChiTiet")]
        public HttpResponseMessage ExportExcelXNTNewChiTiet(ParameterInventory para)
        {
            var ininventoryExp = new TransferObj<ParameterInventory>();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                streamData = _service.ExportExcelXNTNewChiTiet(para);

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
