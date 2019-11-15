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
    [RoutePrefix("api/Ac/CustomerCare")]
    [Route("{id?}")]
    [Authorize]
    public class CustomerCareController : ApiController
    {
        private readonly ICustomerCareService _service;
        private readonly IMdPeriodService _servicePeriod;
        private readonly IMdCustomerService _serviceCustomer;
        public CustomerCareController(ICustomerCareService service, IMdPeriodService servicePeriod, IMdCustomerService serviceCustomer)
        {
            _service = service;
            _servicePeriod = servicePeriod;
            _serviceCustomer = serviceCustomer;
        }



        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var result = new ParameterCustomerCare()
            {
                ToDate = currentDate,
                FromDate = currentDate,
                MinDate = currentDate,
                MaxDate = currentDate,
                UnitCode = unitCode,
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
        [Route("PostCustomerLevelUp")]
        public async Task<IHttpActionResult> PostCustomerLevelUp(ParameterCustomerCare para)
        {
            var result = new TransferObj<List<MdCustomer>>();
            try
            {

                result.Data = _service.CustomerLevelUpCollection(para);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("ReportCustomerLevelUp")]
        public async Task<IHttpActionResult> ReportCustomerLevelUp(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportLoyalCustomer>();
            try
            {
                var reporter = new ReportLoyalCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportCustomerLevelUp(para);
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
        [Route("PostNotBuyCustomer")]
        public async Task<IHttpActionResult> PostNotBuyCustomer(ParameterCustomerCare para)
        {
            TransferObj<List<MdCustomer>> result = new TransferObj<List<MdCustomer>>();
            try
            {

                result.Data = _service.NotBuyCustomerCollection(para);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("ReportNotBuyCustomer")]
        public async Task<IHttpActionResult> ReportNotBuyCustomer(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportLoyalCustomer>();
            try
            {
                var reporter = new ReportLoyalCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportNotBuyCustomer(para);
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
        //[Route("PutTakeCareOfCustomer")]
        //[HttpPut]
        [ResponseType(typeof(MdCustomer))]
        public async Task<IHttpActionResult> Put(string id, MdCustomer instance)
        {
            var result = new TransferObj<MdCustomer>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }
            try
            {
                var item = _serviceCustomer.TakeCareOfCustomer(instance);
                _serviceCustomer.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [Route("ReportLoyalCustomer")]
        public async Task<IHttpActionResult> ReportLoyalCustomer(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportLoyalCustomer>();
            try
            {
                var reporter = new ReportLoyalCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportLoyalCustomer(para);
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
        [Route("ReportBeChangedCardCustomer")]
        public async Task<IHttpActionResult> ReportBeChangedCardCustomer(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportBeChangedCardCustomer>();
            try
            {
                var reporter = new ReportBeChangedCardCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportBeChangedCardCustomer(para);
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
        [Route("ReportHistoryGiveCardCustomer")]
        public async Task<IHttpActionResult> ReportHistoryGiveCardCustomer(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportHistoryGiveCardCustomer>();
            try
            {
                var reporter = new ReportHistoryGiveCardCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportHistoryGiveCardCustomer(para);
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
        [Route("ReportForgetCardCustomer")]
        public async Task<IHttpActionResult> ReportForgetCardCustomer(ParameterCustomerCare para)
        {
            var result = new TransferObj<ReportHistoryGiveCardCustomer>();
            try
            {
                var reporter = new ReportHistoryGiveCardCustomer();
                var data = new List<MdCustomer>();
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
                reporter.Data = _service.ReportForgetCardCustomer(para);
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
