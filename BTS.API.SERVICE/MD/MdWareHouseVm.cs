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
    public class MdWareHouseVm
    {
        public class Search : IDataSearch
        {
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public string UnitCode { get; set; }
            public string MaDonVi { get; set; }
            public string MaCuaHang { get; set; }
            public string TaiKhoanKt { get; set; }
            public int TrangThai { get; set; }
            public string DiaChi { get; set; }
            public string ThongTinBoSung { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdWareHouse().TenKho);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdWareHouse();

                if (!string.IsNullOrEmpty(this.MaKho))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKho),
                        Value = this.MaKho,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenKho))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenKho),
                        Value = this.TenKho,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.UnitCode))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.UnitCode),
                        Value = this.UnitCode,
                        Method = FilterMethod.StartsWith
                    });
                }

                if (!string.IsNullOrEmpty(this.TaiKhoanKt))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TaiKhoanKt),
                        Value = this.TaiKhoanKt,
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
                if (!string.IsNullOrEmpty(this.DiaChi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DiaChi),
                        Value = this.DiaChi,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.ThongTinBoSung))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.ThongTinBoSung),
                        Value = this.ThongTinBoSung,
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
                MaKho = summary;
                TenKho = summary;
                TaiKhoanKt = summary;
                UnitCode = summary;
                MaCuaHang = summary;
                ThongTinBoSung = summary;
                DiaChi = summary;
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public string Id { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public string MaDonVi { get; set; }
            public string MaCuaHang { get; set; }
            public string TaiKhoanKt { get; set; }
            public int TrangThai { get; set; }
            public string DiaChi { get; set; }
            public string ThongTinBoSung { get; set; }
            public string UnitCode { get; set; }
            public bool IsHaveKhoBanLe { get; set; }
            public bool IsKhoBanLe { get; set; }
            public bool IsHaveKhoKM { get; set; }
            public bool IsKhoKM { get; set; }
            public List<Dto> DataKhoHang;
        }


    }
}
