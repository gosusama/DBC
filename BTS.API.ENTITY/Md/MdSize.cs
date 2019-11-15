using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_SIZE")]
    public class MdSize : DataInfoEntity
    {
        [Column("MASIZE")]
        [StringLength(50)]
        [Required]
        [Description("MASIZE")]
        public string MaSize { get; set; }

        [Column("TENSIZE")]
        [StringLength(100)]
        [Description("Tên SIZE")]
        public string TenSize { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
