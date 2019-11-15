using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.NV
{
    public class NvPhieuDieuChuyenNoiBoVm
    {
        public class Search : IDataSearch
        {
            public string MaChungTu { get; set; }//
            public string NguoiVanChuyen { get; set; }//
            public string LenhDieuDong { get; set; }//
            public string PhuongTienVanChuyen { get; set; }//
            public string NoiDung { get; set; }//
            public string MaDonViNhan { get; set; }//
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhoNhap { get; set; }
            public int TrangThai { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvVatTuChungTu().MaChungTu);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvVatTuChungTu();

                if (!string.IsNullOrEmpty(this.MaChungTu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaChungTu),
                        Value = this.MaChungTu,
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
                if (!string.IsNullOrEmpty(this.NguoiVanChuyen))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NguoiVanChuyen),
                        Value = this.NguoiVanChuyen,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.LenhDieuDong))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.LenhDieuDong),
                        Value = this.LenhDieuDong,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.PhuongTienVanChuyen))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.PhuongTienVanChuyen),
                        Value = this.PhuongTienVanChuyen,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaDonViNhan))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaDonViNhan),
                        Value = this.MaDonViNhan,
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
                if (!string.IsNullOrEmpty(this.MaKhoXuat))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoXuat),
                        Value = this.MaKhoXuat,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaKhoNhap))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoNhap),
                        Value = this.MaKhoNhap,
                        Method = FilterMethod.Like
                    });
                }
                if (TrangThai != 1)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TrangThai),
                        Value = this.TrangThai,
                        Method = FilterMethod.EqualTo
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
                NoiDung = summary;
                LenhDieuDong = summary;
                try
                {
                    TrangThai = Convert.ToInt16(summary);
                }
                catch (Exception) // Không có trang thái tìm kiếm
                {
                    TrangThai = 1; 
                }
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<DtoDetail>();
            }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaChungTuPk { get; set; }//
            public string LoaiPhieu { get; set; }//
            public string NguoiVanChuyen { get; set; }//
            public string LenhDieuDong { get; set; }//
            public string PhuongTienVanChuyen { get; set; }//
            public string NoiDung { get; set; }//
            public string MaDonViNhan { get; set; }//
            public string MaDonViXuat { get; set; }
            public DateTime? NgayCT { get; set; }
            public DateTime? NgayDieuDong { get; set; }
            public string MaKhoXuat { get; set; }
            public string MaKhachHang { get; set; }
            public string MaKhoNhap { get; set; }
            public string VAT { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal ChietKhau
            {
                get
                {
                    if (ThanhTienTruocVat != 0)
                    {
                        return (TienChietKhau / ThanhTienTruocVat) * 100;
                    }
                    return 0;
                }
            }
            public int TrangThai { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
        }

        public class DtoDetail
        {
            public string Id { get; set; }//
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }//
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuongLe { get; set; }
            public string MaKhachHang { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal SoLuongBaoCT { get; set; }
            public decimal SoLuongCT { get; set; }
            public decimal SoLuongLeCT { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal SoLuongTon { get; set; }
            public decimal GiaMuaCoVat { get; set; }
            public string Vat { get; set; }
            public decimal TyLeVatVao { get; set; }
            public int Index { get; set; }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
            }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string SoHd { get; set; }
            public DateTime? NgayHoaDon { get; set; }
            public string Id { get; set; }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaChungTuPk { get; set; }
            public string NoiDung { get; set; }
            public string MaDonViXuat { get; set; }
            public string TenDonViXuat { get; set; }
            public string MaKhoXuat { get; set; }
            public string TenKhoXuat { get; set; }
            public string MaDonViNhan { get; set; }
            public string TenDonViNhan { get; set; }
            public string MaKhoNhap { get; set; }
            public string TenKhoNhap { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public string VAT { get; set; }
            public string TienChietKhau { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public DateTime? NgayCT { get; set; }
            public DateTime? NgayDieuDong { get; set; }
            public string NameNhanVienCreate { get; set; }
            public string TenKho { get; set; }
            public string Username { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
            public int TrangThai { get; set; }
            public int Index { get; set; }
            public void CalcResult()
            {

            }
        }
        public class ReportDetailModel
        {
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string MaHang { get; set; }
            public string Barcode { get; set; }
            public string TenHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal TienGiamGia { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal LuongBao { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaBanBuonVat { get; set; }
            public decimal GiaMuaCoVat { get; set; }
            public string VAT { get; set; }
            public int Index { get; set; }
        }
        public class ObjectReport
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienHang { get; set; }
        }
    }
}
