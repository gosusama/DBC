using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.MD
{
    public class MdSupplierVm
    {

        public class Search : IDataSearch
        {
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string DiaChi { get; set; }
            public string TinhThanhPho { get; set; }
            public string MaSoThue { get; set; }
            public string NguoiLienHe { get; set; }
            public string DienThoai { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public string XuatXu { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdSupplier().TenNCC);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdSupplier();

                if (!string.IsNullOrEmpty(this.MaNCC))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaNCC),
                        Value = this.MaNCC,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenNCC))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenNCC),
                        Value = this.TenNCC,
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

                if (!string.IsNullOrEmpty(this.MaSoThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaSoThue),
                        Value = this.MaSoThue,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DienThoai))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DienThoai),
                        Value = this.DienThoai,
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
                MaNCC = summary;
                TenNCC = summary;
                DiaChi = summary;
                MaSoThue = summary;
            }
        }
        public class Dto
        {
            public string Id { get; set; }
            public string MaNCC { get; set; }
            public string TenNCC { get; set; }
            public string DiaChi { get; set; }
            public string TinhThanhPho { get; set; }
            public string MaSoThue { get; set; }
            public string NguoiLienHe { get; set; }
            public int TrangThai { get; set; }
            public string DienThoai { get; set; }
            public string Fax { get; set; }
            public string Email { get; set; }
            public string XuatXu { get; set; }
            public string ChucVu { get; set; }
            public string TaiKhoan_NganHang { get; set; }
            public string ThongTin_NganHang { get; set; }
            public string UnitCode { get; set; }
            public string TieuChiTimKiem { get; set; }
        }
    }
}

