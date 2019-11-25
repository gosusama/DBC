using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Services;
using BTS.API.SERVICE;
using BTS.API.SERVICE.Helper;
using System.Configuration;
using System.Linq;
using BTS.API.ENTITY;

namespace BTS.API.SERVICE.Authorize.AuNhomQuyen
{
    public interface IAuNhomQuyenService : IDataInfoService<AU_NHOMQUYEN>
    {
        Task<TransferObj<List<ChoiceObj>>> GetNhomQuyenConfigByUsername(string phanhe, string username);
        bool DeleteNhomQuyen(string id);
    }
    public class AuNhomQuyenService : DataInfoServiceBase<AU_NHOMQUYEN>, IAuNhomQuyenService
    {
        public AuNhomQuyenService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        protected override Expression<Func<AU_NHOMQUYEN, bool>> GetKeyFilter(AU_NHOMQUYEN instance)
        {
            return x => x.MANHOMQUYEN == instance.MANHOMQUYEN;
        }

        public async Task<TransferObj<List<ChoiceObj>>> GetNhomQuyenConfigByUsername(string phanhe, string username)
        {
            TransferObj<List<ChoiceObj>> response = new TransferObj<List<ChoiceObj>>();
            try
            {
                using (var connection = new OracleConnection(ConfigurationManager.ConnectionStrings["KNQ.Connection"].ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            @"SELECT MANHOMQUYEN,TENNHOMQUYEN FROM AU_NHOMQUYEN WHERE TRANGTHAI=10 AND UNITCODE='" + phanhe + "' AND MANHOMQUYEN NOT IN(SELECT MANHOMQUYEN FROM AU_NGUOIDUNG_NHOMQUYEN WHERE AU_NGUOIDUNG_NHOMQUYEN.UNITCODE='" + phanhe + "' AND AU_NGUOIDUNG_NHOMQUYEN.USERNAME='" + username + "')";
                        using (var oracleDataReader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows) return new TransferObj<List<ChoiceObj>>()
                            {
                                Status = false,
                                Data = new List<ChoiceObj>()
                            };
                            List<ChoiceObj> lst = new List<ChoiceObj>();
                            while (oracleDataReader.Read())
                            {
                                lst.Add(new ChoiceObj()
                                {
                                    Text = oracleDataReader["TENNHOMQUYEN"].ToString(),
                                    Value = oracleDataReader["MANHOMQUYEN"].ToString(),
                                    Selected = false
                                });
                            }
                            response.Status = true;
                            response.Data = lst;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public bool DeleteNhomQuyen(string id)
        {
            var insatance = UnitOfWork.Repository<AU_NHOMQUYEN>().DbSet.Where(x => x.Id == id).FirstOrDefault();
            if (insatance == null)
            {
                return false;
            }

            var auNhomQuyenChucNang = UnitOfWork.Repository<AU_NHOMQUYEN_CHUCNANG>().DbSet.Where(o => o.MANHOMQUYEN == insatance.MANHOMQUYEN).ToList();
            foreach (AU_NHOMQUYEN_CHUCNANG anqcn in auNhomQuyenChucNang)
            {
                anqcn.ObjectState = ObjectState.Deleted;
            }

            var auNguoiDungNhomQuyen = UnitOfWork.Repository<AU_NGUOIDUNG_NHOMQUYEN>().DbSet.Where(o => o.MANHOMQUYEN == insatance.MANHOMQUYEN).ToList();
            foreach (AU_NGUOIDUNG_NHOMQUYEN andnq in auNguoiDungNhomQuyen)
            {
                andnq.ObjectState = ObjectState.Deleted;
            }

            return true;

        }
    }
}
