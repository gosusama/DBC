
using System.Collections.Generic;
using BTS.API.SERVICE.Services;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System;

namespace BTS.API.SERVICE.MD
{
    public class MdContractVm
    {
        public class Search : IDataSearch
        {
            public string MaHd { get; set; }
            public string TenHd { get; set; }
            
            public string GiaTriHD { get; set; }
            public string KhachHang { get; set; }
            public string NguoiLienHe { get; set; }
            public string DonViThucHien { get; set; }
            public string NguoiThucHien { get; set; }
            public string LyDo { get; set; }
            public string DieuKhoanKhac { get; set; }
            public DateTime? TuNgayKy { get; set; }
            public DateTime? DenNgayKy { get; set; }

            public DateTime? TuNgayHetHan { get; set; }
            public DateTime? DenNgayHetHan { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdContract().TenHd);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdContract();

                if (!string.IsNullOrEmpty(this.MaHd))//MaHD
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaHd),
                        Value = this.MaHd,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.TenHd))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.TenHd),
                        Value = this.TenHd,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.GiaTriHD))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.GiaTriHD),
                        Value = this.GiaTriHD,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.KhachHang))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.KhachHang),
                        Value = this.KhachHang,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.NguoiLienHe))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NguoiLienHe),
                        Value = this.NguoiLienHe,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DonViThucHien))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DonViThucHien),
                        Value = this.DonViThucHien,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.NguoiThucHien))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NguoiThucHien),
                        Value = this.NguoiThucHien,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.LyDo))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.LyDo),
                        Value = this.LyDo,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.DieuKhoanKhac))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.DieuKhoanKhac),
                        Value = this.DieuKhoanKhac,
                        Method = FilterMethod.Like
                    });
                }


                if (this.TuNgayKy.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayKy),
                        Value = this.TuNgayKy.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgayKy.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayKy),
                        Value = this.DenNgayKy.Value.AddDays(1),
                        Method = FilterMethod.LessThan
                    });
                }

                if (this.TuNgayHetHan.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayThanhLy),
                        Value = this.TuNgayKy.Value,
                        Method = FilterMethod.GreaterThanOrEqualTo
                    });
                }
                if (this.DenNgayHetHan.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayThanhLy),
                        Value = this.DenNgayHetHan.Value.AddDays(1),
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
                MaHd = summary;//Ma Hop Dong
                TenHd = summary;
                GiaTriHD = summary;
                KhachHang = summary;
                NguoiLienHe = summary;
                DonViThucHien = summary;
                NguoiThucHien = summary;
                LyDo = summary;
                DieuKhoanKhac = summary;
            }
        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<Detail>();
            }
            public string Id { get; set; }
            public string MaHd { get; set; }
            public string TenHd { get; set; }
            public DateTime NgayKy { get; set; }
            public string GiaTriHD { get; set; }
            public string KhachHang { get; set; }
            public string DiaChi { get; set; }
            public string NguoiLienHe { get; set; }
            public string DonViThucHien { get; set; }
            public string NguoiThucHien { get; set; }
            public int TinhTrangHopDong { get; set; }
            public DateTime NgayThanhLy { get; set; }
            public string LyDo { get; set; }
            public string DieuKhoanKhac { get; set; }
            public List<Detail> DataDetails { get; set; }
            
        }
        public class Detail
        {
            public string Id { get; set; }
            public string MaHd { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string DonViTinh { get; set; }
            public string Barcode { get; set; }
            public string MaBaoBi { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal? SoLuongLe { get; set; }
            public decimal LuongBao { get; set; }
            public decimal ThanhTien { get; set; }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
            }
            public string Id { get; set; }
            public string MaHd { get; set; }
            public string MaKhachHang { get; set; }
            public string TenKhachHang { get; set; }
            public string NguoiLap { get; set; }
            public string Fax { get; set; }
            public string DienThoai { get; set; }
            public string ThanhTienSauVat { get; set; }
            public string DiaChi { get; set; }
            public DateTime? NgayKy { get; set; }
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
    }
}
