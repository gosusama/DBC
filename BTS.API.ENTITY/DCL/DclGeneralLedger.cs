using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.DCL
{
    [Table("SOTONGHOP")]
    public class DclGeneralLedger : DetailInfoEntity
    {
        [Column("MACHUNGTUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ pk")]
        public string MaChungTuPk { get; set; }
        [Column("MACHUNGTU")]
        [StringLength(50)]
        [Description("Mã chứng từ")]
        public string MaChungTu { get; set; }
        [Column("NGAYCHUNGTU")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày chứng từ.")]
        public DateTime? NgayCT { get; set; }

        [Column("LOAICHUNGTU")]
        [StringLength(50)]
        [Required]
        [Description("Loại")]
        public string LoaiPhieu { get; set; }//

        [Column("TKNO")]
        [StringLength(50)]
        public string TkNo { get; set; }

        [Column("TKCO")]
        [StringLength(50)]
        public string TkCo { get; set; }

        [Column("SOTIEN")]
        public decimal SoTien { get; set; }

        [Column("DOITUONGNO")]
        [StringLength(50)]
        public string DoiTuongNo { get; set; }

        [Column("DOITUONGCO")]
        [StringLength(50)]
        public string DoiTuongCo { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("INDEX")]
        public int Index { get; set; }

        [Column("NOIDUNG")]
        [StringLength(300)]
        [Description("Nội dung")]
        public string NoiDung { get; set; }

        [Column("UNITCODE")]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string UnitCode { get; set; }
    }
}
