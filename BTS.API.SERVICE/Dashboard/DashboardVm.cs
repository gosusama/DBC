using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Dashboard
{
    public enum LoaiThongKe
    {
        XBANLE,
    }
    public class DashboardVm
    {
        public class Parameter
        {
            public DateTime TuNgay { get; set; }
            public DateTime DenNgay { get; set; }
            public string MaVatTu { get; set; }
        }
        public class RetailRevenue
        {
            public RetailRevenue()
            {
                DataDetails = new List<RetailRevenueDetail>();
            }
            public string UnitCode { get; set; }
            public string TenDonVi { get; set; }
            public List<RetailRevenueDetail> DataDetails { get; set; }
        }
        public class RetailRevenueDetail
        {
            public DateTime? NgayCT { get; set; }
            public string NgayCTMobile { get; set; }
            public decimal DoanhThu { get; set; }
        }
        public class BestMerchandise
        {
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public decimal GiaTri { get; set; }
        }
        public class InventoryMerchandise
        {
            public string UnitCode { get; set; }
            public string MaKho { get; set; }
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string TenKho { get; set; }
            public int SoLuong { get; set; }
            public decimal GiaTri { get; set; }
        }
        public class TransactionAmmount
        {
            public TransactionAmmount()
            {
                DataDetails = new List<TransactionAmmountDetail>();
            }
            public string NgayChungTu { get; set; }
            public List<TransactionAmmountDetail> DataDetails { get; set; }
        }
        public class TransactionAmmountDetail
        {
            public string UnitCode { get; set; }
            public int LoaiNhapXuat { get; set; }
            public string NgayChungTu { get; set; }
            public string LoaiChungTu { get; set; }
            public string TenChungTu { get; set; }
            public decimal GiaTri { get; set; }
        }
    }
}
