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
    [Table("NV_CT_KHUYENMAI")]
    public class NvChuongTrinhKhuyenMai : DataInfoEntity
    {
        [Column("MACHUONGTRINH")]
        [StringLength(50)]
        [Required]
        public string MaChuongTrinh { get; set;}

        [Column("TUNGAY")]
        public DateTime TuNgay { get; set; }
        [Column("DENNGAY")]
        public DateTime DenNgay { get; set; }
        [Column("TUGIO")]
        [StringLength(50)]
        public string TuGio { get; set; }
        [Column("DENGIO")]
        [StringLength(50)]
        public string DenGio { get; set; }

        [Column("GIATRIKHUYENMAI")]
        public decimal? GiaTriKhuyenMai { get; set; }        //Giá tiền khuyến mại
        [Column("TYLEKHUYENMAI")]
        public decimal? TyLeKhuyenMai { get; set; }         
        [Column("MAKHOXUAT")]
        [StringLength(50)]
        public string MaKhoXuat { get; set; }
        [Column("MAKHOKHUYENMAI")]
        [StringLength(200)]
        public string MaKhoXuatKhuyenMai { get; set; }

        [Column("LOAIKHUYENMAI")]
        public int? LoaiKhuyenMai { get; set; } // 1: Chiết khấu, 2: Đồng giá, 7: Voucher

        [Column("NOIDUNG")]
        [StringLength(500)]
        public string NoiDung { get; set; }
        [Column("TRANGTHAI")]
        public int TrangThai { get; set; } // 10 =  active, 0 hết hiệu lực
        //For Voucher
        [Column("TIENBATDAU")]
        public decimal? TienBatDau { get; set; }
        [Column("TIENKETTHUC")]
        public decimal? TienKetThuc { get; set; }
        [Column("MAGIAMGIA")]
        [StringLength(200)]
        public string MaGiamGia { get; set; }
        [Column("DIENTHOAI")]
        [StringLength(15)]
        public string DienThoai { get; set; }
        //For Sales Tịnh tiến
        [Column("TYLEBATDAU")]
        public decimal? TyLeBatDau { get; set; }
        [Column("SOLUONG")]
        public Nullable<decimal> SoLuong { get; set; }
        //For Sales Nhân đôi tích điểm
        [Column("HANGKHACHHANG")]
        [StringLength(50)]
        public string HangKhachHang { get; set; }

        [Column("TRANGTHAISUDUNG")]
        public Nullable<int> TrangThaiSuDung { get; set; } // 10 =  active, 0 hết hiệu lực
    }
}
