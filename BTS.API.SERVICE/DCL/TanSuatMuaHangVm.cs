using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.NV;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.DCL
{
    public class TanSuatMuaHangVm
    {
        public class Search : IDataSearch
        {
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string MaDoanhNghiep { get; set; }
            public string MaDonVi { get; set; }
            public string TenDonVi { get; set; }
            public string TenDoanhNghiep { get; set; }
            public string MaQuayGiaoDich { get; set; }
            public string TenKhachHang { get; set; }
            public string MaKhachHang { get; set; }
            public string MaKho { get; set; }

            public int? TheoKho { get; set; }
            //1 la buon, 2 la le
            public int? BuonHayLe { get; set; }

            public string DefaultOrder
            {

                get
                {
                    return ClassHelper.GetPropertyName(() => new NvVatTuChungTu().MaChungTu);
                }
            }

            public void LoadGeneralParam(string summary)
            {
                MaDoanhNghiep = summary;
                MaQuayGiaoDich = summary;
                MaKhachHang = summary;
                TenKhachHang = summary;

            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();

                var refGiaoDich = new NvGiaoDichQuay();

                if (!string.IsNullOrEmpty(this.MaQuayGiaoDich))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refGiaoDich.MaQuayBan),
                        Value = this.MaQuayGiaoDich,
                        Method = BuildQuery.Query.Types.FilterMethod.EqualTo
                    });
                }

                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refGiaoDich.MaKhachHang),
                        Value = this.MaKhachHang,
                        Method = BuildQuery.Query.Types.FilterMethod.EqualTo
                    });
                }

                if (!string.IsNullOrEmpty(this.MaKho))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refGiaoDich.MaQuayBan),
                        Value = this.MaKho,
                        Method = BuildQuery.Query.Types.FilterMethod.EqualTo
                    });
                }

                if (this.TuNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refGiaoDich.NgayPhatSinh),
                        Value = this.TuNgay,
                        Method = BuildQuery.Query.Types.FilterMethod.GreaterThanOrEqualTo
                    });
                }


                if (this.DenNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refGiaoDich.NgayPhatSinh),
                        Value = this.DenNgay.Value.AddDays(1),
                        Method = BuildQuery.Query.Types.FilterMethod.LessThan
                    });
                }

                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }
        }

        public class Dto
        {
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string MaDonVi { get; set; }
            public string Email { get; set; }
            public string SoDienThoai { get; set; }
            public decimal TongTien { get; set; }
            public int SoLan { get; set; }
        }
    }
}
