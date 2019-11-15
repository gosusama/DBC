using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_KHO")]
    public class MdWareHouse:DataInfoEntity
    {
        [Column("MAKHO")]
        [StringLength(50)]
        public string MaKho { get; set; }

        [Column("TENKHO")]
        [StringLength(200)]
        public string TenKho { get; set; }

        [Column("MADONVI")]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string MaDonVi { get; set; }

        [Column("MACUAHANG")]
        [Description("Mã cửa hàng")]
        [StringLength(50)]
        public string MaCuaHang { get; set; }

        [Column("TAIKHOANKT")]
        [StringLength(50)]
        [Description("Taì khoản kế toán tương ứng")]
        public string TaiKhoanKt { get; set; }

        [Column("DIACHI")]
        [StringLength(200)]
        public string DiaChi { get; set; }

        [Column("THONGTINBOSUNG")]
        [StringLength(1000)]
        public string ThongTinBoSung { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }
    }
}
