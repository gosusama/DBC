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
    [RoutePrefix("api/Ac/SinhNhatKh")]
    [Route("{id?}")]
    [Authorize]
    public class SinhNhatKhController : ApiController
    {
        private readonly ISinhNhatKhService _service;
        private readonly IMdPeriodService _servicePeriod;
        public SinhNhatKhController(ISinhNhatKhService service, IMdPeriodService servicePeriod)
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
                ToDay=currentDate.Day,
                FromDay = currentDate.Day,
                MonthOfBirth=currentDate.Month,
                MinDay = currentDate.Day,
                MaxDay = currentDate.Day,

                ToDate = currentDate,
                FromDate = currentDate,
                MinDate = currentDate,
                MaxDate = currentDate,

                UnitCode = unitCode
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



                result.MaxDay = lastPeriod.ToDate.Day;
                result.MinDay = originalPeriod.FromDate.Day;
                result.ToDay = lastPeriod.ToDate.Day;
                result.FromDay = originalPeriod.ToDate.Day;
                result.MonthOfBirth = lastPeriod.ToDate.Month;
            }

            return Ok(result);
        }
        [Route("ReportSinhNhatKh")]
        public async Task<IHttpActionResult> ReportSinhNhatKh(ParameterSinhNhatKh para)
        {
            var result = new TransferObj<ReportSinhNhatKh>();
            try
            {
                var reporter = new ReportSinhNhatKh();
                var data = new List<MdCustomer>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                var unitCode = _servicePeriod.GetCurrentUnitCode();

                reporter.FromDay = para.FromDay;
                reporter.ToDay = para.ToDay;
                reporter.MonthOfBirth = para.MonthOfBirth;

                //reporter.FromDate = para.FromDate;
                //reporter.ToDate = para.ToDate;
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
                reporter.Data = _service.ReportSinhNhatKh(para);
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
        [Route("ReportDacBietKh")]
        public async Task<IHttpActionResult> ReportDacBietKh(ParameterSinhNhatKh para)
        {
            var result = new TransferObj<ReportDacBietKh>();
            try
            {
                var reporter = new ReportDacBietKh();
                var data = new List<MdCustomer>();
                reporter.UnitCode = _servicePeriod.GetCurrentUnitCode();
                var unitCode = _servicePeriod.GetCurrentUnitCode();

                reporter.FromDay = para.FromDay;
                reporter.ToDay = para.ToDay;
                reporter.MonthOfBirth = para.MonthOfBirth;

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
                reporter.Data = _service.ReportDacBietKh(para);
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