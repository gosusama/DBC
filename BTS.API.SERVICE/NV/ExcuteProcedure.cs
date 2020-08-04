using BTS.API.ENTITY;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.MD;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Helper;

namespace BTS.API.SERVICE.NV
{
    public enum StateProcessApproval
    {
        NoPeriod,
        Success,
        Failed
    }
    public enum TypeSearchMerchandise
    {
        Barcode = 13,
        MaHang = 7,
        MaCan = 5,
        MaHangCon = 9,
    }
    public enum InventoryGroupBy
    {
        MAKHO,
        MALOAIVATTU,
        MANHOMVATTU,
        MAKHACHHANG,
        MAVATTU,
        PHIEU,
        MAGIAODICH,
        MANHACUNGCAP,
        MALOAITHUE,
        MADONVI,
        MADONVIXUAT,
        MADONVINHAN,
        MAXUATXU
    }
    public enum PHUONGTHUCNHAP
    {
        NHAPMUA = 0,
        NHAPBUONTRALAI = 1,
        NHANDIEUCHUYEN = 2,
        NHAPKHAC = 3
    }
    public static class ProcedureCollection
    {
        public static bool IncreaseVoucher(string tableName, int year, int period, string id)
        {
            bool result = false;
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pId = new OracleParameter("pId", OracleDbType.NVarchar2, id, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTableName", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
                        var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XNT.XNT_TANG_PHIEU(:pTableName, :year, :period, :pId); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pYear, pPeriod, pId);
                        dbContextTransaction.Commit();
                        result = true;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        //Procerdure lấy dữ liệu bán lẻ trên Web
        public static List<MdMerchandiseVm.DtoAndPromotion> GetBalanceCode(ERPContext ctx, string strKy)//, string maDonVi = "")
        {
            List<MdMerchandiseVm.DtoAndPromotion> result = null;
            try
            {
                var strQuery = "";
                decimal SoLuong = 0;
                if (strKy.Length > 9 && strKy.Substring(0, 2).Equals("20"))
                {
                    string param = strKy;
                    string maCan = param.Substring(2, 5);
                    string soGram = strKy.Substring(7, 5);
                    Decimal.TryParse(soGram, out SoLuong);
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE ITEMCODE = '" + maCan + "' ";
                }
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.DtoAndPromotion>(strQuery);
                result = data.ToList();
                result.ForEach(x => x.SoLuong = SoLuong / 10000);
            }
            catch
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }

        //Procerdure lấy dữ liệu Chuong trinh khuyen mai Combo
        public static List<NvKhuyenMaiComboVm.ListCombo> GetKhuyenMaiCombo(string unitCode)
        {
            List<NvKhuyenMaiComboVm.ListCombo> result = null;
            using (var ctx = new ERPContext())
            {
                try
                {
                    string _parentUnitCode = unitCode.Substring(0, 3);
                    var strQuery = "";
                    strQuery = @"SELECT MAVATTU AS MaVatTuLeft, MAHANG_KM_BUY1GET1 AS MaVatTuRight,NVL(GIATRIKHUYENMAI,0) AS GiaTriKhuyenMai,MACHUONGTRINH AS MaChuongTrinh,SOLUONG_KHUYENMAI AS SoLuongKhuyenMai FROM V_CHUONGTRINH_KHUYENMAI WHERE LOAIKHUYENMAI = 4 AND UNITCODE LIKE '" + _parentUnitCode + "%' AND TO_DATE(SYSDATE,'DD-MM-YY') >= TO_DATE(TUNGAY,'DD-MM-YY') AND TO_DATE(SYSDATE,'DD-MM-YY') <= TO_DATE(DENNGAY,'DD-MM-YY')";
                    var data = ctx.Database.SqlQuery<NvKhuyenMaiComboVm.ListCombo>(strQuery);
                    result = data.ToList();
                }
                catch
                {
                    result = null;
                }
                return result;
            }
        }
        //Procerdure lấy dữ liệu Chuong trinh Khuyến mại hàng tặng hàng
        public static List<NvKhuyenMaiBuy1Get1Vm.ListHangTangHang> GetKhuyenMaiHangTangHang(string unitCode)
        {
            List<NvKhuyenMaiBuy1Get1Vm.ListHangTangHang> result = null;
            using (var ctx = new ERPContext())
            {
                try
                {
                    string _parentUnitCode = unitCode.Substring(0, 3);
                    var strQuery = "";
                    strQuery = @"SELECT MAVATTU AS MaVatTu, MACHUONGTRINH AS MaChuongTrinh FROM V_CHUONGTRINH_KHUYENMAI WHERE LOAIKHUYENMAI = 3 AND UNITCODE LIKE '" + _parentUnitCode + "%' AND TO_DATE(SYSDATE,'DD-MM-YY') >= TO_DATE(TUNGAY,'DD-MM-YY') AND TO_DATE(SYSDATE,'DD-MM-YY') <= TO_DATE(DENNGAY,'DD-MM-YY')";
                    var data = ctx.Database.SqlQuery<NvKhuyenMaiBuy1Get1Vm.ListHangTangHang>(strQuery);
                    result = data.ToList();
                }
                catch
                {
                    result = null;
                }
                return result;
            }
        }
        
        //Nguyễn Tuấn Hoàng Anh
        //Procerdure lấy dữ liệu bán lẻ trên Web BÓ hàng
        public static MdMerchandiseVm.DataBoHang GetDataBoHang(ERPContext ctx, string maBo,string unitCode)//, string maDonVi = "")
        {
            MdMerchandiseVm.DataBoHang result = new MdMerchandiseVm.DataBoHang();
            List<MdMerchandiseVm.DataBoHang> resultData = new List<MdMerchandiseVm.DataBoHang>();
            try
            {
                var strQuery = @"SELECT A.MABOHANG AS MaBoHang,A.TENBOHANG AS TenBoHang,B.TONGLE AS ThanhTienBoHang,A.UNITCODE,A.TRANGTHAI 
                                FROM DMBOHANG A INNER JOIN DMBOHANGCHITIET B 
                                ON A.MABOHANG = B.MABOHANG 
                                AND A.MABOHANG = '" + maBo + "' AND A.TRANGTHAI = 10 AND A.UNITCODE = '"+ unitCode + "' GROUP BY A.MABOHANG,A.TENBOHANG,B.TONGLE,A.UNITCODE,A.TRANGTHAI ";
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.DataBoHang>(strQuery);
                resultData = data.ToList();
                if(resultData.Count > 0)
                {
                    result.MaBoHang = resultData[0].MaBoHang;
                    result.TenBoHang = resultData[0].TenBoHang;
                    result.ThanhTienBoHang = resultData[0].ThanhTienBoHang;
                    result.UnitCode = resultData[0].UnitCode;
                    result.TrangThai = resultData[0].TrangThai;
                    var strQueryBoHang = @"SELECT B.ID,B.MAHANG AS MaVatTu,B.TENHANG AS TenVatTu,C.BARCODE,C.DONVITINH,C.MAKEHANG AS MaKeHang,C.MAKHACHHANG AS MaKhachHang,
                                            (SELECT TENKH FROM DMKHACHHANG WHERE DMKHACHHANG.MAKH = C.MAKHACHHANG) AS TenNhaCungCap,C.MALOAIVATTU AS MaLoaiVatTu,C.MANHOMVATTU AS MaNhomVatTu,
                                            C.UNITCODE AS MaDonVi,C.GIABANLEVAT AS GiaBanLe,C.GIABANBUON AS GiaBanBuon,C.MAVATVAO AS MaVatVao,C.MAVATRA AS MaVatRa,C.TYLEVATVAO AS TyLeVatVao,C.TYLEVATRA AS TyLeVatRa,
                                            B.SOLUONG AS SoLuong, B.TONGLE AS DONGIA,(C.GIABANLEVAT*B.SOLUONG - B.DONGIA*B.SOLUONG) AS TienHangKhuyenMai
                                            ,A.UNITCODE,A.TRANGTHAI FROM DMBOHANG A INNER JOIN DMBOHANGCHITIET B ON A.MABOHANG = B.MABOHANG INNER JOIN V_VATTU_GIABAN C ON B.MAHANG = C.MAVATTU
                                            AND A.MABOHANG = '" + result.MaBoHang + "'";
                    

                    var dataBoHangChiTiet = ctx.Database.SqlQuery<MdMerchandiseVm.DtoAndPromotion>(strQueryBoHang);
                    result.ListMaHang = dataBoHangChiTiet.ToList();
                    if (result.ListMaHang.Count == 0) return null;
                }
               
            }
            catch
            {
                throw new Exception("KHÔNG TÌM THẤY THÔNG TIN BÓ HÀNG");
            }
            return result;

        }

        //Procerdure lấy dữ liệu bán lẻ trên Web
        public static IQueryable<MdMerchandiseVm.DtoAndPromotion> GetMerchandiseAndPromotion(ERPContext ctx, string strKy, string _parentUnitCode)
        {
            IQueryable<MdMerchandiseVm.DtoAndPromotion> result = null;
            try
            {
                
                var strQuery = "";
                //if (strKy.Length == (int)TypeSearchMerchandise.MaHang || strKy.Length == (int)TypeSearchMerchandise.MaHangCon)
                if (strKy.Length == (int)TypeSearchMerchandise.Barcode )//|| strKy.Length == (int)TypeSearchMerchandise.MaHangCon)
                {
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE BARCODE LIKE '%;" + strKy + ";%' AND TRANGTHAI = 10";
                }
                else
                {
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE lower(MAVATTU) = lower('" + strKy + "') AND UNITCODE LIKE '" + _parentUnitCode + "%' AND TRANGTHAI = 10";
                    //strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE BARCODE LIKE '%;" + strKy + ";%'";
                }
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.DtoAndPromotion>(strQuery);
                result = data.AsQueryable();

            }
            catch
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }
        //end 
        public static bool UpdateCodeGroup(string id, string maVatTu, string maNhomVatTu)
        {
            bool result = false;
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var p_Id = new OracleParameter("p_Id", OracleDbType.NVarchar2, id, ParameterDirection.Input);
                        var p_MaVatTu = new OracleParameter("p_MaVatTu", OracleDbType.NVarchar2, maVatTu, ParameterDirection.Input);
                        var p_NhomVatTu = new OracleParameter("p_NhomVatTu", OracleDbType.NVarchar2, maNhomVatTu, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.CAPNHAT_MANHOM_HANGHOA(:p_Id, :p_MaVatTu, :p_NhomVatTu); END;";
                        ctx.Database.ExecuteSqlCommand(str, p_Id, p_MaVatTu, p_NhomVatTu);
                        dbContextTransaction.Commit();
                        result = true;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }



        public static bool DecreaseVoucher(string tableName, int year, int period, string id)
        {
            bool result = false;
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pId = new OracleParameter("pId", OracleDbType.NVarchar2, id, ParameterDirection.Input);
                        var pTableName = new OracleParameter("pTableName", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var pYear = new OracleParameter("pYear", OracleDbType.Decimal, year, ParameterDirection.Input);
                        var pPeriod = new OracleParameter("period", OracleDbType.Decimal, period, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XNT.XNT_GIAM_PHIEU(:pTableName, :year, :period, :pId); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pYear, pPeriod, pId);
                        dbContextTransaction.Commit();
                        result = true;
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public static IQueryable<MdMerchandiseVm.Dto> QueryMerchandise(ERPContext ctx, string strKey, string subQuery, string maDonVi = "")
        {
            IQueryable<MdMerchandiseVm.Dto> result = null;
            try
            {
                var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
                var pStrKey = new OracleParameter("pStrKey", OracleDbType.NVarchar2, strKey, ParameterDirection.Input);
                var pSubQuery = new OracleParameter("pSubQuery", OracleDbType.NVarchar2, subQuery, ParameterDirection.Input);
                var pCur = new OracleParameter("pCur", OracleDbType.RefCursor, ParameterDirection.Output);
                var str = "BEGIN TBNETERP.PC_VATTU_SEARCH(:pUnitCode, :pStrKey, :pSubQuery,:pCur); END;";
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(str, pUnitCode, pStrKey, pSubQuery, pCur);
                result = data.AsQueryable();
            }
            catch (Exception)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;
        }
        public static PagedObj<MdMerchandiseVm.Dto> QueryPageMerchandise(ERPContext ctx, PagedObj<MdMerchandiseVm.Dto> page, string strKey, string subQuery, string maDonVi)
        {
            PagedObj<MdMerchandiseVm.Dto> result = page;
            try
            {
                var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
                var pStrKey = new OracleParameter("pStrKey", OracleDbType.NVarchar2, strKey, ParameterDirection.Input);
                var pSubQuery = new OracleParameter("pSubQuery", OracleDbType.NVarchar2, subQuery, ParameterDirection.Input);
                var pPageNumber = new OracleParameter("pPageNumber", OracleDbType.Int32, page.CurrentPage, ParameterDirection.Input);
                var pPageSize = new OracleParameter("pPageSize", OracleDbType.Int32, page.ItemsPerPage, ParameterDirection.Input);
                var pTotal = new OracleParameter("pTotal", OracleDbType.Int32, ParameterDirection.Output);
                var pCur = new OracleParameter("pCur", OracleDbType.RefCursor, ParameterDirection.Output);
                var str = "BEGIN  TBNETERP.PC_VATTU_SEARCH_PAGING(:pUnitCode, :pStrKey, :pSubQuery, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
                var strCount = "BEGIN  TBNETERP.PC_VATTU_COUNT(:pUnitCode, :pStrKey, :pSubQuery, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(str, pUnitCode, pStrKey, pSubQuery, pPageNumber, pPageSize, pTotal, pCur);
                var countItem = ctx.Database.SqlQuery<int>(strCount, pUnitCode, pStrKey, pSubQuery, pPageNumber, pPageSize, pTotal, pCur);
                var totalItem = countItem.ToList().First();
                result.Data = data.ToList();
                result.TotalItems = totalItem;
            }
            catch
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;
        }
        public static List<MdCustomerVm.CustomerDto> QueryCustomer(string maDonVi, string strKey)
        {
            List<MdCustomerVm.CustomerDto> result = new List<MdCustomerVm.CustomerDto>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pMaDonVi = new OracleParameter("P_MADONVI", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
                        var pStrKey = new OracleParameter("STR_KEY", OracleDbType.NVarchar2,strKey, ParameterDirection.Input);
                        var pReturnData = new OracleParameter("RETURN_DATA", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.SEARCH_CUSTOMER(:P_MADONVI,:STR_KEY, :RETURN_DATA); END;";
                        ctx.Database.ExecuteSqlCommand(str, pMaDonVi, pStrKey, pReturnData);
                        OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soDiem, tienNguyenGia, tienSale, tongTien = 0;
                            int quenThe = 0;
                            DateTime ngaysinh, ngayDacBiet, ngayHetHan, ngayCapThe;
                            var isSoDiem = decimal.TryParse(reader["SODIEM"].ToString(), out soDiem);
                            var isTienNguyenGia = decimal.TryParse(reader["TIENNGUYENGIA"].ToString(), out tienNguyenGia);
                            var isTienSale = decimal.TryParse(reader["TIENSALE"].ToString(), out tienSale);
                            var isTongTien = decimal.TryParse(reader["TONGTIEN"].ToString(), out tongTien);
                            var isQuenThe = int.TryParse(reader["QUENTHE"].ToString(), out quenThe);
                            var isNgaySinh = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaysinh);
                            var isNgayDacBiet = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                            var isNgayHetHan = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                            var isNgayCapThe = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                            var item = new MdCustomerVm.CustomerDto()
                            {
                                
                                Id = reader["ID"].ToString(),
                                MaKH = reader["MAKH"].ToString(),
                                TenKH = reader["TENKH"].ToString(),
                                DiaChi = reader["DIACHI"].ToString(),
                                DienThoai = reader["DIENTHOAI"].ToString(),
                                ChungMinhThu = reader["CMTND"].ToString(),
                                MaThe = reader["MATHE"].ToString(),
                                HangKhachHang = reader["HANGKHACHHANG"].ToString(),
                                SoDiem = isSoDiem ? soDiem : 0,
                                TienNguyenGia = isTienNguyenGia ? tienNguyenGia : 0,
                                TienSale = isTienSale ? tienSale : 0,
                                TongTien = isTongTien ? tongTien : 0,
                                QuenThe = isQuenThe ? quenThe : 0,
                                NgaySinh = ngaysinh,
                                NgayDacBiet = ngayDacBiet,
                                NgayHetHan = ngayHetHan,
                                NgayCapThe = ngayCapThe,
                            };
                            result.Add(item);
                        }
                        return result;
                    }
                    catch
                    {
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
        }

        public static List<MdMerchandiseVm.FilterData> QueryFilterMerchandise(string maDonVi, string strKey)
        {
            List<MdMerchandiseVm.FilterData> result = new List<MdMerchandiseVm.FilterData>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pMaDonVi = new OracleParameter("P_MADONVI", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
                        var pStrKey = new OracleParameter("STR_KEY", OracleDbType.NVarchar2, strKey, ParameterDirection.Input);
                        var pReturnData = new OracleParameter("RETURN_DATA", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.SEARCH_MERCHANDISE(:P_MADONVI,:STR_KEY, :RETURN_DATA); END;";
                        ctx.Database.ExecuteSqlCommand(str, pMaDonVi, pStrKey, pReturnData);
                        OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                        while (reader.Read())
                        {
                            var item = new MdMerchandiseVm.FilterData()
                            {
                                MaVatTu = reader["MAVATTU"].ToString(),
                                TenVatTu = reader["TENVATTU"].ToString(),
                            };
                            result.Add(item);
                        }
                        return result;
                    }
                    catch
                    {
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
        }

        public static PagedObj<MdMerchandiseVm.DtoAndPromotion> QueryPageMerchandisePromotion(ERPContext ctx, PagedObj<MdMerchandiseVm.DtoAndPromotion> page, string strKey, string subQuery, string maDonVi)
        {
            PagedObj<MdMerchandiseVm.DtoAndPromotion> result = page;

            try
            {
                var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
                var pStrKey = new OracleParameter("pStrKey", OracleDbType.NVarchar2, strKey, ParameterDirection.Input);
                var pSubQuery = new OracleParameter("pSubQuery", OracleDbType.NVarchar2, subQuery, ParameterDirection.Input);
                var pPageNumber = new OracleParameter("pPageNumber", OracleDbType.Int32, page.CurrentPage, ParameterDirection.Input);
                var pPageSize = new OracleParameter("pPageSize", OracleDbType.Int32, page.ItemsPerPage, ParameterDirection.Input);
                var pTotal = new OracleParameter("pTotal", OracleDbType.Int32, ParameterDirection.Output);
                var pCur = new OracleParameter("pCur", OracleDbType.RefCursor, ParameterDirection.Output);
                var str = "BEGIN  TBNETERP.PC_VATTU_SEARCH_PAGING(:pUnitCode, :pStrKey, :pSubQuery, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
                var strCount = "BEGIN  TBNETERP.PC_VATTU_COUNT(:pUnitCode, :pStrKey, :pSubQuery, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.DtoAndPromotion>(str, pUnitCode, pStrKey, pSubQuery, pPageNumber, pPageSize, pTotal, pCur);
                var countItem = ctx.Database.SqlQuery<int>(strCount, pUnitCode, pStrKey, pSubQuery, pPageNumber, pPageSize, pTotal, pCur);
                var totalItem = countItem.ToList().First();
                result.Data = data.ToList();
                result.TotalItems = totalItem;
            }
            catch (Exception e)
            {

                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;
        }
        //New Version
        //public static PagedObj<MdMerchandiseVm.Dto> QueryPageMerchandise(ERPContext ctx, PagedObj<MdMerchandiseVm.Dto> page, string strKey, string subQuery, string orderStr, string maDonVi)
        //{
        //    PagedObj<MdMerchandiseVm.Dto> result = page;

        //    try
        //    {
        //        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, maDonVi, ParameterDirection.Input);
        //        var pStrKey = new OracleParameter("pStrKey", OracleDbType.NVarchar2, strKey, ParameterDirection.Input);
        //        var pSubQuery = new OracleParameter("pSubQuery", OracleDbType.NVarchar2, subQuery, ParameterDirection.Input);
        //        var pOrderStr = new OracleParameter("pOrderStr", OracleDbType.NVarchar2, orderStr, ParameterDirection.Input);
        //        var pPageNumber = new OracleParameter("pPageNumber", OracleDbType.Int32, page.CurrentPage, ParameterDirection.Input);
        //        var pPageSize = new OracleParameter("pPageSize", OracleDbType.Int32, page.ItemsPerPage, ParameterDirection.Input);
        //        var pTotal = new OracleParameter("pTotal", OracleDbType.Int32, ParameterDirection.Output);
        //        var pCur = new OracleParameter("pCur", OracleDbType.RefCursor, ParameterDirection.Output);
        //        var str = "BEGIN  TBNETERP.PC_VATTU_SEARCH_PAGING(:pUnitCode, :pStrKey, :pSubQuery, :pOrderStr, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
        //        var strCount = "BEGIN  TBNETERP.PC_VATTU_COUNT(:pUnitCode, :pStrKey, :pSubQuery, :pPageNumber, :pPageSize, :pTotal,:pCur); END;";
        //        var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(str, pUnitCode, pStrKey, pSubQuery, pOrderStr, pPageNumber, pPageSize, pTotal, pCur);
        //        var countItem = ctx.Database.SqlQuery<int>(strCount, pUnitCode, pStrKey, pSubQuery, pPageNumber, pPageSize, pTotal, pCur);
        //        var totalItem = countItem.ToList().First();
        //        result.Data = data.ToList();
        //        result.TotalItems = totalItem;
        //    }
        //    catch (Exception e)
        //    {

        //        throw new Exception("Lỗi không thể truy xuất hàng hóa");
        //    }
        //    return result;
        //}
        #region Nhap Hang Mua
        public static List<NvNhapHangMuaVm.ObjectReport> PhieuNhapGroupByMerchandise(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvNhapHangMuaVm.ObjectReport> result = new List<NvNhapHangMuaVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvNhapHangMuaVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvNhapHangMuaVm.ObjectReport> PhieuNhapGroupByMerchandiseType(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvNhapHangMuaVm.ObjectReport> result = new List<NvNhapHangMuaVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEOLOAIHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvNhapHangMuaVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvNhapHangMuaVm.ObjectReport> PhieuNhapGroupByMerchandiseGroup(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvNhapHangMuaVm.ObjectReport> result = new List<NvNhapHangMuaVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEONHOMHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvNhapHangMuaVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvNhapHangMuaVm.ObjectReport> PhieuNhapGroupByNhaCungCap(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvNhapHangMuaVm.ObjectReport> result = new List<NvNhapHangMuaVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.NHAPMUATHEONHACUNGCAP(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvNhapHangMuaVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        #endregion
        #region Dieu Chuyen Xuat
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenGroupByMerchandise(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENTHEOHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenGroupByMerchandiseType(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENTHEOLOAIHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenGroupByMerchandiseGroup(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENTHEONHOMHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenGroupByNhaCungCap(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENTHEONHACUNGCAP(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        /// <summary>
        /// Procedure Điều chuyển xuất Tổng hợp
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReport</returns>
        public static List<NvGiaoDichQuayVm.ObjectReport> DCXTongHop(string ky, string groupBy, string pThucXuat, string unitUserCodes, string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_DCX_TONGHOP(:pKy, :pGroupBy, :pPThucXuat, :pUnitUserCode, :pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode, :pCustomerCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy, pPThucXuat, pUnitUserCode,pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode,pCustomerCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                MaDonVi = reader["MADONVIXUAT"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        /// <summary>
        /// Procedure Điều chuyển xuất Chi tiết
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReportCha</returns>
        public static List<NvGiaoDichQuayVm.ObjectReportCha> DCXChiTiet(string ky, string groupBy, string pThucXuat, string unitUserCodes, string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_DCX_CHITIET(:pKy, :pGroupBy, :pPThucXuat,:pUnitUserCode,:pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pCustomerCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy, pPThucXuat, pUnitUserCode, pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pCustomerCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            DateTime ngayGiaoDich;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NgayGiaoDich"].ToString(), out ngayGiaoDich);
                            var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayGiaoDich = isNgayGiaoDich ? ngayGiaoDich : new DateTime(0001, 01, 01),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }

        #endregion
        #region Dieu Chuyen Nhan
        //Dieu Chuyen Nhan
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenNhanGroupByMerchandise(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENNHANTHEOHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenNhanGroupByMerchandiseType(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENNHANTHEOLOAIHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenNhanGroupByMerchandiseGroup(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENNHANTHEONHOMHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }

                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> PhieuDieuChuyenNhanGroupByNhaCungCap(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvPhieuDieuChuyenNoiBoVm.ObjectReport> result = new List<NvPhieuDieuChuyenNoiBoVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DIEUCHUYENNHANTHEONHACUNGCAP(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var item = new NvPhieuDieuChuyenNoiBoVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienHang = isTienHang ? tienHang : 0,
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }
        }
        /// <summary>
        /// Procedure Điều chuyển nhận Tổng hợp
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReport</returns>
        public static List<NvGiaoDichQuayVm.ObjectReport> DCNTongHop(string ky, string groupBy, string pThucXuat, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.BAOCAO_DCN_TONGHOP(:pKy, :pGroupBy, :pPThucXuat, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy, pPThucXuat, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        /// <summary>
        /// Procedure Điều chuyển nhận Chi tiết
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReportCha</returns>
        public static List<NvGiaoDichQuayVm.ObjectReportCha> DCNChiTiet(string ky, string groupBy, string pThucXuat, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.BAOCAO_DCN_CHITIET(:pKy, :pGroupBy, :pPThucXuat, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy, pPThucXuat, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            DateTime ngayGiaoDich;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NgayGiaoDich"].ToString(), out ngayGiaoDich);
                            var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayGiaoDich = isNgayGiaoDich ? ngayGiaoDich : new DateTime(0001, 01, 01),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }

        #endregion
        #region Nhập khác
        /// <summary>
        /// Procedure Nhập khác Tổng hợp
        /// </summary>
        /// <param name="loaiLyDo">Mã lý do</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReport</returns>
        public static List<NvGiaoDichQuayVm.ObjectReport> NKhacTongHop(string loaiLyDo, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pLoaiLyDo = new OracleParameter("pLoaiLyDo", OracleDbType.NVarchar2, loaiLyDo, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.BAOCAO_NKHAC_TONGHOP(:pLoaiLyDo, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pLoaiLyDo, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        /// <summary>
        /// Procedure Nhập khác Chi tiết
        /// </summary>
        /// <param name="loaiLyDo">Mã lý do</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReportCha</returns>
        public static List<NvGiaoDichQuayVm.ObjectReportCha> NKhacChiTiet(string loaiLyDo, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pLoaiLyDo = new OracleParameter("pLoaiLyDo", OracleDbType.NVarchar2, loaiLyDo, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.BAOCAO_NKHAC_CHITIET(:pLoaiLyDo, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pLoaiLyDo, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            DateTime ngayGiaoDich;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NgayGiaoDich"].ToString(), out ngayGiaoDich);
                            var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayGiaoDich = isNgayGiaoDich ? ngayGiaoDich : new DateTime(0001, 01, 01),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }

        #endregion
        #region Xuất Bán
        public static List<NvXuatBanVm.ObjectReport> PhieuXuatBanGroupByMerchandise(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvXuatBanVm.ObjectReport> result = new List<NvXuatBanVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.XUATBANTHEOHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvXuatBanVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvXuatBanVm.ObjectReport> PhieuXuatBanGroupByMerchandiseType(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvXuatBanVm.ObjectReport> result = new List<NvXuatBanVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.XUATBANTHEOLOAIHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvXuatBanVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvXuatBanVm.ObjectReport> PhieuXuatBanGroupByMerchandiseGroup(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvXuatBanVm.ObjectReport> result = new List<NvXuatBanVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.XUATBANTHEONHOMHANGHOA(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvXuatBanVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvXuatBanVm.ObjectReport> PhieuXuatBanGroupByNhaCungCap(ERPContext ctx1, string unitCode, DateTime tuNgay, DateTime denNgay)
        {
            List<NvXuatBanVm.ObjectReport> result = new List<NvXuatBanVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {

                    try
                    {
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, tuNgay.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, denNgay.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.XUATBANTHEONHACUNGCAP(:pUnitCode, :pTuNgay, :pDenNgay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pUnitCode, pTuNgay, pDenNgay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuong, tienHang, tienVat, tongTien, tienChietKhau;
                            var isSoLuong = decimal.TryParse(reader["SoLuong"].ToString(), out soLuong);
                            var isTienHang = decimal.TryParse(reader["TienHang"].ToString(), out tienHang);
                            var isTienVat = decimal.TryParse(reader["TienVat"].ToString(), out tienVat);
                            var isTienChietKhau = decimal.TryParse(reader["TienChietKhau"].ToString(), out tienChietKhau);
                            var isTongTien = decimal.TryParse(reader["TongTien"].ToString(), out tongTien);
                            var item = new NvXuatBanVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                SoLuong = isSoLuong ? soLuong : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                TienVat = isTienVat ? tienVat : 0,
                                TienHang = isTienHang ? tienHang : 0,
                                TongTien = isTongTien ? tongTien : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {

                        throw new Exception("Lỗi không thể truy xuất hàng hóa");
                    }
                }
            }

        }
        public static List<NvGiaoDichQuayVm.ObjectReport> XBTongHop(string ky,string groupBy,string unitUserCodes, string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes, string xuatXuCodes, string unitCode, DateTime fromDate, DateTime toDate,int isPay)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        xuatXuCodes = _convertToArrayCondition(xuatXuCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pXuatXuCode = new OracleParameter("pXuatXuCode", OracleDbType.NVarchar2, xuatXuCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var pIsPay = new OracleParameter("pIsPay", OracleDbType.NVarchar2, isPay.ToString(), ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_XBAN_TONGHOP(:pKy, :pGroupBy,:pUnitUserCode,:pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pCustomerCode, :pXuatXuCode,:pUnitCode, :pFromDate, :pToDate,:pIsPay,:outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy,pUnitUserCode,pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode,pCustomerCode,pXuatXuCode, pUnitCode, pFromDate, pToDate,pIsPay,outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                TienChuyenKhoan = Decimal.Parse(reader["TIENTHE"].ToString()),
                                TienCod = Decimal.Parse(reader["TIENCOD"].ToString()),
                                TienMat = Decimal.Parse(reader["TIENMAT"].ToString()),
                                MaDonVi = reader["MADONVIXUAT"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        public static List<NvGiaoDichQuayVm.ObjectReportCha> XBChiTiet(string ky, string groupBy,string unitUserCodes,string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes,string xuatXuCodes, string unitCode, DateTime fromDate, DateTime toDate, int isPay)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        xuatXuCodes = _convertToArrayCondition(xuatXuCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pXuatXuCode = new OracleParameter("pXuatXuCode", OracleDbType.NVarchar2, xuatXuCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pIsPay = new OracleParameter("pIsPay", OracleDbType.NVarchar2, isPay.ToString(), ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_XBAN_CHITIET(:pKy, :pGroupBy, :pUnitUserCode, :pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode, :pCustomerCode,:pXuatXuCode, :pUnitCode, :pFromDate, :pToDate, :pIsPay, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str,pKy, pGroupBy,pUnitUserCode,pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode,pCustomerCode, pXuatXuCode, pUnitCode, pFromDate, pToDate,pIsPay, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            DateTime ngayGiaoDich;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NgayGiaoDich"].ToString(), out ngayGiaoDich);
                            var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                MaDonVi = reader["MADONVIXUAT"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayGiaoDich = isNgayGiaoDich?ngayGiaoDich:new DateTime(0001,01,01),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => new { x.MaDonVi, x.MaCha }).OrderBy(x=>x.Key.MaDonVi).ToList();
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key.MaCha;
                            model.MaDonVi = x.Key.MaDonVi;
                            var children = resultDetail.Where(i => i.MaCha == x.Key.MaCha && i.MaDonVi == x.Key.MaDonVi).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }
        //public static List<NvGiaoDichQuayVm.ObjectReportCha> XBChiTietGroupByM(string ky, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        //{
        //    List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
        //    List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
        //    using (var ctx = new ERPContext())
        //    {
        //        using (var dbContextTransaction = ctx.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
        //                merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
        //                merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
        //                nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
        //                wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
        //                var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
        //                var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
        //                var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
        //                var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
        //                var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
        //                var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
        //                var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
        //                var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
        //                var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
        //                var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
        //                var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
        //                var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_XBAN_CHITIET_THEOHH(:pKy, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
        //                ctx.Database.ExecuteSqlCommand(str,pKy, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
        //                OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
        //                while (reader.Read())
        //                {
        //                    decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
        //                    DateTime tenCon;
        //                    var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
        //                    var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
        //                    var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
        //                    var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
        //                    var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
        //                    var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
        //                    var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
        //                    var isTenCon = DateTime.TryParse(reader["TENCON"].ToString(), out tenCon);
        //                    var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
        //                    {
        //                        Ma = reader["MaCon"].ToString(),
        //                        Ten = isTenCon ? string.Format("{0}/{1}/{2}", tenCon.Day, tenCon.Month, tenCon.Year) : "",
        //                        MaCha = reader["MaCha"].ToString(),
        //                        TenCha = reader["TenCha"].ToString(),
        //                        SoLuongBan = isSoLuongBan ? soLuongBan : 0,
        //                        Von = isVon ? von : 0,
        //                        TienThue = isTienThue ? tienThue : 0,
        //                        DoanhThu = isdoanhThu ? doanhThu : 0,
        //                        TienBan = isTienBan ? tongBan : 0,
        //                        TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
        //                        LaiBanLe = isLaiBanLe ? laiBanLe : 0

        //                    };
        //                    resultDetail.Add(detailsitem);
        //                }
        //                var temp = resultDetail.GroupBy(x => x.MaCha);
        //                List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
        //                temp.ToList().ForEach(x =>
        //                {
        //                    NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
        //                    model.Ma = x.Key;
        //                    var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
        //                    if (children[0] != null)
        //                    {
        //                        model.Ten = children[0].TenCha;
        //                    }

        //                    model.DataDetails.AddRange(children);
        //                    listCha.Add(model);
        //                });
        //                result.AddRange(listCha);
        //            }
        //            catch (Exception e)
        //            {
        //                dbContextTransaction.Rollback();
        //                throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
        //            }

        //        }
        //    }
        //    return result;
        //}
        #endregion
        #region Xuất Khác
        /// <summary>
        /// Procedure Xuất khác Tổng hợp
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReport</returns>
        public static List<NvGiaoDichQuayVm.ObjectReport> XKTongHop(string ky, string groupBy, string pThucXuat,string unitUserCodes,string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_XKHAC_TONGHOP(:pKy, :pGroupBy, :pPThucXuat,:pUnitUserCode,:pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pCustomerCode,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy,pPThucXuat, pUnitUserCode,pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pCustomerCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                MaDonVi = reader["MADONVIXUAT"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        /// <summary>
        /// Procedure Xuất khác Chi tiết
        /// </summary>
        /// <param name="ky">Table Name - Kỳ hiện tại</param>
        /// <param name="groupBy">Nhóm theo</param>
        /// <param name="pThucXuat">Phương thức xuất</param>
        /// <param name="wareHouseCodes">Mã kho</param>
        /// <param name="merchandiseTypeCodes">Mã loại hàng</param>
        /// <param name="merchandiseGroupCodes">Mã nhóm hàng</param>
        /// <param name="merchandiseCodes">Mã hàng</param>
        /// <param name="nhaCungCapCodes">Mã NCC</param>
        /// <param name="unitCode">Đơn vị</param>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <returns>Danh sách ObjectReportCha</returns>
        public static List<NvGiaoDichQuayVm.ObjectReportCha> XKChiTiet(string ky, string groupBy, string pThucXuat, string unitUserCodes, string taxCodes, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string customerCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        customerCodes = _convertToArrayCondition(customerCodes);
                        unitUserCodes = _convertToArrayCondition(unitUserCodes);
                        taxCodes = _convertToArrayCondition(taxCodes);
                        var pUnitUserCode = new OracleParameter("pUnitUserCode", OracleDbType.NVarchar2, unitUserCodes, ParameterDirection.Input);
                        var pTaxCode = new OracleParameter("pTaxCode", OracleDbType.NVarchar2, taxCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pPThucXuat = new OracleParameter("pPThucXuat", OracleDbType.NVarchar2, pThucXuat, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, customerCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XUATBAN.BAOCAO_XKHAC_CHITIET(:pKy, :pGroupBy, :pPThucXuat, :pUnitUserCode,:pTaxCode, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode, :pCustomerCode,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pGroupBy, pPThucXuat, pUnitUserCode, pTaxCode, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pCustomerCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu;
                            DateTime ngayGiaoDich;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NgayGiaoDich"].ToString(), out ngayGiaoDich);
                            var detailsitem = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayGiaoDich = isNgayGiaoDich ? ngayGiaoDich : new DateTime(0001, 01, 01),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }

        #endregion
        #region Giao Dich Quay
        public static List<NvGiaoDichQuayVm.ObjectReport> GDQTongHop(string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string xuatXuCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        xuatXuCodes = _convertToArrayCondition(xuatXuCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pXuatXuCodes = new OracleParameter("pXuatXu", OracleDbType.NVarchar2, xuatXuCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_QUAYGD_TONGHOP(:pGroupBy,:pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes, :pXuatXuCodes, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes,pXuatXuCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, vonChuaVat, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu, tienChietKhau;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVonChuaVat = decimal.TryParse(reader["VONCHUAVAT"].ToString(), out vonChuaVat);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isTienChietKhau = decimal.TryParse(reader["TIENCHIETKHAU"].ToString(), out tienChietKhau);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                TienVoucher = Decimal.Parse(reader["TIENVOUCHER"].ToString()),
                                TienChuyenKhoan = Decimal.Parse(reader["TIENTHE"].ToString()),
                                TienCod = Decimal.Parse(reader["TIENCOD"].ToString()),
                                TienMat = Decimal.Parse(reader["TIENMAT"].ToString()),
                                MaDonVi = reader["MADONVI"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                VonChuaVat = isVonChuaVat ? vonChuaVat : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        public static List<NvGiaoDichQuayVm.ObjectReportCha> GDQChiTiet(string groupBy,string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes,string xuatXuCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        xuatXuCodes = _convertToArrayCondition(xuatXuCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pXuatXuCodes = new OracleParameter("pXuatXu", OracleDbType.NVarchar2, xuatXuCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_QUAYGD_CHITIET(:pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pXuatXuCodes, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy,pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes,pXuatXuCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            DateTime ngayGiaoDich;
                            decimal soLuongBan, vonChuaVat, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu,tienChietKhau;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVonChuaVat = decimal.TryParse(reader["VONCHUAVAT"].ToString(), out vonChuaVat);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isTienChietKhau = decimal.TryParse(reader["TIENCHIETKHAU"].ToString(), out tienChietKhau);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var isNgayGiaoDich = DateTime.TryParse(reader["NGAYGIAODICH"].ToString(), out ngayGiaoDich);
                            var item = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                TienVoucher = Decimal.Parse(reader["TIENVOUCHER"].ToString()),
                                MaDonVi = reader["MADONVI"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                VonChuaVat = isVonChuaVat ? vonChuaVat : 0,
                                Von = isVon ? von : 0,
                                NgayGiaoDich = isNgayGiaoDich ? ngayGiaoDich : new DateTime(0001, 1, 1),
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            resultDetail.Add(item);
                        }
                        //resultDetail.RemoveAll(x => x.MaCha == null || x.MaDonVi == null);
                        var temp = resultDetail.GroupBy(x => new { x.MaDonVi, x.MaCha}).ToList();
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        for (int j = 0; j < temp.Count; j++)
                        {
                            var x = temp[j];
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key.MaCha;
                            model.MaDonVi = x.Key.MaDonVi;
                            var children = resultDetail.Where(i => i.MaCha == x.Key.MaCha && i.MaDonVi == x.Key.MaDonVi).OrderBy(i => i.Ma).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }
                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        }
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }
        public static List<NvGiaoDichQuayVm.ObjectReport> NhapBanLeTraLaiTongHop(string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReport> result = new List<NvGiaoDichQuayVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_BLETRALAI_TONGHOP(:pGroupBy,:pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, vonChuaVat, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu, tienChietKhau;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVonChuaVat = decimal.TryParse(reader["VONCHUAVAT"].ToString(), out vonChuaVat);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isTienChietKhau = decimal.TryParse(reader["TIENCHIETKHAU"].ToString(), out tienChietKhau);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReport()
                            {
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                TienVoucher = Decimal.Parse(reader["TIENVOUCHER"].ToString()),
                                TienChuyenKhoan = Decimal.Parse(reader["TIENTHE"].ToString()),
                                TienCod = Decimal.Parse(reader["TIENCOD"].ToString()),
                                TienMat = Decimal.Parse(reader["TIENMAT"].ToString()),
                                MaDonVi = reader["MADONVI"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                VonChuaVat = isVonChuaVat ? vonChuaVat : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        public static List<NvGiaoDichQuayVm.ObjectReportCha> NhapBanLeTraLaiChiTiet(string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvGiaoDichQuayVm.ObjectReportCha> result = new List<NvGiaoDichQuayVm.ObjectReportCha>();
            List<NvGiaoDichQuayVm.ObjectReportCon> resultDetail = new List<NvGiaoDichQuayVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCodes = new OracleParameter("pNhaCungCap", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GIAODICHQUAY.BAOCAO_BLETRALAI_CHITIET(:pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCodes,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCodes, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal soLuongBan, vonChuaVat, von, tongBan, tienKhuyenMai, laiBanLe, tienThue, doanhThu,tienChietKhau;
                            var isSoLuongBan = decimal.TryParse(reader["SOLUONGBAN"].ToString(), out soLuongBan);
                            var isVonChuaVat = decimal.TryParse(reader["VONCHUAVAT"].ToString(), out vonChuaVat);
                            var isVon = decimal.TryParse(reader["VON"].ToString(), out von);
                            var isTienThue = decimal.TryParse(reader["TIENTHUE"].ToString(), out tienThue);
                            var isdoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                            var isTienBan = decimal.TryParse(reader["TONGBAN"].ToString(), out tongBan);
                            var isTienKhuyenMai = decimal.TryParse(reader["TIENKHUYENMAI"].ToString(), out tienKhuyenMai);
                            var isTienChietKhau = decimal.TryParse(reader["TIENCHIETKHAU"].ToString(), out tienChietKhau);
                            var isLaiBanLe = decimal.TryParse(reader["LAIBANLE"].ToString(), out laiBanLe);
                            var item = new NvGiaoDichQuayVm.ObjectReportCon()
                            {
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                TienVoucher = Decimal.Parse(reader["TIENVOUCHER"].ToString()),
                                TienChuyenKhoan = Decimal.Parse(reader["TIENTHE"].ToString()),
                                TienCod = Decimal.Parse(reader["TIENCOD"].ToString()),
                                TienMat = Decimal.Parse(reader["TIENMAT"].ToString()),
                                MaDonVi = reader["MADONVI"].ToString(),
                                SoLuongBan = isSoLuongBan ? soLuongBan : 0,
                                VonChuaVat = isVonChuaVat ? vonChuaVat : 0,
                                Von = isVon ? von : 0,
                                TienThue = isTienThue ? tienThue : 0,
                                DoanhThu = isdoanhThu ? doanhThu : 0,
                                TienBan = isTienBan ? tongBan : 0,
                                TienKhuyenMai = isTienKhuyenMai ? tienKhuyenMai : 0,
                                TienChietKhau = isTienChietKhau ? tienChietKhau : 0,
                                LaiBanLe = isLaiBanLe ? laiBanLe : 0
                            };
                            resultDetail.Add(item);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvGiaoDichQuayVm.ObjectReportCha> listCha = new List<NvGiaoDichQuayVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvGiaoDichQuayVm.ObjectReportCha model = new NvGiaoDichQuayVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }
        public static IQueryable<NvGiaoDichQuayVm.ReportExcel> GetReportGDQExcel(DateTime fromDate, DateTime toDate, ERPContext ctx, string maDonVi = "")
        {
            IQueryable<NvGiaoDichQuayVm.ReportExcel> result = null;
            try
            {
                string mFromDate = fromDate.Day + "/" + fromDate.Month + "/" + fromDate.Year;
                string mToDate = toDate.Day + "/" + toDate.Month + "/" + toDate.Year;

                var str = "";
                str = @"SELECT * FROM V_GDQUAY_HANGQUAY WHERE NGAYPHATSINH <= TO_DATE('" + mFromDate + "','DD-MM-YY') AND NGAYPHATSINH >= TO_DATE('" + mToDate + "','DD-MM-YY') AND MADONVI = '" + maDonVi + "'";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<NvGiaoDichQuayVm.ReportExcel>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }

        public static IQueryable<NvGiaoDichQuayVm.ReportExcel> DuLieuGiaoDichQuayTheoDieuKien(DateTime tuNgay, DateTime denNgay, string maDonVi ,ERPContext ctx)
        {
            IQueryable<NvGiaoDichQuayVm.ReportExcel> result = null;
            try
            {
                string mFromDate = tuNgay.Day + "/" + tuNgay.Month + "/" + tuNgay.Year;
                string mToDate = denNgay.Day + "/" + denNgay.Month + "/" + denNgay.Year;

                var str = "";
                str = @"SELECT * FROM V_GDQUAY_HANGQUAY WHERE NGAYPHATSINH >= TO_DATE('" + mFromDate + "','DD-MM-YY') AND NGAYPHATSINH <= TO_DATE('" + mToDate + "','DD-MM-YY') AND MADONVI = '" + maDonVi + "'";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<NvGiaoDichQuayVm.ReportExcel>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }

        public static IQueryable<NvGiaoDichQuayVm.ReportExcel> DuLieuTongHopGiaoDichQuayTheoDieuKien(DateTime tuNgay, DateTime denNgay, string maDonVi, ERPContext ctx)
        {
            IQueryable<NvGiaoDichQuayVm.ReportExcel> result = null;
            try
            {
                string mFromDate = tuNgay.Day + "/" + tuNgay.Month + "/" + tuNgay.Year;
                string mToDate = denNgay.Day + "/" + denNgay.Month + "/" + denNgay.Year;

                var str = "";
                str = @"SELECT a.MAGIAODICH,a.MAGIAODICHQUAYPK,a.LOAIGIAODICH,SUM(a.TTIENCOVAT) AS TTienCoVat FROM V_GDQUAY_HANGQUAY a WHERE a.NGAYPHATSINH >= TO_DATE('" + mFromDate + "','DD-MM-YY') AND a.NGAYPHATSINH <= TO_DATE('" + mToDate + "','DD-MM-YY') AND a.MADONVI = '" + maDonVi + "' GROUP BY a.MAGIAODICH,a.MAGIAODICHQUAYPK,a.LOAIGIAODICH";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<NvGiaoDichQuayVm.ReportExcel>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }
        #endregion
        #region XNT
        public static List<InventoryExpImp> ExportExcelXNTByNCC(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.XNT.BAOCAO_XNT_NGAY_THEONHACC(:pWareHouse, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pWareHouse, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal ClosingQuantity, closingValue, decreaseQuantity, decreaseValue, increaseQuantity, increaseValue, openingBalanceQuantity, openingBalanceValue;
                            var isClosingQ = decimal.TryParse(reader["ClosingQuantity"].ToString(), out ClosingQuantity);
                            var isClosingV = decimal.TryParse(reader["ClosingValue"].ToString(), out closingValue);
                            var isDecreaseQ = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);
                            var isDecreaseV = decimal.TryParse(reader["DecreaseValue"].ToString(), out decreaseValue);
                            var isIncreaseQ = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isIncreaseV = decimal.TryParse(reader["IncreaseValue"].ToString(), out increaseValue);
                            var isOpeningQ = decimal.TryParse(reader["OpeningBalanceQuantity"].ToString(), out openingBalanceQuantity);
                            var isOpeningV = decimal.TryParse(reader["OpeningBalanceValue"].ToString(), out openingBalanceValue);
                            var item = new InventoryExpImp()
                            {
                                ClosingQuantity = isClosingQ ? ClosingQuantity : 0,
                                ClosingValue = isClosingV ? closingValue : 0,
                                DecreaseQuantity = isDecreaseQ ? decreaseQuantity : 0,
                                DecreaseValue = isDecreaseV ? decreaseValue : 0,
                                IncreaseQuantity = isIncreaseQ ? increaseQuantity : 0,
                                IncreaseValue = isIncreaseV ? increaseValue : 0,
                                OpeningBalanceQuantity = isOpeningQ ? openingBalanceQuantity : 0,
                                OpeningBalanceValue = isOpeningV ? openingBalanceValue : 0,
                                Code = reader["Code"].ToString(),
                                Name = reader["Ten"].ToString()
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public static List<InventoryDetailItem> ReportXNTNew_TongHop(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string groupBy, string nhaCungCapCodes)
        {
            List<InventoryDetailItem> result = new List<InventoryDetailItem>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMaLoai = new OracleParameter("pMaLoai", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMaNhom = new OracleParameter("pMaNhom", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMaVatTu = new OracleParameter("pMaVatTu", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pMaKhachHang = new OracleParameter("pMaKhachHang", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var xntCollection = new OracleParameter("xntCollection", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.BAOCAO_XNTNEW_TONGHOP(:pGroupBy,:pWareHouse, :pMaLoai, :pMaNhom,:pMaVatTu,:pMaKhachHang, :pUnitCode, :pTuNgay, :pDenNgay, :xntCollection); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouse, pMaLoai, pMaNhom, pMaVatTu, pMaKhachHang, pUnitCode, pTuNgay, pDenNgay, xntCollection);
                        OracleDataReader reader = ((OracleRefCursor)xntCollection.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal XBANLEQUAY_SL, XBANLEQUAY_GT, XBANLETL_SL, XBANLETL_GT, NMUA_SL, NMUA_GT, NHAPKIEMKE_SL, NHAPKIEMKE_GT, XUATKIEMKE_SL, XUATKIEMKE_GT, NHAPCHUYENKHO_SL, NHAPCHUYENKHO_GT,
                            NHAPSTTHANHVIEN_SL, NHAPSTTHANHVIEN_GT, XUATCHUYENKHO_SL, XBANLE_SL, XBANLE_GT, NHAPDIEUCHINH_SL, NHAPDIEUCHINH_GT, NHAPHANGAM_SL, NHAPHANGAM_GT;
                            decimal XUATHUYHH_SL, XUATHUYHH_GT, XUATHUY_SL, XUATHUY_GT, XUATTRANCC_SL, XUATTRANCC_GT, XUATDC_SL, XUATDC_GT, NHAPBANTL_SL, NHAPBANTL_GT, 
                            TONDAUKY_SL, TONCUOIKY_GT, TONDAUKY_GT, TONCUOIKY_SL, XUATCHUYENKHO_GT, XUATSTTHANHVIEN_SL, XUATSTTHANHVIEN_GT, XBANBUON_SL, XBANBUON_GT;
                            var isXBANLEQUAY_SL = decimal.TryParse(reader["XBANLEQUAY_SL"].ToString(), out XBANLEQUAY_SL);
                            var isXBANLEQUAY_GT = decimal.TryParse(reader["XBANLEQUAY_GT"].ToString(), out XBANLEQUAY_GT);
                            var isXBANLETL_SL = decimal.TryParse(reader["XBANLETL_SL"].ToString(), out XBANLETL_SL);
                            var isXBANLETL_GT = decimal.TryParse(reader["XBANLETL_GT"].ToString(), out XBANLETL_GT);
                            var isNMUA_SL = decimal.TryParse(reader["NMUA_SL"].ToString(), out NMUA_SL);
                            var isNMUA_GT = decimal.TryParse(reader["NMUA_GT"].ToString(), out NMUA_GT);
                            var isNHAPKIEMKE_SL = decimal.TryParse(reader["NHAPKIEMKE_SL"].ToString(), out NHAPKIEMKE_SL);
                            var isNHAPKIEMKE_GT = decimal.TryParse(reader["NHAPKIEMKE_GT"].ToString(), out NHAPKIEMKE_GT);
                            var isXUATKIEMKE_SL = decimal.TryParse(reader["XUATKIEMKE_SL"].ToString(), out XUATKIEMKE_SL);
                            var isXUATKIEMKE_GT = decimal.TryParse(reader["XUATKIEMKE_GT"].ToString(), out XUATKIEMKE_GT);
                            var isNHAPCHUYENKHO_SL = decimal.TryParse(reader["NHAPCHUYENKHO_SL"].ToString(), out NHAPCHUYENKHO_SL);
                            var isNHAPCHUYENKHO_GT = decimal.TryParse(reader["NHAPCHUYENKHO_GT"].ToString(), out NHAPCHUYENKHO_GT);
                            var isNHAPSTTHANHVIEN_SL = decimal.TryParse(reader["NHAPSTTHANHVIEN_SL"].ToString(), out NHAPSTTHANHVIEN_SL);
                            var isNHAPSTTHANHVIEN_GT = decimal.TryParse(reader["NHAPSTTHANHVIEN_GT"].ToString(), out NHAPSTTHANHVIEN_GT);
                            var isXUATCHUYENKHO_SL = decimal.TryParse(reader["XUATCHUYENKHO_SL"].ToString(), out XUATCHUYENKHO_SL);
                            var isXUATCHUYENKHO_GT = decimal.TryParse(reader["XUATCHUYENKHO_GT"].ToString(), out XUATCHUYENKHO_GT);
                            var isXUATSTTHANHVIEN_SL = decimal.TryParse(reader["XUATSTTHANHVIEN_SL"].ToString(), out XUATSTTHANHVIEN_SL);
                            var isXUATSTTHANHVIEN_GT = decimal.TryParse(reader["XUATSTTHANHVIEN_GT"].ToString(), out XUATSTTHANHVIEN_GT);
                            var isXBANLE_SL = decimal.TryParse(reader["XBANLE_SL"].ToString(), out XBANLE_SL);
                            var isXBANLE_GT = decimal.TryParse(reader["XBANLE_GT"].ToString(), out XBANLE_GT);
                            var isXBANBUON_SL = decimal.TryParse(reader["XUATBANBUON_SL"].ToString(), out XBANBUON_SL);
                            var isXBANBUON_GT = decimal.TryParse(reader["XUATBANBUON_GT"].ToString(), out XBANBUON_GT);
                            var isNHAPDIEUCHINH_SL = decimal.TryParse(reader["NHAPDIEUCHINH_SL"].ToString(), out NHAPDIEUCHINH_SL);
                            var isNHAPDIEUCHINH_GT = decimal.TryParse(reader["NHAPDIEUCHINH_GT"].ToString(), out NHAPDIEUCHINH_GT);
                            var isNHAPHANGAM_SL = decimal.TryParse(reader["NHAPHANGAM_SL"].ToString(), out NHAPHANGAM_SL);
                            var isNHAPHANGAM_GT = decimal.TryParse(reader["NHAPHANGAM_GT"].ToString(), out NHAPHANGAM_GT);
                            var isXUATHUYHH_SL = decimal.TryParse(reader["XUATHUYHH_SL"].ToString(), out XUATHUYHH_SL);
                            var isXUATHUYHH_GT = decimal.TryParse(reader["XUATHUYHH_GT"].ToString(), out XUATHUYHH_GT);
                            var isXUATHUY_SL = decimal.TryParse(reader["XUATHUY_SL"].ToString(), out XUATHUY_SL);
                            var isXUATHUY_GT = decimal.TryParse(reader["XUATHUY_GT"].ToString(), out XUATHUY_GT);
                            var isXUATTRANCC_SL = decimal.TryParse(reader["XUATTRANCC_SL"].ToString(), out XUATTRANCC_SL);
                            var isXUATTRANCC_GT = decimal.TryParse(reader["XUATTRANCC_GT"].ToString(), out XUATTRANCC_GT);
                            var isXUATDC_SL = decimal.TryParse(reader["XUATDC_SL"].ToString(), out XUATDC_SL);
                            var isXUATDC_GT = decimal.TryParse(reader["XUATDC_GT"].ToString(), out XUATDC_GT);
                            var isNHAPBANTL_SL = decimal.TryParse(reader["NHAPBANTL_SL"].ToString(), out NHAPBANTL_SL);
                            var isNHAPBANTL_GT = decimal.TryParse(reader["NHAPBANTL_GT"].ToString(), out NHAPBANTL_GT);
                            var isTONDAUKY_SL = decimal.TryParse(reader["TONDAUKY_SL"].ToString(), out TONDAUKY_SL);
                            var isTONDAUKY_GT = decimal.TryParse(reader["TONDAUKY_GT"].ToString(), out TONDAUKY_GT);
                            var isTONCUOIKY_SL = decimal.TryParse(reader["TONCUOIKY_SL"].ToString(), out TONCUOIKY_SL);
                            var isTONCUOIKY_GT = decimal.TryParse(reader["TONCUOIKY_GT"].ToString(), out TONCUOIKY_GT);

                            var item = new InventoryDetailItem() {
                                Ma = reader["MA"].ToString(),
                                Ten = reader["TEN"].ToString(),
                                XBanLeQuay_Sl = isXBANLEQUAY_SL ? XBANLEQUAY_SL : 0,
                                XBanLeQuay_Gt = isXBANLEQUAY_GT ? XBANLEQUAY_GT : 0,
                                XBanLeTL_Sl = isXBANLETL_SL ? XBANLETL_SL : 0,
                                XBanLeTL_Gt = isXBANLETL_GT ? XBANLETL_GT : 0,
                                Nmua_Sl = isNMUA_SL ? NMUA_SL : 0,
                                Nmua_Gt = isNMUA_GT ? NMUA_GT : 0,
                                NhapKiemKe_Sl = isNHAPKIEMKE_SL ? Math.Abs(NHAPKIEMKE_SL) : 0,
                                NhapKiemKe_Gt = isNHAPKIEMKE_GT ? Math.Abs(NHAPKIEMKE_GT) : 0,
                                XuatKiemKe_Sl = isXUATKIEMKE_SL ? XUATKIEMKE_SL : 0,
                                XuatKiemKe_Gt = isXUATKIEMKE_GT ? XUATKIEMKE_GT : 0,
                                NhapChuyenKho_Sl = isNHAPCHUYENKHO_SL ? NHAPCHUYENKHO_SL : 0,
                                NhapChuyenKho_Gt = isNHAPCHUYENKHO_GT ? NHAPCHUYENKHO_GT : 0,
                                NhapSTThanhVien_Sl = isNHAPSTTHANHVIEN_SL ? NHAPSTTHANHVIEN_SL : 0,
                                NhapSTThanhVien_Gt = isNHAPSTTHANHVIEN_GT ? NHAPSTTHANHVIEN_GT : 0,
                                XuatChuyenKho_Sl = isXUATCHUYENKHO_SL ? XUATCHUYENKHO_SL : 0,
                                XuatChuyenKho_Gt = isXUATCHUYENKHO_GT ? XUATCHUYENKHO_GT : 0,
                                XuatSTThanhVien_Sl = isXUATSTTHANHVIEN_SL ? XUATSTTHANHVIEN_SL : 0,
                                XuatSTThanhVien_Gt = isXUATSTTHANHVIEN_GT ? XUATSTTHANHVIEN_GT : 0,
                                XBanLe_Sl = isXBANLE_SL ? XBANLE_SL : 0,
                                XBanLe_Gt = isXBANLE_GT ? XBANLE_GT : 0,
                                NhapDieuChinh_Sl = isNHAPDIEUCHINH_SL ? NHAPDIEUCHINH_SL : 0,
                                NhapDieuChinh_Gt = isNHAPDIEUCHINH_GT ? NHAPDIEUCHINH_GT : 0,
                                NhapHangAm_Sl = isNHAPHANGAM_SL ? NHAPHANGAM_SL : 0,
                                NhapHangAm_Gt = isNHAPHANGAM_GT ? NHAPHANGAM_GT : 0,
                                XuatHuyHH_Sl = isXUATHUYHH_SL ? XUATHUYHH_SL : 0,
                                XuatHuyHH_Gt = isXUATHUYHH_GT ? XUATHUYHH_GT : 0,
                                XuatHuy_Sl = isXUATHUY_SL ? XUATHUY_SL : 0,
                                XuatHuy_Gt = isXUATHUY_GT ? XUATHUY_GT : 0,
                                XuatTraNCC_Sl = isXUATTRANCC_SL ? XUATTRANCC_SL : 0,
                                XuatTraNCC_Gt = isXUATTRANCC_GT ? XUATTRANCC_GT : 0,
                                XuatDC_Sl = isXUATDC_SL ? XUATDC_SL : 0,
                                XuatDC_Gt = isXUATDC_GT ? XUATDC_GT : 0,
                                NhapBanTL_Sl = isNHAPBANTL_SL ? NHAPBANTL_SL : 0,
                                NhapBanTL_Gt = isNHAPBANTL_GT ? NHAPBANTL_GT : 0,
                                TonDauKy_Sl = isTONDAUKY_SL ? TONDAUKY_SL : 0,
                                TonDauKy_Gt = isTONDAUKY_GT ? TONDAUKY_GT : 0,
                                TonCuoiKy_Sl = isTONCUOIKY_SL ? TONCUOIKY_SL : 0,
                                TonCuoiKy_Gt = isTONCUOIKY_GT ? TONCUOIKY_GT : 0,
                                XBanBuon_Sl = isXBANBUON_SL ? XBANBUON_SL : 0,
                                XBanBuon_Gt = isXBANBUON_GT ? XBANBUON_GT : 0,
                            };
                                                     
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        public static List<InventoryDetailItemCha> ReportXNTNew_ChiTiet(DateTime fromDate, DateTime toDate, string unitCode, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string groupBy, string nhaCungCapCodes)
        {
            List<InventoryDetailItemCha> result = new List<InventoryDetailItemCha>();
            List<InventoryDetailItem> lstItem = new List<InventoryDetailItem>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pWareHouse = new OracleParameter("pWareHouse", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMaLoai = new OracleParameter("pMaLoai", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMaNhom = new OracleParameter("pMaNhom", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMaVatTu = new OracleParameter("pMaVatTu", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pMaKhachHang = new OracleParameter("pMaKhachHang", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pTuNgay = new OracleParameter("pTuNgay", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pDenNgay = new OracleParameter("pDenNgay", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var xntCollection = new OracleParameter("xntCollection", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.BAOCAO_XNTNEW_CHITIET(:pGroupBy,:pWareHouse, :pMaLoai, :pMaNhom,:pMaVatTu,:pMaKhachHang, :pUnitCode, :pTuNgay, :pDenNgay, :xntCollection); END;";
                        ctx.Database.ExecuteSqlCommand(str, pGroupBy, pWareHouse, pMaLoai, pMaNhom, pMaVatTu, pMaKhachHang, pUnitCode, pTuNgay, pDenNgay, xntCollection);
                        OracleDataReader reader = ((OracleRefCursor)xntCollection.Value).GetDataReader();
                        
                        while (reader.Read())
                        {
                            decimal XBANLEQUAY_SL, XBANLEQUAY_GT, XBANLETL_SL, XBANLETL_GT, NMUA_SL, NMUA_GT, NHAPKIEMKE_SL, NHAPKIEMKE_GT, XUATKIEMKE_SL, XUATKIEMKE_GT, NHAPCHUYENKHO_SL, NHAPCHUYENKHO_GT,
                            NHAPSTTHANHVIEN_SL, NHAPSTTHANHVIEN_GT, XUATCHUYENKHO_SL, XBANLE_SL, XBANLE_GT, NHAPDIEUCHINH_SL, NHAPDIEUCHINH_GT, NHAPHANGAM_SL, NHAPHANGAM_GT;
                            decimal XUATHUYHH_SL, XUATHUYHH_GT, XUATHUY_SL, XUATHUY_GT, XUATTRANCC_SL, XUATTRANCC_GT, XUATDC_SL, XUATDC_GT, NHAPBANTL_SL, NHAPBANTL_GT,
                            TONDAUKY_SL, TONCUOIKY_GT, TONDAUKY_GT, TONCUOIKY_SL, XUATCHUYENKHO_GT, XUATSTTHANHVIEN_SL, XUATSTTHANHVIEN_GT, XBANBUON_SL, XBANBUON_GT;
                            var isXBANLEQUAY_SL = decimal.TryParse(reader["XBANLEQUAY_SL"].ToString(), out XBANLEQUAY_SL);
                            var isXBANLEQUAY_GT = decimal.TryParse(reader["XBANLEQUAY_GT"].ToString(), out XBANLEQUAY_GT);
                            var isXBANLETL_SL = decimal.TryParse(reader["XBANLETL_SL"].ToString(), out XBANLETL_SL);
                            var isXBANLETL_GT = decimal.TryParse(reader["XBANLETL_GT"].ToString(), out XBANLETL_GT);
                            var isNMUA_SL = decimal.TryParse(reader["NMUA_SL"].ToString(), out NMUA_SL);
                            var isNMUA_GT = decimal.TryParse(reader["NMUA_GT"].ToString(), out NMUA_GT);
                            var isNHAPKIEMKE_SL = decimal.TryParse(reader["NHAPKIEMKE_SL"].ToString(), out NHAPKIEMKE_SL);
                            var isNHAPKIEMKE_GT = decimal.TryParse(reader["NHAPKIEMKE_GT"].ToString(), out NHAPKIEMKE_GT);
                            var isXUATKIEMKE_SL = decimal.TryParse(reader["XUATKIEMKE_SL"].ToString(), out XUATKIEMKE_SL);
                            var isXUATKIEMKE_GT = decimal.TryParse(reader["XUATKIEMKE_GT"].ToString(), out XUATKIEMKE_GT);
                            var isNHAPCHUYENKHO_SL = decimal.TryParse(reader["NHAPCHUYENKHO_SL"].ToString(), out NHAPCHUYENKHO_SL);
                            var isNHAPCHUYENKHO_GT = decimal.TryParse(reader["NHAPCHUYENKHO_GT"].ToString(), out NHAPCHUYENKHO_GT);
                            var isNHAPSTTHANHVIEN_SL = decimal.TryParse(reader["NHAPSTTHANHVIEN_SL"].ToString(), out NHAPSTTHANHVIEN_SL);
                            var isNHAPSTTHANHVIEN_GT = decimal.TryParse(reader["NHAPSTTHANHVIEN_GT"].ToString(), out NHAPSTTHANHVIEN_GT);
                            var isXUATCHUYENKHO_SL = decimal.TryParse(reader["XUATCHUYENKHO_SL"].ToString(), out XUATCHUYENKHO_SL);
                            var isXUATCHUYENKHO_GT = decimal.TryParse(reader["XUATCHUYENKHO_GT"].ToString(), out XUATCHUYENKHO_GT);
                            var isXUATSTTHANHVIEN_SL = decimal.TryParse(reader["XUATSTTHANHVIEN_SL"].ToString(), out XUATSTTHANHVIEN_SL);
                            var isXUATSTTHANHVIEN_GT = decimal.TryParse(reader["XUATSTTHANHVIEN_GT"].ToString(), out XUATSTTHANHVIEN_GT);
                            var isXBANLE_SL = decimal.TryParse(reader["XBANLE_SL"].ToString(), out XBANLE_SL);
                            var isXBANLE_GT = decimal.TryParse(reader["XBANLE_GT"].ToString(), out XBANLE_GT);
                            var isXBANBUON_SL = decimal.TryParse(reader["XUATBANBUON_SL"].ToString(), out XBANBUON_SL);
                            var isXBANBUON_GT = decimal.TryParse(reader["XUATBANBUON_GT"].ToString(), out XBANBUON_GT);
                            var isNHAPDIEUCHINH_SL = decimal.TryParse(reader["NHAPDIEUCHINH_SL"].ToString(), out NHAPDIEUCHINH_SL);
                            var isNHAPDIEUCHINH_GT = decimal.TryParse(reader["NHAPDIEUCHINH_GT"].ToString(), out NHAPDIEUCHINH_GT);
                            var isNHAPHANGAM_SL = decimal.TryParse(reader["NHAPHANGAM_SL"].ToString(), out NHAPHANGAM_SL);
                            var isNHAPHANGAM_GT = decimal.TryParse(reader["NHAPHANGAM_GT"].ToString(), out NHAPHANGAM_GT);
                            var isXUATHUYHH_SL = decimal.TryParse(reader["XUATHUYHH_SL"].ToString(), out XUATHUYHH_SL);
                            var isXUATHUYHH_GT = decimal.TryParse(reader["XUATHUYHH_GT"].ToString(), out XUATHUYHH_GT);
                            var isXUATHUY_SL = decimal.TryParse(reader["XUATHUY_SL"].ToString(), out XUATHUY_SL);
                            var isXUATHUY_GT = decimal.TryParse(reader["XUATHUY_GT"].ToString(), out XUATHUY_GT);
                            var isXUATTRANCC_SL = decimal.TryParse(reader["XUATTRANCC_SL"].ToString(), out XUATTRANCC_SL);
                            var isXUATTRANCC_GT = decimal.TryParse(reader["XUATTRANCC_GT"].ToString(), out XUATTRANCC_GT);
                            var isXUATDC_SL = decimal.TryParse(reader["XUATDC_SL"].ToString(), out XUATDC_SL);
                            var isXUATDC_GT = decimal.TryParse(reader["XUATDC_GT"].ToString(), out XUATDC_GT);
                            var isNHAPBANTL_SL = decimal.TryParse(reader["NHAPBANTL_SL"].ToString(), out NHAPBANTL_SL);
                            var isNHAPBANTL_GT = decimal.TryParse(reader["NHAPBANTL_GT"].ToString(), out NHAPBANTL_GT);
                            var isTONDAUKY_SL = decimal.TryParse(reader["TONDAUKY_SL"].ToString(), out TONDAUKY_SL);
                            var isTONDAUKY_GT = decimal.TryParse(reader["TONDAUKY_GT"].ToString(), out TONDAUKY_GT);
                            var isTONCUOIKY_SL = decimal.TryParse(reader["TONCUOIKY_SL"].ToString(), out TONCUOIKY_SL);
                            var isTONCUOIKY_GT = decimal.TryParse(reader["TONCUOIKY_GT"].ToString(), out TONCUOIKY_GT);

                            var item = new InventoryDetailItem()
                            {
                                Ma = reader["MACON"].ToString(),
                                Ten = reader["TENCON"].ToString(),
                                MaCha = reader["MACHA"].ToString(),
                                TenCha = reader["TENCHA"].ToString(),
                                XBanLeQuay_Sl = isXBANLEQUAY_SL ? XBANLEQUAY_SL : 0,
                                XBanLeQuay_Gt = isXBANLEQUAY_GT ? XBANLEQUAY_GT : 0,
                                XBanLeTL_Sl = isXBANLETL_SL ? XBANLETL_SL : 0,
                                XBanLeTL_Gt = isXBANLETL_GT ? XBANLETL_GT : 0,
                                Nmua_Sl = isNMUA_SL ? NMUA_SL : 0,
                                Nmua_Gt = isNMUA_GT ? NMUA_GT : 0,
                                NhapKiemKe_Sl = isNHAPKIEMKE_SL ? Math.Abs(NHAPKIEMKE_SL) : 0,
                                NhapKiemKe_Gt = isNHAPKIEMKE_GT ? Math.Abs(NHAPKIEMKE_GT) : 0,
                                XuatKiemKe_Sl = isXUATKIEMKE_SL ? XUATKIEMKE_SL : 0,
                                XuatKiemKe_Gt = isXUATKIEMKE_GT ? XUATKIEMKE_GT : 0,
                                NhapChuyenKho_Sl = isNHAPCHUYENKHO_SL ? NHAPCHUYENKHO_SL : 0,
                                NhapChuyenKho_Gt = isNHAPCHUYENKHO_GT ? NHAPCHUYENKHO_GT : 0,
                                NhapSTThanhVien_Sl = isNHAPSTTHANHVIEN_SL ? NHAPSTTHANHVIEN_SL : 0,
                                NhapSTThanhVien_Gt = isNHAPSTTHANHVIEN_GT ? NHAPSTTHANHVIEN_GT : 0,
                                XuatChuyenKho_Sl = isXUATCHUYENKHO_SL ? XUATCHUYENKHO_SL : 0,
                                XuatChuyenKho_Gt = isXUATCHUYENKHO_GT ? XUATCHUYENKHO_GT : 0,
                                XuatSTThanhVien_Sl = isXUATSTTHANHVIEN_SL ? XUATSTTHANHVIEN_SL : 0,
                                XuatSTThanhVien_Gt = isXUATSTTHANHVIEN_GT ? XUATSTTHANHVIEN_GT : 0,
                                XBanLe_Sl = isXBANLE_SL ? XBANLE_SL : 0,
                                XBanLe_Gt = isXBANLE_GT ? XBANLE_GT : 0,
                                NhapDieuChinh_Sl = isNHAPDIEUCHINH_SL ? NHAPDIEUCHINH_SL : 0,
                                NhapDieuChinh_Gt = isNHAPDIEUCHINH_GT ? NHAPDIEUCHINH_GT : 0,
                                NhapHangAm_Sl = isNHAPHANGAM_SL ? NHAPHANGAM_SL : 0,
                                NhapHangAm_Gt = isNHAPHANGAM_GT ? NHAPHANGAM_GT : 0,
                                XuatHuyHH_Sl = isXUATHUYHH_SL ? XUATHUYHH_SL : 0,
                                XuatHuyHH_Gt = isXUATHUYHH_GT ? XUATHUYHH_GT : 0,
                                XuatHuy_Sl = isXUATHUY_SL ? XUATHUY_SL : 0,
                                XuatHuy_Gt = isXUATHUY_GT ? XUATHUY_GT : 0,
                                XuatTraNCC_Sl = isXUATTRANCC_SL ? XUATTRANCC_SL : 0,
                                XuatTraNCC_Gt = isXUATTRANCC_GT ? XUATTRANCC_GT : 0,
                                XuatDC_Sl = isXUATDC_SL ? XUATDC_SL : 0,
                                XuatDC_Gt = isXUATDC_GT ? XUATDC_GT : 0,
                                NhapBanTL_Sl = isNHAPBANTL_SL ? NHAPBANTL_SL : 0,
                                NhapBanTL_Gt = isNHAPBANTL_GT ? NHAPBANTL_GT : 0,
                                TonDauKy_Sl = isTONDAUKY_SL ? TONDAUKY_SL : 0,
                                TonDauKy_Gt = isTONDAUKY_GT ? TONDAUKY_GT : 0,
                                TonCuoiKy_Sl = isTONCUOIKY_SL ? TONCUOIKY_SL : 0,
                                TonCuoiKy_Gt = isTONCUOIKY_GT ? TONCUOIKY_GT : 0,
                                XBanBuon_Sl = isXBANBUON_SL ? XBANBUON_SL : 0,
                                XBanBuon_Gt = isXBANBUON_GT ? XBANBUON_GT : 0,
                            };

                            lstItem.Add(item);
                        }
                        var groupList = lstItem.GroupBy(x => x.MaCha);
                        groupList.ToList().ForEach(x =>
                        {
                            InventoryDetailItemCha itemCha = new InventoryDetailItemCha();
                            itemCha.Ma = x.Key;
                            foreach(InventoryDetailItem y in lstItem)
                            {
                                if (y.MaCha == x.Key)
                                {
                                    itemCha.Ten = y.TenCha;
                                    break;
                                }
                            }
                            itemCha.DataDetails.AddRange(lstItem.FindAll(y => y.MaCha == x.Key));
                            result.Add(itemCha);
                        });
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                }
            }
            return result;
        }
        #endregion
        #region Tồn kho
        public static List<InventoryExpImpLevel2> TonKhoTongHop(string ky, string dkienLoc, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode)
        {
            List<InventoryExpImpLevel2> result = new List<InventoryExpImpLevel2>();
            List<InventoryExpImp> data = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pDKienLoc = new OracleParameter("pDKienLoc", OracleDbType.NVarchar2, dkienLoc, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_TONGHOP(:pKy, :pDKienLoc, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pDKienLoc, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal tonCuoiKySL, tonCuoiKyGT;
                            var isTonCuoiKySL = decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out tonCuoiKySL);
                            var isTonCuoiKyGT = decimal.TryParse(reader["TONCUOIKYGT"].ToString(), out tonCuoiKyGT);
                            var item = new InventoryExpImp()
                            {
                                Code = reader["Ma"].ToString(),
                                Name = reader["Ten"].ToString(),
                                MaDonVi = reader["MaDonVi"].ToString(),
                                ClosingQuantity = isTonCuoiKySL ? tonCuoiKySL : 0,
                                ClosingValue = isTonCuoiKyGT ? tonCuoiKyGT : 0,
                            };
                            data.Add(item);
                        }
                        dbContextTransaction.Commit();
                        if (data.Count > 0)
                        {
                            var group = data.GroupBy(x => x.MaDonVi).ToList();
                            foreach(var item in group)
                            {
                                InventoryExpImpLevel2 obj = new InventoryExpImpLevel2();
                                obj.Ma = item.Key;
                                List<InventoryExpImp> list = data.Where(x => x.MaDonVi == item.Key).ToList();
                                obj.DataDetails.AddRange(list);
                                result.Add(obj);
                            }
                        }

                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        public static List<InventoryExcel> TonKhoChiTiet(string ky, string dkienLoc, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string unitCode)
        {
            List<InventoryExcel> result = new List<InventoryExcel>();
            List<InventoryExcelItem> resultDetail = new List<InventoryExcelItem>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        var pKy = new OracleParameter("pKy", OracleDbType.NVarchar2, ky, ParameterDirection.Input);
                        var pDKienLoc = new OracleParameter("pDKienLoc", OracleDbType.NVarchar2, dkienLoc, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.TONKHO.BAOCAO_TONKHO_CHITIET(:pKy, :pDKienLoc, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pUnitCode, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pKy, pDKienLoc, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pUnitCode, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal tonCuoiKySL, tonCuoiKyGT;
                            var isTonCuoiKySL = decimal.TryParse(reader["TONCUOIKYSL"].ToString(), out tonCuoiKySL);
                            var isTonCuoiKyGT = decimal.TryParse(reader["TONCUOIKYGT"].ToString(), out tonCuoiKyGT);
                            var item = new InventoryExcelItem()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Code = reader["MaCon"].ToString(),
                                TenVatTu = reader["TenCon"].ToString(),
                                CodeParent = reader["MaCha"].ToString(),
                                Name = reader["TenCha"].ToString(),
                                UnitCode = reader["MaDonVi"].ToString(),
                                ClosingQuantity = isTonCuoiKySL ? tonCuoiKySL : 0,
                                ClosingValue = isTonCuoiKyGT ? tonCuoiKyGT : 0,
                            };
                            resultDetail.Add(item);
                        }
                        if (resultDetail.Count > 0)
                        {
                            var temp = resultDetail.GroupBy(x => new { x.CodeParent, x.UnitCode });
                            List<InventoryExcel> listCha = new List<InventoryExcel>();
                            temp.ToList().ForEach(x =>
                            {
                                InventoryExcel model = new InventoryExcel();
                                model.Code = x.Key.CodeParent;
                                model.UnitCode = x.Key.UnitCode;

                                var children = resultDetail.Where(i => i.CodeParent == x.Key.CodeParent && i.UnitCode == x.Key.UnitCode).ToList();
                                if (children[0] != null)
                                {
                                    model.Name = children[0].Name;
                                }

                                model.DetailData.AddRange(children);
                                listCha.Add(model);
                            });
                            result.AddRange(listCha);
                        }
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }
        #endregion  
        #region Kiểm kê
        public static List<NvKiemKeVm.ObjectReport> KKTongHop(string dieuKienLoc, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string keHangCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvKiemKeVm.ObjectReport> result = new List<NvKiemKeVm.ObjectReport>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        keHangCodes = _convertToArrayCondition(keHangCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pDieuKienLoc = new OracleParameter("pDieuKienLoc", OracleDbType.NVarchar2, dieuKienLoc, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pKeHangCode = new OracleParameter("pKeHangCode", OracleDbType.NVarchar2, keHangCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.KIEMKE.BAOCAO_KIEMKE_TONGHOP(:pDieuKienLoc, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pKeHangCode,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pDieuKienLoc, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pKeHangCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal giaVon, soLuongTonMay, soluongKiemKe, soLuongThua, giaTriThua, soLuongThieu, giaTriThieu;
                            var isGiaVon = decimal.TryParse(reader["GIAVON"].ToString(), out giaVon);
                            var isSoLuongTonMay = decimal.TryParse(reader["SOLUONGTONMAY"].ToString(), out soLuongTonMay);
                            var isSoLuongKiemKe = decimal.TryParse(reader["SOLUONGKIEMKE"].ToString(), out soluongKiemKe);
                            var isSoLuongThua = decimal.TryParse(reader["SOLUONGTHUA"].ToString(), out soLuongThua);
                            var isGiaTriThua = decimal.TryParse(reader["GIATRITHUA"].ToString(), out giaTriThua);
                            var isSoLuongThieu = decimal.TryParse(reader["SOLUONGTHIEU"].ToString(), out soLuongThieu);
                            var isGiaTriThieu = decimal.TryParse(reader["GIATRITHIEU"].ToString(), out giaTriThieu);
                            var item = new NvKiemKeVm.ObjectReport()
                            {
                                
                                Ma = reader["Ma"].ToString(),
                                Ten = reader["Ten"].ToString(),
                                GiaVon = isGiaVon ? giaVon : 0,
                                SoLuongTonMay = isSoLuongTonMay? soLuongTonMay:0,
                                SoLuongKiemKe = isSoLuongKiemKe? soluongKiemKe:0,
                                SoLuongThua = isSoLuongThua?soLuongThua:0,
                                GiaTriThua = isGiaTriThua?giaTriThua:0,
                                SoLuongThieu = isSoLuongThieu?soLuongThieu:0,
                                GiaTriThieu = isGiaTriThieu?giaTriThieu:0
                            };
                            result.Add(item);
                        }
                        dbContextTransaction.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
        }
        public static List<NvKiemKeVm.ObjectReportCha> KKChiTiet(string dieuKienLoc, string groupBy, string wareHouseCodes, string merchandiseTypeCodes, string merchandiseGroupCodes, string merchandiseCodes, string nhaCungCapCodes, string keHangCodes, string unitCode, DateTime fromDate, DateTime toDate)
        {
            List<NvKiemKeVm.ObjectReportCha> result = new List<NvKiemKeVm.ObjectReportCha>();
            List<NvKiemKeVm.ObjectReportCon> resultDetail = new List<NvKiemKeVm.ObjectReportCon>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        wareHouseCodes = _convertToArrayCondition(wareHouseCodes);
                        keHangCodes = _convertToArrayCondition(keHangCodes);
                        var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, wareHouseCodes, ParameterDirection.Input);
                        var pDieuKienLoc = new OracleParameter("pDieuKienLoc", OracleDbType.NVarchar2, dieuKienLoc, ParameterDirection.Input);
                        var pGroupBy = new OracleParameter("pGroupBy", OracleDbType.NVarchar2, groupBy, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pMerchandiseCode = new OracleParameter("pMerchandiseCode", OracleDbType.NVarchar2, merchandiseCodes, ParameterDirection.Input);
                        var pNhaCungCapCode = new OracleParameter("pNhaCungCapCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pKeHangCode = new OracleParameter("pKeHangCode", OracleDbType.NVarchar2, keHangCodes, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.KIEMKE.BAOCAO_KIEMKE_CHITIET(:pDieuKienLoc, :pGroupBy, :pWareHouseCode, :pMerchandiseTypeCode, :pMerchandiseGroupCode,:pMerchandiseCode, :pNhaCungCapCode,:pKeHangCode,:pUnitCode, :pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str, pDieuKienLoc, pGroupBy, pWareHouseCode, pMerchandiseTypeCode, pMerchandiseGroupCode, pMerchandiseCode, pNhaCungCapCode, pKeHangCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal giaVon, soLuongTonMay, soluongKiemKe, soLuongThua, giaTriThua, soLuongThieu, giaTriThieu;
                            DateTime ngayKiemKe;
                            var isGiaVon = decimal.TryParse(reader["GIAVON"].ToString(), out giaVon);
                            var isSoLuongTonMay = decimal.TryParse(reader["SOLUONGTONMAY"].ToString(), out soLuongTonMay);
                            var isSoLuongKiemKe = decimal.TryParse(reader["SOLUONGKIEMKE"].ToString(), out soluongKiemKe);
                            var isSoLuongThua = decimal.TryParse(reader["SOLUONGTHUA"].ToString(), out soLuongThua);
                            var isGiaTriThua = decimal.TryParse(reader["GIATRITHUA"].ToString(), out giaTriThua);
                            var isSoLuongThieu = decimal.TryParse(reader["SOLUONGTHIEU"].ToString(), out soLuongThieu);
                            var isGiaTriThieu = decimal.TryParse(reader["GIATRITHIEU"].ToString(), out giaTriThieu);
                            var isNgayKiemke = DateTime.TryParse(reader["NGAYKIEMKE"].ToString(), out ngayKiemKe);
                            var detailsitem = new NvKiemKeVm.ObjectReportCon()
                            {
                                Barcode = reader["BARCODE"].ToString(),
                                Ma = reader["MaCon"].ToString(),
                                Ten = reader["TenCon"].ToString(),
                                MaCha = reader["MaCha"].ToString(),
                                TenCha = reader["TenCha"].ToString(),
                                NgayKiemKe = isNgayKiemke ? ngayKiemKe : new DateTime(0001, 01, 01),
                                GiaVon = isGiaVon ? giaVon : 0,
                                SoLuongTonMay = isSoLuongTonMay ? soLuongTonMay : 0,
                                SoLuongKiemKe = isSoLuongKiemKe ? soluongKiemKe : 0,
                                SoLuongThua = isSoLuongThua ? soLuongThua : 0,
                                GiaTriThua = isGiaTriThua ? giaTriThua : 0,
                                SoLuongThieu = isSoLuongThieu ? soLuongThieu : 0,
                                GiaTriThieu = isGiaTriThieu ? giaTriThieu : 0

                            };
                            resultDetail.Add(detailsitem);
                        }
                        var temp = resultDetail.GroupBy(x => x.MaCha);
                        List<NvKiemKeVm.ObjectReportCha> listCha = new List<NvKiemKeVm.ObjectReportCha>();
                        temp.ToList().ForEach(x =>
                        {
                            NvKiemKeVm.ObjectReportCha model = new NvKiemKeVm.ObjectReportCha();
                            model.Ma = x.Key;
                            var children = resultDetail.Where(i => i.MaCha == x.Key).ToList();
                            if (children[0] != null)
                            {
                                model.Ten = children[0].TenCha;
                            }

                            model.DataDetails.AddRange(children);
                            listCha.Add(model);
                        });
                        result.AddRange(listCha);
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }

                }
            }
            return result;
        }

        #endregion

        public static IQueryable<MdMerchandiseVm.Dto> GetMerchandise(ERPContext ctx, string strKy, string maDonVi = "")
        {
            IQueryable<MdMerchandiseVm.Dto> resultz = null;
            try
            {
                var strQuery = "";
                if (strKy.Length == (int)TypeSearchMerchandise.MaHang || strKy.Length == (int)TypeSearchMerchandise.MaHangCon)
                {
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE lower(MAHANG) = lower('" + strKy + "') AND MADONVI = '" + maDonVi + "'";
                }
                else if (strKy.Length == (int)TypeSearchMerchandise.MaCan)
                {
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE ITEMCODE = '"+ strKy + "' AND MADONVI = '" + maDonVi + "'";
                }
                else
                {
                    strQuery = @"SELECT * FROM V_VATTU_GIABAN WHERE BARCODE LIKE '%;" + strKy + ";%' AND MADONVI = '" + maDonVi + "'";
                }
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(strQuery);
                resultz = data.AsQueryable();
            }
            catch (Exception ez)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return resultz;

        }

        public static IQueryable<MdMerchandiseVm.Dto> GetMerchandiseByType(ERPContext ctx, string lstType, string unitCode = "")
        {
            IQueryable<MdMerchandiseVm.Dto> resultz = null;
            try
            {
                lstType = _convertToArrayCondition(lstType);
                var queryStr = string.Format(@"SELECT a.MAVATTU,TENVATTU,a.BARCODE,a.MAKEHANG,a.MALOAIVATTU,
                                            a.MANHOMVATTU, a.MAKHACHHANG, A.GIAMUA, a.GIAMUAVAT, a.GIABANLE, A.GIABANLEVAT, a.GIABANBUON,
                                            A.GIABANBUONVAT, A.TYLEVATRA, A.TYLEVATVAO, B.TENDVT AS DONVITINH
                                            FROM V_VATTU_GIABAN a LEFT JOIN DM_DONVITINH b ON a.DONVITINH = B.MADVT
                                            WHERE a.MALOAIVATTU IN ({0})  AND a.UNITCODE = '{1}'", lstType, unitCode);
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(queryStr);
                resultz = data.AsQueryable();
            }
            catch (Exception ez)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return resultz;

        }
        public static IQueryable<MdMerchandiseTypeVm.Dto> GetMerchandiseType(ERPContext ctx, string strKey, string maDonVi = "")
        {
            IQueryable<MdMerchandiseTypeVm.Dto> result = null;
            try
            {
                var str = @"SELECT ID, MALOAIVATTU,TENLOAIVT AS TENLOAIVATTU, TRANGTHAI FROM DM_LOAIVATTU WHERE lower(MALOAIVATTU) = lower('" + strKey + "') AND UNITCODE = '" + maDonVi + "'";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<MdMerchandiseTypeVm.Dto>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {

                throw new Exception("Lỗi không thể truy xuất loại hàng hóa");
            }
            return result;
        }
        public static IQueryable<MdNhomVatTuVm.Dto> GetNhomVatTu(ERPContext ctx, string strKey, string maDonVi = "")
        {
            IQueryable<MdNhomVatTuVm.Dto> result = null;
            try
            {
                var str = @"SELECT ID, MALOAIVATTU,MANHOMVTU AS MANHOM,TENNHOMVT AS TENNHOM, TRANGTHAI FROM DM_NHOMVATTU WHERE lower(MANHOMVTU) = lower('" + strKey + "') AND UNITCODE = '" + maDonVi + "'";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<MdNhomVatTuVm.Dto>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {

                throw new Exception("Lỗi không thể truy xuất nhóm hàng hóa");
            }
            return result;
        }
        public static IQueryable<MdMerchandiseVm.Dto> GetMerchandiseItemCode(ERPContext ctx, string maDonVi = "")
        {
            IQueryable<MdMerchandiseVm.Dto> result = null;
            try
            {
                var str = "";
                str = @"SELECT * FROM V_VATTU_GIABAN_ItemCode WHERE MADONVI = '" + maDonVi + "'";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<MdMerchandiseVm.Dto>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }
        
        public static List<NvPhieuDatHangVm.DtoDetail> Get_Data_DatHang_NhaCungCap(string nhaCungCapCodes, string merchandiseTypeCodes, string merchandiseGroupCodes,string wareHouseCode, DateTime fromDate, DateTime toDate, DateTime ngayChungTu, string unitCode, MdMerchandiseVm.FilterQuantity filterQuantity)
        {
            List<NvPhieuDatHangVm.DtoDetail> result = new List<NvPhieuDatHangVm.DtoDetail>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var P_MANHACUNGCAP = new OracleParameter("P_MANHACUNGCAP", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var P_MALOAIVATTU = new OracleParameter("P_MALOAIVATTU", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var P_MANHOMVATTU = new OracleParameter("P_MANHOMVATTU", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var P_WAREHOUSE = new OracleParameter("P_WAREHOUSE", OracleDbType.NVarchar2, wareHouseCode, ParameterDirection.Input);
                        var P_CONDITION = new OracleParameter("P_CONDITION", OracleDbType.NVarchar2, filterQuantity.Operator, ParameterDirection.Input);
                        var P_SOLUONGTON = new OracleParameter("P_SOLUONGTON", OracleDbType.NVarchar2, filterQuantity.Value, ParameterDirection.Input);
                        var P_TUNGAY = new OracleParameter("P_TUNGAY", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var P_DENNGAY = new OracleParameter("P_DENNGAY", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var P_NGAYCHUNGTU = new OracleParameter("P_NGAYCHUNGTU", OracleDbType.Date, ngayChungTu.Date, ParameterDirection.Input);
                        var P_UNITCODE = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);

                        var CURSOR_RESULT = new OracleParameter("CURSOR_RESULT", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.GET_DATA_DATHANG_NHACUNGCAP(:P_MANHACUNGCAP,:P_MALOAIVATTU,:P_MANHOMVATTU,:P_WAREHOUSE,:P_CONDITION,:P_SOLUONGTON,:P_TUNGAY,:P_DENNGAY,:P_NGAYCHUNGTU,:P_UNITCODE,:CURSOR_RESULT); END;";
                        ctx.Database.ExecuteSqlCommand(str, P_MANHACUNGCAP, P_MALOAIVATTU, P_MANHOMVATTU, P_WAREHOUSE, P_CONDITION, P_SOLUONGTON, P_TUNGAY, P_DENNGAY, P_NGAYCHUNGTU, P_UNITCODE, CURSOR_RESULT);
                        OracleDataReader reader = ((OracleRefCursor)CURSOR_RESULT.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal TYLE_VAT_RA = 0;
                            decimal TYLE_VAT_VAO = 0;
                            decimal GIABANLE = 0;
                            decimal GIA_BANLE_VAT = 0;
                            decimal NHAPSL = 0;
                            decimal XUATSL = 0;
                            decimal TONCUOIKYSL = 0;
                            decimal GIAMUA = 0;
                            decimal.TryParse(reader["TYLE_VAT_VAO"] != null ? reader["TYLE_VAT_VAO"].ToString() : "", out TYLE_VAT_VAO);
                            decimal.TryParse(reader["TYLE_VAT_RA"] != null ? reader["TYLE_VAT_RA"].ToString() : "", out TYLE_VAT_RA);
                            decimal.TryParse(reader["GIAMUA"] != null ? reader["GIAMUA"].ToString() : "", out GIAMUA);
                            decimal.TryParse(reader["GIABANLE"] != null ? reader["GIABANLE"].ToString() : "", out GIABANLE);
                            decimal.TryParse(reader["GIA_BANLE_VAT"] != null ? reader["GIA_BANLE_VAT"].ToString() : "", out GIA_BANLE_VAT);
                            decimal.TryParse(reader["NHAPSL"] != null ? reader["NHAPSL"].ToString() : "", out NHAPSL);
                            decimal.TryParse(reader["XUATSL"] != null ? reader["XUATSL"].ToString() : "", out XUATSL);
                            decimal.TryParse(reader["TONCUOIKYSL"] != null ? reader["TONCUOIKYSL"].ToString() : "", out TONCUOIKYSL);
                            var item = new NvPhieuDatHangVm.DtoDetail()
                            {
                                MaHang = reader["MAVATTU"] != null ? reader["MAVATTU"].ToString() : "",
                                TenHang = reader["TENVATTU"] != null ? reader["TENVATTU"].ToString() : "",
                                MaBaoBi = reader["MABAOBI"] != null ? reader["MABAOBI"].ToString() : "",
                                Barcode = reader["BARCODE"] != null ? reader["BARCODE"].ToString() : "",
                                DonViTinh = reader["DONVITINH"] != null ? reader["DONVITINH"].ToString() : "",
                                TyLeVatVao = TYLE_VAT_VAO,
                                TyLeVatRa = TYLE_VAT_RA,
                                DonGia = GIAMUA,
                                SoLuongNhapTrongKy = NHAPSL,
                                SoLuongXuatTrongKy = XUATSL,
                                GiaBanLeVat = GIA_BANLE_VAT,
                                GiaBanLeChuaVat = GIABANLE,
                                SoLuongTon = TONCUOIKYSL
                            };
                            result.Add(item);
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                    return result;
                }
            }

        }
        public static List<InventoryExpImp> GetMerchandiseInventoryForDatHang(DateTime fromDate, DateTime toDate, string unitCode, string merchandiseTypeCodes, string merchandiseGroupCodes, string nhaCungCapCodes)
        {
            List<InventoryExpImp> result = new List<InventoryExpImp>();
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        nhaCungCapCodes = _convertToArrayCondition(nhaCungCapCodes);
                        merchandiseTypeCodes = _convertToArrayCondition(merchandiseTypeCodes);
                        merchandiseGroupCodes = _convertToArrayCondition(merchandiseGroupCodes);
                        var pCustomerCode = new OracleParameter("pCustomerCode", OracleDbType.NVarchar2, nhaCungCapCodes, ParameterDirection.Input);
                        var pMerchandiseTypeCode = new OracleParameter("pMerchandiseTypeCode", OracleDbType.NVarchar2, merchandiseTypeCodes, ParameterDirection.Input);
                        var pMerchandiseGroupCode = new OracleParameter("pMerchandiseGroupCode", OracleDbType.NVarchar2, merchandiseGroupCodes, ParameterDirection.Input);
                        var pToDate = new OracleParameter("pToDate", OracleDbType.Date, toDate.Date, ParameterDirection.Input);
                        var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, fromDate.Date, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);

                        var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                        var str = "BEGIN TBNETERP.NVPHIEU.DATHANG_PHATSINH_XNT_BYDATE(:pMerchandiseTypeCode, :pMerchandiseGroupCode, :pCustomerCode, :pUnitCode,:pFromDate, :pToDate, :outRef); END;";
                        ctx.Database.ExecuteSqlCommand(str,  pMerchandiseTypeCode, pMerchandiseGroupCode, pCustomerCode, pUnitCode, pFromDate, pToDate, outRef);
                        OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                        while (reader.Read())
                        {
                            decimal increaseQuantity, decreaseQuantity;
                            var isIncreaseQuantity = decimal.TryParse(reader["IncreaseQuantity"].ToString(), out increaseQuantity);
                            var isDecreaseQuantity = decimal.TryParse(reader["DecreaseQuantity"].ToString(), out decreaseQuantity);

                            var item = new InventoryExpImp()
                            {
                                IncreaseQuantity = isIncreaseQuantity ? increaseQuantity : 0,
                                DecreaseQuantity = isDecreaseQuantity ? decreaseQuantity : 0,
                                Code = reader["Code"].ToString(),
                            };
                            result.Add(item);
                        }

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                    }
                    return result;
                }
            }

        }
        public static InventoryExpImp GetInventoryItem(string maHang, string maKho, string unitCode)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Format("SELECT TONCUOIKYSL AS ClosingQuantity FROM {0} WHERE MAVATTU = '{1}' AND MAKHO = '{2}' AND UNITCODE = '{3}'", tablleName, maHang, maKho, unitCode);
                try
                {
                    var data = ctx.Database.SqlQuery<InventoryExpImp>(queryStr);
                    return data.FirstOrDefault();
                }
                catch (Exception)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }

        public static List<InventoryExpImp> GetInventoryItemByCustomer(string expression, string unitCode, string findBy)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            using (var ctx = new ERPContext())
            {
                var queryStr = "";
                if (!string.IsNullOrEmpty(expression))
                {
                    queryStr = string.Format("SELECT SUM(a.TONCUOIKYSL) AS ClosingQuantity, a.MAVATTU AS CODE FROM {0} a, DM_VATTU c WHERE c.MAVATTU = a.MAVATTU  AND a.UNITCODE = '{1}' AND c.{3} IN({2}) GROUP BY a.MAVATTU", tablleName, unitCode, expression, findBy);
                }
                else
                {
                    queryStr = string.Format("SELECT SUM(a.TONCUOIKYSL) AS ClosingQuantity, a.MAVATTU AS CODE FROM {0} a, DM_VATTU c WHERE c.MAVATTU = a.MAVATTU  AND a.UNITCODE = '{1}'  GROUP BY a.MAVATTU", tablleName, unitCode);
                }
                try
                {
                    var data = ctx.Database.SqlQuery<InventoryExpImp>(queryStr);
                    return data.ToList();
                }
                catch (Exception)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }
        public static List<InventoryExpImp> GetCostOfGoodsSoldByMerchandises(string unitCode, string wareHouseCode, string merchandiseCode)
        {
            List<InventoryExpImp> listInventoryExpImp = new List<InventoryExpImp>();
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            if (curentDate != null)
            {
                var tablleName = curentDate.GetTableName();
                using (var ctx = new ERPContext())
                {
                    var checkExist = string.Format("SELECT TABLE_NAME FROM DBA_TABLES where TABLE_NAME = '{0}'", tablleName);
                    var exist = ctx.Database.SqlQuery<InventoryExpImp>(checkExist).ToList();
                    if (exist.Count > 0)
                    {
                        merchandiseCode = _convertToArrayCondition(merchandiseCode);
                        var queryStr = "";
                        if (!string.IsNullOrEmpty(wareHouseCode))
                        {
                            queryStr = string.Format("SELECT SUM(NVL(a.TONDAUKYSL,0)) AS OpeningBalanceQuantity, SUM(NVL(a.TONDAUKYGT,0)) as OpeningBalanceValue, SUM(NVL(a.NHAPSL,0)) as IncreaseQuantity, SUM(NVL(a.NHAPGT,0)) as IncreaseValue, NVL(a.GIAVON,0) AS CostOfCapital,NVL(a.TONCUOIKYSL,0) AS ClosingValue,a.MAVATTU AS CODE FROM {0} a  WHERE a.MAVATTU IN ({3})  AND a.UNITCODE = '{1}' AND a.MAKHO = '{2}' GROUP BY a.TONDAUKYSL,a.TONDAUKYGT,a.NHAPSL,a.NHAPGT,a.GIAVON,a.TONCUOIKYSL,a.MAVATTU", tablleName, unitCode, wareHouseCode, merchandiseCode);
                        }
                        else
                        {
                            wareHouseCode = unitCode + "-K2";
                            queryStr = string.Format("SELECT SUM(NVL(a.TONDAUKYSL,0)) AS OpeningBalanceQuantity, SUM(NVL(a.TONDAUKYGT,0)) as OpeningBalanceValue, SUM(NVL(a.NHAPSL,0)) as IncreaseQuantity, SUM(NVL(a.NHAPGT,0)) as IncreaseValue, NVL(a.GIAVON,0) AS CostOfCapital,NVL(a.TONCUOIKYSL,0) AS ClosingValue,a.MAVATTU AS CODE FROM {0} a  WHERE a.MAVATTU IN ({3})  AND a.UNITCODE = '{1}' AND a.MAKHO = '{2}' GROUP BY a.TONDAUKYSL,a.TONDAUKYGT,a.NHAPSL,a.NHAPGT,a.GIAVON,a.TONCUOIKYSL,a.MAVATTU", tablleName, unitCode, wareHouseCode, merchandiseCode);
                        }
                        try
                        {
                            var data = ctx.Database.SqlQuery<InventoryExpImp>(queryStr);
                            listInventoryExpImp = data.ToList();
                        }
                        catch (Exception)
                        {
                            listInventoryExpImp = new List<InventoryExpImp>();
                        }
                    }
                    else
                    {
                        listInventoryExpImp = new List<InventoryExpImp>();
                    }
                }
            }
            return listInventoryExpImp;
        }
        public static InventoryExpImp GetCostOfGoodsSoldByMerchandise(string unitCode, string wareHouseCode, string merchandiseCode)
        {
            InventoryExpImp result = new InventoryExpImp();
            MdPeriod curentDate = CurrentSetting.GetKhoaSo(unitCode);
            if (curentDate != null)
            {
                string tablleName = curentDate.GetTableName();
                using (var ctx = new ERPContext())
                {
                    string checkExist = string.Format("SELECT TABLE_NAME FROM DBA_TABLES where TABLE_NAME = '{0}'" ,tablleName);
                    List<InventoryExpImp> exist = ctx.Database.SqlQuery<InventoryExpImp>(checkExist).ToList();
                    if (exist.Count > 0)
                    {
                        var queryStr = "";
                        if (!string.IsNullOrEmpty(wareHouseCode))
                        {
                            queryStr =
                                string.Format(  

                                    "SELECT SUM(NVL(a.TONDAUKYSL,0)) AS OpeningBalanceQuantity, SUM(NVL(a.TONDAUKYGT,0)) as OpeningBalanceValue, SUM(NVL(a.NHAPSL,0)) as IncreaseQuantity, SUM(NVL(a.NHAPGT,0)) as IncreaseValue,a.MAVATTU AS CODE, NVL(a.GIAVON,0) AS CostOfCapital, NVL(a.TONCUOIKYSL,0) AS ClosingQuantity, NVL(a.NHAPSL,0) AS IncreaseQuantity, NVL(a.XUATSL,0) AS DecreaseQuantity FROM {0} a  WHERE a.MAVATTU = '{3}'  AND a.UNITCODE = '{1}' AND a.MAKHO = '{2}' GROUP BY a.MAVATTU,a.GIAVON,a.TONCUOIKYSL,a.NHAPSL,a.XUATSL",tablleName, unitCode, wareHouseCode, merchandiseCode.ToUpper());
                        }
                        else
                        {
                            string MaKhoBanLe = unitCode + "-K2";
                            queryStr =
                                string.Format(
                                    "SELECT SUM(NVL(a.TONDAUKYSL,0)) AS OpeningBalanceQuantity, SUM(NVL(a.TONDAUKYGT,0)) as OpeningBalanceValue, SUM(NVL(a.NHAPSL,0)) as IncreaseQuantity, SUM(NVL(a.NHAPGT,0)) as IncreaseValue,a.MAVATTU AS CODE, NVL(a.GIAVON,0) AS CostOfCapital, NVL(a.TONCUOIKYSL,0) AS ClosingQuantity, NVL(a.NHAPSL,0) AS IncreaseQuantity, NVL(a.XUATSL,0) AS DecreaseQuantity FROM {0} a WHERE a.MAVATTU = '{1}'  AND a.UNITCODE = '{2}' AND MAKHO = '{3}' GROUP BY a.MAVATTU,a.GIAVON,a.TONCUOIKYSL,a.NHAPSL,a.XUATSL",tablleName, merchandiseCode.ToUpper(),unitCode,MaKhoBanLe);
                        }
                        try
                        {
                            var data = ctx.Database.SqlQuery<InventoryExpImp>(queryStr);
                            if (data.Count() == 1)
                            {
                                result = data.ToList()[0];
                                result.Status = true;
                                result.Message = "Thành công";
                            }
                        }
                        catch (Exception ex)
                        {
                            
                            result = new InventoryExpImp();
                            result.Status = false;
                            result.Message = "Lỗi: "+ ex;
                        }
                    }
                    else
                    {
                        result = new InventoryExpImp();
                        result.Status = false;
                        result.Message = "Không tìm thấy bản ghi nào !";
                    }
                }
            }
            else
            {
                result = new InventoryExpImp();
                result.Status = false;
                result.Message = "Chưa khởi tạo kỳ kế toán";
            }
            return result;
        }

        public static InventoryExpImp GetCostByPeriodMerchandise(string unitCode, string tableName, string merchandiseCode)
        {
            using (var ctx = new ERPContext())
            {
                string MaKhoBanLe = unitCode + "-K2";
                var queryStr = string.Format("SELECT SUM(a.TONDAUKYSL) AS OpeningBalanceQuantity, SUM(a.TONDAUKYGT) as OpeningBalanceValue, SUM(a.NHAPSL) as IncreaseQuantity, SUM(a.NHAPGT) as IncreaseValue,a.MAVATTU AS CODE, a.GIAVON AS CostOfCapital FROM {0} a  WHERE a.MAVATTU = '{1}'  AND a.UNITCODE = '{2}' AND a.MAKHO = '{3}' GROUP BY a.MAVATTU,a.GIAVON", tableName, merchandiseCode.ToUpper(),unitCode, MaKhoBanLe);
                try
                {
                    var data = ctx.Database.SqlQuery<InventoryExpImp>(queryStr);
                    if (data.Count() == 1)
                    {
                        return data.ToList()[0];
                    }
                    return null;
                }
                catch (Exception)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }

        public static List<MdMerchandiseVm.MATHANG> GetInventoryForActionInventory(string unitCode, string wareHouseCode)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Empty;
                queryStr = string.Format("SELECT a.BARCODE as Masieuthiphu, a.MAHANG as Masieuthi, a.BARCODE as Barcode, a.TENVIETTAT as Tenviettat, a.MALOAIVATTU as Manganhhang, a.GIABANLEVAT Giabanlecovat, NVL(b.TONCUOIKYSL, 0) as Soluong from V_VATTU_GIABAN a left join {0} b on a.MAHANG = b.MAVATTU WHERE b.MAKHO = '{1}'", tablleName, wareHouseCode);
                try
                {
                    var data = ctx.Database.SqlQuery<MdMerchandiseVm.MATHANG>(queryStr);
                    return data.ToList();
                }
                catch (Exception e)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }
        public static IQueryable<NvGiaoDichQuayVm.Dto> GetTranferCashieer(ERPContext ctx, string maDonVi = "")
        {
            IQueryable<NvGiaoDichQuayVm.Dto> result = null;
            try
            {
                var str = "";
                str = @"SELECT MANGUOITAO,MAQUAYBAn,NGUOITAO, sum(ttiencovat) as ttiencovat,sum(SOLUONG) as SOLUONG FROM V_GDQUAY_HANGQUAY WHERE MADONVI ='" + maDonVi + "' AND LOAIGIAODICH =1 GROUP BY MANGUOITAO,MAQUAYBAN,NGUOITAO";
                if (string.IsNullOrEmpty(str))
                {
                    return result;
                }
                var data = ctx.Database.SqlQuery<NvGiaoDichQuayVm.Dto>(str);
                result = data.AsQueryable();
            }
            catch (Exception e)
            {

                throw new Exception("Lỗi không thể truy xuất hàng hóa");
            }
            return result;

        }

        public static List<NvKiemKeVm.DtoDetails> GetInventoryForActionInventoryByOne(string unitCode, string wareHouseCode, string merchandiseCodes)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            merchandiseCodes = _convertToArrayCondition(merchandiseCodes);
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Format(@"SELECT a.MAVATTU as MaVatTu, a.BARCODE as Barcode, a.MALOAIVATTU as LoaiVatTuKiemKe, a.MANHOMVATTU as NhomVatTuKiemKe, a.TENVATTU as TenVatTu, a.GIABANLEVAT as GiaBanLeCoVat, a.MAKEHANG as KeKiemKe, NVL(b.TONCUOIKYSL, 0) as SoLuongTonMay, NVL(b.GIAVON, 0) as GiaVon from V_VATTU_GIABAN a left join {0} b on a.MAVATTU = b.MAVATTU WHERE b.MAKHO = '{1}' AND b.MaVatTu IN ({2})", tablleName, wareHouseCode, merchandiseCodes);
                try
                {
                    var data = ctx.Database.SqlQuery<NvKiemKeVm.DtoDetails>(queryStr);
                    return data.ToList();
                }
                catch (Exception e)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }

        //log
        public static List<NvKiemKeVm.ExternalCodeInInventory> GetExternalCodeInventory(string unitCode, string tableName, ParameterKiemKe para)
        {
            List<NvKiemKeVm.ExternalCodeInInventory> result = null;
            using (var ctx = new ERPContext())
            {
                try
                {

                    OracleParameter pMaDonVi = new OracleParameter("pMaDonVi", OracleDbType.NVarchar2, unitCode,
                        ParameterDirection.Input);
                    OracleParameter pTable = new OracleParameter("pTable", OracleDbType.NVarchar2, tableName,
                        ParameterDirection.Input);
                    OracleParameter pMaKho = new OracleParameter("pMaKho", OracleDbType.NVarchar2, para.WareHouseCodes,
                        ParameterDirection.Input);
                    OracleParameter pMaLoai = new OracleParameter("pMaLoai", OracleDbType.NVarchar2, _convertToArrayCondition(para.MerchandiseTypeCodes),
                        ParameterDirection.Input);
                    OracleParameter pMaNhaCungCap = new OracleParameter("pMaNhaCungCap", OracleDbType.NVarchar2, _convertToArrayCondition(para.NhaCungCapCodes),
                        ParameterDirection.Input);
                    OracleParameter pMaKe = new OracleParameter("pMaKe", OracleDbType.NVarchar2, _convertToArrayCondition(para.KeHangCodes),
                        ParameterDirection.Input);
                    OracleParameter pMaNhom = new OracleParameter("pMaNhom", OracleDbType.NVarchar2, _convertToArrayCondition(para.MerchandiseGroupCodes),
                        ParameterDirection.Input);
                    OracleParameter pMaVatTu = new OracleParameter("pMaVatTu", OracleDbType.NVarchar2, _convertToArrayCondition(para.MerchandiseCodes),
                        ParameterDirection.Input);
                    OracleParameter externalCode = new OracleParameter("externalCode", OracleDbType.RefCursor,
                        ParameterDirection.Output);
                    string str = "BEGIN TBNETERP.FILTER_EXTERNALCODE_KIEMKE(:pMaDonVi, :pTable, :pMaKho, :pMaLoai, :pMaNhaCungCap, :pMaKe, :pMaNhom, :pMaVatTu, :externalCode); END;";
                    List<NvKiemKeVm.ExternalCodeInInventory> data = ctx.Database.SqlQuery<NvKiemKeVm.ExternalCodeInInventory>(str, pMaDonVi, pTable, pMaKho, pMaLoai, pMaNhaCungCap, pMaKe, pMaNhom, pMaVatTu, externalCode).ToList();
                    if (data != null)
                    {
                        result = data;
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                catch (Exception ex)
                {

                    throw new Exception("Lỗi không thể truy xuất hàng hóa");
                }
            }
            return result;
        }

        public static List<MdMerchandiseVm.DataXNT> GetDataInventoryByCondition(string unitCode, string wareHouseCode, string merchandiseCodes, string kyKeToan,string _ParentUnitCode)
        {
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Format("SELECT A.TYLEVATRA AS TyLeVATRa,A.TYLEVATVAO AS TyLeVATVao, NVL(B.GIAVON, 0) AS GiaVon FROM V_VATTU_GIABAN A LEFT JOIN {0} B ON A.MAVATTU = B.MAVATTU WHERE B.MAKHO = '{1}' AND A.MAVATTU = '{2}' AND A.UNITCODE LIKE '"+ _ParentUnitCode + "%'", kyKeToan, wareHouseCode, merchandiseCodes, _ParentUnitCode);
                try
                {
                    var data = ctx.Database.SqlQuery<MdMerchandiseVm.DataXNT>(queryStr).ToList(); 
                    return data;
                }
                catch (Exception e)
                {
                    throw new Exception("Không thể tìm được giá vốn của mã hàng: " + merchandiseCodes);
                }

            }
        }



        public static string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            var subStrAray = str.Split(',');
            int length = subStrAray.Length;
            string[] resultArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                resultArray[i] = "'" + subStrAray[i] + "'";
            }
            return String.Join(",", resultArray);
        }

        public static bool CapNhatGiaVonQuayGiaoDich(string tableName, string unitCode, DateTime ngayPhatSinh)
        {
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var pTableName = new OracleParameter("pTableName", OracleDbType.NVarchar2, tableName, ParameterDirection.Input);
                        var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, unitCode, ParameterDirection.Input);
                        var pNgayPhatSinh = new OracleParameter("pNgayPhatSinh", OracleDbType.Date, ngayPhatSinh, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.XNT.CAPNHAT_GIAVON_QUAYGIAODICH(:pTableName, :pUnitCode,:pNgayPhatSinh); END;";
                        ctx.Database.ExecuteSqlCommand(str, pTableName, pUnitCode, pNgayPhatSinh);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// UpdatePriceByDate
        /// </summary>
        /// <param name="UnitCode"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public static bool UpdatePriceByDate(string UnitCode, DateTime FromDate, DateTime ToDate)
        {
            bool result = false;
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var P_UNITCODE = new OracleParameter("P_UNITCODE", OracleDbType.NVarchar2, UnitCode, ParameterDirection.Input);
                        var P_TUNGAY = new OracleParameter("P_TUNGAY", OracleDbType.Date, FromDate, ParameterDirection.Input);
                        var P_DENNGAY = new OracleParameter("P_DENNGAY", OracleDbType.Date, ToDate, ParameterDirection.Input);
                        var str = "BEGIN TBNETERP.UDP_GIAVON_MULTIPLE(:P_UNITCODE, :P_TUNGAY,:P_DENNGAY); END;";
                        ctx.Database.ExecuteSqlCommand(str, P_UNITCODE, P_TUNGAY, P_DENNGAY);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        public static List<MdMerchandiseVm.MATHANGTONAM> LayThongTinHangTonAm(string unitCode, string wareHouseCode)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            if (string.IsNullOrEmpty(wareHouseCode))
            {
                //lấy mã tồn âm trong kho bán lẻ
                wareHouseCode = unitCode + "-K2";
            }
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Format("SELECT a.MAVATTU as MaHang, a.TENVATTU as TenHang,a.BARCODE as Barcode,a.MAKEHANG AS MaKeHang,a.TENNHACUNGCAP AS TenNhaCungCap,a.MALOAIVATTU AS MaLoaiVatTu,a.MANHOMVATTU AS MaNhomVatTu, NVL(b.GIAVON, 0) AS DonGia ,NVL(a.TYLEVATRA, 0) AS TyLeVatRa,NVL(a.TYLEVATVAO, 0) AS TyLeVatVao, NVL(b.TONCUOIKYSL, 0) AS SoLuong, NVL(b.TONCUOIKYGT, 0) AS ThanhTien FROM V_VATTU_GIABAN a LEFT JOIN {0} b on a.MAHANG = b.MAVATTU WHERE b.MAKHO = '{1}' AND b.TONCUOIKYSL < 0 ORDER BY a.MALOAIVATTU,a.MANHOMVATTU", tablleName, wareHouseCode);
                try
                {
                    var data = ctx.Database.SqlQuery<MdMerchandiseVm.MATHANGTONAM>(queryStr);
                    return data.ToList();
                }
                catch (Exception e)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }

        public static List<NvKiemKeVm.DtoDetails> LayThongTinHangThuocKe(string unitCode, string wareHouseCode, string maKe)
        {
            var curentDate = CurrentSetting.GetKhoaSo(unitCode);
            var tablleName = curentDate.GetTableName();
            if (string.IsNullOrEmpty(wareHouseCode))
            {
                //lấy mã tồn âm trong kho bán lẻ
                wareHouseCode = unitCode + "-K2";
            }
            using (var ctx = new ERPContext())
            {
                var queryStr = string.Format("SELECT a.MAVATTU as MaVatTu, a.BARCODE as Barcode, a.MALOAIVATTU as LoaiVatTuKiemKe, a.MANHOMVATTU as NhomVatTuKiemKe, a.TENVATTU as TenVatTu, a.GIABANLEVAT as GiaBanLeCoVat, a.MAKEHANG as KeKiemKe, NVL(b.TONCUOIKYSL, 0) as SoLuongTonMay, NVL(b.GIAVON, 0) as GiaVon from V_VATTU_GIABAN a left join {0} b on a.MAVATTU = b.MAVATTU WHERE b.MAKHO = '{1}' AND a.MAKEHANG = '{2}' AND b.TONCUOIKYSL <> 0", tablleName, wareHouseCode, maKe);
                try
                {
                    var data = ctx.Database.SqlQuery<NvKiemKeVm.DtoDetails>(queryStr);
                    return data.ToList();
                }
                catch (Exception e)
                {
                    throw new Exception("Không thể tìm được số tồn của mặt hàng này");
                }

            }
        }

        public static string GetTableName(int year, int period)
        {
            return string.Format("XNT_{0}_KY_{1}", year, period);
        }


    }
}
