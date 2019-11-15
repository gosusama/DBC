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
    public interface IDoanhSoMoiService : IDetailInfoServiceBase<DclCloseout>
    {
        string GetTableName(int year, int period);
        List<CustomDoanhSoMoiReport> ReportDoanhSoMoi(ParameterDoanhSoMoi para);
        List<CustomDoanhSoMoiReportDetails> ReportDoanhSoMoiDetails(ParameterDoanhSoMoiDetails para);

    }
    public class DoanhSoMoiService : DetailInfoServiceBase<DclCloseout>, IDoanhSoMoiService
    {
        public DoanhSoMoiService(IUnitOfWork unitOfWork)
                : base(unitOfWork)
        {
        }

        public List<CustomDoanhSoMoiReport> ReportDoanhSoMoi(ParameterDoanhSoMoi para)
        {
            List<CustomDoanhSoMoiReport> result = new List<CustomDoanhSoMoiReport>();
            //unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, _convertToArrayCondition(para.WareHouseCodes), ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_BUY_CUSTOMER_NEW(:pUnitCode,:pFromDate,:pToDate, :pWareHouseCode, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pFromDate, pToDate, pWareHouseCode, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            
                            while (reader.Read())
                            {
                                decimal soKhach,soGiaoDich,doanhThu,traLai,chietKhau,thucThu;
                              
                                var isSoKhach = decimal.TryParse(reader["SOKHACH"].ToString(), out soKhach);
                                var isSoGiaoDich = decimal.TryParse(reader["SOGIAODICH"].ToString(), out soGiaoDich);
                                var isDoanhThu = decimal.TryParse(reader["DOANHTHU"].ToString(), out doanhThu);
                                var isTraLai = decimal.TryParse(reader["TRALAI"].ToString(), out traLai);
                                var isChietKhau = decimal.TryParse(reader["CHIETKHAU"].ToString(), out chietKhau);     
                                var isThucThu = decimal.TryParse(reader["THUCTHU"].ToString(), out thucThu);

                                var item = new CustomDoanhSoMoiReport()
                                {
                                    MaDonVi = reader["MADONVI"].ToString(),
                                    SoKhach = isSoKhach ? soKhach : 0,
                                    SoGiaoDich = isSoGiaoDich?soGiaoDich:0,
                                    DoanhThu=isDoanhThu?doanhThu:0,
                                    TraLai=isTraLai?traLai:0,
                                    ChietKhau=isChietKhau?chietKhau:0,
                                    ThucThu=isThucThu?thucThu:0
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

        public List<CustomDoanhSoMoiReportDetails> ReportDoanhSoMoiDetails(ParameterDoanhSoMoiDetails para)
        {
            List<CustomDoanhSoMoiReportDetails> result = new List<CustomDoanhSoMoiReportDetails>();
            //unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, _convertToArrayCondition(para.WareHouseCodes), ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);

                            var pMaDonVi = new OracleParameter("pMaDonVi", OracleDbType.NVarchar2, para.MaDonVi, ParameterDirection.Input);

                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_BUY_DETAILS_NEW(:pUnitCode,:pFromDate,:pToDate, :pWareHouseCode,:pMaDonVi, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pFromDate, pToDate, pWareHouseCode, pMaDonVi, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();

                            while (reader.Read())
                            {
                                
                                DateTime ngayTao,ngaySinh, ngayDacBiet, ngayCapThe, ngayHetHan;
                                var isNgayTao = DateTime.TryParse(reader["NGAYPHATSINH"].ToString(), out ngayTao);
                                var isNgaySinh = DateTime.TryParse(reader["NGAYSINH"].ToString(), out ngaySinh);
                                var isNgayDacBiet = DateTime.TryParse(reader["NGAYDACBIET"].ToString(), out ngayDacBiet);
                                var isNgayCapThe = DateTime.TryParse(reader["NGAYCAPTHE"].ToString(), out ngayCapThe);
                                var isNgayHetHan = DateTime.TryParse(reader["NGAYHETHAN"].ToString(), out ngayHetHan);


                                var item = new CustomDoanhSoMoiReportDetails()
                                {
                                    NgayTao = isNgayTao ? ngayTao : new DateTime(0001, 01, 01),
                                    MaKH = reader["MAKHACHHANG"].ToString(),
                                    TenKH = reader["TENKH"].ToString(),
                                    DienThoai = reader["DIENTHOAI"].ToString(),
                                    NgaySinh = isNgaySinh ? ngaySinh : new DateTime(0001, 1, 1),
                                    NgayDacBiet = isNgayDacBiet ? ngayDacBiet : new DateTime(0001, 01, 01),
                                    Email = reader["EMAIL"].ToString(),
                                    MaThe=reader["MATHE"].ToString(),
                                    NgayCapThe = isNgayCapThe ? ngayCapThe : new DateTime(0001, 01, 01),
                                    NgayHetHan = isNgayHetHan ? ngayHetHan : new DateTime(0001, 01, 01),
                                    MaDonVi = reader["MADONVI"].ToString(),
                                   
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
