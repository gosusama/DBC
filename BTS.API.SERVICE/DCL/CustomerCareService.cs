using BTS.API.ENTITY.DCL;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using BTS.API.ENTITY;
using System.Data.Common;
using Oracle.ManagedDataAccess.Types;
using System.IO;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using BTS.API.SERVICE.NV;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.MD;

namespace BTS.API.SERVICE.DCL
{
    public interface ICustomerCareService : IDetailInfoServiceBase<DclCloseout>
    {
        string GetTableName(int year, int period);
        List<MdCustomer> ReportLoyalCustomer(ParameterCustomerCare para);
        List<MdCustomerVm.CustomerDto> ReportBeChangedCardCustomer(ParameterCustomerCare para);
        List<CustomerReport> ReportHistoryGiveCardCustomer(ParameterCustomerCare para);
        List<CustomerReport> ReportForgetCardCustomer(ParameterCustomerCare para);
        List<MdCustomer> CustomerLevelUpCollection(ParameterCustomerCare para);
        List<MdCustomer> ReportCustomerLevelUp(ParameterCustomerCare para);
        List<MdCustomer> NotBuyCustomerCollection(ParameterCustomerCare para);
        List<MdCustomer> ReportNotBuyCustomer(ParameterCustomerCare para);
    }
    public class CustomerCareService : DetailInfoServiceBase<DclCloseout>, ICustomerCareService
    {
        public CustomerCareService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public List<MdCustomer> CustomerLevelUpCollection(ParameterCustomerCare para)
        {
            List<MdCustomer> result = new List<MdCustomer>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var pStateCustomerCare = new OracleParameter("pStateCustomerCare", OracleDbType.Int32, para.StateCustomerCare, ParameterDirection.Input);

                            //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.CUSTOMER_LEVELUP(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pStateCustomerCare, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate, pStateTypeMoney, pFromMoney, pToMoney, pStateCustomerCare, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                bool care;
                                decimal tienNguyenGia, tongTien,tienNguyenGiaCS,tongTienCS;
                                DateTime ngayDacBiet, ngaySinh, ngayCapThe, ngayHetHan,ngayChamSoc;
                                var isTongTienCS = decimal.TryParse(reader["tongTien_CHAMSOC"].ToString(), out tongTienCS);
                                var istienNguyenGiaCS = decimal.TryParse(reader["tienNguyenGia_CHAMSOC"].ToString(), out tienNguyenGiaCS);
                                var istienNguyenGia = decimal.TryParse(reader["tienNguyenGia"].ToString(), out tienNguyenGia);
                                var istongTien = decimal.TryParse(reader["tongTien"].ToString(), out tongTien);
                                var isNgayDb = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayCT = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHH = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                                var isNgayChamSoc = DateTime.TryParse(reader["NGAYCHAMSOC"].ToString(), out ngayChamSoc);
                                var isCare = bool.TryParse(reader["ISCARE"].ToString(), out care);
                                var item = new MdCustomer()
                                {
                                    Id = reader["ID"].ToString(),
                                    MaKH = reader["MaKH"].ToString(),
                                    TenKH = reader["TenKH"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    DienThoai = reader["DienThoai"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MaThe = reader["MATHE"].ToString(),
                                    GhiChu = reader["GHICHU"].ToString(),
                                    GhiChuCu = reader["GHICHUCU"].ToString(),
                                    IsCare = isCare ? care : false,
                                    NgayDacBiet = isNgayDb ? ngayDacBiet : new DateTime(0001, 1, 1),
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayCapThe = isNgayCT ? ngayCapThe : new DateTime(0001, 1, 1),
                                    NgayHetHan = isNgayHH ? ngayHetHan : new DateTime(0001, 1, 1),
                                    NgayChamSoc = isNgayChamSoc ? ngayChamSoc : new DateTime(0001, 1, 1),
                                    TienNguyenGia = istienNguyenGia ? tienNguyenGia : 0,
                                    TongTien = istongTien ? tongTien : 0,
                                    TienNguyenGia_ChamSoc = istienNguyenGiaCS ? tienNguyenGiaCS : 0,
                                    TongTien_ChamSoc = isTongTienCS ? tongTienCS : 0,
                                };
                                result.Add(item);
                            }
                            

                            dbContextTransaction.Commit();
                            return result;
                        }
                        catch
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MdCustomer> ReportCustomerLevelUp(ParameterCustomerCare para)
        {
            List<MdCustomer> result = new List<MdCustomer>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var pStateCustomerCare = new OracleParameter("pStateCustomerCare", OracleDbType.Int32, para.StateCustomerCare, ParameterDirection.Input);

                            //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_CUSTOMER_LEVELUP(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pStateCustomerCare, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate, pStateTypeMoney, pFromMoney, pToMoney, pStateCustomerCare, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                bool care;
                                decimal tienNguyenGia, tongTien, tienNguyenGiaCS, tongTienCS;
                                DateTime ngayDacBiet, ngaySinh, ngayCapThe, ngayHetHan, ngayChamSoc;
                                var isTongTienCS = decimal.TryParse(reader["tongTien_CHAMSOC"].ToString(), out tongTienCS);
                                var istienNguyenGiaCS = decimal.TryParse(reader["tienNguyenGia_CHAMSOC"].ToString(), out tienNguyenGiaCS);
                                var istienNguyenGia = decimal.TryParse(reader["tienNguyenGia"].ToString(), out tienNguyenGia);
                                var istongTien = decimal.TryParse(reader["tongTien"].ToString(), out tongTien);
                                var isNgayDb = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayCT = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHH = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                                var isNgayChamSoc = DateTime.TryParse(reader["NGAYCHAMSOC"].ToString(), out ngayChamSoc);
                                var isCare = bool.TryParse(reader["ISCARE"].ToString(), out care);
                                var item = new MdCustomer()
                                {
                                    Id = reader["ID"].ToString(),
                                    MaKH = reader["MaKH"].ToString(),
                                    TenKH = reader["TenKH"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    DienThoai = reader["DienThoai"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MaThe = reader["MATHE"].ToString(),
                                    GhiChu = reader["GHICHU"].ToString(),
                                    GhiChuCu = reader["GHICHUCU"].ToString(),
                                    IsCare = isCare ? care : false,
                                    NgayDacBiet = isNgayDb ? ngayDacBiet : new DateTime(0001, 1, 1),
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayCapThe = isNgayCT ? ngayCapThe : new DateTime(0001, 1, 1),
                                    NgayHetHan = isNgayHH ? ngayHetHan : new DateTime(0001, 1, 1),
                                    NgayChamSoc = isNgayChamSoc ? ngayChamSoc : new DateTime(0001, 1, 1),
                                    TienNguyenGia = istienNguyenGia ? tienNguyenGia : 0,
                                    TongTien = istongTien ? tongTien : 0,
                                    TienNguyenGia_ChamSoc = istienNguyenGiaCS ? tienNguyenGiaCS : 0,
                                    TongTien_ChamSoc = isTongTienCS ? tongTienCS : 0,
                                };
                                result.Add(item);
                            }


                            dbContextTransaction.Commit();
                            return result;
                        }
                        catch
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MdCustomer> NotBuyCustomerCollection(ParameterCustomerCare para)
        {
            List<MdCustomer> result = new List<MdCustomer>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var pStateCustomerCare = new OracleParameter("pStateCustomerCare", OracleDbType.Int32, para.StateCustomerCare, ParameterDirection.Input);

                            //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.NOTBUY_CUSTOMER(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pStateCustomerCare, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate, pStateTypeMoney, pFromMoney, pToMoney, pStateCustomerCare, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                bool care;
                                decimal tienNguyenGia, tongTien, tienNguyenGiaCS, tongTienCS;
                                DateTime ngayDacBiet, ngaySinh, ngayCapThe, ngayHetHan, ngayChamSoc;
                                var isTongTienCS = decimal.TryParse(reader["tongTien_CHAMSOC"].ToString(), out tongTienCS);
                                var istienNguyenGiaCS = decimal.TryParse(reader["tienNguyenGia_CHAMSOC"].ToString(), out tienNguyenGiaCS);
                                var istienNguyenGia = decimal.TryParse(reader["tienNguyenGia"].ToString(), out tienNguyenGia);
                                var istongTien = decimal.TryParse(reader["tongTien"].ToString(), out tongTien);
                                var isNgayDb = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayCT = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHH = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                                var isNgayChamSoc = DateTime.TryParse(reader["NGAYCHAMSOC"].ToString(), out ngayChamSoc);
                                var isCare = bool.TryParse(reader["ISCARE"].ToString(), out care);
                                var item = new MdCustomer()
                                {
                                    Id = reader["ID"].ToString(),
                                    MaKH = reader["MaKH"].ToString(),
                                    TenKH = reader["TenKH"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    DienThoai = reader["DienThoai"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MaThe = reader["MATHE"].ToString(),
                                    GhiChu = reader["GHICHU"].ToString(),
                                    GhiChuCu = reader["GHICHUCU"].ToString(),
                                    IsCare = isCare ? care : false,
                                    NgayDacBiet = isNgayDb ? ngayDacBiet : new DateTime(0001, 1, 1),
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayCapThe = isNgayCT ? ngayCapThe : new DateTime(0001, 1, 1),
                                    NgayHetHan = isNgayHH ? ngayHetHan : new DateTime(0001, 1, 1),
                                    NgayChamSoc = isNgayChamSoc ? ngayChamSoc : new DateTime(0001, 1, 1),
                                    TienNguyenGia = istienNguyenGia ? tienNguyenGia : 0,
                                    TongTien = istongTien ? tongTien : 0,
                                    TienNguyenGia_ChamSoc = istienNguyenGiaCS ? tienNguyenGiaCS : 0,
                                    TongTien_ChamSoc = isTongTienCS ? tongTienCS : 0,
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
            catch (Exception)
            {
                throw;
            }
        }
        public List<MdCustomer> ReportNotBuyCustomer(ParameterCustomerCare para)
        {
            List<MdCustomer> result = new List<MdCustomer>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var pStateCustomerCare = new OracleParameter("pStateCustomerCare", OracleDbType.Int32, para.StateCustomerCare, ParameterDirection.Input);

                            //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_NOTBUY_CUSTOMER(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pStateCustomerCare, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate, pStateTypeMoney, pFromMoney, pToMoney, pStateCustomerCare, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                bool care;
                                decimal tienNguyenGia, tongTien, tienNguyenGiaCS, tongTienCS;
                                DateTime ngayDacBiet, ngaySinh, ngayCapThe, ngayHetHan, ngayChamSoc;
                                var isTongTienCS = decimal.TryParse(reader["tongTien_CHAMSOC"].ToString(), out tongTienCS);
                                var istienNguyenGiaCS = decimal.TryParse(reader["tienNguyenGia_CHAMSOC"].ToString(), out tienNguyenGiaCS);
                                var istienNguyenGia = decimal.TryParse(reader["tienNguyenGia"].ToString(), out tienNguyenGia);
                                var istongTien = decimal.TryParse(reader["tongTien"].ToString(), out tongTien);
                                var isNgayDb = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayCT = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHH = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                                var isNgayChamSoc = DateTime.TryParse(reader["NGAYCHAMSOC"].ToString(), out ngayChamSoc);
                                var isCare = bool.TryParse(reader["ISCARE"].ToString(), out care);
                                var item = new MdCustomer()
                                {
                                    Id = reader["ID"].ToString(),
                                    MaKH = reader["MaKH"].ToString(),
                                    TenKH = reader["TenKH"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    DienThoai = reader["DienThoai"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MaThe = reader["MATHE"].ToString(),
                                    GhiChu = reader["GHICHU"].ToString(),
                                    GhiChuCu = reader["GHICHUCU"].ToString(),
                                    IsCare = isCare ? care : false,
                                    NgayDacBiet = isNgayDb ? ngayDacBiet : new DateTime(0001, 1, 1),
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayCapThe = isNgayCT ? ngayCapThe : new DateTime(0001, 1, 1),
                                    NgayHetHan = isNgayHH ? ngayHetHan : new DateTime(0001, 1, 1),
                                    NgayChamSoc = isNgayChamSoc ? ngayChamSoc : new DateTime(0001, 1, 1),
                                    TienNguyenGia = istienNguyenGia ? tienNguyenGia : 0,
                                    TongTien = istongTien ? tongTien : 0,
                                    TienNguyenGia_ChamSoc = istienNguyenGiaCS ? tienNguyenGiaCS : 0,
                                    TongTien_ChamSoc = isTongTienCS ? tongTienCS : 0,
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
            catch (Exception)
            {
                throw;
            }
        }
        public List<MdCustomer> ReportLoyalCustomer(ParameterCustomerCare para)
        {
            List<MdCustomer> result = new List<MdCustomer>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var pMaThe = new OracleParameter("pMaThe", OracleDbType.NVarchar2, para.MaThe, ParameterDirection.Input);
                            var pDiaChi = new OracleParameter("pDiaChi", OracleDbType.NVarchar2, para.DiaChi, ParameterDirection.Input);
                            //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_LOYAL_CUSTOMER(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pMaThe,:pDiaChi, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate, pStateTypeMoney, pFromMoney, pToMoney, pMaThe, pDiaChi, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                decimal tienNguyenGia,tongTien;
                                DateTime ngayDacBiet,ngaySinh,ngayCapThe,ngayHetHan;
                                var istienNguyenGia = decimal.TryParse(reader["tienNguyenGia"].ToString(), out tienNguyenGia);
                                var istongTien = decimal.TryParse(reader["tongTien"].ToString(), out tongTien);
                                var isNgayDb = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayCT = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHH = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);
                                var item = new MdCustomer()
                                {
                                    MaKH = reader["MaKH"].ToString(),
                                    TenKH = reader["TenKH"].ToString(),
                                    DiaChi = reader["DiaChi"].ToString(),
                                    DienThoai = reader["DienThoai"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MaThe = reader["MATHE"].ToString(),
                                    NgayDacBiet = isNgayDb?ngayDacBiet:new DateTime(0001,1,1),
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayCapThe = isNgayCT ? ngayCapThe : new DateTime(0001, 1, 1),
                                    NgayHetHan = isNgayHH ? ngayHetHan : new DateTime(0001, 1, 1),
                                    TienNguyenGia = istienNguyenGia?tienNguyenGia:0,
                                    TongTien = istongTien?tongTien:0,
                                };
                                result.Add(item);
                            }
                            if (para.StateExpiredCard != null && para.StateExpiredCard == 1 && para.NgayHetHan != null)
                            {
                                var lstItem = result.Where(x => x.NgayHetHan <= para.NgayHetHan).ToList();
                                if (lstItem.Count > 0)
                                {
                                     lstItem.ForEach(x => result.Remove(x));
                                }
                            }
                            if (para.StateGiveCard != null)
                            {
                                switch (para.StateGiveCard)
                                {
                                    case 1: // Chưa cấp thẻ => Delete (Ngay cap the != null || != 0001)
                                        var lst = result.Where(x => x.NgayCapThe != null).ToList();
                                        if (lst.Count > 0)
                                        {
                                                lst.ForEach(x => {
                                                    if (x.NgayCapThe.Value.Year != 0001)
                                                        result.Remove(x);
                                                } );
                                        }
                                        break;
                                    case 2: // Đã cấp thẻ => Delete (Ngay cap the = null || = 0001)
                                        lst = result.Where(x => x.NgayCapThe == null || x.NgayCapThe.Value.Year == 0001 ).ToList();
                                        if (lst.Count > 0)
                                        {
                                            lst.ForEach(x => result.Remove(x));
                                        }
                                        break;
                                }
                            }
               
                            dbContextTransaction.Commit();
                            return result;
                        }
                        catch
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<MdCustomerVm.CustomerDto> ReportBeChangedCardCustomer(ParameterCustomerCare para)
        {
            List<MdCustomerVm.CustomerDto> result = new List<MdCustomerVm.CustomerDto>();
            using (var ctx = new ERPContext())
            {
                try
                {
                    string strWhere = string.Empty;
                    string strQuery = string.Empty;
                    if(para.UnitCode != null)
                    {
                        strWhere += string.Format(" AND UNITCODE LIKE '%{0}%' ", para.UnitCode);
                    }
                    if (para.FromDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE >= TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.FromDate.Day, para.FromDate.Month, para.FromDate.Year);
                    }
                    if (para.ToDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE <= TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.ToDate.Day, para.ToDate.Month, para.ToDate.Year);
                    }
                    if (para.HangKhachHang != null)
                    {
                        strWhere += string.Format(" AND (HANGKHACHHANG = '{0}' OR HANGKHACHHANGCU = '{1}') ", para.HangKhachHang, para.HangKhachHang);
                    }
                    strQuery = @"SELECT ID,MAKH,TENKHAC,TENKH,DIACHI,MASOTHUE AS MASOTHUE,TRANGTHAI AS TRANGTHAI,CMTND AS CHUNGMINHTHU,
                                DIENTHOAI,EMAIL,TIENNGUYENGIA,SODIEM,TONGTIEN,HANGKHACHHANG,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,NGAYHETHAN,GHICHU
                                MATHE,NGAYDACBIET,NGAYCAPTHE,NGAYSINH,HANGKHACHHANG,HANGKHACHHANGCU,QUENTHE
                                FROM DMKHACHHANG WHERE 1=1 " + strWhere;

                    var data = ctx.Database.SqlQuery<MdCustomerVm.CustomerDto>(strQuery);
                    result = data.ToList();
                    return result;
                }
                catch
                {
                    throw new Exception("Lỗi không thể truy xuất khách hàng");
                }
            }
        }
        public List<CustomerReport> ReportHistoryGiveCardCustomer(ParameterCustomerCare para)
        {
            List<CustomerReport> result = new List<CustomerReport>();
            using (var ctx = new ERPContext())
            {
                try
                {
                    string strWhere = string.Empty;
                    string strQuery = string.Empty;
                    if (para.UnitCode != null)
                    {
                        strWhere += string.Format(" AND UNITCODE LIKE '%{0}%' ", para.UnitCode);
                    }
                    if (para.FromDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE >= TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.FromDate.Day, para.FromDate.Month, para.FromDate.Year);
                    }
                    if (para.ToDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE < TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.ToDate.Day, para.ToDate.Month, para.ToDate.Year);
                    }
                    if (para.HangKhachHang != null)
                    {
                        strWhere += string.Format(" AND (HANGKHACHHANG = '{0}' OR HANGKHACHHANGCU = '{1}') ", para.HangKhachHang, para.HangKhachHang);
                    }
                    if (para.MaKH != null)
                    {
                        strWhere += string.Format(" AND MAKH = '{0}' ", para.MaKH);
                    }
                    strQuery = @"SELECT ID,MAKH,TENKHAC,TENKH,DIACHI,MASOTHUE AS MASOTHUE,TRANGTHAI AS TRANGTHAI,CMTND AS CHUNGMINHTHU,
                                DIENTHOAI,EMAIL,TIENNGUYENGIA,SODIEM,TONGTIEN,HANGKHACHHANG,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,NGAYHETHAN,GHICHU
                                MATHE,NGAYDACBIET,NGAYCAPTHE,NGAYSINH,HANGKHACHHANG,HANGKHACHHANGCU,QUENTHE,I_CREATE_DATE,I_CREATE_BY
                                FROM DMKHACHHANG WHERE 1=1 " + strWhere;

                    var data = ctx.Database.SqlQuery<CustomerReport>(strQuery);
                    result = data.ToList();
                    return result;
                }
                catch
                {
                    throw new Exception("Lỗi không thể truy xuất khách hàng");
                }
            }
        }
        public List<CustomerReport> ReportForgetCardCustomer(ParameterCustomerCare para)
        {
            List<CustomerReport> result = new List<CustomerReport>();
            using (var ctx = new ERPContext())
            {
                try
                {
                    string strWhere = string.Empty;
                    string strQuery = string.Empty;
                    if (para.UnitCode != null)
                    {
                        strWhere += string.Format(" AND UNITCODE LIKE '%{0}%' ", para.UnitCode);
                    }
                    if (para.FromDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE >= TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.FromDate.Day, para.FromDate.Month, para.FromDate.Year);
                    }
                    if (para.ToDate != null)
                    {
                        strWhere += string.Format(" AND NGAYCAPTHE < TO_DATE('{0}/{1}/{2}','dd/MM/yyyy') ", para.ToDate.Day, para.ToDate.Month, para.ToDate.Year);
                    }
                    if (para.QuenThe != null)
                    {
                        strWhere += string.Format(" AND QUENTHE = '{0}'  ", para.QuenThe);
                    }
                    if (para.MaKH != null)
                    {
                        strWhere += string.Format(" AND MAKH = '{0}' ", para.MaKH);
                    }
                    strQuery = @"SELECT ID,MAKH,TENKHAC,TENKH,DIACHI,MASOTHUE AS MASOTHUE,TRANGTHAI AS TRANGTHAI,CMTND AS CHUNGMINHTHU,
                                DIENTHOAI,EMAIL,TIENNGUYENGIA,SODIEM,TONGTIEN,HANGKHACHHANG,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,NGAYHETHAN,GHICHU
                                MATHE,NGAYDACBIET,NGAYCAPTHE,NGAYSINH,HANGKHACHHANG,HANGKHACHHANGCU,QUENTHE,I_CREATE_DATE,I_CREATE_BY
                                FROM DMKHACHHANG WHERE 1=1 " + strWhere;

                    var data = ctx.Database.SqlQuery<CustomerReport>(strQuery);
                    result = data.ToList();
                    return result;
                }
                catch
                {
                    throw new Exception("Lỗi không thể truy xuất khách hàng");
                }
            }
        }

        private string _convertToArrayCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
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

        public string GetTableName(int year, int period)
        {
            return string.Format("XNT_{0}_KY_{1}", year, period);
        }

    }
}
