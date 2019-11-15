using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.NV
{
    [Table("NVHETHAN_HANGHOA")]
    public class NvNgayHetHanHangHoa : DataInfoEntity
    {
        [Column("MAPHIEU")]
        [StringLength(50)]
        [Required]
        [Description("Mã phiếu date hàng")]
        public string MaPhieu { get; set; }//

        [Column("MAPHIEUPK")]
        [StringLength(50)]
        [Required]
        public string MaPhieuPk { get; set; }

        [Column("NGAYBAODATE")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày báo date")]
        public DateTime? NgayBaoDate { get; set; }

        [Column("THOIGIAN")]
        [StringLength(15)]
        public string ThoiGian { get; set; }

        [Column("NOIDUNG")]
        [StringLength(2000)]
        public string NoiDung { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        public void GenerateMaPhieuPk()
        {
            MaPhieuPk = string.Format("{0}.{1}", UnitCode, MaPhieu);
        }
    }
}
