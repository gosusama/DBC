using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BTS.API.ENTITY.Authorize
{
    [Table("AU_LOG")]
    public class AU_LOG : DataInfoEntity
    {
        [Column("NGAY")]
        public DateTime NGAY { get; set; }

        [Column("TRANGTHAI")]
        [StringLength(50)]
        [Description("Trạng thái Đăng nhập hoặc Đăng xuất")]
        public string TrangThai { get; set; }

        [Column("THOIGIAN")]
        [StringLength(50)]
        [Description("Thời gian đăng nhập, đăng xuất")]
        public string ThoiGian { get; set; }

        [Column("MAMAYBAN")]
        [StringLength(50)]
        [Description("Mã máy bán")]
        public string MaMayBan { get; set; }

        [Column("MANHANVIEN")]
        [StringLength(50)]
        [Description("Mã Nhân viên truy cập")]
        public string MaNhanVien { get; set; }

        [Column("TINHTRANG")]
        [Description("Tình trạng hoạt động")]
        public int TinhTrang { get; set; }
    }
}
