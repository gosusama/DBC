using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_NHOMVATTU")]
    public class MdNhomVatTu : DataInfoEntity
    {
        [Column("MALOAIVATTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã loại vật tư")]
        public string MaLoaiVatTu { get; set; }

        [Column("MANHOMVTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã nhóm vật tư")]
        public string MaNhom { get; set; }

        [Column("MACHA")]
        [StringLength(50)]
        [Description("Mã cha nhóm vật tư")]
        public string MaCha { get; set; }

        [Column("TENNHOMVT")]
        [StringLength(100)]
        [Description("Tên Nhóm vật tư")]
        public string TenNhom { get; set; }

        [Column("TRANGTHAI")]
        [DefaultValue(10)]
        [Required]
        public int TrangThai { get; set; }
    }
}
