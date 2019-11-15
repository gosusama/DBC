using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_DONVI")]
    public class AU_DONVI : DataInfoEntity
    {
        [Column("MADONVI")]
        [Required]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string MaDonVi { get; set; }

        [Column("MADONVICHA")]
        [Description("Mã đơn vị cha")]
        [StringLength(50)]
        public string MaDonViCha { get; set; }

        [Column("TENDONVI")]
        [StringLength(150)]
        [Description("Tên đơn vị")]
        public string TenDonVi { get; set; }

        [Column("SODIENTHOAI")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(50)]
        [Description("Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Column("DIACHI")]
        [StringLength(200)]
        [Description("Địa chỉ đơn vị")]
        public string DiaChi { get; set; }

        [Column("EMAIL")]
        [Description("Hộp thư")]
        [StringLength(50)]
        public string Email { get; set; }

        [Column("MACUAHANG")]
        [Description("Mã cửa hàng")]
        [StringLength(50)]
        public string MaCuaHang { get; set; }

        [Column("TENCUAHANG")]
        [Description("Tên cửa hàng")]
        [StringLength(200)]
        public string TenCuaHang { get; set; }

        [Column("URL")]
        [Description("Url đăng nhập")]
        [StringLength(300)]
        public string Url { get; set; }

        [Column("TRANGTHAI")]
        [Description("Trạng thái")]
        public int TrangThai { get; set; }
    }
}
