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
    public interface IDoanhSoSnService : IDetailInfoServiceBase<DclCloseout>
    {
        string GetTableName(int year, int period);
        List<CustomDoanhSoSnReport> ReportDoanhSoSn(ParameterDoanhSoSn para);
      
    }
    public class DoanhSoSnService : DetailInfoServiceBase<DclCloseout>, IDoanhSoSnService
    {
        public DoanhSoSnService(IUnitOfWork unitOfWork)
                : base(unitOfWork)
        {
        }

        public List<CustomDoanhSoSnReport> ReportDoanhSoSn(ParameterDoanhSoSn para)
        {
            List<CustomDoanhSoSnReport> result = new List<CustomDoanhSoSnReport>();
            //unitCode = GetCurrentUnitCode();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {

                            para.CustomerCodes = _convertToArrayCondition(para.CustomerCodes);
                            para.WareHouseCodes = _convertToArrayCondition(para.WareHouseCodes);
                            var pCustomerCodes = new OracleParameter("pCustomer", OracleDbType.NVarchar2, para.CustomerCodes, ParameterDirection.Input);
                            var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
                            var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);
                            
                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.REPORT_CUSTOMER_BY_BIRTHDAY(:pFromDate,:pToDate,:pUnitCode, :pWareHouseCode, :pFromMoney,:pToMoney,:pCustomerCodes, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pFromDate, pToDate,pUnitCode, pWareHouseCode,  pFromMoney, pToMoney,pCustomerCodes, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            
                            while (reader.Read())
                            {
                                decimal soTien;
                                DateTime ngaySinh;
                                
                                var istongTien = decimal.TryParse(reader["SOTIEN"].ToString(), out soTien);
                                
                                var isNgaySn = DateTime.TryParse(reader["NGAYSINHNHAT"].ToString(), out ngaySinh);

                                var item = new CustomDoanhSoSnReport()
                                {
                                    MaKH = reader["MAKHACHHANG"].ToString(),
                                    TenKH = reader["TENKHACHHANG"].ToString(),
                                    
                                    DienThoai = reader["SODIENTHOAI"].ToString(),
                                   
                                    MaThe = reader["MATHEVIP"].ToString(),
                                    
                                    NgaySinh = isNgaySn ? ngaySinh : new DateTime(0001, 1, 1),
                                    
                                    MaDonVi=reader["MADONVI"].ToString(),
                                    SoTien = istongTien?soTien:0,
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
