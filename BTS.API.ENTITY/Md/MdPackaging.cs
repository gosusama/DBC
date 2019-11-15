using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_BAOBI")]
    public class MdPackaging:DataInfoEntity
    {
        [Column("MABAOBI")]
        [StringLength(50)]
        [Required]
        [Description("Mã bao bì")]
        public string MaBaoBi { get; set; }

        [Column("TENBAOBI")]
        [StringLength(100)]
        [Required]
        [Description("Tên bao bì")]
        public string TenBaoBi { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("SOLUONG")]
        [Required]
        public decimal SoLuong { get; set; }
    }
}
