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
    public class NvXuatBanVm
    {
        public class Search : IDataSearch
        {
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaKhachHang { get; set; }
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }

            public string MaKhoXuat { get; set; }
            public string TkCo { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public DateTime? NgayCT { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNgh { get; set; }
            public string TenNn { get; set; }
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
                if (!string.IsNullOrEmpty(this.MaHoaDon))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaHoaDon),
                        Value = this.MaHoaDon,
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
                if (!string.IsNullOrEmpty(this.MaSoThue))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaSoThue),
                        Value = this.MaSoThue,
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
                if (this.NgayCT.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayCT),
                        Value = this.NgayCT.Value,
                        Method = FilterMethod.EqualTo
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
                        Value = this.DenNgay.Value,
                        Method = FilterMethod.LessThanOrEqualTo
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
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataClauseDetails = new List<DtoClauseDetail>();
                DataDetails = new List<DtoDetail>();
            }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaVatTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaDonViNhan { get; set; }
            public string MaKhoNhap { get; set; }
            public string MaDonViXuat { get; set; }
            public string MaKhachHang { get; set; }
            public string LoaiPhieu { get; set; }
            public string IdPhieuDatHang { get; set; }
            public string SoPhieuDatHang { get; set; }
            public string MaLyDo { get; set; }
            public string NoiDung { get; set; }
            public string MaMayBan { get; set; }
            public string MaSoThue { get; set; }
            public string MaKhoXuat { get; set; }
            public decimal TienThe { get; set; }
            public decimal TienCOD { get; set; }
            public decimal TienMat { get; set; }
            public int TrangThaiThanhToan { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal TongTienGiamGia { get; set; }
            public decimal ThanhTienTruocVatSauCK { get {
            return ThanhTienTruocVat - TienChietKhau;
                } }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public decimal TienNoCu { get; set; }
            public decimal TienThanhToan { get; set; }
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
            public string TkCo { get; set; }
            
            public int TrangThai { get; set; }
            public DateTime? NgayCT { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            //thông tin người giao - người nhận
            public string MaNhanVien { get; set; }
            public string TenNgh { get; set; }
            public string TenNn { get; set; }
            //
            public string UnitCode { get; set; }
            public string VAT { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
            public List<DtoClauseDetail> DataClauseDetails { get; set;}

            public void Calc()
            {
            }
            public void CalcResult()
            {
                //ChietKhau = (TienChietKhau / ThanhTienTruocVat) * 100;
                //ThanhTienTruocVatSauCK = ThanhTienTruocVat - TienChietKhau;

            }

        }

        public class DtoDetail
        {
            public string Id { get; set; }
            public int Index { get; set; }
            public string MaChungTu { get; set; }//
            public decimal GiaVon { get; set; }
            public decimal GiaVonVat { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public string VAT { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongBaoCT { get; set; }
            public decimal SoLuongCT { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal TyLeVatVao { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienTruocGiamGia { get {return (SoLuong * DonGia); } }
            public decimal GiamGia
            {
                get
                {
                    if (SoLuong != 0)
                    {
                        return (TienGiamGia / SoLuong);
                    }
                    return 0;
                }
            }
            public decimal GiaMuaCoVat { get; set; }
            public decimal TienGiamGia { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }

            public void CalcResult()
            {
                //TienTruocGiamGia = SoLuong * DonGia;
                //if (SoLuong != 0)
                //{
                 //   GiamGia = (TienGiamGia / SoLuong);
                //}
            }
        }

        public class DtoClauseDetail
        {
            public string Id { get; set; }
            public decimal Index { get; set; }
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string LoaiPhieu { get; set; }//
            public string TkCo { get; set; }
            public string TkNo { get; set; }
            public decimal SoTien { get; set; }
            public string DoiTuongNo { get; set; }
            public string DoiTuongCo { get; set; }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
            }
             public string Id { get; set; }
            public string MaChungTu { get; set; }//
            public string MaHoaDon { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string DienThoai { get; set; }
            public string NoiDung { get; set; }
            public string SoPhieuDatHang { get; set; }
            public string MaSoThue { get; set; }
            public string DiaChiKhachHang { get; set; }
            public string MaKhoXuat { get; set; }
            public string TenKho { get; set; }
            public string TienChietKhau { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public string TkCo { get; set; }
            public int TrangThai { get; set; }
            public DateTime? NgayCT { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            public string VAT { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string Username { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string NameNhanVienCreate { get; set; }
            public string TenNhanVien { get; set; }
            //thông tin người giao - người nhận
            public string MaNhanVien { get; set; }
            public string TenNgh { get; set; }
            public string TenNn { get; set; }
            public decimal TienNoCu { get; set; }
            public decimal TienThanhToan { get; set; }
            public decimal TienTongNo { get; set; }
            //
            public List<ReportDetailModel> DataReportDetails { get; set; }
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

        }
        public class ReportXBTongHop
        {
            public ReportXBTongHop()
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

            public List<ObjectReport> DataDetails { get; set; }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }

        }
        public class ObjectReport
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuong { get; set; }
            public decimal TienHang { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal TienVat { get; set; }
            public decimal TongTien { get; set; }
            public decimal TienNoCu { get; set; }
            public decimal TienThanhToan { get; set; }
        }
        public class ObjectReportCha
        {
            public ObjectReportCha()
            {
                DataDetails = new List<ObjectReportCon>();
            }
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal Von { get; set; }
            public decimal TienThue { get; set; }
            public decimal DoanhThu { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienNoCu { get; set; }
            public decimal TienThanhToan { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal LaiBanLe { get; set; }
            public List<ObjectReportCon> DataDetails { get; set; }
        }
        public class ObjectReportCon
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public string MaCha { get; set; }
            public string TenCha { get; set; }
            public decimal SoLuongBan { get; set; }
            public decimal Von { get; set; }
            public decimal TienThue { get; set; }
            public decimal DoanhThu { get; set; }
            public decimal TienBan { get; set; }
            public decimal TienKhuyenMai { get; set; }
            public decimal LaiBanLe { get; set; }
            public decimal TienNoCu { get; set; }
            public decimal TienThanhToan { get; set; }
        }

    }
    public class ParameterXuatBan
    {
        public string UnitCode { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string WareHouseCodes { get; set; }
        public string MerchandiseTypeCodes { get; set; }
        public string MerchandiseCodes { get; set; }
        public string MerchandiseGroupCodes { get; set; }
        public string NhaCungCapCodes { get; set; }
        public string CustomerCodes { get; set; }
        public string UnitUserCodes { get; set; }
        public string TaxCodes { get; set; }
        public string XuatXuCodes { get; set; }
        public TypeGroupXuatBan GroupBy { get; set; }
        public TypeReportSelling ReportType { get; set; }
        public TypeReasonDieuChuyen RouteType { get; set; }
        public string ReasonType { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public int IsPay { get; set; }
    }
    public enum TypeGroupXuatBan
    {
        MADONVIXUAT = 0,
        MAKHO = 1,
        MALOAIVATTU = 2,
        MANHOMVATTU = 3,
        MAVATTU = 4,
        MANHACUNGCAP = 5,
        MAGIAODICH = 6,
        MAKHACHHANG = 7,
        MADONVINHAN = 8,
        MALOAITHUE = 9,
        MAXUATXU = 10,
    }
    public enum TypeReportSelling
    {
        XUATBANBUON = 0,
        XUATDIEUCHUYEN =1,
        XUATKHAC =2
    }
    public enum TypeReasonDieuChuyen
    {
        XUATCHUYENKHO = 0,
        XUATSIEUTHI =1
    }
}
