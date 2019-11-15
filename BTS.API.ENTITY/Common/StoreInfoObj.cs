using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTS.API.ENTITY.Md;

namespace BTS.API.ENTITY.Common
{
    public class StoreInfoObj
    {
        public class StoreInfo
        {
            public StoreInfo()
            {
                DataKhoHang = new List<KhoHang>();
                DataNhanVien = new List<NhanVien>();
            }
            public string Id { get; set; }
            public string MaDonVi { get; set; }
            public string MaDonViCha { get; set; }
            public string TenDonVi { get; set; }
            public string Email { get; set; }
            public string MaCuaHang { get; set; }
            public string TenCuaHang { get; set; }
            public string DiaDiemBanHang { get; set; }
            public string NoiDungKhac { get; set; }
            public string LoaiHinhCuaHang { get; set; }
            public string DaiDienDoanhNghiep { get; set; }
            public string SoDienThoai { get; set; }
            public int TrangThai { get; set; }
            public string ChucVu { get; set; }
            public List<KhoHang> DataKhoHang { get; set; }
            public List<NhanVien> DataNhanVien { get; set; }
        }
        public class KhoHang
        {
            public string Id { get; set; }
            public string MaDonVi { get; set; }
            public string MaCuaHang { get; set; }
            public string MaKho { get; set; }
            public string TenKho { get; set; }
            public string TaiKhoanKt { get; set; }
            public string DiaChi { get; set; }
            public string ThongTinBoSung { get; set; }
            public int TrangThai { get; set; }
        }

        public class NhanVien
        {
            public string Id { get; set; }
            public string MaDonVi { get; set; }
            public string MaCuaHang { get; set; }
            public string MaNhanVien { get; set; }
            public string TenNhanVien { get; set; }
            public string SoDienThoai { get; set; }
            public string ChungMinhThu { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int GioiTinh { get; set; }
            public int TrangThai { get; set; }
            public string ThongTinBoSung { get; set; }
        }


    }
}
