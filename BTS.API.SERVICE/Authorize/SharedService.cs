using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY;
using BTS.API.SERVICE.Services;
using System.Linq.Expressions;
using BTS.API.ENTITY.Authorize;

namespace BTS.API.SERVICE.Authorize
{
    public interface ISharedService : IDataInfoService<AU_MENU>
    {
        RoleState GetRoleStateByMaChucNang(string unitCode, string username,string machucnang);
    }
    public class SharedService: DataInfoServiceBase<AU_MENU>, ISharedService
    {
        public SharedService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override Expression<Func<AU_MENU, bool>> GetKeyFilter(AU_MENU instance)
        {
            return x => x.MenuId == instance.MenuId;
        }
        public RoleState GetRoleStateByMaChucNang(string unitCode,string username, string machucnang)
        {
            RoleState roleState = new RoleState();
            roleState.STATE = machucnang;
            if (username == "admin" || username == "hanghanh")
            {
                roleState = new RoleState()
                {
                    STATE = machucnang,
                    View = true,
                    Approve = true,
                    Delete = true,
                    Add = true,
                    Edit = true,
                    Giamua = true,
                    Giaban = true,
                    Giavon = true,
                    Tylelai = true,
                    Banchietkhau = true,
                    Banbuon = true,
                    Bantralai = true
                };
            }
            else
            {
                using (var connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            @"SELECT AU_NHOMQUYEN_CHUCNANG.XEM,AU_NHOMQUYEN_CHUCNANG.THEM,AU_NHOMQUYEN_CHUCNANG.SUA,AU_NHOMQUYEN_CHUCNANG.XOA,AU_NHOMQUYEN_CHUCNANG.DUYET,AU_NHOMQUYEN_CHUCNANG.GIAMUA,AU_NHOMQUYEN_CHUCNANG.GIABAN,AU_NHOMQUYEN_CHUCNANG.GIAVON,AU_NHOMQUYEN_CHUCNANG.TYLELAI,AU_NHOMQUYEN_CHUCNANG.BANCHIETKHAU,AU_NHOMQUYEN_CHUCNANG.BANBUON,AU_NHOMQUYEN_CHUCNANG.BANTRALAI FROM AU_NHOMQUYEN_CHUCNANG WHERE UNITCODE='" + unitCode + "' AND MACHUCNANG='" + machucnang +
                            "' AND MANHOMQUYEN IN (SELECT MANHOMQUYEN FROM AU_NGUOIDUNG_NHOMQUYEN WHERE UNITCODE='" + unitCode + "' AND USERNAME='" +
                            username + "') UNION SELECT AU_NGUOIDUNG_QUYEN.XEM,AU_NGUOIDUNG_QUYEN.THEM,AU_NGUOIDUNG_QUYEN.SUA,AU_NGUOIDUNG_QUYEN.XOA,AU_NGUOIDUNG_QUYEN.DUYET,AU_NGUOIDUNG_QUYEN.GIAMUA,AU_NGUOIDUNG_QUYEN.GIABAN,AU_NGUOIDUNG_QUYEN.GIAVON,AU_NGUOIDUNG_QUYEN.TYLELAI,AU_NGUOIDUNG_QUYEN.BANCHIETKHAU,AU_NGUOIDUNG_QUYEN.BANBUON,AU_NGUOIDUNG_QUYEN.BANTRALAI " +
                            "FROM AU_NGUOIDUNG_QUYEN WHERE AU_NGUOIDUNG_QUYEN.UNITCODE='" + unitCode + "' AND AU_NGUOIDUNG_QUYEN.MACHUCNANG='" + machucnang + "' AND AU_NGUOIDUNG_QUYEN.USERNAME='" + username + "'";
                        using (OracleDataReader oracleDataReader = command.ExecuteReader())
                        {
                            if (!oracleDataReader.HasRows)
                            {
                                roleState = new RoleState()
                                {
                                    STATE = string.Empty,
                                    View = false,
                                    Approve = false,
                                    Delete = false,
                                    Add = false,
                                    Edit = false,
                                    Giamua = false,
                                    Giaban = false,
                                    Giavon = false,
                                    Tylelai = false,
                                    Banchietkhau = false,
                                    Banbuon = false,
                                    Bantralai = false
                                };
                            }

                            else
                            {
                                roleState.STATE = machucnang;
                                while (oracleDataReader.Read())
                                {
                                    int objXem = Int32.Parse(oracleDataReader["XEM"].ToString());
                                    if (objXem == 1)
                                    {
                                        roleState.View = true;
                                    }
                                    int objThem = Int32.Parse(oracleDataReader["THEM"].ToString());
                                    if (objThem == 1)
                                    {
                                        roleState.Add = true;
                                    }
                                    int objSua = Int32.Parse(oracleDataReader["SUA"].ToString());
                                    if (objSua == 1)
                                    {
                                        roleState.Edit = true;
                                    }
                                    int objXoa = Int32.Parse(oracleDataReader["XOA"].ToString());
                                    if (objXoa == 1)
                                    {
                                        roleState.Delete = true;
                                    }
                                    int objDuyet = Int32.Parse(oracleDataReader["DUYET"].ToString());
                                    if (objDuyet == 1)
                                    {
                                        roleState.Approve = true;
                                    }
                                    int objGiamua = Int32.Parse(oracleDataReader["GIAMUA"].ToString());
                                    if (objGiamua == 1)
                                    {
                                        roleState.Giamua = true;
                                    }
                                    int objGiaban = Int32.Parse(oracleDataReader["GIABAN"].ToString());
                                    if (objGiaban == 1)
                                    {
                                        roleState.Giaban = true;
                                    }
                                    int objGiavon = Int32.Parse(oracleDataReader["GIAVON"].ToString());
                                    if (objGiavon == 1)
                                    {
                                        roleState.Giavon = true;
                                    }
                                    int objTylelai = Int32.Parse(oracleDataReader["TYLELAI"].ToString());
                                    if (objTylelai == 1)
                                    {
                                        roleState.Tylelai = true;
                                    }
                                    int objBanchietkhau = Int32.Parse(oracleDataReader["BANCHIETKHAU"].ToString());
                                    if (objBanchietkhau == 1)
                                    {
                                        roleState.Banchietkhau = true;
                                    }
                                    int objBanbuon = Int32.Parse(oracleDataReader["BANBUON"].ToString());
                                    if (objBanbuon == 1)
                                    {
                                        roleState.Banbuon = true;
                                    }
                                    int objBantralai = Int32.Parse(oracleDataReader["BANTRALAI"].ToString());
                                    if (objBantralai == 1)
                                    {
                                        roleState.Bantralai = true;
                                    }
                                }
                                MemoryCacheHelper.Add(unitCode + "|" + machucnang + "|" + username, roleState, DateTimeOffset.Now.AddHours(6));
                            }
                        }
                    }
                }
            }
            return roleState;
        }
    }
}
