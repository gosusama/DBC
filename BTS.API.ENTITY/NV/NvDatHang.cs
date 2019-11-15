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
    [Table("NVDATHANG")]
    public class NvDatHang : DataInfoEntity
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

        [Column("LOAI")]
        [Required]
        [Description("1 - NCC, 2 - Khách hàng")]
        public int Loai { get; set; }//

        [Column("NGAY")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        public DateTime? Ngay { get; set; }

        [Column("NGUOILAP")]
        [StringLength(150)]
        public string NguoiLap { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string MaKhachHang { get; set; }

        [Column("MANHANVIEN")]
        [StringLength(50)]
        public string MaNhanVien{ get; set; }

        [Column("MADONVIDAT")]
        [StringLength(50)]
        public string MaDonViDat { get; set; }

        [Column("MANHACUNGCAP")]
        [StringLength(50)]
        public string MaNhaCungCap { get; set; }

        [Column("MAHD")]
        [StringLength(50)]
        public string MaHd { get; set; }

        [Column("NOIDUNG")]
        [StringLength(250)]
        public string NoiDung { get; set; }

        [Column("THANHTIENTRUOCVAT")]
        public decimal? ThanhTienTruocVat { get; set; }

        [Column("VAT")]
        [StringLength(50)]

        public string VAT { get; set; }
        [Column("TIENVAT")]
        public decimal? TienVat { get; set; }

        [Column("TIENCHIETKHAU")]
        public decimal? TienChietKhau { get; set; }

        [Column("THANHTIENSAUVAT")]
        public decimal? ThanhTienSauVat { get; set; } //Tiền gốc

        [Column("TRANGTHAI")]
        public int TrangThai { get; set; } // 10 =  hoàn thành, 0 = chua duyet, 20- duyệt

        [Column("TENNN")]
        [StringLength(250)]
        public string TenNn { get; set; }

        [Column("SDTNN")]
        [StringLength(50)]
        public string SdtNn { get; set; }

        [Column("DIACHINN")]
        [StringLength(250)]
        public string DiaChiNn { get; set; }

        [Column("TRANGTHAITT")]
        public int? TrangThaiTt { get; set; } 

        [Column("HINHTHUCTT")]
        [StringLength(10)]
        public string HinhThucTt { get; set; }

        [Column("TENNGH")]
        [StringLength(250)]
        public string TenNgh { get; set; }

        [Column("SDTNGH")]
        [StringLength(50)]
        public string SdtNgh { get; set; }

        [Column("CMNDNGH")]
        [StringLength(250)]
        public string CmndNgh { get; set; }

        [Column("SOPHIEUCON")]
        [StringLength(250)]
        public string SoPhieuCon { get; set; }
        [Column("ISBANBUON")]
        public int IsBanBuon { get; set; } // 1 =  Bán lẻ, 2 = Bán buôn

        public void GenerateMaChungTuPk()
        {
            SoPhieuPk = string.Format("{0}.{1}", UnitCode, SoPhieu);
        }

    }
}
