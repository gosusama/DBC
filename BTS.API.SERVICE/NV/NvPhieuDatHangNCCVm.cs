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
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.Authorize;

namespace BTS.API.SERVICE.NV
{
    public class NvPhieuDatHangNCCVm
    {
        public class Search : IDataSearch
        {
            public string SoPhieu { get; set; }
            public string MaNhaCungCap { get; set; }
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
                    return ClassHelper.GetPropertyName(() => new NvDatHang().SoPhieuPk);
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
                ListAdd = new List<DtoDetail>();
                ListEdit = new List<DtoDetail>();
            }
            public int Loai { get; set; }
            public string SoPhieu { get; set; }
            public string SoPhieuPk { get; set; }
            public DateTime? Ngay { get; set; }
            public string NguoiLap { get; set; }
            public string MaHd { get; set; }
            public decimal ThanhTien { get; set; }
            public string NoiDung { get; set; }
            public string SoPhieuCon { get; set; }
            public string MaNhaCungCap  { get; set; }
            public string MaDonViDat { get; set; }
            public int TrangThai { get; set; } // 10 =  hoàn thành, 0 = chua duyet, 20- duyệt
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
            public List<DtoDetail> ListAdd { get; set; }
            public List<DtoDetail> ListEdit { get; set; }

            public void Calc()
            {

            }
            public void CalcResult()
            {

            }
        }

        public class DtoDetail
        {
            public string SoPhieu { get; set; }
            public string SoPhieuPk { get; set; }
            public string Id { get; set; }
            public string MaHd { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaBaoBi { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal SoTonMax { get; set; }
            public decimal SoTonMin { get; set; }
            public decimal SoLuongTon { get; set; }
            public decimal SoLuongNhapTrongKy { get; set; }
            public decimal SoLuongXuatTrongKy { get; set; }
            public decimal SoLuongDuyet { get; set; }
            public decimal DonGiaDuyet { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongBaoDuyet { get; set; }
            public decimal SoLuongLeDuyet { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal TyLeVatVao { get; set; }
            public decimal DonGiaDeXuat { get; set; }
            public decimal SoLuongThucTe { get; set; }
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
            public DateTime? Ngay { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public MdSupplier NhaCungCap { get; set; }
            public AU_NGUOIDUNG NhanVien { get; set; }
            public AU_DONVI DonVi { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
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
            public decimal? LuongBao { get; set; }
            public decimal? SoLuongThucTe { get; set; }
            public decimal? DonGiaDeXuat { get; set; }
        }
    }
}
