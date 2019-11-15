using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    //Tao bởi Thieu, 26/8/2016
    [Table("DM_CHIETKHAUKH")]
    public class MdChietKhauKH : DataInfoEntity
    {
        [Column("MACHIETKHAU")]
        [StringLength(50)]
        [Required]
        [Description("MACHIETKHAU")]
        public string MaChietKhau { get; set; }

        [Column("TIENTU")]
        [Description("Số tiền bắt đầu hưởng mức chiết khấu")]
        public Nullable<decimal> TienTu { get; set; }

        [Column("TIENDEN")]
        [Description("Số tiền kết thúc hưởng mức chiết khấu")]
        public Nullable<decimal> TienDen { get; set; }

        [Column("TYLECHIETKHAU")]
        [Description("Tỷ lệ chiết khấu")]
        public Nullable<decimal> TyLeChietKhau { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

    }
}
