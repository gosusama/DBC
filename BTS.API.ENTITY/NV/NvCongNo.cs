using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY.NV
{
    [Table("NVCONGNO")]
    public class NvCongNo : DataInfoEntity
    {
        [Column("MACHUNGTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ")]
        public string MaChungTu { get; set; }

        [Column("LOAICHUNGTU")]
        [StringLength(50)]
        [Required]
        [Description("Loại")]
        public string LoaiChungTu { get; set; }

        [Column("MACHUNGTUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ pk")]
        public string MaChungTuPk { get; set; }

        [Column("NGAYCT")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayCT { get; set; }

        [Column("THOIGIANDUYETPHIEU")]
        [Description("Thời gian duyệt phiếu")]
        public int? ThoiGianDuyetPhieu { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        [Description("Mã khách hàng")]
        public string MaKhachHang { get; set; }

        [Column("MANHACUNGCAP")]
        [StringLength(50)]
        [Description("Mã nhà cung cấp")]
        public string MaNhaCungCap { get; set; }

        [Column("GHICHU")]
        [StringLength(200)]
        [Description("Ghi chú")]
        public string GhiChu { get; set; }

        [Column("THANHTIEN")]
        public decimal? ThanhTien { get; set; }

        [Column("THANHTIENCANTRA")]
        public decimal? ThanhTienCanTra { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; } //10 là duyệt

        public void GenerateMaChungTuPk()
        {
            MaChungTuPk = string.Format("{0}.{1}", UnitCode, MaChungTu);
        }
    }
}
