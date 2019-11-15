using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.NV
{
    [Table("NVHETHAN_HANGHOA_CHITIET")]
    public class NvNgayHetHanHangHoaChiTiet : DataInfoEntity
    {
        [Column("MAPHIEUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã phiếu date hàng table NvNgayHetHanHangHoa")]
        public string MaPhieuPk { get; set; }

        [Column("MANHACUNGCAP")]
        [StringLength(50)]
        public string MaNhaCungCap { get; set; }

        [Column("TENNHACUNGCAP")]
        [StringLength(500)]
        public string TenNhaCungCap { get; set; }

        [Column("MAVATTU")]
        [StringLength(50)]
        public string MaVatTu { get; set; }

        [Column("TENVATTU")]
        [StringLength(500)]
        public string TenVatTu { get; set; }

        [Column("BARCODE")]
        [StringLength(2000)]
        public string BarCode { get; set; }

        [Column("SOLUONG")]
        public Nullable<decimal> SoLuong { get; set; }

        [Column("NGAYSANXUAT")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày sản xuất")]
        public DateTime? NgaySanXuat { get; set; }

        [Column("NGAYHETHAN")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày hết hạn")]
        public DateTime? NgayHetHan { get; set; }

        [Column("CONLAI_NGAYBAO")]
        public Nullable<decimal> ConLai_NgayBao { get; set; }

        [Column("CONLAI_NGAYHETHAN")]
        public Nullable<decimal> ConLai_NgayHetHan { get; set; }

        [Column("GHICHU")]
        [StringLength(1000)]
        public string GhiChu { get; set; }
    }
}
