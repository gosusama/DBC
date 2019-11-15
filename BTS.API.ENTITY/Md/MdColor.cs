using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_COLOR")]
    public class MdColor : DataInfoEntity
    {
        [Column("MACOLOR")]
        [StringLength(50)]
        [Required]
        [Description("MACOLOR")]
        public string MaColor { get; set; }

        [Column("TENCOLOR")]
        [StringLength(500)]
        [Description("Tên Color")]
        public string TenColor { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

    }
}
