using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_BOHANG")]
    public class MdBoHang:DataInfoEntity
    {
        [Column("MABOHANG")]
        [Required]
        [StringLength(50)]
        public string MaBoHang { get; set; }

        [Column("TENBOHANG")]
        [Required]
        [StringLength(300)]
        public string TenBoHang { get; set; }


        [Column("NGAYCHUNGTU")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày phát sinh")]
        public DateTime? NgayCT { get; set; }


        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }


        [Column("GHICHU")]
        [StringLength(500)]
        public string GhiChu { get; set; }
    }
}
