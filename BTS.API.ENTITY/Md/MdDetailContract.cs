using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_HOPDONGCHITIET")]
    public class MdDetailContract : DetailInfoEntity
    {
        [Column("MAHD")]
        [Required]
        [StringLength(50)]
        public string MaHd { get; set; }
        [Column("MAHANG")]
        [StringLength(50)]
        [Description("MaHang")]
        public string MaHang { get; set; }
        [Column("TENHANG")]
        [StringLength(300)]
        [Description("Tên hàng")]
        public string TenHang { get; set; }
        [Column("DONVITINH")]
        [StringLength(50)]
        [Description("Đơn vị tính")]
        public string DonViTinh { get; set; }
        [Column("MABAOBI")]
        [StringLength(50)]
        [Description("Mã bao bì")]
        public string MaBaoBi { get; set; }

        [Column("SOLUONGBAO")]
        [Description("Số lượng bao")]
        public decimal SoLuongBao { get; set; }

        [Column("SOLUONG")]
        [Description("Số lượng nhập")]
        public decimal SoLuong { get; set; }

        [Column("DONGIA")]
        [Description("Đơn giá hàng hóa")]
        public decimal DonGia { get; set; }

        [Column("SOLUONGLE")]
        [Description("Số lượng lẻ")]
        public decimal? SoLuongLe { get; set; }
        [Column("LUONGQUYCACH")]
        [Description("Lượng quy cách đóng bao")]
        public decimal? LuongBao { get; set; }

        [Column("THANHTIEN")]
        [Description("ThanhTien")]
        public decimal ThanhTien { get; set; }
    }
}
