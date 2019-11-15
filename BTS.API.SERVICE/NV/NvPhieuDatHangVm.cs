using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.NV
{
    public class NvPhieuDatHangVm
    {
        public class Search : IDataSearch
        {
            public string SoPhieu { get; set; }
            public string NguoiLap { get; set; }
            public string MaHd { get; set; }
            public string NoiDung { get; set; }
            public string UnitCode { get; set; }
            public int? TrangThai { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvDatHang().SoPhieu);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvDatHang();
                if (TrangThai.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TrangThai),
                        Value = this.TrangThai.Value,
                        Method = FilterMethod.EqualTo
                    });
                }
                if (!string.IsNullOrEmpty(this.UnitCode))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.UnitCode),
                        Value = this.UnitCode,
                        Method = FilterMethod.EqualTo
                    });
                }
                if (!string.IsNullOrEmpty(this.MaHd))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaHd),
                        Value = this.MaHd,
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
                if (!string.IsNullOrEmpty(this.NguoiLap))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NguoiLap),
                        Value = this.NguoiLap,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.SoPhieu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.SoPhieu),
                        Value = this.SoPhieu,
                        Method = FilterMethod.Like
                    });
                }
                if (this.TuNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Ngay),
                        Value = this.TuNgay.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgay.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Ngay),
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
                SoPhieu = summary;
                NoiDung = summary;
                NguoiLap = summary;
                MaHd = summary;
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<DtoDetail>();
            }
            public string SoPhieu { get; set; }
            public string SoPhieuPk { get; set; }
            public string SoPhieuDatHang { get; set; }
            public int Loai { get; set; }
            public DateTime? Ngay { get; set; }
            public string NguoiLap { get; set; }
            public string MaHd { get; set; }
            public decimal ThanhTien { get; set; }
            public string NoiDung { get; set; }
            public string MaKhachHang { get; set; }
            public string SdtKhachHang { get; set; }
            public string DiaChiKH { get; set; }
            public string TenNn { get; set; }
            public string SdtNn { get; set; }
            public string DiaChiNn { get; set; }
            public string TenNgh { get; set; }
            public string SdtNgh { get; set; }
            public string CmndNgh { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNql { get; set; }
            public string SdtNql { get; set; }
            public string EmailNql { get; set; }

            public string EmailKH { get; set; }
            public int TrangThai { get; set; } // 10 =  hoàn thành, 0 = chua duyet, 20- duyệt
            public int TrangThaiTt { get; set; } // 10 =  hoàn thành, 0 = chua duyet, 20- duyệt
            public string HinhThucTt { get; set; } // 10 =  hoàn thành, 0 = chua duyet, 20- duyệt
            public bool IsBuon { get; set; }
            public int IsBanBuon { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public string VAT { get; set; }
            public decimal TienVat { get; set; }
            public decimal TienChietKhau { get; set; }
            public decimal ThanhTienSauVat { get; set; } //Tiền gốc
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
            public List<DtoDetail> DataDetails { get; set; }

            public void Calc()
            {

            }
            public void CalcResult()
            {

            }
        }

        public class DtoDetail
        {
            public string Id { get; set; }
            public string MaHd { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public decimal? SoLuongBao { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? DonGia { get; set; }
            public decimal? SoTonMax { get; set; }
            public decimal? SoTonMin { get; set; }
            public decimal? SoLuongTon { get; set; }
            public decimal? SoLuongNhapTrongKy { get; set; }
            public decimal? SoLuongXuatTrongKy { get; set; }
            public decimal? SoLuongDuyet { get; set; }
            public decimal? DonGiaDuyet { get; set; }
            public decimal? LuongBao { get; set; }
            public decimal? SoLuongBaoDuyet { get; set; }
            public decimal? SoLuongLeDuyet { get; set; }
            public decimal? SoLuongLe { get; set; }
            public decimal? ThanhTien { get; set; }
            public decimal? TyLeVatRa { get; set; }
            public decimal? TyLeVatVao { get; set; }
            public decimal? GiaBanLeChuaVat { get; set; }
            public decimal? GiaBanLeVat { get; set; }
            public string GhiChu { get; set; }
            public void DefaultApproval()
            {
                SoLuongBaoDuyet = SoLuongBao;
                SoLuongDuyet = SoLuong;
                SoLuongLeDuyet = SoLuongLe;
                DonGiaDuyet = DonGia;
            }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
            }
            public string Id { get; set; }
            public string SoPhieu { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string NguoiLap { get; set; }
            public string Fax { get; set; }
            public string DienThoai { get; set; }
            public string ThanhTienSauVat { get; set; }
            public DateTime? Ngay { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string Username { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
            public void CalcResult()
            {

            }
        }

        public class ReportDetailModel
        {
            public string MaChungTu { get; set; }//
            public string MaChungTuPk { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal TienGiamGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class ParameterDatHang
        {
            public string WareHouseCodes { get; set; }
            public string NhanVienCodes { get; set; }
            public string MerchandiseCodes { get; set; }
            public string TrangThaiDatHang { get; set; }

            public string MerchandiseGroupCodes { get; set; }
            public string NhaCungCapCodes { get; set; }
            public string UnitCode { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }
            public TypeGroupDatHang GroupBy { get; set; }

        }

        public class DatHangReport
        {
            public DatHangReport()
            {
                DetailData = new List<DatHangExpImpModel>();
            }
            public int Period { get; set; }
            public int Year { get; set; }
            public string UnitCode { get; set; }
            public string UnitUserName { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }
            public string GroupType { get; set; }
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
            public List<DatHangExpImpModel> DetailData { get; set; }
            public void MapUnitUserName(IUnitOfWork unitOfWork)
            {
                //var wareHouse =  unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.WareHouseCode);
                var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
                //if (wareHouse != null)
                //{
                //    this.WareHouseName = wareHouse.TenKho;
                //}
                if (unitUser != null)
                {
                    this.UnitUserName = unitUser.TenDonVi;
                }

            }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }
        }

        public class DatHangExpImpModel
        {
            public string SoPhieu { get; set; }//
            public string SoPhieuPk { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string MaNhanVien{ get; set; }
            public string TenNhanVien { get; set; }
            public string TrangThaiDonGia { get; set; }
            public decimal ThanhTien { get; set; }

        }

        public class DatHangExpImpChiTiet
        {
            public DatHangExpImpChiTiet()
            {
                List<DatHangExpImpDetailChiTiet> DataDetail = new List<DatHangExpImpDetailChiTiet>();
            }
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public List<DatHangExpImpDetailChiTiet> DataDetail{get;set;}
        }

        public class DatHangExpImpDetailChiTiet
        {
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class DatHangReportChiTiet
        {
            public DatHangReportChiTiet()
            {
                DetailData = new List<DatHangExpImpChiTiet>();
            }
            public int Period { get; set; }
            public int Year { get; set; }
            public string UnitCode { get; set; }
            public string UnitUserName { get; set; }
            public DateTime? ToDate { get; set; }
            public DateTime? FromDate { get; set; }
            public string GroupType { get; set; }
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
            public List<DatHangExpImpChiTiet> DetailData { get; set; }
            public void MapUnitUserName(IUnitOfWork unitOfWork)
            {
                //var wareHouse =  unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.WareHouseCode);
                var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
                //if (wareHouse != null)
                //{
                //    this.WareHouseName = wareHouse.TenKho;
                //}
                if (unitUser != null)
                {
                    this.UnitUserName = unitUser.TenDonVi;
                }

            }
            public void CreateDateNow()
            {
                var createDate = DateTime.Now;
                this.CreateDay = createDate.Day;
                this.CreateMonth = createDate.Month;
                this.CreateYear = createDate.Year;
            }
        }

        public enum TypeGroupDatHang
        {
            TRANGTHAI = 1,
            MANHANVIEN = 2,
            MERCHANDISE = 3,
            MAKHACHHANG = 4,
        }

    }
}
