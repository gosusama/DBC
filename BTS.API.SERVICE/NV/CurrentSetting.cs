using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
namespace BTS.API.SERVICE.NV
{
    public static class CurrentSetting
    {
        public static DateTime GetNgayHachToan() // for accounting
        {
            return DateTime.Now;
        }
        public static string GetUnitName(string unitCode)
        {
            string result = "";
            using (var ctx = new ERPContext())
            {
                var unit = ctx.AU_DONVIs.FirstOrDefault(x => x.MaDonVi == unitCode);
                if (unit != null)
                {
                    result = unit.TenDonVi;
                }
                return result;
            }
        }
        public static string GetUnitAddress(string unitCode)
        {
            string result = "";
            using (var ctx = new ERPContext())
            {
                var unit = ctx.AU_DONVIs.FirstOrDefault(x => x.MaDonVi == unitCode);
                if (unit != null)
                {
                    result = unit.DiaChi;
                }
                return result;
            }
        }
        public static DateTime GetNgayKhoaSo(string unitCode)
        {
            using (var ctx = new ERPContext())
            {
                var periodCloseds = ctx.MdPeriods.OrderByDescending(x => new { x.Year, x.Period }).FirstOrDefault(x => x.TrangThai == 10 && x.UnitCode == unitCode);
                if (periodCloseds != null)
                {
                    MdPeriod lastPeriod = periodCloseds;
                    return lastPeriod.ToDate;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }

        public static MdPeriod GetKhoaSo(string unitCode)
        {
            using (var ctx = new ERPContext())
            {
                var periodCloseds = ctx.MdPeriods.OrderByDescending(x => new { x.Year, x.Period }).FirstOrDefault(x => x.TrangThai == 10 && x.UnitCode == unitCode);
                if (periodCloseds != null)
                {
                    MdPeriod lastPeriod = periodCloseds;
                    return lastPeriod;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string FormatTienViet(string _string)
        {
            string str;
            try
            {
                str = ConvertVNCurencyFormat(decimal.Parse(_string));
            }
            catch
            {
                str = "0";
            }
            return str;
        }
        public static string ConvertVNCurencyFormat(decimal number)
        {
            return number.ToString("C", GetVNeseCultureInfo());
        }
        public static String UnicodetoTCVN222(String strUnicode)
        {
            string strReturn = string.Empty;
            string strTest = "a,à,á,ả,ã,ạ,â,ầ,ấ,ẩ,ẫ,ậ,ă,ằ,ắ,ẳ,ẵ,ặ,e,è,é,ẻ,ẽ,ẹ,ê,ề,ế,ể,ễ,ệ,i,ì,í,ỉ,ĩ,ị,o,ò,ó,ỏ,õ,ọ,ơ,ờ,ớ,ở,ỡ,ợ,ô,ồ,ố,ổ,ỗ,ộ,u,ù,ú,ủ,ũ,ụ,ư,ừ,ứ,ử,ữ,ự,y,ỳ,ý,ỷ,ỹ,ỵ,đ";
            for (int j = 0; j < strUnicode.Length; j++)
            {
                if (strTest.Contains(strUnicode[j].ToString().ToLower()))
                {
                    //convert sang TCVN
                    StringBuilder strB = new StringBuilder(strUnicode[j].ToString().ToLower());
                    StringBuilder strtemp = new StringBuilder(strUnicode[j].ToString().ToLower());
                    #region chuyển mã kí tự unicode thường sang TCVN


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] Unicode_char = {             '\u1EF3','\u00FD','\u1EF7','\u1EF9','\u1EF5',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u01B0','\u1EEB','\u1EE9','\u1EED','\u1EEF','\u1EF1',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00F2','\u00F3','\u1ECF','\u00F5','\u1ECD',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u01A1','\u1EDD','\u1EDB','\u1EDF','\u1EE1','\u1EE3',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00F4','\u1ED3','\u1ED1','\u1ED5','\u1ED7','\u1ED9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00EC','\u00ED','\u1EC9','\u0129','\u1ECB',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00EA','\u1EC1','\u1EBF','\u1EC3','\u1EC5','\u1EC7',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00E8','\u00E9','\u1EBB','\u1EBD','\u1EB9',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u0103','\u1EB1','\u1EAF','\u1EB3','\u1EB5','\u1EB7',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00E0','\u00E1','\u1EA3','\u00E3','\u1EA1',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00E2','\u1EA7','\u1EA5','\u1EA9','\u1EAB','\u1EAD',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00F9','\u00FA','\u1EE7','\u0169','\u1EE5',
                //                            đ
                                            '\u0111'};


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] TCVN_char = {                '\u00FA', '\u00FD','\u00FB','\u00FC','\u00FE',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u00AD','\u00F5','\u00F8','\u00F6','\u00F7','\u00F9',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00DF','\u00E3','\u00E1','\u00E2','\u00E4',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u00AC','\u00EA','\u00ED','\u00EB','\u00EC','\u00EE',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00AB','\u00E5','\u00E8','\u00E6','\u00E7','\u00E9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00D7','\u00DD','\u00D8','\u00DC','\u00DE',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00AA','\u00D2','\u00D5','\u00D3','\u00D4','\u00D6',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00CC','\u00D0','\u00CE','\u00CF','\u00D1',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u00A8','\u00BB','\u00BE','\u00BC','\u00BD','\u00C6',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00B5','\u00B8','\u00B6','\u00B7','\u00B9',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00A9','\u00C7','\u00CA','\u00C8','\u00C9','\u00CB',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00EF','\u00F3','\u00F1','\u00F2','\u00F4',
                //                            đ
                                            '\u00AE'};

                    for (int i = 0; i < Unicode_char.Length; i++)
                    {
                        char a = Unicode_char[i];
                        char b = TCVN_char[i];
                        strB.Replace(a, b);
                        if (strtemp.ToString() != strB.ToString())
                        {
                            break;
                        }
                    }
                    strReturn = strReturn + strB.ToString();
                    #endregion
                }
                else
                {
                    //ko convert
                    strReturn = strReturn + strUnicode[j].ToString();
                }

            }
            return strReturn;
        }
        public static CultureInfo GetVNeseCultureInfo()
        {
            int[] numArray = new int[] { 3 };
            int[] numArray1 = numArray;
            numArray = new int[1];
            int[] numArray2 = numArray;
            CultureInfo cultureInfo = new CultureInfo("vi-VN", true);
            cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyGroupSeparator = ",";
            cultureInfo.NumberFormat.CurrencyGroupSizes = numArray1;
            cultureInfo.NumberFormat.CurrencySymbol = "";
            cultureInfo.NumberFormat.NumberDecimalDigits = 2;
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberGroupSeparator = ",";
            cultureInfo.NumberFormat.NumberGroupSizes = numArray1;
            cultureInfo.NumberFormat.PercentDecimalDigits = 1;
            cultureInfo.NumberFormat.PercentGroupSizes = numArray2;
            cultureInfo.NumberFormat.PercentDecimalSeparator = ".";
            cultureInfo.NumberFormat.PercentGroupSeparator = ",";
            cultureInfo.NumberFormat.PercentSymbol = "%";
            return cultureInfo;
        }
        public static string UnicodeToASCII(string strUnicode)
        {
            StringBuilder stringBuilder = new StringBuilder(strUnicode);
            string[] strArrays = new string[] { "ỹ", "Ỹ", "ỷ", "Ỷ", "ỵ", "Ỵ", "ỳ", "Ỳ", "ự", "Ự", "ữ", "Ữ", "ử", "Ử", "ừ", "Ừ", "ứ", "Ứ", "ủ", "Ủ", "ụ", "Ụ", "ợ", "Ợ", "ỡ", "Ỡ", "ở", "Ở", "ờ", "Ờ", "ớ", "Ớ", "ộ", "Ộ", "ỗ", "Ỗ", "ổ", "Ổ", "ồ", "Ồ", "ố", "Ố", "ỏ", "Ỏ", "ọ", "Ọ", "ị", "Ị", "ỉ", "Ỉ", "ệ", "Ệ", "ễ", "Ễ", "ể", "Ể", "ề", "Ề", "ế", "Ế", "ẽ", "Ẽ", "ẻ", "Ẻ", "ẹ", "Ẹ", "ặ", "Ặ", "ẵ", "Ẵ", "ẳ", "Ẳ", "ằ", "Ằ", "ắ", "Ắ", "ậ", "Ậ", "ẫ", "Ẫ", "ẩ", "Ẩ", "ầ", "Ầ", "ấ", "Ấ", "ả", "Ả", "ạ", "Ạ", "ư", "Ư", "ơ", "Ơ", "ũ", "Ũ", "ĩ", "Ĩ", "đ", "ă", "Ă", "ý", "ú", "ù", "õ", "ô", "ó", "ò", "í", "ì", "ê", "é", "è", "ã", "â", "á", "à", "Ý", "Ú", "Ù", "Õ", "Ô", "Ó", "Ò", "Đ", "Í", "Ì", "Ê", "É", "È", "Ã", "Â", "Á", "À" };
            string[] strArrays1 = strArrays;
            strArrays = new string[] { "y", "Y", "y", "Y", "y", "Y", "y", "Y", "u", "U", "u", "U", "u", "U", "u", "U", "u", "U", "u", "U", "u", "U", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "o", "O", "i", "I", "i", "I", "e.", "E", "e", "E", "e", "E", "e", "E", "e", "E", "e", "E", "e", "E", "e", "E", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "a", "A", "u", "U", "o", "O", "u", "U", "i", "I", "d", "a", "A", "y", "u", "u", "o", "o", "o", "o", "i", "i", "e", "e", "e", "a", "a", "a", "a", "Y", "U", "U", "O", "O", "O", "O", "D", "I", "I", "E", "E", "E", "A", "A", "A", "A" };
            string[] strArrays2 = strArrays;
            for (int i = 0; i < (int)strArrays2.Length; i++)
            {
                stringBuilder.Replace(strArrays1[i], strArrays2[i]);
            }
            return stringBuilder.ToString();
        }
    }
}
