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
    public class ParameterDoanhSoSn
    {
        public string UnitCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public decimal FromMoney { get; set; }
        public decimal ToMoney { get; set; }

        public string CustomerCodes { get; set; }
        public string WareHouseCodes { get; set; }
      
     
    }

    public class ReportDSSN
    {
        public ReportDSSN()
        {
            Data = new List<CustomDoanhSoSnReport>();
        }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
        public List<CustomDoanhSoSnReport> Data { get; set; }
        public string UnitCode { get; set; }
        public string TenDonVi { get; set; }
        public string DiaChiDonVi { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public void MapUnitUserName(IUnitOfWork unitOfWork)
        {
            var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
            if (unitUser != null)
            {
                this.TenDonVi = unitUser.TenDonVi;
            }
        }
    }
    public class CustomDoanhSoSnReport
    {
        public string MaKH { get; set; }
        public string TenKH { get; set; }

        public string DiaChi { get; set; }

        public string DienThoai { get; set; }

        public decimal? SoTien { get; set; }

        public string MaThe { get; set; }

        public string GhiChu { get; set; }

        public DateTime? NgaySinh { get; set; }

        public DateTime? I_Create_Date { get; set; }
        public string I_Create_By { get; set; }
        public string MaDonVi { get; set; }

        public void MapCustomerName(IUnitOfWork unitOfWork)
        {
            var customer = unitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == this.MaKH);
            if (customer != null)
            {
                this.TenKH = customer.TenKH;
            }
        }



    }


}
