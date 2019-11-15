using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.DCL
{
    public class DclGeneralLedgerVm
    {
        /// <summary>
        /// TODO: Test
        /// </summary>
        /// 
        public class TongHopBaoCao
        {
            public string SOCT { get; set; }
            public string NGAYCT { get; set; }
            public string NOIDUNG { get; set; }
            public string TAIKHOAN { get; set; }
            public decimal? NO { get; set; }
            public decimal? CO { get; set; }
            public string STT { get; set; }
        }
        #region GeneralJourl
        public class ParameterGeneralJournal
        {
            public DateTime TuNgay { get; set; }
            public DateTime DenNgay { get; set; }
            public string UnitCode { get; set; }
        }

        public class ChiTietChungTu
        {
            public ThongTinChungTu ChungTu { get; set; }
            public List<ThongTinChungTu> ChiTiet { get; set; }
            public ThongTinChungTu Total { get; set;}
        }

        public class ThongTinChungTu
        {
            public string SoCT { get; set; }
            public string NgayCT { get; set; }
            public string NoiDung { get; set; }
            public string TaiKhoan { get; set;}
            public decimal? No { get; set; }
            public decimal? Co { get; set; }
        }

        public class BanBaoCaoNKChung
        {
            public BanBaoCaoNKChung()
            {
                Data = new List<ChiTietChungTu>();
            }
            public string NamBC { get; set; }
            public string ThangBC { get; set; }
            public List<ChiTietChungTu> Data { get; set; }
        }
        #endregion

        #region Subsidiary
        public class ParameterSubsidiaryLedger
        {
            public string TaiKhoan { get; set; }
            public DateTime TuNgay { get; set; }
            public DateTime DenNgay { get; set; }
            public string UnitCode { get; set; }
        }
        public class BanBaoCaoChiTietTaiKhoan
        {
            public BanBaoCaoChiTietTaiKhoan()
            {
                Data = new List<TaiKhoanChiTiet>();
            }
            public string SoTaiKhoan { get; set; }
            public string TenTaiKhoan { get; set; }
            public string NamBC { get; set; }
            public string ThangBC { get; set; }
            public string NoiDung { get; set; }
            public decimal SoDuNoDauKy { get; set; }
            public decimal SoDuCoDauKy { get; set; }
            public List<TaiKhoanChiTiet> Data { get; set; }
        }
        public class TaiKhoanChiTiet
        {
            public TaiKhoanChiTiet()
            {
                Details = new List<DuLieuChiTiet>();
            }
            public List<DuLieuChiTiet> Details { get; set; }
            public decimal DuNoDauKy { get; set; }
            public decimal DuCoDauKy { get; set; }
            public string SoTaiKhoan { get; set; }
            public string TenTaiKhoan { get; set; }
        }
        public class DuLieuChiTiet
        {
            public string SOCT { get; set; }
            public string NGAYCT { get; set; }
            public string NOIDUNG { get; set; }
            public string TAIKHOAN { get; set; }
            public string TAIKHOANDOI { get; set; }
            public decimal? NO { get; set; }
            public decimal? CO { get; set; }
            public string STT { get; set; }
        }
        #endregion
    }
}
