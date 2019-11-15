using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Md
{
    [Table("DM_NHACUNGCAP")]
    public class MdSupplier : DataInfoEntity
    {
        [Column("MANCC")]
        [StringLength(50)]
        public string MaNCC { get; set; }

        [Column("TENNCC")]
        [StringLength(500)]
        public string TenNCC { get; set; }

        [Column("DIACHI")]
        [StringLength(500)]
        public string DiaChi { get; set; }

        [Column("TINH/THANHPHO")]
        [StringLength(50)]
        public string TinhThanhPho { get; set; }

        [Column("MASOTHUE")]
        [StringLength(50)]
        public string MaSoThue { get; set; }

        [Column("TAIKHOAN_NGANHANG")]
        [StringLength(30)]
        public string TaiKhoan_NganHang { get; set; }

        [Column("THONGTIN_NGANHANG")]
        [StringLength(500)]
        public string ThongTin_NganHang { get; set; }

        [Column("NGUOILIENHE")]
        [StringLength(300)]
        public string NguoiLienHe { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("DIENTHOAI")]
        [StringLength(50)]
        public string DienThoai { get; set; }

        [Column("FAX")]
        [StringLength(50)]
        public string Fax { get; set; }

        [Column("CHUCVU")]
        [StringLength(50)]
        public string ChucVu { get; set; }

        [Column("EMAIL")]
        [StringLength(100)]
        public string Email { get; set; }

        [Column("XUATXU")]
        [StringLength(100)]
        public string XuatXu { get; set; }
    }
}
