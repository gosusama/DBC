using BTS.API.ENTITY;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.NV
{
    public class NvChuongTrinhKhuyenMaiVm
    {
        public class Search : IDataSearch
        {
            public string MaChuongTrinh { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhoXuatKhuyenMai { get; set; }
            public string DanhSachKhachHang { get; set; }
            public string NoiDung { get; set; }
            public string UnitCode { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvChuongTrinhKhuyenMai().MaChuongTrinh);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvChuongTrinhKhuyenMai();

                if (!string.IsNullOrEmpty(this.UnitCode))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.UnitCode),
                        Value = this.UnitCode,
                        Method = FilterMethod.EqualTo
                    });
                }
                if (!string.IsNullOrEmpty(this.MaChuongTrinh))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaChuongTrinh),
                        Value = this.MaChuongTrinh,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.NoiDung))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NoiDung),
                        Value = this.NoiDung,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhoXuat))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoXuat),
                        Value = this.MaKhoXuat,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhoXuatKhuyenMai))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoXuatKhuyenMai),
                        Value = this.MaKhoXuatKhuyenMai,
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
                MaChuongTrinh = summary;
                MaKhoXuat = summary;
                MaKhoXuatKhuyenMai = summary;
                DanhSachKhachHang = summary;
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<DtoDetail>();
            }
            public string Id { get; set; }
            public string MaChuongTrinh { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public decimal GiaTriKhuyenMai { get; set; }
            public decimal TyLeKhuyenMai { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhoXuatKhuyenMai { get; set; }
            public string DanhSachKhachHang { get; set; }
            public string NoiDung { get; set; }
            public int TrangThai { get; set; }
            public string TuGio { get; set; }
            public string DenGio { get; set; }
            public int LoaiKhuyenMai { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
            public decimal? TienBatDau { get; set; }
            public decimal? TienKetThuc { get; set; }
            public string MaGiamGia { get; set; }
            public string DienThoai { get; set; }

            public int TrangThaiSuDung { get; set; }
        }

        //Model lấy dữ liệu khuyến mại bán lẻ trên Web
        public class Promotion
        {
            public string MaChuongTrinh { get; set; }
            public string MaGiamGia { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string TuGio { get; set; }
            public string DenGio { get; set; }
            public decimal? GiaTriKhuyenMai { get; set; }
            public decimal? TyLeKhuyenMai { get; set; }
            public decimal? TyLeBatDau { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhoKhuyenMai { get; set; }
            public LoaiKhuyenMai LoaiKhuyenMai { get; set; }
            public string NoiDung { get; set; }
            public int TrangThai { get; set; }
            public int TrangThaiSuDung { get; set; }
            public decimal? TienBatDau { get; set; }
            public decimal? TienKetThuc { get; set; }
            public string DienThoai { get; set; }
            public string UnitCode { get; set; }
            public string MaVatTu { get; set; }
            public decimal? SoLuong { get; set; }
            public string MaHang_KhuyenMai { get; set; }
            public decimal? SoLuong_KhuyenMai { get; set; }
            public decimal? TyLeKhuyenMaiChildren { get; set; }
            public decimal? GiaTriKhuyenMaiChildren { get; set; }
            public int HangKmChinh { get; set; }
            public string MaHang_Km_Buy1Get1 { get; set; }
            public string TenHang_Km_Buy1Get1 { get; set; }
            public int SoLuong_Km_Buy1Get1 { get; set; }
        }
        //end model

        public class DtoDetail
        {
            public string Id { get; set; }
            public string MaChuongTrinh { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhoXuatKhuyenMai { get; set; }
            public TypePromotion LoaiChuongTrinh { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public bool IsParent { get; set; }
            public decimal SoLuong { get; set; }
            public string MaHangKhuyenMai { get; set; }
            public decimal SoLuongKhuyenMai { get; set; }
            public decimal TyLeKhuyenMai { get; set; }
            public decimal GiaTriKhuyenMai { get; set; }
            public string TuGio { get; set; }
            public string DenGio { get; set; }
        }

        public class ObjImportExcel
        {
            public decimal SoThuTu { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public decimal TyLeKhuyenMai { get; set; }
            public decimal GiaTriKhuyenMai { get; set; }
        }
    }
}
