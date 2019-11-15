using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_DONVITINH")]
    public class MdDonViTinh : DataInfoEntity
    {
        [Column("MADVT")]
        [StringLength(50)]
        [Required]
        [Description("MaDVT")]
        public string MaDVT { get; set; }

        [Column("TENDVT")]
        [StringLength(100)]
        [Description("Tên DVT")]
        public string TenDVT { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

    }
}
