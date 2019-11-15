using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
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
    [RoutePrefix("api/Ac/BaoCaoTanSuatMuaHang")]
    [Route("{id?}")]
    [Authorize]
    public class TanSuatMuaHangController : ApiController
    {
        // private readonly ITanSuatMuaHangService _service;
        private readonly INvXuatBanService _serviceBan;
        private readonly INvGiaoDichQuayService _serviceGiaoDich;

        public TanSuatMuaHangController(INvXuatBanService serviceBan, INvGiaoDichQuayService serviceGiaoDich)
        {
            _serviceBan = serviceBan;
            _serviceGiaoDich = serviceGiaoDich;

        }

        [Route("GetNewInstance")]
        public async Task<IHttpActionResult> GetNewInstance()
        {
            try
            {
                var newInstance = new TanSuatMuaHangVm.Search();
                var today = DateTime.Today;
                newInstance.TuNgay = new DateTime(today.Year, today.Month, 1);
                newInstance.DenNgay = newInstance.TuNgay.Value.AddMonths(1).AddDays(-1);
                var unitCode = _serviceBan.GetCurrentUnitCode();
                var company = _serviceBan.UnitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(o => o.MaDonVi == unitCode);
                if (!string.IsNullOrEmpty(company.MaDonVi))
                {
                    newInstance.MaDonVi = company.MaDonVi;
                    newInstance.TenDonVi = company.TenDonVi;
                }
                newInstance.TheoKho = 1;
                return Ok(newInstance);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [Route("PostQueryForRetail")]
        public async Task<IHttpActionResult> PostQueryForRetail(JObject jsonData)
        {
            try
            {
                var convertJson = ((dynamic)jsonData);
                var convertedData = ((JObject)convertJson.filtered).ToObject<FilterObj<TanSuatMuaHangVm.Search>>();
                var paged = ((JObject)convertJson.paged).ToObject<PagedObj<TanSuatMuaHangVm.Dto>>();
                var result = new TransferObj<TanSuatMuaHangVm.Dto>();
                var data1 = _serviceGiaoDich.Repository.DbSet.AsQueryable();
                var item = convertedData.AdvanceData;
                if(convertedData.IsAdvance)
                {
                    if(!string.IsNullOrEmpty(item.MaDonVi))
                    {
                        data1 = data1.Where(o => o.MaDonVi == item.MaDonVi);
                    }
                    if(item.TuNgay > DateTime.MinValue)
                    {
                        data1 = data1.Where(o => o.NgayPhatSinh.Value >= item.TuNgay);
                    }
                    if (item.DenNgay > DateTime.MinValue)
                    {
                        data1 = data1.Where(o => o.NgayPhatSinh.Value <= item.DenNgay);
                    }
                    if (!string.IsNullOrEmpty(item.MaQuayGiaoDich))
                    {
                        data1 = data1.Where(o => o.MaQuayBan == item.MaQuayGiaoDich);
                    }

                }
                var data = data1.GroupBy(o => new { o.MaDonVi, o.MaKhachHang }).Select(x => new TanSuatMuaHangVm.Dto()
                {
                    MaDonVi = x.Key.MaDonVi,
                    MaKhachHang = x.Key.MaKhachHang,
                    TongTien = x.Sum(o => o.TTienCoVat.Value),
                    SoLan = x.Count(o => o.MaGiaoDich != null)

                });
                //var resultPaged =
                result.ExtData = data.ToList();
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

    }
}
