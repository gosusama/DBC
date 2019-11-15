using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_MAYBANHANG")]
    public class MdSellingMachine : DataInfoEntity
    {
        [Column("CODE")]
        [StringLength(50)]
        public string Code { get; set; }
        [Column("NAME")]
        [StringLength(500)]
        public string Name { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
