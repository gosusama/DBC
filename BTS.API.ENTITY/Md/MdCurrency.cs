using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_NGOAITE")]
    public class MdCurrency:DataInfoEntity
    {
        [Column("MANGOAITE")]
        [Required]
        [StringLength(50)]
        public string MaNgoaiTe { get; set; }

        [Column("TENNGOAITE")]
        [StringLength(100)]
        public string TenNgoaiTe { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
