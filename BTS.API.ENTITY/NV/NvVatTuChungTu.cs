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
    [Table("VATTUCHUNGTU")]
    public class NvVatTuChungTu : DataInfoEntity
    {
        [Column("LOAICHUNGTU")]
        [StringLength(50)]
        [Required]
        [Description("Loại")]
        public string LoaiPhieu { get; set; }//

        [Column("MACHUNGTUPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ pk")]
        public string MaChungTuPk { get; set; }//

        [Column("MACHUNGTU")]
        [StringLength(50)]
        [Required]
        [Description("Mã chứng từ")]
        public string MaChungTu { get; set; }//

        [Column("NGAYCHUNGTU")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày chứng từ. Chính là ngày xuất kho và ngày nhập kho")]
        public DateTime? NgayCT { get; set; }

        [Column("NGAYDUYETPHIEU")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày duyệt. Ngày duyệt phiếu để tính xuất nhập tồn theo ngày duyệt")]
        public DateTime? NgayDuyetPhieu { get; set; }

        [Column("THOIGIANDUYETPHIEU")]
        [Description("Thời gian duyệt phiếu")]
        public int? ThoiGianDuyetPhieu { get; set; }

        [Column("MAKHOXUAT")]
        [StringLength(50)]
        public string MaKhoXuat { get; set; }

        [Column("SOPHIEUDATHANG")]
        [StringLength(50)]
        public string SoPhieuDatHang { get; set; }

        [Column("MAKHONHAP")]
        [StringLength(50)]
        public string MaKhoNhap { get; set; }
        [Column("SOHOADON")]
        [StringLength(50)]
        [Description("Số hóa đơn")]//Với Diều chuyển nội là là Mã chứng từ của nhập từ phiếu nhập mua
        public string MaHoaDon { get; set; }//
        [Column("NGAYHOADON")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày hóa đơn")]
        public DateTime? NgayHoaDon { get; set; }

        [Column("MADONVINHAN")]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string MaDonViNhan { get; set; }//

        [Column("MADONVIXUAT")]
        [Description("Mã đơn vị")]
        [StringLength(50)]
        public string MaDonViXuat { get; set; }//

        [Column("LOAITAIKHOAN")]
        [Description("Loại tài khoản nợ/có")]
        [StringLength(50)]
        public string LoaiTaiKhoan { get; set; }

        [Column("SOHD")]
        [StringLength(50)]
        [Description("Số Hợp đồng")]
        public string SoHd { get; set; }//

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        [Description("Mã Khách hàng")]
        public string MaKhachHang { get; set; }//

        [Column("MAHANG")]
        [StringLength(50)]
        [Description("Mã hàng")]
        public string MaHang { get; set; }//

        [Column("MASOTHUE")]
        [StringLength(50)]
        [Description("Mã số thuế được ăn theo thông tin khách hàng")]
        public string MaSoThue { get; set; }

        [Column("NGUOIVANCHUYEN")]
        [StringLength(300)]
        [Description("Người vận chuyển")]
        public string NguoiVanChuyen { get; set; }//

        [Column("PHUONGTIENVANCHUYEN")]
        [StringLength(300)]
        [Description("Phương tiện vận chuyển")]
        public string PhuongTienVanChuyen { get; set; }//

        [Column("NOIDUNG")]
        [StringLength(400)]
        [Description("Nội Dung")]
        public string NoiDung { get; set; }//

        [Column("MALYDOKHAC")]
        [StringLength(50)]
        [Description("Mã lý do khác")]
        public string MaLyDo { get; set; }//

        [Column("MAMAYBAN")]
        [StringLength(50)]
        [Description("Mã máy bán hàng")]
        public string MaMayBan { get; set; }

        [Column("LENHDIEUDONG")]
        [StringLength(400)]
        [Description("Lệnh điều động")]
        public string LenhDieuDong { get; set; }//

        [Column("THANHTIENTRUOCVAT")]
        public decimal? ThanhTienTruocVat { get; set; }

        [Column("TIENCHIETKHAU")]
        public decimal? TienChietKhau { get; set; }

        [Column("TIENVAT")]
        public decimal? TienVat { get; set; }

        [Column("TONGTIENGIAMGIA")]
        public decimal? TongTienGiamGia { get; set; }

        [Column("THANHTIENSAUVAT")]
        public decimal? ThanhTienSauVat { get; set; } //Tiền gốc

        [Column("VAT")]
        [StringLength(50)]
        public string VAT { get; set; }

        [Column("NGAYDIEUDONG")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày điều động")]
        public DateTime? NgayDieuDong { get; set; }

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; } //10 là duyệt
        public void GenerateMaChungTuPk()
        {
            MaChungTuPk = string.Format("{0}.{1}", UnitCode, MaChungTu);
        }

        [Column("TRANGTHAITHANHTOAN")]
        public Nullable<int> TrangThaiThanhToan { get; set; } //10 là đã thanh toán
        
        [Column("TIENMAT")]
        public decimal? TienMat { get; set; }

        [Column("TIENTHE")]
        public decimal? TienThe { get; set; }

        [Column("TIENCOD")]
        public decimal? TienCOD { get; set; }

        [Column("MANHANVIEN")]
        [StringLength(50)]
        public string MaNhanVien { get; set; }

        [Column("TENNGH")]
        [StringLength(250)]
        public string TenNgh { get; set; }

        [Column("TENNN")]
        [StringLength(250)]
        public string TenNn { get; set; }

        [Column("TIENNOCU")]
        [Description("Tiền nợ cũ")]
        public decimal? TienNoCu { get; set; }

        [Column("TIENTHANHTOAN")]
        [Description("Tiền thanh toán")]
        public decimal? TienThanhToan { get; set; }
    }
}
