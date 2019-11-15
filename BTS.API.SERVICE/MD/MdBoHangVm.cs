using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
namespace BTS.API.SERVICE.MD
{
    public class MdBoHangVm
    {
        public class Search : IDataSearch
        {
            public string MaBoHang { get; set; }
            public string TenBoHang { get; set; }
            public DateTime? NgayCT { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdBoHang().TenBoHang);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdBoHang();

                if (!string.IsNullOrEmpty(this.MaBoHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaBoHang),
                        Value = this.MaBoHang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenBoHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenBoHang),
                        Value = this.TenBoHang,
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
                MaBoHang = summary;//Ma Hop Dong
                TenBoHang = summary;
            }

        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List <DtoDetail> ();
            }
            public string MaBoHang { get; set; }
            public string TenBoHang { get; set; }
            public DateTime? NgayCT { get; set; }
            public int TrangThai { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal SoLuong { get; set; }
            public decimal SoLuongIn { get; set; }
            public string GhiChu { get; set; }
            public decimal TongLe { get; set; }
            public decimal TongBuon { get; set; }
            public List<DtoDetail> DataDetails { get; set; }

        }
        public class DtoDetail{
        
            public string Id { get; set; }
            public string MaBo { get; set; }
            public string MaHang { get ; set; }
            public string TenHang { get; set; }
            public int SoLuong { get; set; }
            public decimal TyLeCKLe { get; set; }
            public decimal TyLeCKBuon { get; set; }
            public decimal TongBanLe { get; set; }
            public decimal TongBanBuon { get; set; }
            public decimal DonGia { get; set; }
            public string UnitCode { get; set; }
        }
    }
    public class TDS_BoHangVm
    {
        public class SearchBoHang : IDataSearch
        {
            public string Mabohang { get; set; }
            public string Tenbo { get; set; }
            public string Manguoitao { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new TDS_Dmbohang().Mabohang);
                }
            }
            public void LoadGeneralParam(string summary)
            {
                Mabohang = summary;
                Tenbo = summary;
                Manguoitao = summary;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new TDS_Dmbohang();

                if (!string.IsNullOrEmpty(this.Mabohang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Mabohang),
                        Value = this.Mabohang,
                        Method = FilterMethod.Like,

                    });
                }
                if (!string.IsNullOrEmpty(this.Tenbo))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Tenbo),
                        Value = this.Tenbo,
                        Method = FilterMethod.Like,

                    });
                }
                if (!string.IsNullOrEmpty(this.Manguoitao))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Manguoitao),
                        Value = this.Manguoitao,
                        Method = FilterMethod.Like,
                    });
                }
                return result;
            }
        }
        public class Dto
        {
            public Dto()
            {
                DataDetails = new List<Detail>();
            }

            public string Mabohang { get; set; }
            public string Tenbo { get; set; }
            public string Manguoitao { get; set; }
            public DateTime? Ngaytao { get; set; }
            public DateTime Ngayphatsinh { get; set; }
            public int? Trangthai { get; set; }
            public string Ghichu { get; set; }
            public List<Detail> DataDetails { get; set; }

        }
        public class Detail
        {
            public string Mabohang { get; set; }
            public string Masieuthi { get; set; }
            public string Tenhang { get; set; }
            public decimal? Dongia { get; set; }

            public int? Soluong { get; set; }

            public decimal? Tylechietkhaule { get; set; }

            public decimal? Tylechietkhaubuon { get; set; }

            public int? Trangthai { get; set; }
            public string Ghichu { get; set; }

            public decimal? Tongtienbanbuon { get; set; }
            public decimal? Tongtienbanle { get; set; }
            public string UnitCode { get; set; }
            public void Calc()
            {

            }
        }
    }
}
