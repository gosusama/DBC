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
    [RoutePrefix("api/Ac/DoanhSoMoi")]
    [Route("{id?}")]
    [Authorize]
    public class DoanhSoMoiController : ApiController
    {
        private readonly IDoanhSoMoiService _service;
        private readonly IMdPeriodService _servicePeriod;
        public DoanhSoMoiController(IDoanhSoMoiService service, IMdPeriodService servicePeriod)
        {
            _service = service;
            _servicePeriod = servicePeriod;
        }


        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var result = new ParameterSinhNhatKh()
            {
                ToDate = currentDate.AddMonths(1).AddDays(-(currentDate.Day)),
                FromDate = currentDate.AddDays((-currentDate.Day) + 1),
                MinDate = currentDate.AddDays((-currentDate.Day) + 1),
                MaxDate = currentDate.AddMonths(1).AddDays(-(currentDate.Day)),
                UnitCode = unitCode
            };

            var periodCollection = _servicePeriod.Repository.DbSet.Where(x => x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);

            if (periodCollection != null && periodCollection.Count() > 0)
            {
                var lastPeriod = periodCollection.OrderByDescending(x => x.Year).OrderByDescending(x => x.Period).FirstOrDefault();
                var originalPeriod = periodCollection.OrderBy(x => x.Period).FirstOrDefault();
                //result.MaxDate = lastPeriod.ToDate;
                //result.MinDate = originalPeriod.FromDate;
                //result.ToDate = lastPeriod.ToDate;
                //if (lastPeriod.ToDate.AddMonths(-1) >= originalPeriod.FromDate)
                //{
                //    result.FromDate = lastPeriod.ToDate.AddMonths(-1);
                //}
                //else
                //{
                //    result.FromDate = originalPeriod.FromDate;
                //}

                result.ToDate = currentDate.AddMonths(1).AddDays(-(currentDate.Day));
                result.FromDate = currentDate.AddDays((-currentDate.Day) + 1);
                result.MinDate = currentDate.AddDays((-currentDate.Day) + 1);
                result.MaxDate = currentDate.AddMonths(1).AddDays(-(currentDate.Day));


                //result.MaxDay = lastPeriod.ToDate.Day;
                //result.MinDay = originalPeriod.FromDate.Day;
                //result.ToDay = lastPeriod.ToDate.Day;
                //result.FromDay = originalPeriod.ToDate.Day;
                //result.MonthOfBirth = lastPeriod.ToDate.Month;
            }

            return Ok(result);
        }
        [Route("ReportDoanhSoMoi")]
        public async Task<IHttpActionResult> ReportDoanhSoMoi(ParameterDoanhSoMoi para)
        {
            var result = new TransferObj<ReportDSMoi>();
            try
            {
                var reporter = new ReportDSMoi();
                var data = new List<CustomDoanhSoMoiReport>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                var unitCode = _servicePeriod.GetCurrentUnitCode();

                reporter.FromDate = para.FromDate;
                reporter.ToDate = para.ToDate;
                reporter.CreateDate = DateTime.Now;
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

                data = _service.ReportDoanhSoMoi(para);
                reporter.Data = data.ToList();
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
        [Route("ReportDoanhSoMoiDetails")]
        public async Task<IHttpActionResult> ReportDoanhSoMoiDetails(ParameterDoanhSoMoiDetails para)
        {
            var result = new TransferObj<ReportDSMoiDetails>();
            try
            {
                var reporter = new ReportDSMoiDetails();
                var data = new List<CustomDoanhSoMoiReportDetails>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                var unitCode = _servicePeriod.GetCurrentUnitCode();

                reporter.FromDate = para.FromDate;
                reporter.ToDate = para.ToDate;
                reporter.CreateDate = DateTime.Now;
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

                data = _service.ReportDoanhSoMoiDetails(para);
                reporter.Data = data.ToList();
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
    }
}