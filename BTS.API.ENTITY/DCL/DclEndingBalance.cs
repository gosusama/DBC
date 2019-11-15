using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.DCL
{
    [Table("DCL_SODUCUOIKY")]
    public class DclEndingBalance : DetailInfoEntity
    {
        [Column("MAKHOASO")]
        [Required]
        [StringLength(50)]
        public string MaKhoaSo { get; set; }

        [Column("TAIKHOAN")]
        [Required]
        [StringLength(50)]
        public string TaiKhoan { get; set; }
        [Column("SODU_NO_CUOIKY")]
        public decimal SoDuNoCuoiKy { get; set; }
        [Column("SODU_CO_CUOIKY")]
        public decimal SoDuCoCuoiKy { get; set; }
        [Column("SODU_CUOIKY")]
        public decimal SoDuCuoiKy { get; set; }
    }
}
