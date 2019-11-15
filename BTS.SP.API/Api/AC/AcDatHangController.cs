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
    [RoutePrefix("api/Ac/DatHang")]
    [Route("{id?}")]
    [Authorize]
    public class AcDatHangController : ApiController
    {
        private readonly INvPhieuDatHangService _service;
        public AcDatHangController(INvPhieuDatHangService service)
        {
            _service = service;
        }
        [Route("PostReportDatHang")]
        public async Task<IHttpActionResult> PostReportDatHang(NvPhieuDatHangVm.ParameterDatHang para)
        {
            var result = new TransferObj<NvPhieuDatHangVm.DatHangReport>();
            try
            {
                var data = new List<NvPhieuDatHangVm.DatHangExpImpModel>();
                var reporter = new NvPhieuDatHangVm.DatHangReport();
                reporter.UnitCode = _service.GetCurrentUnitCode();
                reporter.Year = para.ToDate.Year;
                var period = _service.UnitOfWork.Repository<MdPeriod>().DbSet.Where(x => x.ToDate == para.ToDate).FirstOrDefault();
                if (period != null)
                {
                    reporter.Period = period.Period;
                }
                var unitCode = _service.GetCurrentUnitCode();
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
                data = _service.ReportDatHangTongHop(para);
                reporter.DetailData.AddRange(data);
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

        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            DateTime date = DateTime.Now.Date;

            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var result = new NvPhieuDatHangVm.ParameterDatHang()
            {
                ToDate = firstDayOfMonth,
                FromDate = lastDayOfMonth,
                UnitCode = unitCode,
                GroupBy =  NvPhieuDatHangVm.TypeGroupDatHang.TRANGTHAI

            };

            return Ok(result);
        }

    }
}