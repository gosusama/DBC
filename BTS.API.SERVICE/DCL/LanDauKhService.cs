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
    public interface ILanDauKhService : IDetailInfoServiceBase<DclCloseout>
    {
        string GetTableName(int year, int period);
        List<CustomLanDauKhReportLevel2> ReportLanDauKh(ParameterLanDauKh para);
        
    }
    public class LanDauKhService : DetailInfoServiceBase<DclCloseout>, ILanDauKhService
    {
        public LanDauKhService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        public List<CustomLanDauKhReportLevel2> ReportLanDauKh(ParameterLanDauKh para)
        {
            List<CustomLanDauKhReportLevel2> result = new List<CustomLanDauKhReportLevel2>();
            List<CustomLanDauKhReport> data = new List<CustomLanDauKhReport>();
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
                            
                            //var pWareHouseCode = new OracleParameter("pWareHouseCode", OracleDbType.NVarchar2, "DV2-CH2-KBL", ParameterDirection.Input);
                            var pFromDate = new OracleParameter("pFromDate", OracleDbType.Date, para.FromDate.Date, ParameterDirection.Input);
                            var pToDate = new OracleParameter("pToDate", OracleDbType.Date, para.ToDate.Date, ParameterDirection.Input);

                            var pFromMoney = new OracleParameter("pFromMoney", OracleDbType.Decimal, para.FromMoney, ParameterDirection.Input);
                            var pToMoney = new OracleParameter("pToMoney", OracleDbType.Decimal, para.ToMoney, ParameterDirection.Input);
                            var outRef = new OracleParameter("outRef", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.CUSTOMERCARE.REPORT_BUY_ONETIME(:pUnitCode, :pFromDate,:pToDate,:pFromMoney,:pToMoney,:pWareHouseCode, :outRef); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUnitCode, pFromDate, pToDate, pFromMoney, pToMoney, pWareHouseCode, outRef);
                            OracleDataReader reader = ((OracleRefCursor)outRef.Value).GetDataReader();
                            while (reader.Read())
                            {
                                DateTime ngayPhatSinh;
                                var isNgayPs = DateTime.TryParse(reader["NGAYPHATSINH"].ToString(), out ngayPhatSinh);
                                
                                decimal soLuong,donGia,thanhTien,chietKhau;
                                var isSoLuong = decimal.TryParse(reader["SOLUONG"].ToString(), out soLuong);
                                var isDonGia = decimal.TryParse(reader["DONGIA"].ToString(), out donGia);
                                var isThanhTien = decimal.TryParse(reader["THANHTIEN"].ToString(), out thanhTien);
                                var isChietKhau = decimal.TryParse(reader["CHIETKHAU"].ToString(), out chietKhau);
                                var item = new CustomLanDauKhReport()
                                {

                                    NgayPhatSinh = isNgayPs ? ngayPhatSinh : new DateTime(0001, 1, 1),
                                    MaGiaoDichQuayPk = reader["MAGIAODICHQUAYPK"].ToString(),
                                    TenKhachHang = reader["TENKHACHHANG"].ToString(),
                                    MaVatTu = reader["MAVATTU"].ToString(),
                                    DienThoai = reader["DIENTHOAI"].ToString(),
                                    MaKho = reader["MAKHO"].ToString(),
                                    SoLuong = isSoLuong ? soLuong : 0,
                                    DonGia = isDonGia ? donGia : 0,
                                    ThanhTien = isThanhTien ? thanhTien : 0,
                                    ChietKhau = isChietKhau ? chietKhau : 0,
                                };
                                data.Add(item);
                            }
                          //  dbContextTransaction.Commit();
                        }
                        catch
                        {
                            dbContextTransaction.Rollback();
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }

                var groupBy = data.GroupBy(x => x.MaGiaoDichQuayPk).ToList();
                foreach (var item in groupBy)
                {
                    CustomLanDauKhReportLevel2 obj = new CustomLanDauKhReportLevel2();
                    obj.MaGiaoDichQuayPk = item.Key;
                 
                    List<CustomLanDauKhReport> lst = data.Where(x => x.MaGiaoDichQuayPk == obj.MaGiaoDichQuayPk).ToList();
                    var groupByDkLoc = lst.GroupBy(x => new { x.MaGiaoDichQuayPk}).Select(group => new CustomLanDauKhReport()
                                            {
                                              //  MaVatTu = group.FirstOrDefault(x => x.MaVatTu),
                                                //Ma = group.Key.Ma,
                                                MaGiaoDichQuayPk = group.Key.MaGiaoDichQuayPk,
                                                //
                                                ChietKhau = group.Sum(a => a.ChietKhau),
                                                ThanhTien = group.Sum(a => a.ThanhTien),
                                                
                                            }
                                            ).ToList();
                    obj.ThanhTien = lst.Sum(a => a.ThanhTien);
                    obj.ChietKhau = lst.Sum(a => a.ChietKhau);
                    obj.DataDetails.AddRange(lst);
                    result.Add(obj);
                }
                return result;    
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
