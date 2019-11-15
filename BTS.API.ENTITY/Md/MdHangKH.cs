using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_HANGKHACHHANG")]
    public class MdHangKH : DataInfoEntity
    {
        [Column("MAHANGKH")]
        [StringLength(50)]
        [Required]
        [Description("MAHANGKH")]
        public string MaHangKh { get; set; }

        [Column("TENHANGKH")]
        [StringLength(200)]
        [Description("Tên Hạng Khách Hàng")]
        public string TenHangKh { get; set; }

        [Column("SOTIEN")]
        [Description("SỐ tiền được lên hạng")]
        public Nullable<decimal> SoTien { get; set; }

        [Column("TYLEGIAMGIASN")]
        [Description("Tỷ lệ giảm giá sinh nhật")]
        public Nullable<decimal> TyLeGiamGiaSn { get; set; }

        [Column("TYLEGIAMGIA")]
        [Description("Tỷ lệ giảm giá")]
        public Nullable<decimal> TyLeGiamGia { get; set; }

        [Column("QUYDOITIEN_THANH_DIEM")]
        [Description("Quy đổi tiền thành điểm")]
        public Nullable<decimal> QuyDoiTienThanhDiem { get; set; }

        [Column("QUYDOIDIEM_THANH_TIEN")]
        [Description("Quy đổi điểm thành tiền")]
        public Nullable<decimal> QuyDoiDiemThanhTien { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("HANG_KHOIDAU")]
        [Description("Hạng khởi đầu")]
        public Nullable<decimal> HangKhoiDau { get; set; }
    }
}
