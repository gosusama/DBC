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
    public class ParameterLanDauKh
    {
        public string UnitCode { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public decimal? FromMoney { get; set; }
        public decimal? ToMoney { get; set; }    
        public string WareHouseCodes { get; set; }
    }
    public class ReportLanDauKh
    {
        public ReportLanDauKh()
        {
            //Data = new List<CustomLanDauKhReport>();
            DataDetails = new List<CustomLanDauKhReportLevel2>();
        }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public List<CustomLanDauKhReportLevel2> DataDetails { get; set; }
        public string UnitCode { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiDonVi { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
      
    }
    public class CustomLanDauKhReportLevel2
    {
        public CustomLanDauKhReportLevel2()
        {
            DataDetails = new List<CustomLanDauKhReport>();
        }
        //public string MaKh { get; set; }
        //public string TenKhachHang { get; set; }
        public string MaGiaoDichQuayPk { get; set; }
        public Decimal? ThanhTien { get; set; }
        public Decimal? ChietKhau { get; set; }
        public List<CustomLanDauKhReport> DataDetails { get; set; }
    }
    public class CustomLanDauKhReport
    {   
        public DateTime? NgayPhatSinh { get; set; }
        public string MaGiaoDichQuayPk { get; set; }
        public string TenKhachHang { get; set; }
        public string MaVatTu { get; set; }
        public Decimal? SoLuong { get; set; }
        public Decimal? DonGia { get; set; }
        public Decimal? ThanhTien { get; set; }
        public Decimal? ChietKhau { get; set; }
        public string DienThoai { get; set; }
        public string MaKho { get; set; }
    }
}
