using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_HOPDONG")]
    public class MdContract : DataInfoEntity
    {
        [Column("MAHD")]
        [Required]
        [StringLength(50)]
        public string MaHd { get; set; }

        [Column("TENHOPDONG")]
        [Required]
        [StringLength(300)]
        public string TenHd { get; set; }

        [Column("NGAYKY")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime NgayKy { get; set; }


        [Column("GIATRIHD")]
        public decimal? GiaTriHD { get; set; }

        [Column("KHACHHANG")]
        [StringLength(50)]
        public string KhachHang { get; set; }

        [Column("DIACHI")]
        [StringLength(50)]
        public string DiaChi { get; set; }

        [Column("NGUOILIENHE")]
        [StringLength(50)]
        public string NguoiLienHe { get; set; }

        [Column("DONVITHUCHIEN")]
        [StringLength(50)]
        public string DonViThucHien { get; set; }

        [Column("NGUOITHUCHIEN")]
        [StringLength(50)]
        public string NguoiThucHien { get; set; }

        [Column("TINHTRANGHOPDONG")]
        public int TinhTrangHopDong { get; set; }

        [Column("NGAYTHANHLY")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime NgayThanhLy { get; set; }

        [Column("LYDO")]
        [StringLength(100)]
        public string LyDo { get; set; }

        [Column("DIEUKHOANKHAC")]
        [StringLength(100)]
        public string DieuKhoanKhac { get; set; }
    }
}
