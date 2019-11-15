using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System.Collections.Generic;
using BTS.API.ENTITY.Authorize;
namespace BTS.API.SERVICE.Authorize.AuDonVi
{
    public class AuDonViVm
    {
        public class Search : IDataSearch
        {
            public string MaDonVi { get; set; }
            public string MaDonViCha { get; set; }
            public string TenDonVi { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }
            public string MaCuaHang { get; set; }
            public string TenCuaHang { get; set; }
            public int TrangThai { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new AU_DONVI().TenDonVi);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new AU_DONVI();

                if (!string.IsNullOrEmpty(this.MaDonVi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaDonVi),
                        Value = this.MaDonVi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaDonViCha))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaDonViCha),
                        Value = this.MaDonViCha,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenDonVi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenDonVi),
                        Value = this.TenDonVi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DiaChi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DiaChi),
                        Value = this.DiaChi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Email))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Email),
                        Value = this.Email,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaCuaHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaCuaHang),
                        Value = this.MaCuaHang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenCuaHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenCuaHang),
                        Value = this.TenCuaHang,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }
         
            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                MaDonVi = summary;
                TenDonVi = summary;
                DiaChi = summary;
                Email = summary;
                TenCuaHang = summary;
                MaCuaHang = summary;
            }
        }
        public class Dto
        {
            public string Id { get; set; }
            public string MaDonVi { get; set; }
            public string MaDonViCha { get; set; }
            public string TenDonVi { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public string Email { get; set; }
            public int TrangThai { get; set; }
            public string UnitCode { get; set; }
        }
    }
}
