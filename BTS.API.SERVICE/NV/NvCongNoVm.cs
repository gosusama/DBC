using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.NV
{
    public class NvCongNoVm
    {
        public class Search : IDataSearch
        {
            public string MaChungTu { get; set; }
            public string LoaiChungTu { get; set; }
            public string MaKhachHang { get; set; }
            public string MaNhaCungCap { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string GhiChu { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvCongNo().MaChungTu);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvCongNo();

                if (!string.IsNullOrEmpty(this.MaChungTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaChungTu),
                        Value = this.MaChungTu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhachHang),
                        Value = this.MaKhachHang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaNhaCungCap))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaNhaCungCap),
                        Value = this.MaNhaCungCap,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.GhiChu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.GhiChu),
                        Value = this.GhiChu,
                        Method = FilterMethod.Like
                    });
                }
                if (this.TuNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayCT),
                        Value = this.TuNgay.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayCT),
                        Value = this.DenNgay.Value.AddDays(1),
                        Method = FilterMethod.LessThan
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
                MaChungTu = summary;
                MaNhaCungCap = summary;
                MaKhachHang = summary;
                GhiChu = summary;
            }
        }

        public class Dto
        {
            public Dto()
            {
            }
            public string Id { get; set; }
            public string MaChungTu { get; set; }//
            public string LoaiChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string MaKhachHang { get; set; }
            public string MaNhaCungCap { get; set; }
            public string GhiChu { get; set; }

            public decimal? ThanhTien { get; set; }
            public decimal? ThanhTienCanTra { get; set; }
            public decimal? TienThanhToan { get; set; }
            public int ThoiGianDuyetPhieu { get; set; }
            public int TrangThai { get; set; }
            public DateTime? NgayCT { get; set; }

        }

    }
    public class ParameterCongNo
    {
        public string UnitCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int LoaiBC { get; set; }
        public string CustomerCodes { get; set; }
        public string NhaCungCapCodes { get; set; }
        public string LoaiBaoCao { get; set; }
    }
}
