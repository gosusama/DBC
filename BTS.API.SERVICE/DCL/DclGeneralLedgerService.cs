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

namespace BTS.API.SERVICE.DCL
{
    public interface IDclGeneralLedgerService : IDetailInfoServiceBase<DclGeneralLedger>
    {
        List<DclGeneralLedgerVm.TongHopBaoCao> ProcedureSoNhatKyChung(DateTime _tungay, DateTime _denngay, string _unitCode);
        List<DclGeneralLedgerVm.DuLieuChiTiet> ProcedureSoChiTietTaiKhoan(string _taiKhoan, DateTime _tungay, DateTime _denngay, string _unitCode);

    }
    public class DclGeneralLedgerService : DetailInfoServiceBase<DclGeneralLedger>, IDclGeneralLedgerService
    {
        public DclGeneralLedgerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Expression<Func<DclGeneralLedger, bool>> GetKeyFilter(DclGeneralLedger instance)
        {
            return x => x.Id == instance.Id;
        }
        public List<DclGeneralLedgerVm.TongHopBaoCao> ProcedureSoNhatKyChung(DateTime _tungay, DateTime _denngay, string _unitCode)
        {
            List<DclGeneralLedgerVm.TongHopBaoCao> result = null;
            var tungay = new OracleParameter("tungay",
                OracleDbType.Date, _tungay,
                ParameterDirection.Input);
            var denngay = new OracleParameter("denngay",
                OracleDbType.Date, _denngay,
                ParameterDirection.Input);
            var unitCode = new OracleParameter("unitcode", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
            using (var ctx = new ERPContext())
            {

                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var str = @"BEGIN BTSOFT_PCK_BAOCAOKETOAN.THBC_SONHATKYCHUNG(:tungay, :denngay, :unitcode); END;";
                        var strQuery = @"select * from TAB_TONGHOPBC_TMP";
                        ctx.Database.ExecuteSqlCommand(str, tungay, denngay, unitCode);
                        result = ctx.Database.SqlQuery<DclGeneralLedgerVm.TongHopBaoCao>(strQuery).ToList();
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }

                return result;
            }

        }

        public List<DclGeneralLedgerVm.DuLieuChiTiet> ProcedureSoChiTietTaiKhoan(string _taiKhoan, DateTime _tungay, DateTime _denngay, string _unitCode)
        {
            List<DclGeneralLedgerVm.DuLieuChiTiet> result = null;
            var tungay = new OracleParameter("tungay",
                OracleDbType.Date, _tungay,
                ParameterDirection.Input);
            var denngay = new OracleParameter("denngay",
                OracleDbType.Date, _denngay,
                ParameterDirection.Input);
            var taiKhoan = new OracleParameter("taikhoan", OracleDbType.NVarchar2, _taiKhoan, ParameterDirection.Input);
            var unitCode = new OracleParameter("unitcode", OracleDbType.NVarchar2, _unitCode, ParameterDirection.Input);
            using (var ctx = new ERPContext())
            {
                using (var dbContextTransaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var str = @"BEGIN BTSOFT_PCK_BAOCAOKETOAN.THBC_SOCHITIETTAIKHOAN(:taikhoan,:tungay, :denngay, :unitcode); END;";

                        var strQuery = @"select * from TAB_TONGHOPBC_TMP";
                        ctx.Database.ExecuteSqlCommand(str, taiKhoan, tungay, denngay, unitCode);
                        result = ctx.Database.SqlQuery<DclGeneralLedgerVm.DuLieuChiTiet>(strQuery).ToList();
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return result;
        }

       
    }
}
