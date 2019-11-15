using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
namespace BTS.API.SERVICE.Authorize.AuNguoiDung
{
    public class AuNguoiDungVm
    {
        public class Search : IDataSearch
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public int GioiTinh { get; set; }
            public string MaPhong { get; set; }
            public string ChucVu { get; set; }
            public string ParentUnitcode { get; set; }
            public Nullable<int> Level { get; set; }
            public int TrangThai { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new AU_NGUOIDUNG().Username);
                }
            }
            public void LoadGeneralParam(string summary)
            {
                Username = summary;
                TenNhanVien = summary;
            }
            public List<BuildQuery.IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new AU_NGUOIDUNG();

                if (!string.IsNullOrEmpty(this.Username))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Username),
                        Value = this.Username,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenNhanVien))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenNhanVien),
                        Value = this.TenNhanVien,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.ChucVu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.ChucVu),
                        Value = this.ChucVu,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }

            public List<BuildQuery.IQueryFilter> GetQuickFilters()
            {
                return null;
            }
        }

        public class Dto : DataDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public int GioiTinh { get; set; }
            public string ChucVu { get; set; }
            public int TrangThai { get; set; }
            public string UnitCode { get; set; }
            public string MaDonViCha { get; set; }
            public string MaPhong { get; set; }
            public string ParentUnitcode { get; set; }
            public Nullable<int> Level { get; set; }
        }

        public class ModelRegister : DataDto
        {
            public string UserName { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string GioiTinh { get; set; }
            public string ChucVu { get; set; }
            public string MaPhong { get; set; }
            public string ParentUnitcode { get; set; }
            public Nullable<int> Level { get; set; }
        }
        public class ModelLogin
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class CurrentUser
        {
            public string UnitUser { get; set; }
            public string UserName { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string GioiTinh { get; set; }
            public string ChucVu { get; set; }
            public string MaPhong { get; set; }
            public string MaMayBan { get; set; }
            public string UnitCode { get; set; }
            public string ParentUnitcode { get; set; }
        }

        public class InfoDataCurrentUser
        {
            public string UserName { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string GioiTinh { get; set; }
            public string ChucVu { get; set; }
            public string MaPhong { get; set; }

        }
    }
}
