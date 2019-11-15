using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DATA_DONGBO")]
    public class MdAsync
    {
        [Column("MAVATTU")]
        [Key]
        [Required]
        [StringLength(20)]
        public string MaVatTu { get; set; }

        [Column("MAMAYBAN")]
        [StringLength(2000)]
        public string MaMayBan { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("NGAYDONGBO")]
        public DateTime NgayDongBo { get; set; }
    }
}
