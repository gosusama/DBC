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
    [Table("NVGDQUAY_ASYNCCLIENT")]
    public class NvGiaoDichQuay : DataInfoEntity
    {
        [Column("MAGIAODICH")]
        [StringLength(50)]
        [Description("Mã giao dịch")]
        public string MaGiaoDich { get; set; }

        [Column("MAGIAODICHQUAYPK")]
        [StringLength(50)]
        [Required]
        [Description("Mã giao dịch quầy pk")]
        public string MaGiaoDichQuayPK { get; set; }

        [Column("MADONVI")]
        [StringLength(50)]
        [Description("Mã đơn vị")]
        public string MaDonVi { get; set; }

        [Column("LOAIGIAODICH")]
        [Description("Loại giao dịch")]
        public decimal? LoaiGiaoDich { get; set; }

        [Column("NGAYTAO")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày tạo")]
        public DateTime? NgayTao { get; set; }

        [Column("MANGUOITAO")]
        [StringLength(300)]
        [Description("Mã người tạo")]
        public string MaNguoiTao { get; set; }

        [Column("NGUOITAO")]
        [StringLength(300)]
        [Description("Người tạo")]
        public string NguoiTao { get; set; }

        [Column("MAQUAYBAN")]
        [StringLength(300)]
        [Description("Mã quầy bán")]
        public string MaQuayBan { get; set; }

        [Column("NGAYPHATSINH")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "yyyy/MM/dd", ApplyFormatInEditMode = true)]
        [Description("Ngày tạo")]
        public DateTime? NgayPhatSinh { get; set; }

        [Column("HINHTHUCTHANHTOAN")]
        [StringLength(200)]
        public string HinhThucThanhToan { get; set; }

        [Column("MAVOUCHER")]
        [StringLength(50)]
        public string MaVoucher { get; set; }

        [Column("TIENKHACHDUA")]
        public decimal? TienKhachDua { get; set; }

        [Column("TIENVOUCHER")]
        public decimal? TienVoucher { get; set; }

        [Column("TIENTHEVIP")]
        public decimal? TienTheVip { get; set; }

        [Column("TIENTRALAI")]
        public decimal? TienTraLai { get; set; }

        [Column("TIENTHE")]
        [Description("Tiền mua bằng thẻ ngân hàng")]
        public decimal? TienThe { get; set; }

        [Column("TIENCOD")]
        [Description("Tiền COD ngân hàng")]
        public decimal? TienCOD { get; set; }

        [Column("TIENMAT")]
        [Description("Tiền mặt")]
        public decimal? TienMat { get; set; }

        [Column("TTIENCOVAT")]
        [Description("Tổng các loại thanh toán THẺ,COD,TIỀN MẶT")]
        public decimal? TTienCoVat { get; set; }

        [Column("THOIGIAN")]
        [StringLength(150)]
        public string ThoiGian { get; set; }

        [Column("MAKHACHHANG")]
        [StringLength(50)]
        public string MaKhachHang { get; set; }

    }
}
