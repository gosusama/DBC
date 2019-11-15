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
using AutoMapper.Internal;

namespace BTS.API.SERVICE.NV
{
    public class NvKiemKeVm
    {
        public class Search : IDataSearch
        {
            public string MaPhieuKiemKe { get; set; }
            public string MaKho { get; set; }
            public DateTime? NgayKiemKe { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            public DateTime? NgayIn { get; set; }
            public string LoaiVatTu { get; set; }
            public string NhomVatTu { get; set; }
            public string SoPhieuKiemKe { get; set; }
            public string KeKiemKe { get; set; }
            public string NguoiTao { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvKiemKe().MaPhieuKiemKe);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvKiemKe();
                if (!string.IsNullOrEmpty(this.KeKiemKe))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.KeKiemKe),
                        Value = this.LoaiVatTu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.MaPhieuKiemKe))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaPhieuKiemKe),
                        Value = this.MaPhieuKiemKe,
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

                if (!string.IsNullOrEmpty(this.SoPhieuKiemKe))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.SoPhieuKiemKe),
                        Value = this.SoPhieuKiemKe,
                        Method = FilterMethod.Like
                    });
                }
                if (this.NgayKiemKe.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayKiemKe),
                        Value = this.NgayKiemKe.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
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
                MaPhieuKiemKe = summary;
                SoPhieuKiemKe = summary;
                NguoiTao = summary;
                KeKiemKe = summary;
            }


        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<DtoDetails>();
            }
            public string MaPhieuKiemKe { get; set; }//
            public string MaDonVi { get; set; }
            public DateTime? NgayKiemKe { get; set; }
            public DateTime? NgayIn { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            public string KhoKiemKe { get; set; }
            public string SoPhieuKiemKe { get; set; }
            public string LoaiVatTuKiemKe { get; set; }
            public string NhomVatTuKiemKe { get; set; }
            public string KeKiemKe { get; set; }
            public int TrangThai { get; set; }
            public string NguoiTao { get; set; }
            public List<DtoDetails> DataDetails { get; set; }

        }

        public class DtoDetails
        {
            public string Id { get; set; }//
            public string MaPhieuKiemKe { get; set; }
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string Barcode { get; set; }
            public decimal GiaBanLeCoVat { get; set; }
            public string MaDonVi { get; set; }
            public string KhoKiemKe { get; set; }
            public string SoPhieuKiemKe { get; set; }
            public string LoaiVatTuKiemKe { get; set; }
            public string NhomVatTuKiemKe { get; set; }
            public string KeKiemKe { get; set; }
            public decimal SoLuongTonMay { get; set; }
            public decimal SoLuongKiemKe { get; set; }
            public decimal SoLuongChenhLech { get; set; }
            public decimal TienTonMay { get; set; }
            public decimal TienKiemKe { get; set; }
            public decimal TienChenhLech { get; set; }
            public decimal GiaVon { get; set; }
            public string GhiChu { get; set; }
        }

        public class ExternalCodeInInventory
        {
            public string Id { get; set; }//
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string MaKeHang { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaKhachHang { get; set; }
            public string Barcode { get; set; }
            public string MaKho { get; set; }
            public decimal TonDauKySl { get; set; }
            public decimal TonDauKyGt { get; set; }
            public decimal NhapSl { get; set; }
            public decimal NhapGt { get; set; }
            public decimal XuatSl { get; set; }
            public decimal XuatGt { get; set; }
            public decimal TonCuoiKySl { get; set; }
            public decimal TonCuoiKyGt { get; set; }
            public decimal GiaVon { get; set; }
        }

 public class ReportTongHop
        {
            public ReportTongHop()
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
            public string NameGroupBy { get; set; }
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
            public decimal GiaVon { get; set; }
            public decimal SoLuongTonMay { get; set; }
            public decimal SoLuongKiemKe { get; set; }
            public decimal SoLuongThua { get; set; }
            public decimal GiaTriThua { get; set; }
            public decimal SoLuongThieu { get; set; }
            public decimal GiaTriThieu { get; set; }
        }
        public class ObjectReportCha
        {
            public ObjectReportCha()
            {
                DataDetails = new List<ObjectReportCon>();
            }
            public string Ma { get; set; }
            public string Ten { get; set; }
            public decimal GiaVon { get; set; }
            public decimal SoLuongTonMay { get; set; }
            public decimal SoLuongKiemKe { get; set; }
            public decimal SoLuongThua { get; set; }
            public decimal GiaTriThua { get; set; }
            public decimal SoLuongThieu { get; set; }
            public decimal GiaTriThieu { get; set; }
            public List<ObjectReportCon> DataDetails { get; set; }
        }
        public class ObjectReportCon
        {
            public string Ma { get; set; }
            public string Ten { get; set; }
            public string Barcode { get; set; }
            public DateTime NgayKiemKe { get; set; }
            public string MaCha { get; set; }
            public string TenCha { get; set; }
            public decimal GiaVon { get; set; }
            public decimal SoLuongTonMay { get; set; }
            public decimal SoLuongKiemKe { get; set; }
            public decimal SoLuongThua { get; set; }
            public decimal GiaTriThua { get; set; }
            public decimal SoLuongThieu { get; set; }
            public decimal GiaTriThieu { get; set; }
        }

    }
    public class ParameterKiemKe
    {
        public string UnitCode { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string WareHouseCodes { get; set; }
        public string MerchandiseTypeCodes { get; set; }
        public string MerchandiseCodes { get; set; }
        public string MerchandiseGroupCodes { get; set; }
        public string NhaCungCapCodes { get; set; }
        public string KeHangCodes { get; set; }
        public TypeGroupKiemKe GroupBy { get; set; }
        public TypeReportKiemKe ReportType { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

    }
    public enum TypeGroupKiemKe{
        WAREHOUSE = 1,
        TYPE = 2,
        GROUP = 3,
        MERCHANDISE = 4,
        NHACUNGCAP = 5,
        GIAODICH = 6,
        CUSTOMER = 7,
        KEHANG = 8
    }
    public enum TypeReportKiemKe
    {
        BAOCAODAYDU = 1,
        BAOCAOTHUA = 2,
        BAOCAOTHIEU = 3
    }
}
