using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_LOAITHUE")]
    public class MdTax:DataInfoEntity
    {
        [Column("MALOAITHUE")]
        [StringLength(50)]
        [Required]
        [Description("Mã loại thuế: thuế suất, thuế VAT,...")]
        public string MaLoaiThue { get; set; }

        [Column("LOAITHUE")]
        [StringLength(100)]
        [Description("Tên loại thuế: thuế suất, thuế VAT,...")]
        public string LoaiThue { get; set; }

        [Column("TYGIA")]
        public decimal TaxRate { get; set; }

        [Column("TAIKHOANKT")]
        [StringLength(50)]
        [Description("Taì khoản kế toán tương ứng")]
        public string TaiKhoanKt { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
