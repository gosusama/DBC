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
    [Table("NVDATHANGCHITIET")]
    public class NvDatHangChiTiet : DetailInfoEntity
    {
        [Column("SOPHIEU")]
        [Required]
        [StringLength(50)]
        public string SoPhieu { get; set; }

        [Column("SOPHIEUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ pk")]
        public string SoPhieuPk { get; set; }//


        [Column("MAHD")]
        [StringLength(50)]
        public string MaHd { get; set; }
        [Column("MAHANG")]
        [StringLength(50)]
        [Description("MaHang")]
        public string MaHang { get; set; }

        [Column("TENHANG")]
        [StringLength(300)]
        [Description("Tên hàng")]
        public string TenHang { get; set; }

        [Column("MABAOBI")]
        [StringLength(50)]
        [Description("Mã bao bì")]
        public string MaBaoBi { get; set; }
        [Column("DONVITINH")]
        [StringLength(50)]
        [Description("Đơn vị tính")]
        public string DonViTinh { get; set; }
        [Column("BARCODE")]
        [StringLength(2000)]
        [Description("Barcode")]
        public string Barcode { get; set; }

        [Column("SOLUONGBAO")]
        [Description("Số lượng bao")]
        public decimal? SoLuongBao { get; set; }

        [Column("SOLUONG")]
        [Description("Số lượng nhập")]
        public decimal? SoLuong { get; set; }

        [Column("DONGIA")]
        [Description("Đơn giá hàng hóa")]
        public decimal? DonGia { get; set; }
        [Column("SOTONMAX")]
        [Description("Số tồn max")]
        public decimal? SoTonMax { get; set; }
        [Column("SOTONMIN")]
        [Description("Số tồn min")]
        public decimal? SoTonMin { get; set; }
        [Column("SOLUONGTON")]
        [Description("Số lượng tồn")]
        public decimal? SoLuongTon { get; set; }

        [Column("LUONGQUYCACH")]
        [Description("Lượng quy cách đóng bao")]
        public decimal? LuongBao { get; set; }

        [Column("SOLUONGDUYET")]
        [Description("Số lượng duyệt")]
        public decimal? SoLuongDuyet { get; set; }
        [Column("SOLUONGBAODUYET")]
        [Description("Số lượng bao duyệt")]
        public decimal? SoLuongBaoDuyet { get; set; }
        [Column("SOLUONGLEDUYET")]
        [Description("Số lượng lẻ duyệt")]
        public decimal? SoLuongLeDuyet { get; set; }
        [Column("DONGIADUYET")]
        [Description("Đơn giá duyệt")]
        public decimal? DonGiaDuyet { get; set; }
        [Column("SOLUONGLE")]
        [Description("Số lượng lẻ")]
        public decimal? SoLuongLe { get; set; }

        [Column("THANHTIEN")]
        [Description("ThanhTien")]
        public decimal? ThanhTien { get; set; }

        [Column("GHICHU")]
        [StringLength(500)]
        public string GhiChu { get; set; }

        [Column("DONGIADEXUAT")]
        [Description("Đơn giá đề xuất")]
        public decimal? DonGiaDeXuat { get; set; }

        [Column("SOLUONGTHUCTE")]
        [Description("Số lượng thực nhận ở kho")]
        public decimal? SoLuongThucTe { get; set; }
    }
}
