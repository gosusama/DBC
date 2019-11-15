using BTS.API.ENTITY;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using System;
using System.Web;
using BTS.API.SERVICE.Authorize.AuNguoiDung;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuDonVi;
using BTS.API.SERVICE.Authorize.AuThamSoHeThong;
using AutoMapper;
using BTS.API.ASYNC.DatabaseContext;
namespace BTS.API.SERVICE
{
    public static class AutoMapperConfig
    {
        public static void Config()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap(typeof(PagedObj), typeof(PagedObj));
                cfg.CreateMap(typeof(PagedObj<>), typeof(PagedObj<>));
                //Nghiệp vụ
                cfg.CreateMap<NvDatHang, ChoiceObj>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.SoPhieu))
                 .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.SoPhieu, src.NoiDung)))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.SoPhieuPk))
                 .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                 .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //Nv 
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvChuongTrinhKhuyenMaiVm.Dto>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvChuongTrinhKhuyenMaiVm.DtoDetail>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                cfg.CreateMap<NvDatHang, NvPhieuDatHangVm.Dto>();
                cfg.CreateMap<NvDatHangChiTiet, NvPhieuDatHangVm.DtoDetail>();
                cfg.CreateMap<NvPhieuDatHangVm.Dto, NvDatHang>();
                cfg.CreateMap<NvPhieuDatHangVm.DtoDetail, NvDatHangChiTiet>();
                //DatHang NCC
                cfg.CreateMap<NvDatHang, NvPhieuDatHangNCCVm.Dto>();
                cfg.CreateMap<NvDatHangChiTiet, NvPhieuDatHangNCCVm.DtoDetail>();
                cfg.CreateMap<NvPhieuDatHangNCCVm.Dto, NvDatHang>();
                cfg.CreateMap<NvPhieuDatHangNCCVm.DtoDetail, NvDatHangChiTiet>();
                //Nv DieuChuyenNoiBo
                cfg.CreateMap<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvPhieuDieuChuyenNoiBoVm.DtoDetail>();
                //Nv NhapHangBanTraLai
                cfg.CreateMap<NvVatTuChungTu, NvNhapHangBanTraLaiVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapHangBanTraLaiVm.DtoDetail>();
                //Nv NhapHangMua
                cfg.CreateMap<NvVatTuChungTu, NvNhapHangMuaVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapHangMuaVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvNhapHangMuaVm.DtoClauseDetail>();
                //Nv NhapKhac
                cfg.CreateMap<NvVatTuChungTu, NvNhapKhacVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapKhacVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvNhapKhacVm.DtoClauseDetail>();
                //Nv XuatBan
                cfg.CreateMap<NvVatTuChungTu, NvXuatBanVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatBanVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvXuatBanVm.DtoClauseDetail>();
                //Nv XuatBanLe
                cfg.CreateMap<NvVatTuChungTu, NvXuatBanLeVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatBanLeVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvXuatBanLeVm.DtoClauseDetail>();
                //Nv XuatKhac
                cfg.CreateMap<NvVatTuChungTu, NvXuatKhacVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatKhacVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvXuatKhacVm.DtoClauseDetail>();
                //TonDauKy
                cfg.CreateMap<NvVatTuChungTu, NvTonDauKyVm.Dto>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvTonDauKyVm.DtoDetail>();
                cfg.CreateMap<DclGeneralLedger, NvTonDauKyVm.DtoClauseDetail>();
                // quay giao dich
                cfg.CreateMap<NvGiaoDichQuay, NvGiaoDichQuayVm.Dto>();
                cfg.CreateMap<NvGiaoDichQuayChiTiet, NvGiaoDichQuayVm.DtoDetail>();
                //kiem ke
                cfg.CreateMap<NvKiemKe, NvKiemKeVm.Dto>();
                cfg.CreateMap<NvKiemKeVm.Dto, NvKiemKe>();
                cfg.CreateMap<NvKiemKeChiTiet, NvKiemKeVm.DtoDetails>();
                cfg.CreateMap<NvKiemKeVm.DtoDetails, NvKiemKeChiTiet>();
                //HuyNQ: Khuyến mại đồng giá 
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiDongGiaVm.Dto>();
                cfg.CreateMap<NvKhuyenMaiDongGiaVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiDongGiaVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiDongGiaVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                //HuyNQ: Khuyến mại Voucher
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiVoucherVm.Dto>();
                cfg.CreateMap<NvKhuyenMaiVoucherVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiVoucherVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiVoucherVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                //HuyNQ: Khuyến mại TinhTien
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiTinhTienVm.Dto>();
                cfg.CreateMap<NvKhuyenMaiTinhTienVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiTinhTienVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiTinhTienVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                //HuyNQ: Khuyến mại Buy1Get1
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiBuy1Get1Vm.Dto>();
                cfg.CreateMap<NvKhuyenMaiBuy1Get1Vm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiBuy1Get1Vm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiBuy1Get1Vm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiHangKM, NvKhuyenMaiBuy1Get1Vm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiBuy1Get1Vm.DtoDetail, NvChuongTrinhKhuyenMaiHangKM>();
                //HuyNQ: Khuyến mại Combo
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiComboVm.Dto>();
                cfg.CreateMap<NvKhuyenMaiComboVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiComboVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiComboVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiHangKM, NvKhuyenMaiComboVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiComboVm.DtoDetail, NvChuongTrinhKhuyenMaiHangKM>();
                //HuyNQ: Khuyến mại TichDiem
                cfg.CreateMap<NvChuongTrinhKhuyenMai, NvKhuyenMaiTichDiemVm.Dto>();
                cfg.CreateMap<NvKhuyenMaiTichDiemVm.Dto, NvChuongTrinhKhuyenMai>();
                cfg.CreateMap<NvChuongTrinhKhuyenMaiChiTiet, NvKhuyenMaiTichDiemVm.DtoDetail>();
                cfg.CreateMap<NvKhuyenMaiTichDiemVm.DtoDetail, NvChuongTrinhKhuyenMaiChiTiet>();
                //AnhPT: cfg Giao Dịch Quầy
                cfg.CreateMap<NvGiaoDichQuay, NvGiaoDichQuayVm.DataDto>();
                cfg.CreateMap<NvGiaoDichQuayVm.DataDto, NvGiaoDichQuay>();
                //.ForMember(dest => dest.KhachCanTra, opt => opt.MapFrom(src => src.ThoiGian))
                cfg.CreateMap<NvGiaoDichQuayChiTiet, NvGiaoDichQuayVm.DataDetails>();
                cfg.CreateMap<NvGiaoDichQuayVm.DataDetails, NvGiaoDichQuayChiTiet>();
                //HuyNQ: Công nợ
                cfg.CreateMap<NvCongNo, NvCongNoVm.Dto>();
                cfg.CreateMap<NvCongNoVm.Dto, NvCongNo>();

                cfg.CreateMap<NvNgayHetHanHangHoa, NvNgayHetHanHangHoaVm.Dto>();
                cfg.CreateMap<NvNgayHetHanHangHoaVm.Dto, NvNgayHetHanHangHoa>();

                cfg.CreateMap<NvNgayHetHanHangHoaChiTiet, NvNgayHetHanHangHoaVm.DtoDetail>();
                cfg.CreateMap<NvNgayHetHanHangHoaVm.DtoDetail, NvNgayHetHanHangHoaChiTiet>();

                //Danh mục
                cfg.CreateMap<MdBoHang, MdBoHangVm.Dto>();
                cfg.CreateMap<MdBoHangChiTiet,MdBoHangVm.DtoDetail>();
                cfg.CreateMap<MdCustomer, MdCustomerVm.CustomerDto>();
                cfg.CreateMap<MdCustomerVm.CustomerDto, MdCustomer>();
                cfg.CreateMap<MdCustomer, MdCustomerVm.Dto>();
                cfg.CreateMap<MdCustomerVm.Dto, MdCustomer>();
                cfg.CreateMap<MdMerchandise, MdMerchandiseVm.MasterDto>();
                cfg.CreateMap<MdMerchandisePrice, MdMerchandiseVm.DtoDetail>();
                cfg.CreateMap<MdMerchandiseVm.MasterDto, MdMerchandise>();
                cfg.CreateMap<MdMerchandiseVm.DtoDetail, MdMerchandisePrice>();
                cfg.CreateMap<MdMerchandiseType, MdMerchandiseTypeVm.Dto>();
                cfg.CreateMap<MdMerchandiseTypeVm.Dto, MdMerchandiseType>();
                cfg.CreateMap<MdNhomVatTu, MdNhomVatTuVm.Dto>();
                cfg.CreateMap<MdNhomVatTuVm.Dto, MdNhomVatTu>();
                cfg.CreateMap<MdColor, MdColorVm.Dto>();
                cfg.CreateMap<MdColorVm.Dto, MdColor>();
                cfg.CreateMap<MdSize, MdSizeVm.Dto>();
                cfg.CreateMap<MdSizeVm.Dto, MdSize>();
                cfg.CreateMap<MdXuatXu, MdXuatXuVm.Dto>();
                cfg.CreateMap<MdXuatXuVm.Dto, MdXuatXu>();

                cfg.CreateMap<MdPeriod, ChoiceObj>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                       .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                       .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.Period, src.Name)))
                       .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
                       .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                       .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                cfg.CreateMap<MdSupplier, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaNCC))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaNCC, src.TenNCC)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenNCC))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                cfg.CreateMap<MdSellingMachine, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Code))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.Code, src.Name)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Name))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // dm nha kho
                cfg.CreateMap<MdWareHouse, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaKho))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaKho, src.TenKho)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenKho))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // danh muc quoc gia
                cfg.CreateMap<MdCountry, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Code))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.Code, src.Description)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc ngoai te
                cfg.CreateMap<MdCurrency, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaNgoaiTe))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaNgoaiTe, src.TenNgoaiTe)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenNgoaiTe))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc khach hang
                cfg.CreateMap<MdCustomer, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaKH))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaKH, src.TenKH)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenKH))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc city
                cfg.CreateMap<MdCity, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.CityId))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("{1}", src.CityName)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CityName))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc districts
                cfg.CreateMap<MdDistricts, ChoiceObj>()
                         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                         .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.DistrictsId))
                         .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("{1}", src.DistrictsName)))
                         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DistrictsName))
                         .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.CityId))
                         .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                         .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));

                //danh muc hang hoa
                cfg.CreateMap<MdMerchandise, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaVatTu))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaVatTu, src.TenHang)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenHang))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // danh muc loai hang hoa
                cfg.CreateMap<MdMerchandiseType, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaLoaiVatTu))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaLoaiVatTu, src.TenLoaiVatTu)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenLoaiVatTu))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc nhom vat tu
                cfg.CreateMap<MdNhomVatTu, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaNhom))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaNhom, src.TenNhom)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenNhom))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // danh muc baobi
                cfg.CreateMap<MdPackaging, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaBaoBi))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaBaoBi, src.TenBaoBi)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenBaoBi))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc ke hang
                cfg.CreateMap<MdShelves, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaKeHang))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaKeHang, src.TenKeHang)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenKeHang))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc thue
                cfg.CreateMap<MdTax, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaLoaiThue))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaLoaiThue, src.LoaiThue)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.LoaiThue))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //danh muc loai ly do
                cfg.CreateMap<MdTypeReason, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaLyDo))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaLyDo, src.TenLyDo)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenLyDo))
                        .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Loai))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // dm xuat xu
                cfg.CreateMap<MdXuatXu, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaXuatXu))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaXuatXu, src.TenXuatXu)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenXuatXu))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                // dm bo hang
                cfg.CreateMap<MdBoHang, ChoiceObj>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MaBoHang))
                        .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]", src.TenBoHang)))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenBoHang))
                        .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                        .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //Authorize
                //Au Người dùng
                cfg.CreateMap<AU_NGUOIDUNG, AuNguoiDungVm.Dto>();
                cfg.CreateMap<AuNguoiDungVm.Dto, AU_NGUOIDUNG>();
                cfg.CreateMap<AU_NGUOIDUNG, ChoiceObj>()
                       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                       .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Username))
                       .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.Username, src.TenNhanVien)))
                       .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenNhanVien))
                       .ForMember(dest => dest.Selected, opt => opt.UseValue(false))
                       .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));
                //Au Đơn vị
                cfg.CreateMap<AU_DONVI, AuDonViVm.Dto>();
                cfg.CreateMap<AuDonViVm.Dto, AU_DONVI>();
                //Au Tham số hệ thống
                cfg.CreateMap<AU_THAMSOHETHONG, AuThamSoHeThongVm.Dto>();
                cfg.CreateMap<AuThamSoHeThongVm.Dto, AU_THAMSOHETHONG>();
                //DCL
                cfg.CreateMap<NvNhapHangMuaVm.DtoClauseDetail, DclGeneralLedger>();
                cfg.CreateMap<NvXuatBanVm.DtoClauseDetail, DclGeneralLedger>();

                cfg.CreateMap<DclGeneralLedger, NvNhapHangBanTraLaiVm.DtoClauseDetail>();
                cfg.CreateMap<NvNhapHangBanTraLaiVm.DtoClauseDetail, DclGeneralLedger>();
                //thêm mới
                cfg.CreateMap<NvVatTuChungTu, NvXuatKhacVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatKhacVm.ReportDetailModel>();
                cfg.CreateMap<NvXuatKhacVm.DtoClauseDetail, DclGeneralLedger>();
                cfg.CreateMap<NvXuatKhacVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvXuatKhacVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvXuatKhacVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvXuatKhacVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTu, NvXuatBanVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatBanVm.ReportDetailModel>();
                cfg.CreateMap<NvXuatBanVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvXuatBanVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTu, NvXuatBanLeVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvXuatBanLeVm.ReportDetailModel>();
                cfg.CreateMap<NvXuatBanLeVm.DtoClauseDetail, DclGeneralLedger>();
                cfg.CreateMap<NvXuatBanLeVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvXuatBanLeVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTu, NvTonDauKyVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvTonDauKyVm.ReportDetailModel>();
                cfg.CreateMap<NvTonDauKyVm.DtoClauseDetail, DclGeneralLedger>();
                cfg.CreateMap<NvTonDauKyVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvTonDauKyVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvPhieuDieuChuyenNoiBoVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvPhieuDieuChuyenNoiBoVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvPhieuDieuChuyenNoiBoVm.ReportDetailModel>();
                cfg.CreateMap<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.ReportModel>();
                cfg.CreateMap<NvDatHang, NvPhieuDatHangVm.ReportModel>();
                cfg.CreateMap<NvDatHangChiTiet, NvPhieuDatHangVm.ReportDetailModel>();
                cfg.CreateMap<NvDatHang, NvPhieuDatHangNCCVm.ReportModel>();
                cfg.CreateMap<NvDatHangChiTiet, NvPhieuDatHangNCCVm.ReportDetailModel>();
                cfg.CreateMap<NvNhapKhacVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvNhapKhacVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvNhapKhacVm.DtoClauseDetail, DclGeneralLedger>();
                cfg.CreateMap<NvVatTuChungTu, NvNhapKhacVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapKhacVm.ReportDetailModel>();
                cfg.CreateMap<NvNhapHangMuaVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvNhapHangMuaVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTu, NvNhapHangMuaVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapHangMuaVm.ReportDetailModel>();
                cfg.CreateMap<AU_NGUOIDUNG, AuNguoiDungVm.CurrentUser>();
                cfg.CreateMap<MdBoHangVm.Dto, MdBoHang>();
                cfg.CreateMap<MdBoHangVm.DtoDetail, MdBoHangChiTiet>();
                cfg.CreateMap<MdContractVm.Dto, MdContract>();
                cfg.CreateMap<MdContractVm.Detail, MdDetailContract>();
                cfg.CreateMap<MdContract, MdContractVm.ReportModel>().ForMember(dest => dest.MaKhachHang, opt => opt.MapFrom(src => src.KhachHang));
                cfg.CreateMap<MdDetailContract, MdContractVm.ReportDetailModel>();

                cfg.CreateMap<TDS_Dmgiaban, MdMerchandisePrice>()
                .ForMember(dest => dest.GiaMua, opt => opt.MapFrom(u => u.Giamuachuavat))
                .ForMember(dest => dest.GiaMuaVat, opt => opt.MapFrom(u => u.Giamuacovat))
                .ForMember(dest => dest.GiaBanBuon, opt => opt.MapFrom(u => u.Giabanbuonchuavat))
                .ForMember(dest => dest.GiaBanBuonVat, opt => opt.MapFrom(u => u.Giabanbuoncovat))
                .ForMember(dest => dest.GiaBanLe, opt => opt.MapFrom(u => u.Giabanlechuavat))
                .ForMember(dest => dest.GiaBanLeVat, opt => opt.MapFrom(u => u.Giabanlecovat))
                .ForMember(dest => dest.TyLeLaiLe, opt => opt.MapFrom(u => u.Tylelaile))
                .ForMember(dest => dest.TyLeLaiBuon, opt => opt.MapFrom(u => u.Tylelaibuon));

                cfg.CreateMap<TDS_Dmkhachhang, MdSupplier>()
                .ForMember(dest => dest.MaNCC, opt => opt.MapFrom(u => u.Makhachhang))
                .ForMember(dest => dest.TenNCC, opt => opt.MapFrom(u => u.Tenkhachhang))
                .ForMember(dest => dest.DiaChi, opt => opt.MapFrom(u => u.Diachi))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(u => u.Email))
                .ForMember(dest => dest.DienThoai, opt => opt.MapFrom(u => u.Dienthoai))
                .ForMember(dest => dest.TrangThai, opt => opt.UseValue(10));

                cfg.CreateMap<MdWareHouseVm.Dto, MdWareHouse>();
                cfg.CreateMap<NvNgayHetHanHangHoa, NvNgayHetHanHangHoaVm.ReportModel>();
                cfg.CreateMap<NvNgayHetHanHangHoaChiTiet, NvNgayHetHanHangHoaVm.ReportDetailModel>();
                cfg.CreateMap<NvNhapHangBanTraLaiVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvNhapHangBanTraLaiVm.Dto, NvVatTuChungTu>();
                cfg.CreateMap<NvNhapHangBanTraLaiVm.DtoDetail, NvVatTuChungTuChiTiet>();
                cfg.CreateMap<NvVatTuChungTu, NvNhapHangBanTraLaiVm.ReportModel>();
                cfg.CreateMap<NvVatTuChungTuChiTiet, NvNhapHangBanTraLaiVm.ReportDetailModel>();
                cfg.CreateMap<NvXuatKhacVm.DtoClauseDetail, DclGeneralLedger>();

                cfg.CreateMap<AU_DONVI, ChoiceObj>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaDonVi, src.TenDonVi)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TenDonVi))
                .ForMember(dest => dest.Selected, opt => opt.UseValue(true))
                .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));

                cfg.CreateMap<AU_THAMSOHETHONG, ChoiceObj>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => string.Format("[{0}]-{1}", src.MaThamSo, src.TenThamSo)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.MaThamSo))
                .ForMember(dest => dest.Selected, opt => opt.UseValue(true))
                .ForMember(dest => dest.OldSelected, opt => opt.UseValue(true));

                cfg.CreateMap<TDS_MatHangVm.Dto, TDS_Dmmathang>()
                .ForMember(dest => dest.Ngaytao, opt => opt.UseValue(DateTime.Now.Date))
                .ForMember(dest => dest.Ngayphatsinh, opt => opt.UseValue(DateTime.Now.Date))
                .ForMember(dest => dest.Isshowweb, opt => opt.UseValue(true))
                .ForMember(dest => dest.Trangthai, opt => opt.UseValue(10))
                .ForMember(dest => dest.Trangthaikd, opt => opt.UseValue(1));

                cfg.CreateMap<TDS_MatHangVm.Dto, TDS_Dmgiaban>();
                cfg.CreateMap<MdMerchandise, NvPhieuDatHangVm.DtoDetail>().ForMember(dest => dest.MaHang, opt => opt.MapFrom(x => x.MaVatTu));
                cfg.CreateMap<TDS_Dmgiaban, TDS_MatHangVm.Details>();
                cfg.CreateMap<TDS_Dmmathang, TDS_MatHangVm.Dto>();
                cfg.CreateMap<TDS_Dmkhachhang, TDS_KhachHangVM.Dto>();
                cfg.CreateMap<TDS_Dmvat, TDS_VatVm>();
                cfg.CreateMap<TDS_KhachHangVM.Dto, TDS_Dmkhachhang>();
                cfg.CreateMap<TDS_Dmgiaban,MdMerchandisePrice>();

                cfg.CreateMap<TDS_Dmmathang, MdMerchandise>()
                    .ForMember(dest => dest.MaVatTu, opt => opt.MapFrom(u => u.Masieuthi))
                    .ForMember(dest => dest.MaLoaiVatTu, opt => opt.MapFrom(u => u.Manganh))
                    .ForMember(dest => dest.MaNhomVatTu, opt => opt.MapFrom(u => u.Manhomhang))
                    .ForMember(dest => dest.TenHang, opt => opt.MapFrom(u => u.Tendaydu))
                    .ForMember(dest => dest.TenVietTat, opt => opt.MapFrom(u => u.Tenviettat))
                    .ForMember(dest => dest.Barcode, opt => opt.MapFrom(u => u.Barcode))
                    .ForMember(dest => dest.MaKhachHang, opt => opt.MapFrom(u => u.Makhachhang))
                    .ForMember(dest => dest.MaVatRa, opt => opt.MapFrom(u => u.Mavatban))
                    .ForMember(dest => dest.MaVatVao, opt => opt.MapFrom(u => u.Mavatmua))
                    .ForMember(dest => dest.ItemCode, opt => opt.MapFrom(u => u.Itemcode))
                    .ForMember(dest => dest.DonViTinh, opt => opt.MapFrom(u => u.Madvtinh))
                    .ForMember(dest => dest.TrangThai, opt => opt.UseValue(10))
                    .ForMember(dest => dest.IState, opt => opt.UseValue(50));
                });
        }
        public static IMappingExpression<TSource, TDestination> IgnoreDataInfoSelfMapping<TSource, TDestination>(
           this IMappingExpression<TSource, TDestination> map)
           where TSource : DataInfoEntity
           where TDestination : DataInfoEntity
        {
            map.ForMember(dest => dest.ICreateDate, config => config.Ignore());
            map.ForMember(dest => dest.ICreateBy, config => config.Ignore());
            map.ForMember(dest => dest.UnitCode, config => config.Ignore());
            map.AfterMap((src, dest) =>
            {
                if (string.IsNullOrEmpty(dest.ICreateBy) && dest.ICreateDate == null)
                {
                    if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                        dest.ICreateBy = HttpContext.Current.User.Identity.Name;
                    dest.ICreateDate = DateTime.Now;
                }
            });
            return map;
        }

        public static IMappingExpression<TSource, TDestination> IgnoreDataInfo<TSource, TDestination>(
          this IMappingExpression<TSource, TDestination> map)
            where TSource : DataDto
            where TDestination : DataInfoEntity
        {
            map.ForMember(dest => dest.ICreateDate, config => config.Ignore());
            map.ForMember(dest => dest.ICreateBy, config => config.Ignore());
            map.ForMember(dest => dest.IUpdateDate,
                config => config.ResolveUsing<UpdateDateResolver>().FromMember(x => x.IUpdateDate));
            map.ForMember(dest => dest.IUpdateBy,
                config => config.ResolveUsing<UpdateByResolver>().FromMember(x => x.IUpdateBy));
            map.ForMember(dest => dest.IState, config => config.Ignore());
            map.AfterMap((src, dest) =>
            {
                if (string.IsNullOrEmpty(dest.ICreateBy) && dest.ICreateDate == null)
                {
                    if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                        dest.ICreateBy = HttpContext.Current.User.Identity.Name;
                    dest.ICreateDate = DateTime.Now;
                }
            });
            return map;
        }

        public class IdResolver : ValueResolver<string, string>
        {
            protected override string ResolveCore(string source)
            {
                return Guid.NewGuid().ToString();
            }
        }

        public class UpdateDateResolver : ValueResolver<DateTime?, DateTime?>
        {
            protected override DateTime? ResolveCore(DateTime? source)
            {
                return DateTime.Now;
            }
        }

        public class UpdateByResolver : ValueResolver<string, string>
        {
            protected override string ResolveCore(string source)
            {
                var result = source;
                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    result = HttpContext.Current.User.Identity.Name;
                return result;
            }
        }

    }
}
