using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Authorize;
using System.Web;
using System.Security.Claims;

namespace BTS.API.SERVICE.DCL
{
    public class InventoryExpImpLevel2
    {
        public InventoryExpImpLevel2()
        {
            DataDetails = new List<InventoryExpImp>();
        }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public List<InventoryExpImp> DataDetails { get; set; }
    }
    public class InventoryExpImp
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; } // Đơn vị tính
        public decimal OpeningBalanceQuantity { get; set; } // tồn số lượng đầu kỳ
        public decimal OpeningBalanceValue { get; set; } // tồn giá trị đầu kỳ
        public decimal IncreaseQuantity { get; set; } // Phát sinh tăng
        public decimal IncreaseValue { get; set; } // Phát sinh tăng
        public decimal DecreaseQuantity { get; set; } // Phát sinh giảm
        public decimal DecreaseValue { get; set; } // Phát sinh giảm
        public decimal ClosingQuantity { get; set; } // Tồn cuối kỳ
        public decimal ClosingValue { get; set; } // Tồn đầu kỳ
        public decimal CostOfGoodsSold { get; set; }
        public string UnitCode { get; set; }
        public string WareHouseCode { get; set; }
        public decimal CostOfCapital { get; set; }
        public string MaDonVi { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public virtual string GetParentUnitCode()
        {
            if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
            {
                var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                var parentunit = currentUser.Claims.FirstOrDefault(x => x.Type == "parentUnitCode");
                if (parentunit != null) return parentunit.Value;
            }
            return "";
        }

        public void MapSupplierName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var customer = unitOfWork.Repository<MdSupplier>().DbSet.FirstOrDefault(x => x.MaNCC == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (customer != null)
            {
                this.Name = customer.TenNCC;
            }
        }

        public void MapUnitUserName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var customer = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (customer != null)
            {
                this.Name = customer.TenDonVi;
            }
        }

        public void MapCustomerName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var customer = unitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (customer != null)
            {
                this.Name = customer.TenKH;
            }
        }

      
        public void MapWareHouseName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var wareHouse = unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (wareHouse != null)
            {
                this.Name = wareHouse.TenKho;
            }
        }
        public void MapGroupName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var group = unitOfWork.Repository<MdNhomVatTu>().DbSet.FirstOrDefault(x => x.MaNhom == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (group != null)
            {
                this.Name = group.TenNhom;
            }
        }
        public void MapTypeName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var type = unitOfWork.Repository<MdMerchandiseType>().DbSet.FirstOrDefault(x => x.MaLoaiVatTu == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (type != null)
            {
                this.Name = type.TenLoaiVatTu;
            }
        }
        public void MapMerchandiseName(IUnitOfWork unitOfWork)
        {
            var _ParentUnitCode = GetParentUnitCode();
            var type = unitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu == this.Code && x.UnitCode.StartsWith(_ParentUnitCode));
            if (type != null)
            {
                this.Name = type.TenHang;
            }
        }
    }
    public class InventoryExcelItem
    {
        public string Code { get; set; }
        public string CodeParent { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Unit { get; set; } // Đơn vị tính
        public decimal OpeningBalanceQuantity { get; set; } // tồn số lượng đầu kỳ
        public decimal OpeningBalanceValue { get; set; } // tồn giá trị đầu kỳ
        public decimal IncreaseQuantity { get; set; } // Phát sinh tăng
        public decimal IncreaseValue { get; set; } // Phát sinh tăng
        public decimal DecreaseQuantity { get; set; } // Phát sinh giảm
        public decimal DecreaseValue { get; set; } // Phát sinh giảm
        public decimal ClosingQuantity { get; set; } // Tồn cuối kỳ
        public decimal ClosingValue { get; set; } // Tồn đầu kỳ
        public string UnitCode { get; set; }
        public string WareHouseCode { get; set; }
        public string TenVatTu { get; set; }


    }

    public class InventoryExcel
    {
        public InventoryExcel()
        {
            DetailData = new List<InventoryExcelItem>();
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitCode { get; set; }
        public decimal OpeningBalanceQuantity { get; set; } // tồn số lượng đầu kỳ
        public decimal OpeningBalanceValue { get; set; } // tồn giá trị đầu kỳ
        public decimal IncreaseQuantity { get; set; } // Phát sinh tăng
        public decimal IncreaseValue { get; set; } // Phát sinh tăng
        public decimal DecreaseQuantity { get; set; } // Phát sinh giảm
        public decimal DecreaseValue { get; set; } // Phát sinh giảm
        public decimal ClosingQuantity { get; set; } // Tồn cuối kỳ
        public decimal ClosingValue { get; set; } // Tồn đầu kỳ
        public List<InventoryExcelItem> DetailData { get; set; }

    }
    public class InventoryReport
    {
        public InventoryReport()
        {
            DetailData = new List<InventoryExpImpLevel2>();
        }
        public int Period { get; set; }
        public int Year { get; set; }
        public string UnitCode { get; set; }
        public string UnitUserName { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public string GroupType { get; set; }
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
        public List<InventoryExpImpLevel2> DetailData { get; set; }
        public void MapUnitUserName(IUnitOfWork unitOfWork)
        {
            //var wareHouse =  unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.WareHouseCode);
            var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
            //if (wareHouse != null)
            //{
            //    this.WareHouseName = wareHouse.TenKho;
            //}
            if (unitUser != null)
            {
                this.UnitUserName = unitUser.TenDonVi;
            }

        }
        public void CreateDateNow()
        {
            var createDate = DateTime.Now;
            this.CreateDay = createDate.Day;
            this.CreateMonth = createDate.Month;
            this.CreateYear = createDate.Year;
        }
    }
    public class ParameterInventory
    {
        public int Period { get; set; }
        public int Year { get; set; }
        public string UnitCode { get; set; }
        public string WareHouseCodes { get; set; }
        public string MerchandiseTypeCodes { get; set; }
        public string MerchandiseCodes { get; set; }
        public string MerchandiseGroupCodes { get; set; }
        public string NhaCungCapCodes { get; set; }
        public string WareHouseRecieveCode { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        public TypeValueInventory TypeValue { get; set; }
        public int IsOnlyInventory { get; set; }
        public TypeGroupInventory GroupBy { get; set; }
    }
    public class InventoryDetailReport
    {
        public InventoryDetailReport()
        {
            DataDetails = new List<InventoryDetailItem>();
        }
        public int Period { get; set; }
        public int Year { get; set; }
        public string UnitCode { get; set; }
        public string UnitUserName { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public string GroupType { get; set; }
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
        public List<InventoryDetailItem> DataDetails { get; set; }
        public void MapUnitUserName(IUnitOfWork unitOfWork)
        {
            //var wareHouse =  unitOfWork.Repository<MdWareHouse>().DbSet.FirstOrDefault(x => x.MaKho == this.WareHouseCode);
            var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
            //if (wareHouse != null)
            //{
            //    this.WareHouseName = wareHouse.TenKho;
            //}
            if (unitUser != null)
            {
                this.UnitUserName = unitUser.TenDonVi;
            }

        }
        public void CreateDateNow()
        {
            var createDate = DateTime.Now;
            this.CreateDay = createDate.Day;
            this.CreateMonth = createDate.Month;
            this.CreateYear = createDate.Year;
        }
    }
    public class InventoryDetailNewReport
    {
        public InventoryDetailNewReport()
        {
            DataDetails = new List<InventoryDetailItemCha>();
        }
        public int Period { get; set; }
        public int Year { get; set; }
        public string UnitCode { get; set; }
        public string UnitUserName { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public string GroupType { get; set; }
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
        public List<InventoryDetailItemCha> DataDetails { get; set; }
        public void MapUnitUserName(IUnitOfWork unitOfWork)
        {
            var unitUser = unitOfWork.Repository<AU_DONVI>().DbSet.FirstOrDefault(x => x.MaDonVi == this.UnitCode);
            if (unitUser != null)
            {
                this.UnitUserName = unitUser.TenDonVi;
            }

        }
        public void CreateDateNow()
        {
            var createDate = DateTime.Now;
            this.CreateDay = createDate.Day;
            this.CreateMonth = createDate.Month;
            this.CreateYear = createDate.Year;
        }
    }

    public class InventoryReportExcel
    {
        public InventoryReportExcel()
        {
            DetailData = new List<InventoryExpImp>();
        }
        public int Period { get; set; }
        public int Year { get; set; }
        public string UnitCode { get; set; }
        public string UnitUserName { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public string GroupType { get; set; }
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
        public List<InventoryExpImp> DetailData { get; set; }
    }
    public class InventoryDetailItemCha
    {
        public InventoryDetailItemCha()
        {
            DataDetails = new List<InventoryDetailItem>();
        }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public List<InventoryDetailItem> DataDetails { get; set; }
    }
    public class InventoryDetailItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MaCha { get; set; }
        public string TenCha { get; set; }
        public string UnitCode { get; set; }
        public decimal XBanLeQuay_Sl { get; set; }
        public decimal XBanLeQuay_Gt { get; set; }
        public decimal XBanLeTL_Sl { get; set; }
        public decimal XBanLeTL_Gt { get; set; }
        public decimal Nmua_Sl { get; set; }
        public decimal Nmua_Gt { get; set; }
        public decimal NhapKiemKe_Sl { get; set; }
        public decimal NhapKiemKe_Gt { get; set; }
        public decimal XuatKiemKe_Sl { get; set; }
        public decimal XuatKiemKe_Gt { get; set; }
        public decimal NhapChuyenKho_Sl { get; set; }
        public decimal NhapChuyenKho_Gt { get; set; }
        public decimal NhapSTThanhVien_Sl { get; set; }
        public decimal NhapSTThanhVien_Gt { get; set; }
        public decimal XuatChuyenKho_Sl { get; set; }
        public decimal XuatChuyenKho_Gt { get; set; }
        public decimal XuatSTThanhVien_Sl { get; set; }
        public decimal XuatSTThanhVien_Gt { get; set; }
        public decimal XBanLe_Sl { get; set; }
        public decimal XBanLe_Gt { get; set; }
        public decimal XBanBuon_Sl { get; set; }
        public decimal XBanBuon_Gt { get; set; }
        public decimal NhapDieuChinh_Sl { get; set; }
        public decimal NhapDieuChinh_Gt { get; set; }
        public decimal NhapHangAm_Sl { get; set; }
        public decimal NhapHangAm_Gt { get; set; }
        public decimal XuatHuyHH_Sl { get; set; }
        public decimal XuatHuyHH_Gt { get; set; }
        public decimal XuatHuy_Sl { get; set; }
        public decimal XuatHuy_Gt { get; set; }
        public decimal XuatTraNCC_Sl { get; set; }
        public decimal XuatTraNCC_Gt { get; set; }
        public decimal XuatDC_Sl { get; set; }
        public decimal XuatDC_Gt { get; set; }
        public decimal NhapBanTL_Sl { get; set; }
        public decimal NhapBanTL_Gt { get; set; }
        public decimal TonDauKy_Sl { get; set; }
        public decimal TonDauKy_Gt { get; set; }
        public decimal TonCuoiKy_Sl { get; set; }
        public decimal TonCuoiKy_Gt { get; set; }
    }

    public class InventoryUniCode
    {
        public string MA { get; set; }
        public string TEN { get; set; }
        public decimal CH1 { get; set; }
        public decimal CH2 { get; set; }
        public decimal CH3 { get; set; }
        public decimal CH4 { get; set; }
        public decimal CH5 { get; set; }
        public decimal CH6 { get; set; }
        public decimal CH7 { get; set; }
        public decimal CH8 { get; set; }
        public decimal CH9 { get; set; }
        public decimal CH10 { get; set; }
    }
    public enum TypeGroupInventory
    {
        MADONVI = 0,
        WAREHOUSE = 1,
        TYPE = 2,
        GROUP = 3,
        MERCHANDISE = 4,
        NHACUNGCAP = 5
    }

    public enum TypeValueInventory
    {
        ALL,
        POSITIVE,
        NEGATIVE,
        ZERO
    }
}
