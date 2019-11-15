using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Services;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace BTS.API.SERVICE.Authorize.AuMenu
{
    public interface IAuMenuService : IDataInfoService<AU_MENU>
    {
        //Add function here
        List<AU_MENU> GetAllForConfigNhomQuyen(string manhomquyen, string unitCode);
        List<AU_MENU> GetAllForConfigQuyen(string username, string unitCode);
        List<AU_MENU> GetAllForStarting(string username, string unitCode);
    }
    public class AuMenuService : DataInfoServiceBase<AU_MENU>, IAuMenuService
    {
        public AuMenuService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<AU_MENU, bool>> GetKeyFilter(AU_MENU instance)
        {
            return x => x.MenuId == instance.MenuId;
        }
        public List<AU_MENU> GetAllForConfigNhomQuyen(string manhomquyen, string unitCode)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = @"SELECT B.MENUID, B.MENUIDCHA, B.TITLE,B.SORT FROM AU_MENU B WHERE B.TRANGTHAI = 10 AND B.MENUID 
                                                NOT IN (SELECT A.MACHUCNANG FROM AU_NHOMQUYEN_CHUCNANG A WHERE A.UNITCODE='" + unitCode + "' AND A.MANHOMQUYEN='" + manhomquyen + "') AND B.UNITCODE='" + unitCode + "' AND B.MENUID IS NOT NULL OR B.MENUIDCHA IS NULL ORDER BY B.SORT";
                        using (OracleDataReader oracleDataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows) return null;
                            List<AU_MENU> lst = new List<AU_MENU>();
                            while (oracleDataReader.Read())
                            {
                                AU_MENU item = new AU_MENU();
                                item.MenuId = oracleDataReader["MENUID"].ToString();
                                item.Sort = int.Parse(oracleDataReader["SORT"].ToString());
                                item.MenuIdCha = oracleDataReader["MENUIDCHA"].ToString();
                                item.Title = oracleDataReader["TITLE"].ToString();
                                lst.Add(item);
                            }
                            return lst;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public List<AU_MENU> GetAllForConfigQuyen(string username, string unitCode)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = @"SELECT SYSCHUCNANG.MENUID,SYSCHUCNANG.TITLE,SYSCHUCNANG.SORT,SYSCHUCNANG.MENUIDCHA FROM AU_MENU SYSCHUCNANG 
                                                WHERE SYSCHUCNANG.TRANGTHAI = 10 AND UNITCODE='" + unitCode + "' AND MENUID IS NOT NULL AND SYSCHUCNANG.MENUID NOT IN(SELECT MACHUCNANG FROM AU_NGUOIDUNG_QUYEN WHERE AU_NGUOIDUNG_QUYEN.UNITCODE='" + unitCode + "' AND AU_NGUOIDUNG_QUYEN.USERNAME='" + username + "') OR SYSCHUCNANG.MENUIDCHA IS NULL ORDER BY SYSCHUCNANG.SORT";

                        using (OracleDataReader oracleDataReader =
                            command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows) return null;
                            List<AU_MENU> lst = new List<AU_MENU>();
                            while (oracleDataReader.Read())
                            {
                                AU_MENU item = new AU_MENU();
                                item.MenuId = oracleDataReader["MENUID"].ToString();
                                item.Sort = int.Parse(oracleDataReader["SORT"].ToString());
                                item.MenuIdCha = oracleDataReader["MENUIDCHA"].ToString();
                                item.Title = oracleDataReader["TITLE"].ToString();
                                lst.Add(item);
                            }
                            return lst;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public List<AU_MENU> GetAllForStarting(string username, string unitCode)
        {
            List<AU_MENU> result = new List<AU_MENU>();
            try
            {
                using (var ctx = new ERPContext())
                {
                    using (var dbContextTransaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            var pUserName = new OracleParameter("P_USERNAME", OracleDbType.Varchar2, username, ParameterDirection.Input);
                            var pUnitCode = new OracleParameter("P_UNITCODE", OracleDbType.Varchar2, unitCode, ParameterDirection.Input);
                            var pReturnData = new OracleParameter("RETURN_DATA", OracleDbType.RefCursor, ParameterDirection.Output);
                            var str = "BEGIN TBNETERP.GET_MENU(:P_USERNAME,:P_UNITCODE, :RETURN_DATA); END;";
                            ctx.Database.ExecuteSqlCommand(str, pUserName, pUnitCode, pReturnData);
                            OracleDataReader reader = ((OracleRefCursor)pReturnData.Value).GetDataReader();
                            while (reader.Read())
                            {
                                var item = new AU_MENU()
                                {
                                    MenuId = reader["MENUID"].ToString(),
                                    Sort = int.Parse(reader["SORT"].ToString()),
                                    MenuIdCha = reader["MENUIDCHA"].ToString(),
                                    Title = reader["TITLE"].ToString()
                                };
                                result.Add(item);

                            }
                            return result;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Xảy ra lỗi trong khi chạy Store_Procude. Tự động rollback!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
