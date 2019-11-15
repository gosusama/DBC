using System;
using System.Collections.Generic;
using System.Data;

using Oracle.ManagedDataAccess.Client;
using BTS.API.SERVICE.Services;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.Authorize.AuNhomQuyenChucNang
{
    public interface IAuNhomQuyenChucNangService : IDataInfoService<AU_NHOMQUYEN_CHUCNANG>
    {
        List<AuNhomQuyenChucNangVm.ViewModel> GetByMaNhomQuyen(string phanhe,string manhomquyen);
    }
    public class AuNhomQuyenChucNangService: DataInfoServiceBase<AU_NHOMQUYEN_CHUCNANG>, IAuNhomQuyenChucNangService
    {
        public AuNhomQuyenChucNangService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public List<AuNhomQuyenChucNangVm.ViewModel> GetByMaNhomQuyen(string phanhe, string manhomquyen)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = @"SELECT VC.ID,VC.MANHOMQUYEN,VC.MACHUCNANG,CN.TITLE AS TENCHUCNANG,CN.MENUID AS STATE,CN.SORT AS SOTHUTU,VC.XEM,VC.THEM,VC.SUA,VC.XOA,VC.DUYET,VC.GIAMUA,VC.GIABAN,VC.GIAVON,VC.TYLELAI,VC.BANCHIETKHAU,VC.BANBUON,VC.BANTRALAI 
                            FROM AU_NHOMQUYEN_CHUCNANG VC RIGHT JOIN AU_MENU CN ON VC.MACHUCNANG = CN.MENUID WHERE VC.UNITCODE='" + phanhe+ "' AND CN.UNITCODE='" + phanhe + "' AND VC.MANHOMQUYEN = '" + manhomquyen + "' ORDER BY CN.SORT";
                        using (OracleDataReader oracleDataReader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows) return new List<AuNhomQuyenChucNangVm.ViewModel>();
                            List<AuNhomQuyenChucNangVm.ViewModel> lst = new List<AuNhomQuyenChucNangVm.ViewModel>();
                            while (oracleDataReader.Read())
                            {
                                AuNhomQuyenChucNangVm.ViewModel item = new AuNhomQuyenChucNangVm.ViewModel();
                                item.Id = oracleDataReader["ID"].ToString();
                                item.MACHUCNANG = oracleDataReader["MACHUCNANG"].ToString();
                                item.MANHOMQUYEN = oracleDataReader["MANHOMQUYEN"]?.ToString() ?? string.Empty;
                                item.SOTHUTU = oracleDataReader["SOTHUTU"].ToString();
                                item.STATE = oracleDataReader["STATE"]?.ToString() ?? string.Empty;
                                item.TENCHUCNANG = oracleDataReader["TENCHUCNANG"].ToString();
                                if (oracleDataReader["XEM"] != null)
                                {
                                    item.XEM = oracleDataReader["XEM"].ToString().Equals("1");
                                }
                                if (oracleDataReader["THEM"] != null)
                                {
                                    item.THEM = oracleDataReader["THEM"].ToString().Equals("1");
                                }
                                if (oracleDataReader["SUA"] != null)
                                {
                                    item.SUA = oracleDataReader["SUA"].ToString().Equals("1");
                                }
                                if (oracleDataReader["XOA"] != null)
                                {
                                    item.XOA = oracleDataReader["XOA"].ToString().Equals("1");
                                }
                                if (oracleDataReader["DUYET"] != null)
                                {
                                    item.DUYET = oracleDataReader["DUYET"].ToString().Equals("1");
                                }
                                if (oracleDataReader["GIAMUA"] != null)
                                {
                                    item.GIAMUA = oracleDataReader["GIAMUA"].ToString().Equals("1");
                                }
                                if (oracleDataReader["GIABAN"] != null)
                                {
                                    item.GIABAN = oracleDataReader["GIABAN"].ToString().Equals("1");
                                }
                                if (oracleDataReader["GIAVON"] != null)
                                {
                                    item.GIAVON = oracleDataReader["GIAVON"].ToString().Equals("1");
                                }
                                if (oracleDataReader["TYLELAI"] != null)
                                {
                                    item.TYLELAI = oracleDataReader["TYLELAI"].ToString().Equals("1");
                                }
                                if (oracleDataReader["BANCHIETKHAU"] != null)
                                {
                                    item.BANCHIETKHAU = oracleDataReader["BANCHIETKHAU"].ToString().Equals("1");
                                }
                                if (oracleDataReader["BANBUON"] != null)
                                {
                                    item.BANBUON = oracleDataReader["BANBUON"].ToString().Equals("1");
                                }
                                if (oracleDataReader["BANTRALAI"] != null)
                                {
                                    item.BANTRALAI = oracleDataReader["BANTRALAI"].ToString().Equals("1");
                                }
                                lst.Add(item);
                            }
                            return lst;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
