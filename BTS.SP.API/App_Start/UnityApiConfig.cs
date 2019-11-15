using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE;
using BTS.API.SERVICE.Authorize;
using BTS.API.SERVICE.Authorize.AuNguoiDung;
using BTS.API.SERVICE.Authorize.AuNguoiDungNhomQuyen;
using BTS.API.SERVICE.Authorize.AuNguoiDungQuyen;
using BTS.API.SERVICE.Authorize.AuNhomQuyen;
using BTS.API.SERVICE.Authorize.AuNhomQuyenChucNang;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using System.Web.Http;
using BTS.API.SERVICE.Authorize.AuDonVi;
using BTS.API.SERVICE.Authorize.AuMenu;
using BTS.API.SERVICE.Authorize.AuThamSoHeThong;
using BTS.SP.API.Utils;
using BTS.API.SERVICE.Dashboard;
using Unity.Lifetime;
using Unity;
namespace BTS.SP.API.App_Start
{
    public static class UnityApiConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            var container = new UnityContainer();
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            // e.g. container.RegisterType<ITestService, TestService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDataContext, ERPContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager());
            //Dashboard
            container.RegisterType<IDashboardService, DashboardService>(new HierarchicalLifetimeManager());
            //AU
            container.RegisterType<IClientService, ClientService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<Client>, Repository<Client>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<RefreshToken>, Repository<RefreshToken>>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_NGUOIDUNG>, Repository<AU_NGUOIDUNG>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuNguoiDungService, AuNguoiDungService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_DONVI>, Repository<AU_DONVI>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuDonViService, AuDonViService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_MENU>, Repository<AU_MENU>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuMenuService, AuMenuService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_THAMSOHETHONG>, Repository<AU_THAMSOHETHONG>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuThamSoHeThongService, AuThamSoHeThongService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_NHOMQUYEN>, Repository<AU_NHOMQUYEN>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuNhomQuyenService, AuNhomQuyenService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_NHOMQUYEN_CHUCNANG>, Repository<AU_NHOMQUYEN_CHUCNANG>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuNhomQuyenChucNangService, AuNhomQuyenChucNangService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_NGUOIDUNG_NHOMQUYEN>, Repository<AU_NGUOIDUNG_NHOMQUYEN>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuNguoiDungNhomQuyenService, AuNguoiDungNhomQuyenService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<AU_NGUOIDUNG_QUYEN>, Repository<AU_NGUOIDUNG_QUYEN>>(new HierarchicalLifetimeManager());
            container.RegisterType<IAuNguoiDungQuyenService, AuNguoiDungQuyenService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedService, SharedService>(new HierarchicalLifetimeManager());
            //end AU

            container.RegisterType<IRepository<MdIdBuilder>, Repository<MdIdBuilder>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdIdBuilderService, MdIdBuilderService>(new HierarchicalLifetimeManager());
            //Register Md
            container.RegisterType<IRepository<MdMonitorProcess>, Repository<MdMonitorProcess>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdCountry>, Repository<MdCountry>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdShelves>, Repository<MdShelves>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdShelvesService, MdShelvesService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdTypeReason>, Repository<MdTypeReason>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdTypeReasonService, MdTypeReasonService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdSize>, Repository<MdSize>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdSizeService, MdSizeService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdColor>, Repository<MdColor>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdColorService, MdColorService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdDonViTinh>, Repository<MdDonViTinh>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdDonViTinhService, MdDonViTinhService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdSupplier>, Repository<MdSupplier>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdSupplierService, MdSupplierService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdPeriod>, Repository<MdPeriod>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdPeriodService, MdPeriodService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdTax>, Repository<MdTax>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdTaxService, MdTaxService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdCurrency>, Repository<MdCurrency>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdCurrencyService, MdCurrencyService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdCustomer>, Repository<MdCustomer>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdCustomerService, MdCustomerService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdMerchandise>, Repository<MdMerchandise>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdMerchandisePrice>, Repository<MdMerchandisePrice>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdMerchandisePriceService, MdMerchandisePriceService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdMerchandiseService, MdMerchandiseService>(new HierarchicalLifetimeManager());

            container.RegisterType<INvKiemKeService, NvKiemKeService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdDetailContract>, Repository<MdDetailContract>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdPackaging>, Repository<MdPackaging>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdPackagingService, MdPackagingService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdWareHouse>, Repository<MdWareHouse>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdWareHouseService, MdWareHouseService>(new HierarchicalLifetimeManager());

            //ANH PT DANH MỤC XUẤT XỨ
            container.RegisterType<IRepository<MdXuatXu>, Repository<MdXuatXu>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdXuatXuService, MdXuatXuService>(new HierarchicalLifetimeManager());

            //city
            //districts
            container.RegisterType<IRepository<MdCity>, Repository<MdCity>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdCityService, MdCityService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdDistricts>, Repository<MdDistricts>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdDistrictsService, MdDistrictsService>(new HierarchicalLifetimeManager());

            //container.Register<IRepository<MdFormula>, Repository<MdFormula>>(new HierarchicalLifetimeManager());
            //container.Register<IMdFormulaService, MdFormulaService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdMerchandiseType>, Repository<MdMerchandiseType>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdMerchandiseTypeService, MdMerchandiseTypeService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdContract>, Repository<MdContract>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdContractService, MdContractService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdNhomVatTu>, Repository<MdNhomVatTu>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdNhomVatTuService, MdNhomVatTuService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdSellingMachine>, Repository<MdSellingMachine>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdSellingMachineService, MdSellingMachineService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdBoHang>,Repository<MdBoHang>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdBoHangService, MdBoHangService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdBoHangChiTiet>, Repository<MdBoHangChiTiet>>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<MdHangKH>, Repository<MdHangKH>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdHangKhService, MdHangKhService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdChietKhauKH>, Repository<MdChietKhauKH>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdChietKhauKhService, MdChietKhauKhService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<MdDepartment>, Repository<MdDepartment>>(new HierarchicalLifetimeManager());
            container.RegisterType<IMdDepartmentService, MdDepartmentService>(new HierarchicalLifetimeManager());
        

            //Ban Le Hang Hoa
            container.RegisterType<INvRetailsService, NvBanLeService>(new HierarchicalLifetimeManager());

            container.RegisterType<ICustomerCareService, CustomerCareService>(new HierarchicalLifetimeManager());

            container.RegisterType<ISinhNhatKhService, SinhNhatKhService>(new HierarchicalLifetimeManager());
            container.RegisterType<ILanDauKhService, LanDauKhService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDoanhSoSnService, DoanhSoSnService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDoanhSoMoiService, DoanhSoMoiService>(new HierarchicalLifetimeManager());

            //Nghiệp vụ
            //CTKM
            container.RegisterType<IRepository<NvChuongTrinhKhuyenMai>, Repository<NvChuongTrinhKhuyenMai>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvChuongTrinhKhuyenMaiChiTiet>, Repository<NvChuongTrinhKhuyenMaiChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvChuongTrinhKhuyenMaiHangKM>, Repository<NvChuongTrinhKhuyenMaiHangKM>>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiDongGiaService, NvKhuyenMaiDongGiaService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiVoucherService, NvKhuyenMaiVoucherService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiTinhTienService, NvKhuyenMaiTinhTienService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiBuy1Get1Service, NvKhuyenMaiBuy1Get1Service>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiComboService, NvKhuyenMaiComboService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKhuyenMaiTichDiemService, NvKhuyenMaiTichDiemService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvChuongTrinhKhuyenMaiService, NvChuongTrinhKhuyenMaiService>(new HierarchicalLifetimeManager());
            //
            container.RegisterType<IRepository<NvNgayHetHanHangHoaChiTiet>, Repository<NvNgayHetHanHangHoaChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvNgayHetHanHangHoa>, Repository<NvNgayHetHanHangHoa>>(new HierarchicalLifetimeManager());

            container.RegisterType<INvNgayHetHanHangHoaService, NvNgayHetHanHangHoaService>(new HierarchicalLifetimeManager());
            // authorize
            //Data
            container.RegisterType<IRepository<DclGeneralLedger>, Repository<DclGeneralLedger>>(new HierarchicalLifetimeManager());
            container.RegisterType<IDclGeneralLedgerService, DclGeneralLedgerService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<DclEndingBalance>, Repository<DclEndingBalance>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<DclCloseout>, Repository<DclCloseout>>(new HierarchicalLifetimeManager());
            container.RegisterType<IDclCloseoutService, DclCloseoutService>(new HierarchicalLifetimeManager());

            //log
            //báo cáo xuất nhập tồn chi tiết -- Phạm Tuấn Anh
            container.RegisterType<IXuatNhapTonChiTietService, XuatNhapTonChiTietService>(new HierarchicalLifetimeManager());
            //end

            //DieuChuyen noi bo
            container.RegisterType<IRepository<NvVatTuChungTu>, Repository<NvVatTuChungTu>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvVatTuChungTuChiTiet>, Repository<NvVatTuChungTuChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<INvPhieuDieuChuyenNoiBoService, NvPhieuDieuChuyenNoiBoService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvXuatBanService, NvXuatBanService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvXuatBanLeService, NvXuatBanLeService>(new HierarchicalLifetimeManager());
            //nhap hang mua
            container.RegisterType<INvTonDauKyService, NvTonDauKyService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvNhapHangBanTraLaiService, NvNhapHangBanTraLaiService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvNhapHangMuaService, NvNhapHangMuaService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvNhapKhacService, NvNhapKhacService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvXuatKhacService, NvXuatKhacService>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvDatHang>, Repository<NvDatHang>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvDatHangChiTiet>, Repository<NvDatHangChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<INvPhieuDatHangService, NvPhieuDatHangService>(new HierarchicalLifetimeManager());
            container.RegisterType<INvPhieuDatHangNCCService, NvPhieuDatHangNCCService>(new HierarchicalLifetimeManager());
            //Công nợ
            container.RegisterType<INvCongNoService, NvCongNoService>(new HierarchicalLifetimeManager());
            // Giao Dich Quay Async client
            container.RegisterType<IRepository<NvGiaoDichQuay>,Repository<NvGiaoDichQuay>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvGiaoDichQuayChiTiet>, Repository<NvGiaoDichQuayChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<INvGiaoDichQuayService, NvGiaoDichQuayService>(new HierarchicalLifetimeManager());

            container.RegisterType<IRepository<NvKiemKe>, Repository<NvKiemKe>>(new HierarchicalLifetimeManager());
            container.RegisterType<IRepository<NvKiemKeChiTiet>, Repository<NvKiemKeChiTiet>>(new HierarchicalLifetimeManager());
            container.RegisterType<INvKiemKeService, NvKiemKeService>(new HierarchicalLifetimeManager());
            //registering Unity for web API
            config.DependencyResolver = new UnityResolver(container);

        }
    }
}