using System;
using System.Collections.Generic;
using System.Linq;
using BTS.API.ENTITY;
using System.Globalization;

namespace BTS.SP.API.Utils
{
    public class Utils
    {
        public static string CurrenDate()
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }
        public static string DateToShortString(Nullable<DateTime> date)
        {
            if (date != null)
            {
                return date.Value.ToString("dd/MM/yyyy");
            }
            return DateTime.Now.ToString("dd/MM/yyyy");
        }

        public static string FromDateToYear(Nullable<DateTime> date)
        {
            if (date != null)
            {
                return date.Value.ToString("yyyy");
            }
            return DateTime.Now.ToString("yyyy");
        }

        public static string DateNowToString()
        {
            DateTime now = DateTime.Now;
            return "Ngày " + now.Day + " tháng " + now.Month + " năm " + now.Year;
        }

        public static string FromDateToMonth(Nullable<DateTime> date)
        {
            if (date != null)
            {
                return date.Value.ToString("MM");
            }
            return DateTime.Now.ToString("MM");
        }
        public static double ConvertToNumber(string number)
        {
            if (number != null)
            {
                double so = 1;
                double.TryParse(number, out so);
                return so;
            }
            return 1;
        }

        public static string ConvertToMoney(string number)
        {
            return string.Format("{0:n0}", Math.Round(ConvertToNumber(number),2));
        }

        public static string ConvertUnitNameByCode(string code)
        {
            string result = "";
            if (!string.IsNullOrEmpty(code))
            {
                using (var context = new ERPContext())
                {
                    var donvis = context.AU_DONVIs.FirstOrDefault(x => x.MaDonVi == code);
                    if (donvis != null)
                    {
                        result = donvis.TenDonVi;
                    }
                }
            }
            return result;
        }

        public static string ConvertWareHouseByCode(string code)
        {
            string result = "";
            if (!string.IsNullOrEmpty(code))
            {
                using(var context = new ERPContext())
                {
                    var wareHouse = context.MdWareHouses.FirstOrDefault(x => x.MaKho == code);
                    if(wareHouse != null)
                    {
                        result = wareHouse.TenKho;
                    }
                }
            }
            return result;
        }

        public static string ConvertUnitAddressByCode(string code)
        {
            string result = "";
            if (!string.IsNullOrEmpty(code))
            {
                using (var context = new ERPContext())
                {
                    var donvis = context.AU_DONVIs.FirstOrDefault(x => x.MaDonVi == code);
                    if (donvis != null)
                    {
                        result = donvis.DiaChi;
                    }
                }
            }
            return result;
        }

        public static string ConvertDieuKienLoc(string code)
        {
            string result = "";
            if (code == "ALL") { result = "Tất cả"; }
            else if (code == "POSITIVE") { result = "Hàng tồn dương"; }
            else if (code == "NEGATIVE") { result = "Hàng tồn âm"; }
            else if (code == "ZERO") { result = "Hàng tồn 0"; }
            return result;
        }
        public static string ConvertLoaiDieuChuyen(string code)
        {
            string result = "";
            if (code == "NHANCHUYENKHO") { result = "Nhận chuyển kho"; }
            else if (code == "NHANSIEUTHITHANHVIEN") { result = "Nhận siêu thị thành viên"; }
            else if (code == "0") { result = "Xuất chuyển kho"; }
            else if (code == "1") { result = "Xuất siêu thị thành viên"; }
            return result;
        }
        public static string ConvertLoaiXuat(string code)
        {
            return code.Substring(8);
        }
        public static string ConvertPhuongThucNhap(string code)
        {
            string result = "";
            if (code == "0") { result = "Nhập mua"; }
            else if (code == "1") { result = "Nhập bán buôn trả lại"; }
            else if (code == "2") { result = "Nhập điều chuyển nội bộ"; }
            else if (code == "3") { result = "Nhập khác"; }
            return result;
        }
        public static string ConvertPhuongThucXuat(string code)
        {
            string result = "";
            if (code == "0") { result = "Xuất bán buôn"; }
            else if (code == "1") { result = "Xuất khác"; }
            else if (code == "2") { result = "Xuất điều chuyển nội bộ"; }
            return result;
        }
        public static string ConvertLoaiNhapKhac(string code)
        {
            string result = "";
            if (code == "N3") { result = "Nhập điều chỉnh"; }
            else if (code == "N2") { result = "Nhập hàng xuất âm"; }
            return result;
        }
        public static string ConvertNhomTheoDieuKien(string code)
        {
            string result = "";
            if (code == "MAKHO") { result = "Mã kho"; }
            else if (code == "MADONVI") { result = "Mã đơn vị"; }
            else if (code == "MALOAIVATTU") { result = "Mã loại vật tư"; }
            else if (code == "MANHOMVATTU") { result = "Mã nhóm vật tư"; }
            else if (code == "MAVATTU") { result = "Mã vật tư"; }
            else if (code == "MAKHACHHANG") { result = "Mã nhà cung cấp"; }
            else if (code == "MANHACUNGCAP") { result = "Mã nhà cung cấp"; }
            else if (code == "MALOAITHUE") { result = "Mã loại thuế"; }
            else if (code == "PHIEU") { result = "Mã phiếu"; }
            else if (code == "NGUOIDUNG") { result = "Mã người tạo phiếu"; }
            else if (code == "MADONVIXUAT") { result = "Mã đơn vị xuất"; }
            return result;
        }
        public static string ConvertNhomTheoDieuKienXuat(string code)
        {
            string result = "";
            if (code == "MAKHO") { result = "Mã kho"; }
            else if (code == "MADONVIXUAT") { result = "Mã đơn vị xuất"; }
            else if (code == "MADONVINHAN") { result = "Mã đơn vị nhận"; }
            else if (code == "MALOAIVATTU") { result = "Mã loại vật tư"; }
            else if (code == "MANHOMVATTU") { result = "Mã nhóm vật tư"; }
            else if (code == "MAVATTU") { result = "Mã vật tư"; }
            else if (code == "MAKHACHHANG") { result = "Mã nhà cung cấp"; }
            else if (code == "MANHACUNGCAP") { result = "Mã nhà cung cấp"; }
            else if (code == "MALOAITHUE") { result = "Mã loại thuế"; }
            else if (code == "MAGIAODICH") { result = "Mã giao dịch"; }
            else if (code == "NGUOIDUNG") { result = "Mã người tạo phiếu"; }
            return result;
        }
    }
}