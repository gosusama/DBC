using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using System.Data.Entity;
using System.Diagnostics;
namespace BTS.API.ENTITY
{
    public class ERPContext : DataContext
    {
        public ERPContext()
            : base("name=KNQ.Connection")
        {
            Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer<ERPContext>(new CreateDatabaseIfNotExists<ERPContext>());
            this.Database.Log = s => Debug.WriteLine(s);
        }
        //AU
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<AU_MENU> AU_MENUs { get; set; }
        public virtual DbSet<AU_DONVI> AU_DONVIs { get; set; }
        public virtual DbSet<AU_THAMSOHETHONG> AU_THAMSOHETHONGs { get; set; }
        public virtual DbSet<AU_NGUOIDUNG> AU_NGUOIDUNGs { get; set; }
        public virtual DbSet<AU_NGUOIDUNG_NHOMQUYEN> AU_NGUOIDUNG_NHOMQUYENs { get; set; }
        public virtual DbSet<AU_NGUOIDUNG_QUYEN> AU_NGUOIDUNG_QUYENs { get; set; }
        public virtual DbSet<AU_NHOMQUYEN> AU_NHOMQUYENs { get; set; }
        public virtual DbSet<AU_NHOMQUYEN_CHUCNANG> AU_NHOMQUYEN_CHUCNANGs { get; set; }
        public virtual DbSet<AU_LOG> AU_LOGs { get; set; }
        //
        public virtual DbSet<MdMonitorProcess> MdMonitorProcesses { get; set; }
        public virtual DbSet<MdCustomer> MdCustomers { get; set; }
        public virtual DbSet<MdDepartment> MdDepartments { get; set; }
        //Nguyễn Tuấn Hoàng Anh thêm danh mục Xuât Xứ
        public virtual DbSet<MdXuatXu> MdXuatXu { get; set; }
        public virtual DbSet<MdCurrency> MdCurrencies { get; set; }
        public virtual DbSet<MdPackaging> MdPackagings { get; set; }
        public virtual DbSet<MdTax> Taxs { get; set; }
        public virtual DbSet<MdWareHouse> MdWareHouses { get; set; }
        public virtual DbSet<MdSize> MdSizes { get; set; }
        public virtual DbSet<MdColor> MdColors { get; set; }
        public virtual DbSet<MdHangKH> MdHangKHs { get; set; }
        public virtual DbSet<MdChietKhauKH> MdChietKhauKHs { get; set; }
        public virtual DbSet<MdShelves> MdShelves { get; set; }
        public virtual DbSet<MdMerchandisePrice> MdMerchandisePrices { get; set; }
        public virtual DbSet<MdMerchandise> MdMerchandises { get; set; }
        public virtual DbSet<MdMerchandiseType> MdMerchandiseTypes { get; set; }
        public virtual DbSet<MdIdBuilder> MdIdBuilders { get; set; }
        public virtual DbSet<MdNhomVatTu> MdNhomVatTus { get; set; }
        public virtual DbSet<MdPeriod> MdPeriods { get; set; }
        public virtual DbSet<MdTypeReason> MdTypeReason { get; set; }
        public virtual DbSet<MdSellingMachine> MdSellingMachine { get; set; }
        public virtual DbSet<MdBoHang> MdBoHang { get; set; }
        public virtual DbSet<MdBoHangChiTiet> MdBoHangChiTiet { get; set; }
        public virtual DbSet<MdSupplier> MdSuppliers { get; set; }
        public virtual DbSet<MdDonViTinh> MdDonViTinhs { get; set; }
        public virtual DbSet<MdAsync> MdAsyncs { get; set; }
        //NV

        public virtual DbSet<NvChuongTrinhKhuyenMai> NvChuongTrinhKhuyenMais { get; set; }
        public virtual DbSet<NvChuongTrinhKhuyenMaiChiTiet> NvChuongTrinhKhuyenMaiChiTiets { get; set; }
        public virtual DbSet<NvChuongTrinhKhuyenMaiHangKM> NvChuongTrinhKhuyenMaiHangKMs { get; set; }
      
        public virtual DbSet<NvVatTuChungTu> NvVatTuChungTus { get; set; }
        public virtual DbSet<NvVatTuChungTuChiTiet> NvVatTuChungTuChiTiets { get; set; }
        public virtual DbSet<NvDatHang> NvDatHangs { get; set; }
        public virtual DbSet<NvDatHangChiTiet> NvDatHangChiTiets { get; set; }
        public virtual DbSet<NvCongNo> NvCongNos { get; set; }

        public virtual DbSet<NvNgayHetHanHangHoa> NvNgayHetHanHangHoas { get; set; }
        public virtual DbSet<NvNgayHetHanHangHoaChiTiet> NvNgayHetHanHangHoaChiTiets { get; set; }

        public virtual DbSet<DclCloseout> DclCloseouts { get; set; }
        public virtual DbSet<DclEndingBalance> DclEndingBlance { get; set; }
        //Giao dịch quầy
        public virtual DbSet<NvGiaoDichQuay> NvGiaoDichQuays { get; set; }
        public virtual DbSet<NvGiaoDichQuayChiTiet> NvGiaoDichQuayChiTiets { get; set; }
        //Kiểm kê hàng hóa
        public virtual DbSet<NvKiemKe> NvKiemKes { get; set; }
        public virtual DbSet<NvKiemKeChiTiet> NvKiemKeChiTiets { get; set; }
        public virtual DbSet<MdCity> MdCitys { get; set; }
        public virtual DbSet<MdDistricts> MdDistrictss { get; set; }
        public virtual DbSet<DclGeneralLedger> DclGeneralBooks { get; set; }
        //Thống kê
        public virtual DbSet<ThongKe> ThongKes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TBNETERP");
            base.OnModelCreating(modelBuilder);
        }
        #region Các bảng dữ liệu tạm thời bỏ
        
        //public virtual DbSet<MdContract> MdContracts { get; set; }
        //public virtual DbSet<MdDetailContract> MdDetailContracts { get; set; }
        //public virtual DbSet<MdVoucherType> MdvoucherTypes { get; set; }
        //public virtual DbSet<MdVoucher> MdVouchers { get; set; }
        //public virtual DbSet<MdAccount> MdAccounts { get; set; }
        //public virtual DbSet<MdAccountType> MdAccountTypes { get; set; }
        //public virtual DbSet<MdBankAccount> MdBankAccount { get; set; }
        //public virtual DbSet<MdKhoanMuc> MdKhoanMucs { get; set; }
        //public virtual DbSet<MdCountry> MdCountry { get; set; }
        //public virtual DbSet<NvUyNhiemChi> NvUyNhiemChis { get; set; }
        //public virtual DbSet<NvChungTu> NvChungTus { get; set; }
        //public virtual DbSet<NvChungTuChiTiet> NvChungTuChiTiets { get; set; }
        //public virtual DbSet<PCCongThucGiaThanhChiTiet> PCCongsThucGiaThanhChiTiets { get; set; }
        //public virtual DbSet<PCCongThucGiaThanh> PCCongThucGiaThanhs { get; set; }
        # endregion
    }
}
