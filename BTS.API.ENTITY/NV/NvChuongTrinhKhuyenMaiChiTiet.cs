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
    [Table("NV_CT_KHUYENMAI_CHITIET")]
    public class NvChuongTrinhKhuyenMaiChiTiet : DetailInfoEntity
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
        [Required]
        public string MaHang { get; set; }
        [Column("HANGKMCHINH")]
        public bool? IsParent { get; set; }
        [Column("SOLUONG")]
        public decimal? SoLuong { get; set; }
        [Column("TUGIO")]
        [StringLength(50)]
        public string TuGio { get; set; }
        [Column("DENGIO")]
        [StringLength(50)]
        public string DenGio { get; set; }
        [Column("MAHANG_KHUYENMAI")]
        [StringLength(50)]
        public string MaHangKhuyenMai { get; set; }
        [Column("SOLUONG_KHUYENMAI")]
        public decimal? SoLuongKhuyenMai { get; set; }
        [Column("TYLEKHUYENMAI")]
        public decimal? TyLeKhuyenMai { get; set; }
        [Column("GIATRIKHUYENMAI")]
        public decimal? GiaTriKhuyenMai { get; set; }
        [Column("TENHANG")]
        [StringLength(200)]
        public string TENHANG { get; set; }
    }
}
