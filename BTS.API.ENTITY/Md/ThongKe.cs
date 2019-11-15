using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("THONGKE")]
    public class ThongKe : DataInfoEntity
    {

        [Column("LOAI")]
        [Required]
        [StringLength(10)]
        public string Loai { get; set; }

        [Column("MA")]
        [Required]
        [StringLength(20)]
        public string Ma { get; set; }

        [Column("Ten")]
        [StringLength(100)]
        public string Ten { get; set; }

        [Column("TUNGAY")]
        public Nullable<DateTime> TuNgay { get; set; }

        [Column("DENNGAY")]
        public Nullable<DateTime> DenNgay { get; set; }

        [Column("GIATRI")]
        public decimal GiaTri { get; set; }

    }
}
