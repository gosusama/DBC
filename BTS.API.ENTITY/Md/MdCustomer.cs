using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTS.API.ENTITY.Md
{
    [Table("DM_KHACHHANG")]
    public class MdCustomer : DataInfoEntity
    {
        [Column("MAKH")]
        [StringLength(50)]
        public string MaKH {get;set;}

        [Column("TENKH")]
        [StringLength(500)]
        public string TenKH {get;set;}

        [Column("TENKHAC")]
        [StringLength(500)]
        public string TenKhac { get; set; }

        [Column("DIACHI")]
        [StringLength(500)]
        public string DiaChi {get;set;}

        [Column("TINH/THANHPHO")]
        [StringLength(50)]
        public string TinhThanhPho {get;set;}

        [Column("QUANHUYEN")]
        [StringLength(50)]
        public string QuanHuyen { get; set; }

        [Column("MASOTHUE")]
        [StringLength(50)]
        public string MaSoThue {get;set;}

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; }

        [Column("DIENTHOAI")]
        [StringLength(50)]
        public string DienThoai {get;set;}

        [Column("CMTND")]
        [StringLength(50)]
        public string ChungMinhThu { get; set; }

        [Column("EMAIL")]
        [StringLength(100)]
        public string Email {get;set;}

        [Column("LOAIKHACHHANG")]
        public TypeCustomer? LoaiKhachHang { get; set; }
            
        [Column("SODIEM")]
        public decimal? SoDiem { get; set; }

        [Column("TIENNGUYENGIA")]
        public decimal? TienNguyenGia { get; set; }

        [Column("TIENSALE")]
        public decimal? TienSale { get; set; }

        [Column("TONGTIEN")]
        public decimal? TongTien { get; set; }

        [Column("MATHE")]
        [StringLength(50)]
        public string MaThe { get; set; }

        [Column("NGAYCAPTHE")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayCapThe { get; set; }

        [Column("NGAYHETHAN")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayHetHan { get; set; }

        [Column("GHICHU")]
        [StringLength(500)]
        public string GhiChu { get; set; }

        [Column("HANGKHACHHANG")]
        [StringLength(50)]
        public string HangKhachHang { get; set; }

        [Column("HANGKHACHHANGCU")]
        [StringLength(50)]
        public string HangKhachHangCu { get; set; }

        [Column("NGAYSINH")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgaySinh { get; set; }

        [Column("NGAYDACBIET")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayDacBiet { get; set; }

        [Column("QUENTHE")]
        public Nullable<int> QuenThe { get; set; }

        [Column("NGAYCHAMSOC")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayChamSoc { get; set; }

        [Column("ISCARE")]
        public bool? IsCare { get; set; }

        [Column("TIENNGUYENGIA_CHAMSOC")]
        public decimal? TienNguyenGia_ChamSoc { get; set; }

        [Column("TONGTIEN_CHAMSOC")]
        public decimal? TongTien_ChamSoc { get; set; }

        [Column("GHICHUCU")]
        [StringLength(1000)]
        public string GhiChuCu { get; set; }

        [Column("NGAYMUAHANG")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> NgayMuaHang { get; set; }
    }
}
