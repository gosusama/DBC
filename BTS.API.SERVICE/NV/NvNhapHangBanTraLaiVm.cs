﻿using BTS.API.SERVICE.Services;
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
    public class NvNhapHangBanTraLaiVm
    {
        public class Search : IDataSearch
        {
            public string MaChungTu { get; set; }
            public string MaHoaDon { get; set; }
            public string MaKhachHang { get; set; }
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }
            public string MaKhoNhap { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }
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
                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhachHang),
                        Value = this.MaKhachHang,
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
                if (!string.IsNullOrEmpty(this.MaKhoNhap))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhoNhap),
                        Value = this.MaKhoNhap,
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
                MaKhachHang = summary;
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
            public string MaChungTu { get; set; }
            public string MaHoaDon { get; set; }

            public string MaChungTuPk { get; set; }
            public string MaKhachHang { get; set; }

            public string LoaiPhieu { get; set; }
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }
            public string MaKhoNhap { get; set; }

            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }

            public decimal TienVat { get; set; }
            public string TkCo { get; set; }
            public int TrangThai { get; set; }
            public DateTime? NgayDuyetPhieu { get; set; }
            public DateTime? NgayCT { get; set; }
            public string VAT { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
            public List<DtoClauseDetail> DataClauseDetails { get; set; }

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
            public int Index { get; set; }
            public string MaChungTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public decimal SoLuongLe { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string MaBaoBi { get; set; }
            public decimal SoLuong { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal LuongBao { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
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
            public string NoiDung { get; set; }
            public string MaSoThue { get; set; }
            public string DiaChiKhachHang { get; set; }
            public string MaKhoNhap { get; set; }
            public decimal ThanhTienTruocVat { get; set; }
            public decimal ThanhTienSauVat { get; set; }
            public decimal TienVat { get; set; }
            public DateTime? NgayCT { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public string VAT { get; set; }
            public string DienThoai { get; set; }
            public string TenKho { get; set; }
            public string Username { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public string NameNhanVienCreate { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
            public int TrangThai { get; set; }

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
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class DtoClauseDetail
        {
            public string Id { get; set; }
            public decimal Index { get; set; }
            public string MaChungTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string LoaiPhieu { get; set; }
            public string TkCo { get; set; }
            public string TkNo { get; set; }
            public decimal SoTien { get; set; }
            public string DoiTuongNo { get; set; }
            public string DoiTuongCo { get; set; }
        }
    }
}
