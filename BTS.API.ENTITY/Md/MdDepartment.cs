using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_PHONGBAN")]
    public class MdDepartment : DataInfoEntity
    {
        [Column("MAPHONG")]
        [StringLength(50)]
        public string MaPhong { get; set; }

        [Column("TENPHONG")]
        [StringLength(200)]
        public string TenPhong { get; set; }

        [Column("THONGTINBOSUNG")]
        [StringLength(1000)]
        public string ThongTinBoSung { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
