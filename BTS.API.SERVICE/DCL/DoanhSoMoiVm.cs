using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BTS.API.SERVICE.DCL
{
   
        public class ParameterDoanhSoMoi
        {
            public string UnitCode { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public DateTime MinDate { get; set; }
            public DateTime MaxDate { get; set; }
            public string WareHouseCodes { get; set; }


        }

        public class ReportDSMoi
        {
            public ReportDSMoi()
            {
                Data = new List<CustomDoanhSoMoiReport>();
            }
            public DateTime CreateDate { get; set; }
            public string Username { get; set; }
            public List<CustomDoanhSoMoiReport> Data { get; set; }
            public string UnitCode { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            
        }
        public class CustomDoanhSoMoiReport
        {
            public string MaDonVi { get; set; }
            public decimal? SoKhach { get; set; }
            public decimal? SoGiaoDich { get; set; }
            public decimal? DoanhThu { get; set; }
            public decimal? TraLai { get; set; }
            public decimal? ChietKhau { get; set; }
            public decimal? ThucThu { get; set; }


        }
        public class ParameterDoanhSoMoiDetails
        {
            public string MaDonVi { get; set; }
            public decimal? SoKhach { get; set; }
            public decimal? SoGiaoDich { get; set; }
            public decimal? DoanhThu { get; set; }
            public decimal? TraLai { get; set; }
            public decimal? ChietKhau { get; set; }
            public decimal? ThucThu { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string UnitCode { get; set; }
            public string WareHouseCodes { get; set; }

        }
        public class ReportDSMoiDetails
        {
            public ReportDSMoiDetails()
            {
                Data = new List<CustomDoanhSoMoiReportDetails>();
            }
            public DateTime CreateDate { get; set; }
            public string Username { get; set; }
            public List<CustomDoanhSoMoiReportDetails> Data { get; set; }
            public string UnitCode { get; set; }
            public string TenDonVi { get; set; }
            public string DiaChiDonVi { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

        }
        public class CustomDoanhSoMoiReportDetails
        {
            public DateTime? NgayTao { get; set; }
            public string MaKH { get; set; }
            public string TenKH { get; set; }
            public string DienThoai { get; set; }
            public DateTime? NgaySinh { get; set; }
            public DateTime? NgayDacBiet { get; set; } 
            public string Email { get; set; }
            public string MaThe { get; set; }
            public DateTime? NgayCapThe { get; set; }
            public DateTime? NgayHetHan { get; set; }   
            public string MaDonVi { get; set; }

        }
  
}
