using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_NGUOIDUNG")]
    public class AU_NGUOIDUNG : DataInfoEntity
    {
        [Required]
        [Column("USERNAME")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("PASSWORD")]
        [StringLength(50, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Column("MANHANVIEN")]
        [StringLength(50)]
        public string MaNhanVien { get; set; }

        [Column("TENNHANVIEN")]
        [StringLength(200)]
        public string TenNhanVien { get; set; }

        [Column("SODIENTHOAI")]
        [StringLength(20)]
        public string SoDienThoai { get; set; }

        [Column("SOCHUNGMINHTHU")]
        [StringLength(20)]
        public string ChungMinhThu { get; set; }

        [Column("GIOITINH")]
        public Nullable<int> GioiTinh { get; set; }

        [Column("MAPHONG")]
        [Description("Phòng ban nhân viên")]
        [StringLength(50)]
        public string MaPhong { get; set; }

        [Column("CHUCVU")]
        [Description("Chức vụ nhân viên")]
        [StringLength(50)]
        public string ChucVu { get; set; }

        [Column("LEVEL")]
        [Description("Cap Tai Khoan")]
        public Nullable<int> Level { get; set; }

        [Column("TRANGTHAI")]
        public Nullable<int> TrangThai { get; set; }

        [Column("PARENT_UNITCODE")]
        [Description("Mã đơn vị cha")]
        [StringLength(50)]
        public string ParentUnitcode { get; set; }
    }
}
