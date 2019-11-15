using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.DCL
{

    public class ParameterCustomerCare
    {
        public string UnitCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public decimal FromMoney { get; set; }
        public decimal ToMoney { get; set; }
        public string MaThe { get; set; }
        public string MaKH { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string DiaChi { get; set; }
        public int? StateExpiredCard { get; set; } //0:Tất, 1: Thẻ đã hết hạn
        public int? StateGiveCard { get; set; } //0: Tất, 1: Chưa cấp, 2: Đã cấp
        public int? StateTypeMoney { get; set; } //0: Tổng tiền, 1: Tiền nguyên giá
        public int? StateCustomerCare { get; set; } //0: Tất, 1: Chưa Chăm sóc
        public string WareHouseCodes { get; set; }
        public string HangKhachHang { get; set; }
        public int? QuenThe { get; set; }
    }

    public class ReportLoyalCustomer
    {
        public ReportLoyalCustomer()
        {
            Data = new List<MdCustomer>();
        }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public List<MdCustomer> Data { get; set; }
        public string UnitCode { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiDonVi { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class ReportBeChangedCardCustomer
    {
        public ReportBeChangedCardCustomer()
        {
            Data = new List<MdCustomerVm.CustomerDto>();
        }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public List<MdCustomerVm.CustomerDto> Data { get; set; }
        public string UnitCode { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiDonVi { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class ReportHistoryGiveCardCustomer
    {
        public ReportHistoryGiveCardCustomer()
        {
            Data = new List<CustomerReport>();
        }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public List<CustomerReport> Data { get; set; }
        public string UnitCode { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiDonVi { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class CustomerReport
    {
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string TenKhac { get; set; }
        public string DiaChi { get; set; }
        public string TinhThanhPho { get; set; }
        public string MaSoThue { get; set; }
        public int TrangThai { get; set; }
        public string DienThoai { get; set; }
        public string ChungMinhThu { get; set; }
        public string Email { get; set; }
        public decimal? SoDiem { get; set; }
        public decimal? TienNguyenGia { get; set; }
        public decimal? TienSale { get; set; }
        public decimal? TongTien { get; set; }
        public int? QuenThe { get; set; }
        public string MaThe { get; set; }
        public DateTime? NgayCapThe { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string GhiChu { get; set; }
        public string HangKhachHang { get; set; }
        public string HangKhachHangCu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public DateTime? NgayDacBiet { get; set; }
        public DateTime? I_Create_Date { get; set; }
        public string I_Create_By { get; set; }
    }
}
