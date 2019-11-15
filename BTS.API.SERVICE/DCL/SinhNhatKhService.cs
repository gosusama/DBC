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

namespace BTS.API.SERVICE.DCL
{
    public interface ISinhNhatKhService : IDetailInfoServiceBase<DclCloseout>
    {
        string GetTableName(int year, int period);
        List<CustomerReport1> ReportSinhNhatKh(ParameterSinhNhatKh para); 
        List<CustomerReport1> ReportDacBietKh(ParameterSinhNhatKh para);
    }
    public class SinhNhatKhService : DetailInfoServiceBase<DclCloseout>, ISinhNhatKhService
    {
        public SinhNhatKhService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        //public List<MdCustomer> ReportSinhNhatKh(ParameterSinhNhatKh para)
        //{
        //    List<MdCustomer> result = new List<MdCustomer>();
        //    try
        //    {
        //        using (var ctx = new ERPContext())
        //        {
        //            using (var dbContextTransaction = ctx.Database.BeginTransaction())
        //            {
        //                try
        //                {
        //                    var pUnitCode = new OracleParameter("pUnitCode", OracleDbType.NVarchar2, para.UnitCode, ParameterDirection.Input);
        //                    var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, para.WareHouseCodes, ParameterDirection.Input);
        //                    //var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
        //                    //var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);

        //                    var pFromDay = new OracleParameter("pFromDay", OracleDbType.Int32, para.FromDay, ParameterDirection.Input);
        //                    var pToDay = new OracleParameter("pToDay", OracleDbType.Int32, para.ToDay, ParameterDirection.Input);
        //                    var pMonthOfBirth = new OracleParameter("pMonthOfBirth", OracleDbType.Int32, para.MonthOfBirth, ParameterDirection.Input);

        //                    var pStateTypeMoney = new OracleParameter("pStateTypeMoney", OracleDbType.Int32, para.StateTypeMoney, ParameterDirection.Input);
        //                    var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
        //                    var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
        //                    //var pMaThe = new OracleParameter("pMaThe", OracleDbType.NVarchar2, para.MaThe, ParameterDirection.Input);
        //                    //var pDiaChi = new OracleParameter("pDiaChi", OracleDbType.NVarchar2, para.DiaChi, ParameterDirection.Input);
        //                    //var pNgayHetHan = new OracleParameter("pNgayHetHan", OracleDbType.Date, para.NgayHetHan.Date, ParameterDirection.Input);
        //                    var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
        //                    //var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_SINHNHAT_KH(:pUnitCode, :pWareHouseCode, :pFromDate,:pToDate,:pStateTypeMoney, :pFromMoney,:pToMoney, :pMaThe,:pDiaChi, :outRef); END;";
        //                    //ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDate, pToDate,pStateTypeMoney, pFromMoney, pToMoney, pMaThe, pDiaChi, outRef);

        //                    var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_SINHNHAT_KH(:pUnitCode, :pWareHouseCode, :pFromDay,:pToDay,:pMonthOfBirth, :pStateTypeMoney, :pFromMoney,:pToMoney,  :outRef); END;";
        //                    ctx.Database.ExecuteSqlCommand(str, pUnitCode, pWareHouseCode, pFromDay, pToDay, pMonthOfBirth, pStateTypeMoney, pFromMoney, pToMoney, outRef);
                       
        //                    OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();

        //                    while (reader.Read())
        //                    {
        //                        var item = reader.Cast<MdCustomer>().ToList();
        //                        if(item.Count>0)
        //                            result.Add(item[0]);
        //                    }
        //                    //if(para.StateExpiredCard !=null && para.StateExpiredCard == 1 && para.NgayHetHan != null)
        //                    //{
        //                    //    var lstItem = result.Where(x => x.NgayHetHan <= para.NgayHetHan).ToList();
        //                    //    lstItem.ForEach(x => result.Remove(x));
        //                    //}

        //                    dbContextTransaction.Commit();
        //                    return result;
        //                }
        //                catch (Exception e)
        //                {
        //                    dbContextTransaction.Rollback();
        //                    throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
        //                }
        //            }
        //        }
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<CustomerReport1> ReportSinhNhatKh(ParameterSinhNhatKh para)
        {
            List<CustomerReport1> result = new List<CustomerReport1>();
            using (var ctx = new ERPContext())
            {
                try
                {
                    string strWhere = string.Empty;
                    string strQuery = string.Empty;
                    if (para.UnitCode != null)
                    {
                        strWhere += string.Format(" AND UNITCODE LIKE SUBSTR('{0}',0,3) || '%' ", para.UnitCode);
                    }
                    
                    if (para.MonthOfBirth != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(MONTH FROM NGAYSINH) = {0} ", para.MonthOfBirth);
                    }
                    if (para.FromDay != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(DAY FROM NGAYSINH) >= {0} ", para.FromDay);
                    }
                    if (para.ToDay != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(DAY FROM NGAYSINH) < {0} ", para.ToDay);
                    }
                    if (para.StateTypeMoney != null  && para.StateTypeMoney==2)
                    {
                        if (para.FromMoney != null)
                        {
                            strWhere += string.Format(" AND TIENNGUYENGIA >= {0} ", para.FromMoney);
                        }
                        if (para.ToMoney != null)
                        {
                            strWhere += string.Format(" AND TIENNGUYENGIA <= {0} ", para.ToMoney);
                        }
                    }
                    else
                    {
                        if (para.FromMoney != null)
                        {
                            strWhere += string.Format(" AND TONGTIEN >= {0} ", para.FromMoney);
                        }
                        if (para.ToMoney != null)
                        {
                            strWhere += string.Format(" AND TONGTIEN < {0} ", para.ToMoney);
                        }
                    }
                    
                    strQuery = @"SELECT ID,MAKH,TENKHAC,TENKH,DIACHI,MASOTHUE AS MASOTHUE,TRANGTHAI AS TRANGTHAI,CMTND AS CHUNGMINHTHU,
                                DIENTHOAI,EMAIL,TIENNGUYENGIA,SODIEM,TONGTIEN,HANGKHACHHANG,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,NGAYHETHAN,GHICHU
                                MATHE,NGAYDACBIET,NGAYCAPTHE,NGAYSINH,HANGKHACHHANG,HANGKHACHHANGCU,QUENTHE,I_CREATE_DATE,I_CREATE_BY
                                FROM DMKHACHHANG WHERE 1=1 " + strWhere;

                    var data = ctx.Database.SqlQuery<CustomerReport1>(strQuery);
                    result = data.ToList();
                    return result;
                }
                catch
                {
                    throw new Exception("Lỗi không thể truy xuất khách hàng");
                }
            }
        }


        public List<CustomerReport1> ReportDacBietKh(ParameterSinhNhatKh para)
        {
            List<CustomerReport1> result = new List<CustomerReport1>();
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
                    if (para.MonthOfBirth != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(MONTH FROM NGAYDACBIET) = {0} ", para.MonthOfBirth);
                    }
                    if (para.FromDay != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(DAY FROM NGAYDACBIET) >= {0} ", para.FromDay);
                    }
                    if (para.ToDay != null)
                    {
                        strWhere += string.Format(" AND EXTRACT(DAY FROM NGAYDACBIET) <= {0} ", para.ToDay);
                    }
                    if (para.StateTypeMoney != null && para.StateTypeMoney == 2)
                    {
                        if (para.FromMoney != null)
                        {
                            strWhere += string.Format(" AND TIENNGUYENGIA >= {0} ", para.FromMoney);
                        }
                        if (para.ToMoney != null)
                        {
                            strWhere += string.Format(" AND TIENNGUYENGIA < {0} ", para.ToMoney);
                        }
                    }
                    else
                    {
                        if (para.FromMoney != null)
                        {
                            strWhere += string.Format(" AND TONGTIEN >= {0} ", para.FromMoney);
                        }
                        if (para.ToMoney != null)
                        {
                            strWhere += string.Format(" AND TONGTIEN < {0} ", para.ToMoney);
                        }
                    }

                    strQuery = @"SELECT ID,MAKH,TENKHAC,TENKH,DIACHI,MASOTHUE AS MASOTHUE,TRANGTHAI AS TRANGTHAI,CMTND AS CHUNGMINHTHU,
                                DIENTHOAI,EMAIL,TIENNGUYENGIA,SODIEM,TONGTIEN,HANGKHACHHANG,LOAIKHACHHANG,SODIEM,TIENNGUYENGIA,TIENSALE,NGAYHETHAN,GHICHU
                                MATHE,NGAYDACBIET,NGAYCAPTHE,NGAYSINH,HANGKHACHHANG,HANGKHACHHANGCU,QUENTHE,I_CREATE_DATE,I_CREATE_BY
                                FROM DMKHACHHANG WHERE 1=1 " + strWhere;

                    var data = ctx.Database.SqlQuery<CustomerReport1>(strQuery);
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
