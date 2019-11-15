using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Internal;

namespace BTS.API.SERVICE.NV
{
    public class NvGiaoDichQuayVm
    {
        public class Search : IDataSearch
        {
            public string MaGiaoDich { get; set; }
            public decimal? LoaiGiaoDich { get; set; }
            public decimal? TTienCoVat { get; set; }
            public DateTime? NgayTao { get; set; }
            public string MaNguoiTao { get; set; }
            public string NguoiTao { get; set; }
            public string MaQuayBan { get; set; }
            public DateTime? NgayPhatSinh { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvGiaoDichQuay().MaGiaoDich);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvGiaoDichQuay();

                if (!string.IsNullOrEmpty(this.MaGiaoDich))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaGiaoDich),
                        Value = this.MaGiaoDich,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaNguoiTao))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaNguoiTao),
                        Value = this.MaNguoiTao,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.NguoiTao))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NguoiTao),
                        Value = this.NguoiTao,
                        Method = FilterMethod.Like
                    });
                }

                if (!string.IsNullOrEmpty(this.MaQuayBan))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaQuayBan),
                        Value = this.MaQuayBan,
                        Method = FilterMethod.Like
                    });
                }
                if (this.TuNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayPhatSinh),
                        Value = this.TuNgay.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ()
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayPhatSinh),
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
                MaGiaoDich = summary;
                MaNguoiTao = summary;
                NguoiTao = summary;
                MaQuayBan = summary;
            }


        }
      
        public class Dto
        {
            public Dto()
            {
                DataDetails = new List<DtoDetail>();
            }
            public string Id { get; set; }
            public string MaGiaoDich { get; set; }
            public string MaGiaoDichQuayPK { get; set; }
            public string MaDonVi { get; set; }
            public decimal? LoaiGiaoDich { get; set; }
            public decimal? TTienCoVat { get; set; }
            public DateTime? NgayTao { get; set; }
            public string MaNguoiTao { get; set; }
            public string NguoiTao { get; set; }
            public string MaQuayBan { get; set; }
            public DateTime? NgayPhatSinh { get; set; }
            public string HinhThucThanhToan { get; set; }
            public decimal? TienPhieuMuaHang { get; set; }
            public string ThoiGian { get; set; }
            public string MaKhachHang { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
            public int SoLanMua { get; set; }
            public string MaVoucher { get; set; }
            public decimal? TienKhachDua { get; set; }
            public decimal? TienVoucher { get; set; }
            public decimal? TienTheVip { get; set; }
            public decimal? TienTraLai { get; set; }
            public decimal? TienThe { get; set; }
            public decimal? TienCOD { get; set; }
            public decimal? TienMat { get; set; }
            public string ChucVu { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string Email { get; set; }
            public string GhiChu { get; set; }
            public string UserName { get; set; }
            public string GioiTinh { get; set; }
            public string KhachCanTra { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; }
            public DateTime? NgayChungTu { get; set; }
            public DateTime? ICreateDate { get; set; }
            public string MaGiaoDichQuayPk { get; set; }
            public string MaThe { get; set; }
            public string Makh { get; set; }
            public string TenKH { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string TrangThaiThanhToan { get; set; }
            public string PhieuDatCoc { get; set; }
            public int? QuenThe { get; set; }
            public string Voucher { get; set; } //mã voucher
            public decimal? TienDatCoc { get; set; }
            public decimal? SumTienHang { get; set; }
            public decimal? TienThua { get; set; }
            public decimal? TienKhuyenMai { get; set; }
            public string TenKhachHang { get; set; }
            public decimal? TongTien { get; set; }
            public decimal? TienNguyenGia { get; set; }
            public decimal? TienSale { get; set; }
            public string HangKhachHang { get; set; }
        }
        //Model use Insert data to Databse --bán lẻ hàng hóa trên web
        public class DataDto
        {
            public DataDto()
            {
                DataDetails = new List<DataDetails>();
            }
            public bool IsTichDiem { get; set; }
            public string Id { get; set; }
            public string ChucVu { get; set; }
            public string DiaChi { get; set; }
            public string DienThoai { get; set; }
            public string Email { get; set; }
            public string GhiChu { get; set; }
            public string UserName { get; set; }
            public string GioiTinh { get; set; }
            public string KhachCanTra { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; }
            public DateTime? NgayPhatSinh { get; set; }
            public DateTime? NgayChungTu { get; set; }
            public DateTime? ICreateDate { get; set; }
            public string MaGiaoDich { get; set; }
            public string MaGiaoDichQuayPk { get; set; }
            public string MaDonVi { get; set; }
            public string MaThe { get; set; }
            public string Makh { get; set; }
            public string TenKH { get; set; }
            public decimal? LoaiGiaoDich { get; set; }
            public DateTime? NgayTao { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string MaQuayBan { get; set; }
            public string HinhThucThanhToan { get; set; }
            public string TrangThaiThanhToan { get; set; }
            public string PhieuDatCoc { get; set; }
            public int? QuenThe { get; set; }
            public string Voucher { get; set; } //mã voucher
            public decimal? TienDatCoc { get; set; }
            public decimal? TienKhachDua { get; set; }
            public decimal? SumTienHang { get; set; }
            public decimal? TienThua { get; set; }
            public decimal? TyLeKhuyenMai { get; set; } //ty le khuyen mai
            public decimal? TienKhuyenMai { get; set; }
            public decimal? TyLeVoucher { get; set; } //ty le voucher
            public decimal? TienVoucher { get; set; } //tiền voucher
            public decimal? TienTheVip { get; set; }
            public decimal? TienTraLai { get; set; }
            public decimal? TienThe { get; set; }
            public decimal? TienCOD { get; set; }
            public decimal? TienMat { get; set; }
            public decimal? TTienCoVat { get; set; } 
            public string ThoiGian { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string TenKhac { get; set; }
            public List<DataDetails> DataDetails { get; set; }

        }

        public class DataDetails
        {
            public string Id { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public DateTime? NgayChungTu { get; set; }
            public string MaGDQuayPK { get; set; }
            public string MaKhoHang { get; set; }
            public string MaDonVi { get; set; }
            public string MaVatTu { get; set; }
            public string Image { get; set; }
            public string MaColor { get; set; }
            public string MaSize { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaVatVao { get; set; }
            public string MaVatRa { get; set; }
            public string TyLeVatVao { get; set; }
            public string TyLeVatRa { get; set; }
            public string Path_Image { get; set; }
            public byte[] Avatar { get; set; }
            public string DonViTinh { get; set; }
            public string Barcode { get; set; }
            public string TenDayDu { get; set; }
            public string TenVatTu { get; set; }
            public string NguoiTao { get; set; }
            public string MaBoPK { get; set; }
            public DateTime? NgayTao { get; set; }
            public DateTime? NgayPhatSinh { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? ThanhTien { get; set; }
            public decimal? TrangThaiCon { get; set; }
            public decimal? TTienCoVat { get; set; }
            public decimal? VatBan { get; set; }
            public decimal? GiaBanLeVat { get; set; }
            public decimal? GiaBanLeCoVat { get; set; }
            public string MaKhachHang { get; set; }
            public string MaKeHang { get; set; }
            public string MaChuongTrinhKhuyenMai { get; set; }
            public decimal? TyLeKhuyenMai { get; set; }
            public decimal? TienKhuyenMai { get; set; }
            public decimal? TyLeChietKhau { get; set; }
            public decimal? TienChietKhau { get; set; }
            public decimal? TyLeVoucher { get; set; }
            public decimal? TienVoucher { get; set; }
            public decimal? TyLeLaiLe { get; set; }
            public decimal? GiaVon { get; set; }
            public string LoaiKhuyenMai { get; set; }
        }
      
        public class DtoDetail
        {
            public string Id { get; set; }
            public string MaGDQuayPK { get; set; }
            public string MaKhoHang { get; set; }
            public string MaDonVi { get; set; }
            public string MaVatTu { get; set; }
            public string Barcode { get; set; }
            public string TenDayDu { get; set; }
            public string NguoiTao { get; set; }
            public string MaBoPK { get; set; }
            public DateTime? NgayTao { get; set; }
            public DateTime? NgayPhatSinh { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? TTienCoVat { get; set; }
            public decimal? VatBan { get; set; }
            public decimal? GiaBanLeCoVat { get; set; }
            public string MaKhachHang { get; set; }
            public string MaKeHang { get; set; }
            public string MaChuongTrinhTrinhKhuyenMai { get; set; }
            public decimal? TienKhuyenMai { get; set; }
            public decimal? TyLeLaiLe { get; set; }
            public decimal? GiaVon { get; set; }
            public string MaChuongTrinhKhuyenMai { get; set; }
            public string LoaiKhuyenMai { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public DateTime? NgayChungTu { get; set; }
            public string Image { get; set; }
            public string MaColor { get; set; }
            public string MaSize { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaVatVao { get; set; }
            public string MaVatRa { get; set; }
            public string TyLeVatVao { get; set; }
            public string TyLeVatRa { get; set; }
            public string Path_Image { get; set; }
            public byte[] Avatar { get; set; }
            public string DonViTinh { get; set; }
            public string TenVatTu { get; set; }
            public decimal? ThanhTien { get; set; }
            public decimal? TrangThaiCon { get; set; }
            public decimal? GiaBanLeVat { get; set; }
            public decimal? TienVoucher { get; set; }
        }
        public class ReportGDQ
        {
            public ReportGDQ()
            {
                DataDetails = new List<ReportGDQDetailLevel2>();
            }
            public int FromDay { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int ToDay { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string Username { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }

            public List<ReportGDQDetailLevel2> DataDetails { get; set; }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }

        }

        public class ReportHistoryBuyOfCustomer
        {
            public ReportHistoryBuyOfCustomer()
            {
                DataDetails = new List<ReportHistoryBuyOfCustomerLevel2>();
            }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public DateTime? NgayMuaHang { get; set; }
            public List<ReportHistoryBuyOfCustomerLevel2> DataDetails { get; set; }
        }
        public class ReportHistoryBuyOfCustomerLevel2
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal TienVoucher { get; set; }
            public decimal TienMat { get; set; }
            public decimal TienChuyenKhoan { get; set; }
            public DateTime? NgayMuaHang { get; set; }
            public decimal TienCod { get; set; }
        }
        public class ReportGDQTongHopNew
        {
            public ReportGDQTongHopNew()
            {
                DataDetails = new List<ObjectReportLevel2>();
            }
            public int FromDay { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int ToDay { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public int IsPay { get; set; }
            public string Username { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }

            public List<ObjectReportLevel2> DataDetails { get; set; }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }

        }
        public class ReportGDQTongHop
        {
            public ReportGDQTongHop()
            {
                DataDetails = new List<ObjectReport>();
            }
            public int FromDay { get; set; }
            public int FromMonth { get; set; }
            public int FromYear { get; set; }
            public int ToDay { get; set; }
            public int ToMonth { get; set; }
            public int ToYear { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string Username { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }
            public int IsNotPay { get; set; }
            public int IsPay { get; set; }

            public List<ObjectReport> DataDetails { get; set; }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }

        }
        public class ReportGDQDetailLevel2
        {
            public ReportGDQDetailLevel2()
            {
                DataDetails = new List<ReportGDQDetail>();
            }
            public string MaDonVi { get; set; }
            public List<ReportGDQDetail> DataDetails { get; set; }
        }
        public class ReportGDQDetail
        {
            public string MaDonVi { get; set; }
            public string MaNguoiTao { get; set; }
            public string NguoiTao { get; set; }
            public string MaQuayBan { get; set; }
            public decimal? TongBan { get; set; }
            public decimal? TongTraLai { get; set; }
            public decimal? ThucThu { get; set; }
        }
        public class ConditionTranfer
        {
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }
        }
        public class ParameterCashieer
        {
            public string UnitCode { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }

            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
        }
        public class ReportExcel
        {
            public string Id { get; set; }
            public string MaGiaoDich { get; set; }
            public string MaGiaoDichQuayPK { get; set; }
            public string MaDonVi { get; set; }
            public decimal? LoaiGiaoDich { get; set; }
            public DateTime? NgayTao { get; set; }
            public string MaNguoiTao { get; set; }
            public string NguoiTao { get; set; }
            public string MaQuayBan { get; set; }
            public DateTime NgayPhatSinh { get; set; }
            public string MaGDQuayPK { get; set; }
            public string MaKhoHang { get; set; }
            public string MaVatTu { get; set; }
            public string Barcode { get; set; }
            public string TenDayDu { get; set; }
            public string MaBoPK { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? TTienCoVat { get; set; }
            public decimal? VatBan { get; set; }
            public decimal? GiaBanLeCoVat { get; set; }

        }
        public class ObjectReportLevel2
        {
            public ObjectReportLevel2()
            {
                DataDetails = new List<ObjectReport>();
            }
            public string Ma { get; set; }
            public string Ten { get; set; }
            public List<ObjectReport> DataDetails { get; set; }
        }
        public class ObjectReport
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal VonChuaVat { get; set; }
            public decimal Von { get; set; }
            public decimal TienThue { get; set; }
            public decimal DoanhThu { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal TienVoucher { get; set; }
            public decimal TienMat { get; set; }
            public decimal TienChuyenKhoan { get; set; }
            public decimal TienCod { get; set; }
            public decimal LaiBanLe { get; set; }
            public string MaDonVi { get; set; }
        }
        public class ObjectReportCha
        {
            public ObjectReportCha ()
            {
                DataDetails = new List<ObjectReportCon>();
            }
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal Von { get; set; }
            public decimal VonChuaVat { get; set; }
            public decimal TienThue { get; set; }
            public decimal DoanhThu { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienChuyenKhoan { get; set; }
            public string MaDonVi { get; set; }
            public decimal TienCod { get; set; }
            public decimal TienMat { get; set; }
            public decimal TienVoucher { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal LaiBanLe { get; set; }
            public List<ObjectReportCon> DataDetails { get; set; }
        }
        public class ObjectReportCon
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public string Barcode { get; set; }
            public string MaCha { get; set; }
            public string TenCha { get; set; }
            public DateTime NgayGiaoDich { get; set; }
            public decimal TienChuyenKhoan { get; set; }
            public string MaDonVi { get; set; }
            public decimal TienCod { get; set; }
            public decimal TienMat { get; set; }
            public decimal TienVoucher { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal VonChuaVat { get; set; }
            public decimal Von { get; set; }
            public decimal TienThue { get; set; }
            public decimal DoanhThu { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal LaiBanLe { get; set; }
        }



    }
}
