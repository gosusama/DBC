using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_LOAIVATTU")]
    public class MdMerchandiseType : DataInfoEntity
    {
        [Column("MALOAIVATTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã loại vật tư")]
        public string MaLoaiVatTu { get; set; }

        [Column("MACHA")]
        [StringLength(50)]
        [DefaultValue("0")]
        [Description("Mã cha loại vật tư")]
        public string MaCha { get; set; }
        [Column("TENLOAIVT")]
        [StringLength(100)]
        [Required]
        [Description("Tên loại vật tư")]
        public string TenLoaiVatTu { get; set; }

        [Column("TRANGTHAI")]
        [DefaultValue(10)]
        [Required]
        public int TrangThai { get; set; }
    }
}
