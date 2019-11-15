using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.NV
{
    [Table("NVHANGGDQUAY_ASYNCCLIENT")]
    public class NvGiaoDichQuayChiTiet : DetailInfoEntity
    {
        [Column("MAGDQUAYPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã giao dịch quầy pk")]
        public string MaGDQuayPK { get; set; }

        [Column("MAKHOHANG")]
        [StringLength(50)]
        [Required]
        [Description("Mã kho hàng")]
        public string MaKhoHang { get; set; }

        [Column("MADONVI")]
        [StringLength(50)]
        [Description("Mã đơn vị")]
        public string MaDonVi { get; set; }

        [Column("MAVATTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã vật tư")]
        public string MaVatTu { get; set; }

        [Column("BARCODE")]
        [StringLength(2000)]
        [Description("Barcode")]
        public string Barcode { get; set; }

        [Column("TENDAYDU")]
        [StringLength(300)]
        [Description("Tên đầy đủ")]
        public string TenDayDu { get; set; }

        [Column("NGUOITAO")]
        [StringLength(300)]
        [Description("Người tạo")]
        public string NguoiTao { get; set; }

        [Column("MABOPK")]
        [StringLength(50)]
        [Description("Mã bó PK")]
        public string MaBoPK { get; set; }

        [Column("NGAYTAO")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [Column("NGAYPHATSINH")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày tạo")]
        public DateTime? NgayPhatSinh { get; set; }

        [Column("SOLUONG")]
        [Description("Số lượng")]
        public decimal? SoLuong { get; set; }

        [Column("TTIENCOVAT")]
        [Description("Tổng tiền mặt hàng và tiền khuyến mại")]
        public decimal? TTienCoVat { get; set; }

        [Column("MAVAT")]
        [StringLength(50)]
        [Description("Mã VAT ra")]
        public string MaVatBan { get; set; }

        [Column("VATBAN")]
        [Description("vat bán")]
        public decimal? VatBan { get; set; }

        [Column("GIABANLECOVAT")]
        [Description("giá bán lẻ có vat")]
        public decimal? GiaBanLeCoVat { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string MaKhachHang { get; set; }
        [Column("MAKEHANG")]
        [StringLength(50)]
        public string MaKeHang { get; set; }

        [Column("MACHUONGTRINHKM")]
        [StringLength(50)]
        public string MaChuongTrinhKhuyenMai { get; set; }

        [Column("LOAIKHUYENMAI")]
        [StringLength(50)]
        public string LoaiKhuyenMai { get; set; }

        [Column("TIENCHIETKHAU")]
        public decimal? TienChietKhau { get; set; }

        [Column("TYLECHIETKHAU")]
        public decimal? TyLeChietKhau { get; set; }

        [Column("TYLEKHUYENMAI")]
        public decimal? TyLeKhuyenMai { get; set; }

        [Column("TIENKHUYENMAI")]
        public decimal? TienKhuyenMai { get; set; }

        [Column("TYLEVOUCHER")]
        public decimal? TyLeVoucher { get; set; }

        [Column("TIENVOUCHER")]
        public decimal? TienVoucher { get; set; }

        [Column("TYLELAILE")]
        public decimal? TyLeLaiLe { get; set; }

        [Column("GIAVON")]
        public decimal? GiaVon { get; set; }

        [Column("ISBANAM")]
        [Description("Is Bán âm -- 0 Không bán âm -- 1 : Bán âm")]
        public Nullable<int> IsBanAm { get; set; }
    }
}
