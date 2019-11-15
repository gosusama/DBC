using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    public enum TypeXN
    {
        XUAT,
        NHAP
    }
    [Table("DM_LOAILYDO")]
    public class MdTypeReason : DataInfoEntity
    {
        [Column("MALYDO")]
        [Required]
        [StringLength(50)]
        public string MaLyDo { get; set; }

        [Column("MACHA")]
        [StringLength(50)]
        public string MaCha { get; set; }

        [Column("TENLYDO")]
        [StringLength(300)]
        public string TenLyDo { get; set; }
        [Column("LOAI")]
        public TypeXN Loai { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
