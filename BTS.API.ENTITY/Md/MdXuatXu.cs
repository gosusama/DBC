using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_XUATXU")]
    public class MdXuatXu : DataInfoEntity
    {
        [Column("MAXUATXU")]
        [Required]
        [StringLength(50)]
        public string MaXuatXu { get; set; }
        [Column("TENXUATXU")]
        [Required]
        [StringLength(200)]
        public string TenXuatXu { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
        [Column("GHICHU")]
        [StringLength(500)]
        public string GhiChu { get; set; }
    }
}
