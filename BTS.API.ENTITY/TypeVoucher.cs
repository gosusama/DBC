using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.ENTITY
{
    public enum TypeVoucher
    {

        NMUA, // Nhập mua
        NHBANTL,//Nhập hàng bán trả lại
        XBAN, // Xuất bán
        XBANLE, //Xuất bán lẻ
        DCX,   // Điều chuyển xuất,
        DCN,   // Điều chuyển nhập
        TDK, //Tồn đầu kỳ,
        NKHAC, //Nhập khác
        XKHAC,
        CTKM, //chuong trinh khuyen mai
        DH, //dat hang
        N_KK
    }
    public enum LoaiCongNo
    {

        CNKH, //Phiếu công nợ thu khách hàng
        CNNCC, // Phiếu công nợ trả nhà cung cấp
    }
    public enum TrangThaiDonHang
    {
        MOI = 1,
        DANGXACNHAN = 2,
        DAXACNHAN = 3,
        DANGCHUYEN = 4,
        THANHCONG = 5,
        THATBAI=6,
        CHUYENHOAN=7,
        TRALAI = 8,
        DOI = 9,
        HUY = 10,
        HET =11,
    }
    public enum LoaiDonDatHang
    {
        BANLE = 1,
        BANBUON = 2,
    }
    public enum LoaiDatHang
    {
        NHACUNGCAP = 1,
        KHACHHANG = 2,
    }
}
