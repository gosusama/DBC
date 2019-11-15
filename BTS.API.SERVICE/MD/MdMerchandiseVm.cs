using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Implimentations;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BTS.API.SERVICE.MD
{
    public class MdMerchandiseVm
    {
        public class Search : IDataSearch
        {
            public string MaVatTu { get; set; }
            public string Barcode { get; set; }
            public string TenVatTu { get; set; }
            public string TenVietTat { get; set; }

            public string MaKhachHang { get; set; }
            public string MaNhaCungCap { get; set; }
            public string DonViTinh { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public decimal GiaMua { get; set; }
            public decimal ChietKhauNCC { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanBuon { get; set; }

            public string PtTinhGia { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdMerchandise().TenHang);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdMerchandise();

                if (!string.IsNullOrEmpty(this.MaLoaiVatTu))
                {

                    var codeTypes = this.MaLoaiVatTu.Split(',').ToList();
                    IQueryFilter queryFilterByGroup = new QueryFilterLinQ();
                    if (this.MaNhomVatTu != null)
                    {
                        var codeGroups = this.MaNhomVatTu.Split(',').ToList();
                        queryFilterByGroup.Method = FilterMethod.In;
                        queryFilterByGroup.Value = codeGroups;
                        queryFilterByGroup.Property = ClassHelper.GetProperty(() => refObj.MaNhomVatTu);
                    }

                    result.Add(new QueryFilterLinQ
                    {
                        Method = FilterMethod.And,
                        SubFilters = new List<IQueryFilter>()
                        {
                            new QueryFilterLinQ()
                            {
                                Property = ClassHelper.GetProperty(() => refObj.MaLoaiVatTu),
                                Value = codeTypes,
                                Method = FilterMethod.In
                            },
                            queryFilterByGroup,
                            new QueryFilterLinQ()
                            {
                                Method = FilterMethod.Or,
                                SubFilters = new List<IQueryFilter>()
                                {
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.TenHang),
                                        Value = this.TenVatTu,
                                        Method = FilterMethod.Like
                                    },
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.MaVatTu),
                                        Value = this.MaVatTu,
                                        Method = FilterMethod.Like
                                    }
                                }
                            }
                        }
                    });
                }
                else
                {
                    result.Add(new QueryFilterLinQ()
                    {
                        Method = FilterMethod.Or,
                        SubFilters = new List<IQueryFilter>()
                                {
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.TenHang),
                                        Value = this.TenVatTu,
                                        Method = FilterMethod.Like
                                    },
                                    new QueryFilterLinQ
                                    {
                                        Property = ClassHelper.GetProperty(() => refObj.MaVatTu),
                                        Value = this.MaVatTu,
                                        Method = FilterMethod.Like
                                    }
                                }
                    });
                }

                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                MaVatTu = summary;
                TenVatTu = summary;
            }


        }
        public class SearchProcedure : IDataSearch
        {
            public string TieuChiTimKiem { get; set; }
            public string MaHang { get; set; }
            public string MaVatTu { get; set; }
            public string Barcode { get; set; }
            public string TenHang { get; set; }
            public decimal? GiaBanLeVat { get; set; }
            public decimal? GiaMuaVat { get; set; }
            public decimal? TyLeLaiLe { get; set; }
            public string TenVietTat { get; set; }
            public string MaKhachHang { get; set; }
            public string ItemCode { get; set; }
            public string MaKho { get; set; }
            public bool WithGiaVon { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdMerchandiseVm.Dto().TenHang);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdMerchandiseVm.Dto();
                if (!string.IsNullOrEmpty(this.MaHang))
                {
                    if (this.MaHang.Length == 7)
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAHANG",
                            Value = this.MaHang,
                            Method = FilterMethod.EqualTo,
                            ValueAsOtherField = true
                        });
                    }
                    else
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAHANG",
                            Value = this.MaHang,
                            Method = FilterMethod.Like,
                            ValueAsOtherField = true
                        });
                    }
                }
                if (!string.IsNullOrEmpty(this.MaVatTu))
                {
                    if (this.MaVatTu.Length == 7)
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAVATTU",
                            Value = this.MaVatTu,
                            Method = FilterMethod.EqualTo,
                            ValueAsOtherField = true
                        });
                    }
                    else
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAVATTU",
                            Value = this.MaVatTu,
                            Method = FilterMethod.Like,
                            ValueAsOtherField = true
                        });
                    }
                }
                if (!string.IsNullOrEmpty(this.ItemCode))
                {
                    if (this.MaHang.Length == 5)
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "ITEMCODE",
                            Value = this.ItemCode,
                            Method = FilterMethod.EqualTo,
                            ValueAsOtherField = true
                        });
                    }
                    else
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "ITEMCODE",
                            Value = this.ItemCode,
                            Method = FilterMethod.Like,
                            ValueAsOtherField = true
                        });
                    }
                }
                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    if (this.MaKhachHang.Length == 4)
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAKHACHHANG",
                            Value = this.MaKhachHang,
                            Method = FilterMethod.EqualTo,
                            ValueAsOtherField = true
                        });
                    }
                    else
                    {
                        result.Add(new QueryFilterSQL
                        {
                            Field = "MAKHACHHANG",
                            Value = this.MaKhachHang,
                            Method = FilterMethod.Like,
                            ValueAsOtherField = true
                        });
                    }

                }
                if (!string.IsNullOrEmpty(this.TenHang))
                {
                    result.Add(new QueryFilterSQL
                    {
                        Field = "TENHANG",
                        Value = this.TenHang,
                        Method = FilterMethod.Like,
                        ValueAsOtherField = true
                    });
                }

                if (!string.IsNullOrEmpty(this.Barcode))
                {
                    result.Add(new QueryFilterSQL
                    {
                        Field = "BARCODE",
                        Value = this.Barcode,
                        Method = FilterMethod.Like,
                        ValueAsOtherField = true
                    });
                }
                if (this.GiaBanLeVat.HasValue)
                {
                    result.Add(new QueryFilterSQL
                    {
                        Field = "GIABANLEVAT",
                        Value = this.GiaBanLeVat,
                        Method = FilterMethod.EqualTo,
                        ValueAsOtherField = true
                    });
                }
                if (this.GiaMuaVat.HasValue)
                {
                    result.Add(new QueryFilterSQL
                    {
                        Field = "GIAMUAVAT",
                        Value = this.GiaMuaVat,
                        Method = FilterMethod.EqualTo,
                        ValueAsOtherField = true
                    });
                }
                if (this.TyLeLaiLe.HasValue)
                {
                    result.Add(new QueryFilterSQL
                    {
                        Field = "TYLELAILE",
                        Value = this.TyLeLaiLe,
                        Method = FilterMethod.EqualTo,
                        ValueAsOtherField = true
                    });
                }
                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
                MaHang = summary;
                TenHang = summary;
                Barcode = summary;
            }


        }
        public class Dto : DataInfoDtoVm
        {
            public Dto()
            {

            }
            public string Id { get; set; }
            public string MaHang { get; set; }
            public string MaVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaKeHang { get; set; }
            public string MaKhac { get; set; }
            public string TenHang { get; set; }
            public string TenVatTu { get; set; }
            public string TenVietTat { get; set; }
            public string MaKhachHang { get; set; }
            public string DonViTinh { get; set; }
            public string MaSize { get; set; }
            public string MaColor { get; set; }
            public string Path_Image { get; set; }
            public byte[] Avatar { get; set; }
            public decimal TrangThaiCon { get; set; }
            public string Image { get; set; }
            public string Barcode { get; set; }
            public string MaBaoBi { get; set; }
            public string TenBaoBi { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public string MaDonVi { get; set; }
            public decimal GiaMua { get; set; }
            public decimal GiaMuaVat { get; set; }
            public decimal GiaVon { get; set; }
            public decimal GiaVonVat { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanBuon { get; set; }
            public decimal TyLeLaiBuon { get; set; }
            public decimal TyLeLaiLe { get; set; }
            public string MaVatVao { get; set; }
            public decimal TyLeVatVao { get; set; }
            public string MaVatRa { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaBanBuonVat { get; set; }
            public decimal? SoLuongLe { get; set; }
            public decimal SoTonMax { get; set; }
            public decimal SoLuongNhapTrongKy { get; set; }
            public decimal SoLuongXuatTrongKy { get; set; }
            public decimal SoTonMin { get; set; }
            public decimal LuongBao { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal TyLeCKLe { get; set; }
            public decimal TyLeCKBuon { get; set; }
            public decimal TongBanLe { get; set; }
            public decimal TongBanBuon { get; set; }
            public string ItemCode { get; set; }
            public int TrangThai { get; set; }
            public decimal SoLuongTon { get; set; }
            public DateTime? ICreateDate { get; set; }
            public string KeKiemKe { get; set; }
            public string MaNCC { get; set; }
        }

        public class MessageImportExcel
        {
            public string Message1 { get; set; }
            public string Message2 { get; set; }
            public string Message3 { get; set; }
            public string Message4 { get; set; }
            public string Message5 { get; set; }
        }

        //list này phục vụ khuyến mại combo, hàng tặng hàng
        public class ListDataDetails
        {
            public string MaVatTuCon { get; set; }
            public string TenVatTuCon { get; set; }
        }

        //Model bán lẻ hàng trên Web, Phạm TUấn Anh
        public class DtoAndPromotion : DataInfoDtoVm
        {
            public DtoAndPromotion()
            {
                ListMaHangKhuyenMai = new List<ListDataDetails>();
            }
            public List<ListDataDetails> ListMaHangKhuyenMai { get; set; }
            public bool IsTichDiem { get; set; }
            public string Id { get; set; }
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
            public string Barcode { get; set; }
            public string DonViTinh { get; set; }
            public string MaKeHang { get; set; }
            public string MaKhachHang { get; set; }
            public string TenNhaCungCap { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaDonVi { get; set; }
            public decimal? GiaBanLe { get; set; }
            public decimal? GiaBanBuon { get; set; }
            public decimal? GiaBanBuonVat { get; set; }
            public string MaVatVao { get; set; }
            public string MaVatRa { get; set; }
            public string MaColor { get; set; }
            public string MaSize { get; set; }
            public int TrangThaiCon { get; set; }
            public string Image { get; set; }
            public string Path_Image { get; set; }
            public byte[] Avatar { get; set; }
            public decimal? TyLeVatVao { get; set; }
            public decimal? TyLeVatRa { get; set; }
            public decimal? GiaBanLeVat { get; set; }
            public decimal? GiaVon { get; set; }
            public string ItemCode { get; set; }
            public decimal? SoLuong { get; set; }
            public decimal? DonGia { get; set; }

            public decimal? TyLeKhuyenMai_ChietKhau { get; set; }
            public decimal? GiaTriKhuyenMai_ChietKhau { get; set; }

            public decimal? TyLeKhuyenMai_DongGia { get; set; }
            public decimal? GiaTriKhuyenMai_DongGia { get; set; }

            public string MaHang_Km_Buy1Get1 { get; set; }
            public string TenHang_Km_Buy1Get1 { get; set; }
            public int SoLuong_Km_Buy1Get1 { get; set; }

            public decimal? TyLeBatDau_TinhTien { get; set; }
            public decimal? TyLeKhuyenMai_TinhTien { get; set; }
            public decimal? GiaTriKhuyenMai_TinhTien { get; set; }

            public decimal? TyLeKhuyenMai_TichDiem { get; set; }
            public decimal? GiaTriKhuyenMai_TichDiem { get; set; }

            public decimal? TyLeKhuyenMai_Voucher { get; set; }
            public decimal? GiaTriKhuyenMai_Voucher { get; set; }

            public decimal? SoLuongKhuyenMai_Combo { get; set; }
            public decimal? GiaTriKhuyenMai_Combo { get; set; }

            public string TuGio { get; set; }
            public string DenGio { get; set; }
            public string LoaiKhuyenMai { get; set; }
            public string MaKhoKhuyenMai { get; set; }
            public decimal? SoLuong_KhuyenMai { get; set; }
            public string MaChuongTrinhKhuyenMai { get; set; }
            public string NoiDungKhuyenMai { get; set; }
            public decimal? TienHangKhuyenMai { get; set; }
            public int LogKhuyenMaiError { get; set; }
            public bool Status { get; set; }
            public string Message { get; set; }
            public bool IsBanAm { get; set; }
            public decimal? TonCuoiKySl { get; set; }

        }
        //Phục vụ lấy dữ liệu mã bó hàng- Bán lẻ web Phạm Tuấn Anh
        public class DataBoHang
        {
            public string MaBoHang { get; set; }
            public string TenBoHang { get; set; }
            public decimal ThanhTienBoHang { get; set; }
            public string UnitCode { get; set; }
            public int TrangThai { get; set; }
            public DataBoHang()
            {
                ListMaHang = new List<DtoAndPromotion>();
            }
            public List<DtoAndPromotion> ListMaHang { get; set; }
        }
        public class DataPrintItem
        {
            public int Stt { get; set; }
            public string Masieuthi { get; set; }
            public string Tenviettat { get; set; }
            public string Barcode { get; set; }
            public decimal Giabanbuoncovat { get; set; }
            public decimal Giabanlecovat { get; set; }
            public decimal Giabanbuon { get; set; }
            public decimal Giabanle { get; set; }
            public string Makhachhang { get; set; }
            public decimal Soluong { get; set; }
        }
        public class FilterData
        {
            public string MaVatTu { get; set; }
            public string TenVatTu { get; set; }
        }
        //end model
        public class MasterDto : DataInfoDtoVm
        {
            public MasterDto()
            {
                DataDetails = new List<DtoDetail>();
                FileNames = new List<string>();
            }
            public string Id { get; set; }
            public string MaVatTu { get; set; }
            public string TenHang { get; set; }
            public string TenVietTat { get; set; }
            public string DonViTinh { get; set; }
            public string MaKeHang { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaBaoBi { get; set; }
            public string MaKhachHang { get; set; }
            public string ItemCode { get; set; }
            public decimal ChietKhauNCC { get; set; }
            public string MaNCC { get; set; }
            public decimal GiaMua { get; set; }
            public decimal GiaMuaVat { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanBuon { get; set; }
            public decimal TyLeLaiBuon { get; set; }
            public decimal TyLeLaiLe { get; set; }
            public decimal SoTonMax { get; set; }
            public decimal SoTonMin { get; set; }
            public string Barcode { get; set; }
            public string MaKhac { get; set; }
            public string MaVat { get; set; }
            public decimal TyLeVat { get; set; }
            public decimal GiaBanVat { get; set; }
            public string MaVatVao { get; set; }
            public decimal TyLeVatVao { get; set; }
            public string MaVatRa { get; set; }
            public decimal TyLeVatRa { get; set; }
            public string PtTinhGia { get; set; }
            public string MaSize { get; set; }
            public string MaColor { get; set; }
            public string PathImage { get; set; }
            public byte[] Avatar { get; set; }
            public string AvatarName { get; set; }
            public string Image { get; set; }
            public string MaCha { get; set; }
            public string MaHang { get; set; }
            public string TenVatTu { get; set; }
            public string TenBaoBi { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public string MaDonVi { get; set; }
            public decimal GiaVon { get; set; }
            public decimal GiaVonVat { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaBanBuonVat { get; set; }
            public decimal? SoLuongLe { get; set; }
            public decimal SoLuongNhapTrongKy { get; set; }
            public decimal SoLuongXuatTrongKy { get; set; }
            public decimal LuongBao { get; set; }
            public decimal ThanhTien { get; set; }
            public decimal TyLeCKLe { get; set; }
            public decimal TyLeCKBuon { get; set; }
            public decimal TongBanLe { get; set; }
            public decimal TongBanBuon { get; set; }
            public decimal SoLuongTon { get; set; }
            public string KeKiemKe { get; set; }
            public string Title { get; set; }
            public int TrangThai { get; set; }
            public decimal TrangThaiCon { get; set; }
            public bool UseGenCode { get; set; }
            public List<string> FileNames { get; set; }
            public string UnitCode { get; set; }
            public List<DtoDetail> DataDetails { get; set; }
        }
        public class DtoDetail
        {

            public string MaVatTu { get; set; }
            public string MaDonVi { get; set; }
            public decimal GiaMuaVat { get; set; }
            public decimal GiaMua { get; set; }
            public decimal GiaBanLe { get; set; }
            public decimal GiaBanBuon { get; set; }
            public decimal TyLeLaiBuon { get; set; }
            public decimal TyLeLaiLe { get; set; }
            public string MaVatVao { get; set; }
            public decimal TyLeVatVao { get; set; }
            public string MaVatRa { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal GiaBanLeVat { get; set; }
            public decimal GiaBanBuonVat { get; set; }
            public decimal SoTonMax { get; set; }
            public decimal SoTonMin { get; set; }
        }
        public class FilterForDatHang : IDataSearch
        {
            public string MaKhachHang { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public string MaNhaCungCap { get; set; }
            public string MaKhoHang { get; set; }
            public DateTime? TuNgay { get; set; }
            public DateTime? DenNgay { get; set; }

            public DateTime? NgayChungTu { get; set; }
            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new MdMerchandise().TenHang);
                }
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new MdMerchandise();
                if (!string.IsNullOrEmpty(this.MaKhachHang))
                {
                    var codes = this.MaKhachHang.Split(',').ToList();
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaKhachHang),
                        Value = codes,
                        Method = FilterMethod.In
                    });
                }
                if (!string.IsNullOrEmpty(this.MaLoaiVatTu))
                {
                    var codes = this.MaLoaiVatTu.Split(',').ToList();
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaLoaiVatTu),
                        Value = codes,
                        Method = FilterMethod.In
                    });
                }
                if (!string.IsNullOrEmpty(this.MaNhomVatTu))
                {
                    var codes = this.MaNhomVatTu.Split(',').ToList();
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.MaNhomVatTu),
                        Value = codes,
                        Method = FilterMethod.In
                    });
                }

                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }

            public void LoadGeneralParam(string summary)
            {
            }


        }

        public class FilterQuantity
        {
            public string Operator { get; set; }
            public int Value { get; set; }
        }
        public class DataXNT
        {
            public string MaVatTu { get; set; }
            public string UnitCode { get; set; }
            public string Ky { get; set; }
            public string MaKho { get; set; }
            public DateTime Nam { get; set; }
            public decimal GiaVon { get; set; }
            public decimal TyLeVATRa { get; set; }
            public decimal TyLeVATVao { get; set; }
            public decimal TonCuoiKySL { get; set; }
            public decimal TonCuoiKyGT { get; set; }
        }


        public class MATHANG
        {
            public string Masieuthiphu { get; set; }
            public string Masieuthi { get; set; }
            public string Barcode { get; set; }
            public string Tenviettat { get; set; }
            public string Manganhhang { get; set; }
            public decimal Giabanlecovat { get; set; }
            public decimal Giavon { get; set; }
            public decimal Soluong { get; set; }
            public decimal SoluongKiemKe { get; set; }
            public string KeKiemKe { get; set; }
        }

        public class MATHANGTONAM
        {
            public string Id { get; set; }
            public string MaChungTu { get; set; }
            public string MaChungTuPk { get; set; }
            public string MaBaoBi { get; set; }
            public decimal LuongBao { get; set; }
            public decimal SoLuongLe { get; set; }
            public decimal SoLuongBao { get; set; }
            public decimal TyLeVatRa { get; set; }
            public decimal TyLeVatVao { get; set; }
            public decimal SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien { get; set; }
            public string MaHang { get; set; }
            public string TenHang { get; set; }
            public string Barcode { get; set; }
            public string MaKeHang { get; set; }
            public string TenNhaCungCap { get; set; }
            public string MaLoaiVatTu { get; set; }
            public string MaNhomVatTu { get; set; }
            public decimal GiaVon { get; set; }
        }
    }

    public class TDS_MatHangVm
    {
        public class SearchMatHang : IDataSearch
        {
            public string Masieuthi { get; set; }
            public string Barcode { get; set; }
            public string Tendaydu { get; set; }
            public string Tenviettat { get; set; }
            public string Mahangcuancc { get; set; }

            public string DefaultOrder
            {
                get
                {
                    return ClassHelper.GetPropertyName(() => new TDS_Dmmathang().Masieuthi);
                }
            }

            public void LoadGeneralParam(string summary)
            {
                Barcode = summary;
                Tendaydu = summary;
                Tenviettat = summary;
                Mahangcuancc = summary;
            }

            public List<IQueryFilter> GetFilters()
            {
                var result = new List<IQueryFilter>();
                var refObj = new TDS_Dmmathang();

                if (!string.IsNullOrEmpty(this.Masieuthi))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Masieuthi),
                        Value = this.Masieuthi,
                        Method = FilterMethod.Like,

                    });
                }
                if (!string.IsNullOrEmpty(this.Barcode))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Barcode),
                        Value = this.Barcode,
                        Method = FilterMethod.Like
                    });
                }

                if (!string.IsNullOrEmpty(this.Tendaydu))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Tendaydu),
                        Value = this.Tendaydu,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Mahangcuancc))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Mahangcuancc),
                        Value = this.Mahangcuancc,
                        Method = FilterMethod.Like
                    });
                }
                if (!string.IsNullOrEmpty(this.Tenviettat))
                {
                    result.Add(new QueryFilterLinQ
                    {
                        Property = ClassHelper.GetProperty(() => refObj.Tenviettat),
                        Value = this.Tenviettat,
                        Method = FilterMethod.Like
                    });
                }
                return result;
            }

            public List<IQueryFilter> GetQuickFilters()
            {
                return null;
            }
        }
        public class Dto
        {
            public string Masieuthi { get; set; }
            public string Manganh { get; set; }
            public string Manhomhang { get; set; }
            public string Makhachhang { get; set; }
            public string Makehang { get; set; }
            public string Madvtinh { get; set; }

            public string Tendaydu { get; set; }
            public string Tenviettat { get; set; }
            public string Mahangcuancc { get; set; }

            public string Mavatmua { get; set; }

            public string Mavatban { get; set; }

            public decimal? Trietkhauncc { get; set; }

            public bool? Isshowweb { get; set; }

            public int? Trangthai { get; set; }

            public int? Trangthaikd { get; set; }

            public DateTime Ngaytao { get; set; }

            public DateTime Ngayphatsinh { get; set; }

            public string Manguoitao { get; set; }

            public string Itemcode { get; set; }

            public int Quycach { get; set; }

            public string Barcode { get; set; }

            public bool Checked { get; set; }

            public string Madonvi { get; set; }

            public decimal Giabanlecovat { get; set; }

            public decimal Giabanbuoncovat { get; set; }

            public decimal Giabanlechuavat { get; set; }

            public decimal Giabanbuonchuavat { get; set; }

            public decimal Giamuacovat { get; set; }

            public decimal Giamuachuavat { get; set; }

            public decimal Tylelaibuon { get; set; }

            public decimal Tylelaile { get; set; }

            public string KeKiemKe { get; set; }
        }
        public class Details
        {

            public string Masieuthi { get; set; }

            public string Madonvi { get; set; }

            public decimal Giabanlecovat { get; set; }

            public decimal Giabanbuoncovat { get; set; }

            public decimal Giabanlechuavat { get; set; }

            public decimal Giabanbuonchuavat { get; set; }

            public decimal Giamuacovat { get; set; }

            public decimal Giamuachuavat { get; set; }

            public decimal Tylelaibuon { get; set; }

            public decimal Tylelaile { get; set; }
        }
        public class DataGet
        {

            public string Masieuthi { get; set; }
            public string Manganh { get; set; }
            public string Manhomhang { get; set; }
            public string Makhachhang { get; set; }
            public string Makehang { get; set; }
            public string Madvtinh { get; set; }

            public string Tendaydu { get; set; }
            public string Tenviettat { get; set; }
            public string Mahangcuancc { get; set; }

            public string Mavatmua { get; set; }

            public string Mavatban { get; set; }

            public decimal? Trietkhauncc { get; set; }

            public bool? Isshowweb { get; set; }

            public int? Trangthai { get; set; }

            public int? Trangthaikd { get; set; }

            public DateTime Ngaytao { get; set; }

            public DateTime Ngayphatsinh { get; set; }

            public string Manguoitao { get; set; }

            public string Itemcode { get; set; }

            public int Quycach { get; set; }

            public string Barcode { get; set; }

            public bool Checked { get; set; }

            public string Madonvi { get; set; }

            public decimal Giabanlecovat { get; set; }

            public decimal Giabanbuoncovat { get; set; }

            public decimal Giabanlechuavat { get; set; }

            public decimal Giabanbuonchuavat { get; set; }

            public decimal Giamuacovat { get; set; }

            public decimal Giamuachuavat { get; set; }

            public decimal Tylelaibuon { get; set; }

            public decimal Tylelaile { get; set; }
            public int? Soluong { get; set; }

            public decimal? Tylechietkhaule { get; set; }

            public decimal? Tylechietkhaubuon { get; set; }
            public decimal? Tongtienbanbuon { get; set; }
            public decimal? Tongtienbanle { get; set; }


        }

    }
}
