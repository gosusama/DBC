using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_KEHANG")]
    public class MdShelves: DataInfoEntity
    {
        [Column("MAKEHANG")]
        [Required]
        [StringLength(50)]
        [Description("Mã kệ hàng")]
        public string MaKeHang { get; set; }

        [Column("TENKEHANG")]
        [Required]
        [StringLength(150)]
        [Description("Tên kệ hàng")]
        public string TenKeHang { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; } //10 = hiệu lực
    }
}
