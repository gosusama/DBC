using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.DCL
{
    [Table("DCL_KHOASO")]
    public class DclCloseout : DetailInfoEntity
    {
        [Column("MAKHOASO")]
        [Required]
        [StringLength(50)]
        public string MaKhoaSo { get; set; }
        [Column("NGAYKHOASO")]
        [Required]
        public DateTime NgayKhoaSo { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
        [Column("USER_KHOASO")]
        [StringLength(150)]
        public string User { get; set; }

        [Column("UNITCODE")]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string UnitCode { get; set; }
    }
}
