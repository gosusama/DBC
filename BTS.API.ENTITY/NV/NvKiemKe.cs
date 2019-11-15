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
    [Table("NVKIEMKE")]
    public class NvKiemKe : DataInfoEntity
    {
        [Column("MAPHIEUKIEMKE")]
        [StringLength(50)]
        [Required]
        [Description("Mã phiếu kiểm kê")]
        public string MaPhieuKiemKe { get; set; }//

        [Column("MADONVI")]
        [StringLength(50)]
        public string MaDonVi { get; set; }

        [Column("NGAYKIEMKE")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày kiểm kê, là ngày thực hiện kiểm kê")]
        public DateTime? NgayKiemKe { get; set; }

        [Column("KHOKIEMKE")]
        [StringLength(50)]
        public string KhoKiemKe { get; set; }

        [Column("SOPHIEUKIEMKE")]
        [StringLength(50)]
        public string SoPhieuKiemKe { get; set; }

        [Column("LOAIVATTUKK")]
        [StringLength(50)]
        public string LoaiVatTuKiemKe { get; set; }

        [Column("NHOMVATTUKK")]
        [StringLength(50)]
        public string NhomVatTuKiemKe { get; set; }

        [Column("KEKIEMKE")]
        [Description("Kệ kiểm kê")]
        [StringLength(50)]
        public string KeKiemKe { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("NGUOITAO")]
        [StringLength(200)]
        public string NguoiTao { get; set; }

        [Column("NGAYDUYETPHIEU")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày duyệt, là ngày thực hiện duyệt kiểm kê")]
        public DateTime? NgayDuyetPhieu { get; set; }
    }
}
