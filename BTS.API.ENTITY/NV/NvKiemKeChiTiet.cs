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
    [Table("NVKIEMKECHITIET")]
    public class NvKiemKeChiTiet : DataInfoEntity
    {
        [Column("MAPHIEUKIEMKE")]
        [StringLength(50)]
        [Required]
        [Description("Mã phiếu kiểm kê")]
        public string MaPhieuKiemKe { get; set; }

        [Column("MAVATTU")]
        [StringLength(50)]
        public string MaVatTu { get; set; }

        [Column("BARCODE")]
        [StringLength(2000)]
        public string Barcode { get; set; }

        [Column("TENVATTU")]
        [StringLength(500)]
        public string TenVatTu { get; set; }

        [Column("MADONVI")]
        [StringLength(50)]
        public string MaDonVi { get; set; }

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

        [Column("SOLUONGTONMAY")]
        public decimal ? SoLuongTonMay { get; set; }

        [Column("SOLUONGKIEMKE")]
        public decimal ? SoLuongKiemKe { get; set; }

        [Column("SOLUONGCHENHLECH")]
        public decimal ? SoLuongChenhLech { get; set; }

        [Column("TIENTONMAY")]
        public decimal ? TienTonMay { get; set; }

        [Column("TIENKIEMKE")]
        public decimal ? TienKiemKe { get; set; }

        [Column("TIENCHENHLECH")]
        public decimal ? TienChenhLech { get; set; }

        [Column("GIAVON")]
        public decimal ? GiaVon { get; set; }

        [Column("GHICHU")]
        [StringLength(5000)]
        public string GhiChu { get; set; }
    }
}
