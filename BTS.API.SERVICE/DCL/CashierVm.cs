using BTS.API.SERVICE.NV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.DCL
{
    public class CashierVm
    {
        public string NguoiTao { get; set; }
        public string MaMayBan { get; set; }
        public decimal TongBan { get; set; }
        public decimal TongTra { get; set; }
        public decimal ThucThu { get; set; }
    }
    public class ParameterCashier
    {
        public string UnitCode { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string SellingMachineCodes { get; set; }
        public string CashieerCodes { get; set; }

        public string WareHouseCodes { get; set; }
        public string MerchandiseTypeCodes { get; set; }
        public string MerchandiseCodes { get; set; }
        public string MerchandiseGroupCodes { get; set; }
        public string NhaCungCapCodes { get; set; }
        public string XuatXuCodes { get; set; }
        public TypeGroupInventoryCashier GroupBy { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public PHUONGTHUCNHAP phuongthucnhap { get; set; }
        public TypeGiaoDich LoaiGiaoDich { get; set; }
    }

    public class ParameterExcelByCondition
    {
        public DateTime DenNgay { get; set; }
        public DateTime TuNgay { get; set; }
        public string MaChungTu { get; set; }
        public string MaHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string MaSoThue { get; set; }
        public string MaKhoNhap { get; set; }
        public string NoiDung { get; set; }
        public string UnitCode { get; set; }
    }
    public enum TypeGroupInventoryCashier
    {
        MAKHO,
        MALOAIVATTU,
        MANHOMVATTU,
        MAVATTU,
        MAKHACHHANG,
        MAGIAODICH,
        MADONVI,
    }
    public enum TypeGiaoDich
    {
        XUATBANLE = 0,
        NHAPBANLETRALAI = 1
    }
    public class ReportCashier
    {
        public ReportCashier()
        {
            Data = new List<CashierVm>();
        }
        public int CreateDay { get; private set; }

        public int CreateMonth { get; private set; }
        public int CreateYear { get; private set; }
        public string CreateBy { get; private set; }
        public List<CashierVm> Data { get; set; }
        public void CreateDate()
        {
            var currentDate = DateTime.Now.Date;
            this.CreateDay = currentDate.Day;
            this.CreateMonth = currentDate.Month;
            this.CreateYear = currentDate.Year;
        }
        public void SetWhoCreateThis(string name)
        {
            this.CreateBy = name;
        }
    }
}
