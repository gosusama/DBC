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
    [RoutePrefix("api/Ac/TheoNhaCungCap")]
    [Route("{id?}")]
    [Authorize]
    public class TheoNhaCungCapController : ApiController
    {
        private readonly IDclCloseoutService _service;
        private readonly IMdPeriodService _servicePeriod;
        public TheoNhaCungCapController(IDclCloseoutService service, IMdPeriodService servicePeriod)
        {
            _service = service;
            _servicePeriod = servicePeriod;
        }
        [Route("ReportTheoNCCByMerChandise")]
        public async Task<IHttpActionResult> ReportTheoNCCByMerChandise(ParameterInventory para)
        {
            var result = new TransferObj<InventoryReport>();
            try
            {
                var reporter = new InventoryReport();
                var data = new List<InventoryExpImp>();
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
                //switch (para.GroupBy)
                //{
                //    case TypeGroupInventory.WAREHOUSE:
                //        data = _service.CreateReportInventoryByWareHouse(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                //        reporter.DetailData = data.ToList();
                //        reporter.DetailData.ForEach(x => x.MapWareHouseName(_service.UnitOfWork));
                //        reporter.MapUnitUserName(_service.UnitOfWork);
                //        reporter.GroupType = "Kho hàng";
                //        break;
                //    case TypeGroupInventory.TYPE:
                //        data = _service.CreateReportInventoryByType(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                //        reporter.DetailData = data.ToList();
                //        reporter.DetailData.ForEach(x => x.MapTypeName(_service.UnitOfWork));
                //        reporter.MapUnitUserName(_service.UnitOfWork);
                //        reporter.GroupType = "Loại hàng hóa";
                //        break;
                //    case TypeGroupInventory.GROUP:
                //        data = _service.CreateReportInventoryByGroup(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                //        reporter.DetailData = data.ToList();
                //        reporter.DetailData.ForEach(x => x.MapGroupName(_service.UnitOfWork));
                //        reporter.MapUnitUserName(_service.UnitOfWork);
                //        reporter.GroupType = "Nhóm hàng hóa";
                //        break;
                //    case TypeGroupInventory.MERCHANDISE:
                //        data = _service.CreateReportInventoryByMerchandise(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                //        reporter.DetailData = data.ToList();
                //        reporter.DetailData.ForEach(x => x.MapMerchandiseName(_service.UnitOfWork));
                //        reporter.MapUnitUserName(_service.UnitOfWork);
                //        reporter.GroupType = "Hàng hóa";
                //        break;

                //    default:
                //        //data = _service.CreateReportInventoryByDay(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes);
                //        break;
                //}
                //result.Data = reporter;
                result.Status = true;
                result.Message = "Xuất báo cáo thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }


        [Route("PostExportExcelXNTByMerchandiseByNCC")]
        public HttpResponseMessage PostExportExcelXNTByMerchandiseByNCC(ParameterInventory para)
        {
            var ininventoryExp = new TransferObj<ParameterInventory>();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                var streamData = _service.ExportExcelXNTByMerchandiseByNCC(para.FromDate, para.ToDate, para.UnitCode, para.WareHouseCodes, para.MerchandiseTypeCodes, para.MerchandiseGroupCodes, para.MerchandiseCodes,para.NhaCungCapCodes);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "XuatNhapTonTheoHangNhomTheoHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
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
                IsOnlyInventory = 2,
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

    }
}
