using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_BOHANGCHITIET")]
    public class MdBoHangChiTiet:DataInfoEntity
    {
        [Column("MABOHANG")]
        [Required]
        [StringLength(50)]
        public string MaBoHang { get; set; }

        [Column("MAHANG")]
        [Required]
        [StringLength(50)]
        public string MaHang { get; set; }

        [Column("TENHANG")]      
        [StringLength(500)]
        public string TenHang { get; set; }

        [Column("SOLUONG")]
        [Description("Số lượng hàng trong bó")]
        public decimal? SoLuong { get; set; }

        [Column("TYLECKLE")]
        [Description("tỷ lệ chiết khấu lẻ")]
        public decimal? TyLeCKLe { get; set; }

        [Column("TYLECKBUON")]
        [Description("tỷ lệ chiết khấu buôn")]
        public decimal? TyLeCKBuon { get; set; }

        [Column("TONGLE")]
        [Description("tổng bán lẻ")]
        public decimal? TongBanLe { get; set; }

        [Column("DONGIA")]
        [Description("đơn giá")]
        public decimal? DonGia { get; set; }

        [Column("TONGBUON")]
        [Description("tổng bán buôn")]
        public decimal? TongBanBuon { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("GHICHU")]
        [StringLength(500)]
        public string GhiChu { get; set; }

    }
}
