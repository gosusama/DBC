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
    [Table("VATTUCHUNGTUCHITIET")]
    public class NvVatTuChungTuChiTiet : DetailInfoEntity
    {
        [Column("MACHUNGTU")]
        [StringLength(50)]
        [Description("Mã chứng từ")]
        public string MaChungTu { get; set; }//

        [Column("MACHUNGTUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ pk")]
        public string MaChungTuPk { get; set; }//

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
        [Column("MAKHOANMUC")]
        [StringLength(50)]
        [Description("MaKhoanMuc")]
        public string MaKhoanMuc { get; set; }
        [Column("MAKHACHHANG")]
        [StringLength(50)]
        [Description("Mã khách hàng")]
        public string MaKhachHang { get; set; }
        [Column("DONVITINH")]
        [StringLength(50)]
        [Description("Đơn vị tính")]
        public string DonViTinh { get; set; }
        [Column("BARCODE")]
        [StringLength(2000)]
        [Description("Barcode")]
        public string Barcode { get; set; }

        [Column("VAT")]
        [StringLength(50)]
        public string VAT { get; set; }

        [Column("LUONGQUYCACH")]
        [Description("Lượng quy cách đóng bao")]
        public decimal? LuongBao { get; set; }
        [Column("SOLUONGLE")]
        [Description("Số lượng lẻ")]
        public decimal? SoLuongLe { get; set; }
        [Column("SOLUONGLECT")]
        [Description("Số lượng lẻ chứng từ")]
        public decimal? SoLuongLeCT { get; set; }
        [Column("SOLUONGBAOCT")]
        [Description("Số lượng bao chứn từ")]
        public decimal? SoLuongBaoCT { get; set; }

        [Column("SOLUONGCT")]
        [Description("Tổng Số lượng nhập (Số lương bao * Quy cách) + số lượng lẻ")]
        public decimal? SoLuongCT { get; set; }

        [Column("SOLUONGBAO")]
        [Description("Số lượng bao")]
        public decimal? SoLuongBao { get; set; }

        [Column("SOLUONG")]
        [Description("Số lượng nhập")]
        public decimal? SoLuong { get; set; }                         

        [Column("DONGIA")]
        [Description("Đơn giá hàng hóa")]
        public decimal? DonGia { get; set; }

        [Column("THANHTIEN")]
        [Description("ThanhTien")]
        public decimal? ThanhTien { get; set; }

        [Column("TIENGIAMGIA")]
        [Description("tiền giảm giá")]
        public decimal? TienGiamGia { get; set; }

        [Column("GIAVON")]
        [Description("Giá vốn")]
        public decimal? GiaVon { get; set; }

        [Column("GIAMUACOVAT")]
        [Description("Giá mua có VAT")]
        public decimal? GiaMuaCoVat { get; set; }

        [Column("INDEX")]
        public int? Index { get; set; }
    }
}
