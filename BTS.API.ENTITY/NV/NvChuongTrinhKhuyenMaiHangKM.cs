using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.NV
{
    [Table("NV_CT_KHUYENMAI_HANGKM")]
    public class NvChuongTrinhKhuyenMaiHangKM : DetailInfoEntity
    {
        [Column("MACHUONGTRINH")]
        [StringLength(50)]
        [Required]
        public string MaChuongTrinh { get; set; }
        [Column("MAKHOXUAT")]
        [StringLength(50)]
        public string MaKhoXuat { get; set; }
        [Column("MAKHOKHUYENMAI")]
        [StringLength(50)]
        public string MaKhoXuatKhuyenMai { get; set; }
        [Column("LOAICHUONGTRINH")]
        public TypePromotion LoaiChuongTrinh { get; set; }

        [Column("MAHANG")]
        [StringLength(50)]
        public string MaHang { get; set; }

        [Column("TENHANG")]
        [StringLength(200)]
        public string TENHANG { get; set; }
        [Column("SOLUONG")]
        public decimal? SoLuong { get; set; }

        [Column("TYLEKHUYENMAI")]
        public decimal? TyLeKhuyenMai { get; set; }
        [Column("GIATRIKHUYENMAI")]
        public decimal? GiaTriKhuyenMai { get; set; }

    }
}
