using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;

namespace BTS.API.SERVICE.NV
{
    public class NvNgayHetHanHangHoaVm
    {
        public class Search : IDataSearch
        {
            public string MaPhieu { get; set; }
            public string MaPhieuPk { get; set; }
            public DateTime? NgayBaoDate { get; set; }
            public string ThoiGian { get; set; }
            public int TrangThai { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new NvNgayHetHanHangHoa().MaPhieu);
                }
            }
            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new NvNgayHetHanHangHoa();

                if (!string.IsNullOrEmpty(this.MaPhieu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaPhieu),
                        Value = this.MaPhieu,
                        Method = FilterMethod.Like
                    });
                }
                if (this.NgayBaoDate.HasValue)
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.NgayBaoDate),
                        Value = this.NgayBaoDate.Value,
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
                MaPhieu = summary;
            }
        }

        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {
                DataDetails = new List<DtoDetail>();
            }
            public string Id { get; set; }
            public string MaPhieu { get; set; }
            public string MaPhieuPk { get; set; }
            public DateTime? NgayBaoDate { get; set; }
            public string ThoiGian { get; set; }
            public string NoiDung { get; set; }
            public int TrangThai { get; set; }
            public string IState { get; set; }
            public string ICreateBy { get; set; }
            public DateTime? ICreateDate { get; set; }
            public DateTime? IUpdateDate { get; set; }
            public string IUpdateBy { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
        }

        public class DtoDetail
        {
            public int Index { get; set; }
            public string Id { get; set; }
            public string MaPhieuPk { get; set; }
            public string MaNhaCungCap { get; set; }
            public string TenNhaCungCap { get; set; }
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string BarCode { get; set; }
            public Nullable<decimal> SoLuong { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public string IState { get; set; }
            public string ICreateBy { get; set; }
            public DateTime? ICreateDate { get; set; }
            public DateTime? IUpdateDate { get; set; }
            public string IUpdateBy { get; set; }
            public Nullable<decimal> ConLai_NgayBao { get; set; }
            public Nullable<decimal> ConLai_NgayHetHan { get; set; }
        }

        public class ReportModel
        {
            public ReportModel()
            {
                DataReportDetails = new List<ReportDetailModel>();
            }
            public string Id { get; set; }
            public string MaPhieu { get; set; }
            public string MaPhieuPk { get; set; }
            public DateTime? NgayBaoDate { get; set; }
            public string ThoiGian { get; set; }
            public string NoiDung { get; set; }
            public int TrangThai { get; set; }
            public string NameNhanVienCreate { get; set; }
            public int CreateDay { get; set; }
            public int CreateMonth { get; set; }
            public int CreateYear { get; set; }
            public List<ReportDetailModel> DataReportDetails { get; set; }
        }

        public class ReportDetailModel
        {
            public string Id { get; set; }
            public string MaPhieuPk { get; set; }
            public string MaNhaCungCap { get; set; }
            public string TenNhaCungCap { get; set; }
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string BarCode { get; set; }
            public Nullable<decimal> SoLuong { get; set; }
            public DateTime? NgaySanXuat { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public Nullable<decimal> ConLai_NgayBao { get; set; }
            public Nullable<decimal> ConLai_NgayHetHan { get; set; }
        }

        public class ParameterNgayHetHanHangHoa
        {
            public string UnitCode { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime FromDate { get; set; }
            public string MerchandiseTypeCodes { get; set; }
            public string MerchandiseCodes { get; set; }
            public string MerchandiseGroupCodes { get; set; }
            public string NhaCungCapCodes { get; set; }
            public string GroupBy { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
        }
    }
}