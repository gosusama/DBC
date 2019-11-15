using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_VATTU")]
    public class MdMerchandise : DataInfoEntity
    {
        [Column("MAVATTU")]
        [StringLength(50)]
        [Required]
        public string MaVatTu { get; set; }

        [Column("TENVATTU")]
        [StringLength(200)]
        public string TenHang { get; set; }
        [Column("TENVIETTAT")]
        [StringLength(200)]
        public string TenVietTat { get; set; }

        [Column("DONVITINH")]
        [StringLength(50)]
        public string DonViTinh { get; set; }
        [Column("MAKEHANG")]
        [StringLength(50)]
        [Description("Mã kệ hàng")]
        public string MaKeHang { get; set; }

        [Column("MALOAIVATTU")]
        [StringLength(50)]
        [Required]
        public string MaLoaiVatTu { get; set; }

        [Column("MANHOMVATTU")]
        [StringLength(50)]
        public string MaNhomVatTu { get; set; }
        [Column("MABAOBI")]
        [StringLength(50)]
        [Description("Mã bao bì")]
        public string MaBaoBi { get; set; }
        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string MaKhachHang { get; set; }

        [Column("MANCC")]
        [StringLength(50)]
        public string MaNCC { get; set; }

        [Column("CHIETKHAUNCC")]
        public decimal ChietKhauNCC { get; set; }

        [Column("GIAMUA")]
        public decimal GiaMua { get; set; }
        [Column("GIABANLE")]
        public decimal GiaBanLe { get; set; }
        [Column("GIABANBUON")]
        public decimal GiaBanBuon { get; set; }
        [Column("TYLELAIBUON")]
        public decimal TyLeLaiBuon { get; set; }
        [Column("TYLELAILE")]
        public decimal TyLeLaiLe { get; set; }
        [Column("SOTONMAX")]
        public decimal SoTonMax { get; set; }
        [Column("SOTONMIN")]
        public decimal SoTonMin { get; set; }
        [Column("BARCODE")]
        [StringLength(2000)]
        public string Barcode { get; set; }
        [Column("MAKHAC")]
        [StringLength(50)]
        public string MaKhac { get; set; }

        [Column("MAVATVAO")]
        [StringLength(50)]
        public string MaVatVao { get; set; }
        [Column("TYLE_VAT_VAO")]
        public decimal TyLeVatVao { get; set; }
        [Column("MAVATRA")]
        [StringLength(50)]
        public string MaVatRa { get; set; }
        [Column("TYLE_VAT_RA")]
        public decimal TyLeVatRa { get; set; }
        [Column("GIABANVAT")]
        public decimal GiaBanVat { get; set; }

        [Column("GIAVON")]
        public decimal? GiaVon { get; set; }

        [Column("PTTINHGIA")]
        [StringLength(200)]
        [Description("Phương thức tính giá trị hàng tồn kho")]
        public string PtTinhGia { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("TKDOANHTHU")]
        [StringLength(50)]
        [Description("Tài khoản doanh thu")]
        public string TkDoanhThu { get; set; }

        [Column("TKGIAVON")]
        [StringLength(50)]
        [Description("Tài khoản giá vốn")]
        public string TkGiaVon { get; set; }

        [Column("TKHANGHOA")]
        [StringLength(50)]
        [Description("Tài khoản hàng hóa")]
        public string TkHangHoa { get; set; }

        [Column("TKPHAITRA")]
        [StringLength(50)]
        [Description("Tài khoản phải trả người bán")]
        public string TkPhaiTra { get; set; }
        [Column("ITEMCODE")]
        [StringLength(50)]
        public string ItemCode { get; set; }

        [Column("KEKIEMKE")]
        [StringLength(100)]
        [Description("Kệ kiểm kê")]
        public string KeKiemKe { get; set; }

        [Column("MASIZE")]
        [StringLength(50)]
        [Description("MASIZE")]
        public string MaSize { get; set; }

        [Column("MACOLOR")]
        [StringLength(50)]
        [Description("MACOLOR")]
        public string MaColor { get; set; }

        [Column("PATH_IMAGE")]
        [StringLength(2000)]
        [Description("PATH_IMAGE")]
        public string PathImage { get; set; }

        [Column("AVATAR")]
        [Description("AVATAR")]
        public byte[] Avatar { get; set; }

        [Column("IMAGE")]
        [StringLength(2000)]
        [Description("IMAGE")]
        public string Image { get; set; }

        [Column("MACHA")]
        [StringLength(50)]
        public string MaCha { get; set; }

        [Column("TITLE")]
        [StringLength(2000)]
        public string Title { get; set; }

        [Column("TRANGTHAICON")]
        public Nullable<decimal> TrangThaiCon { get; set; }
    }
}
