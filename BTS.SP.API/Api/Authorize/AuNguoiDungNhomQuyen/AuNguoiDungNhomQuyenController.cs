using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuNguoiDungNhomQuyen;
namespace BTS.SP.API.Api.Authorize.AuNguoiDungNhomQuyen
{
    [RoutePrefix("api/Authorize/AuNguoiDungNhomQuyen")]
    [Route("{id?}")]
    [Authorize]
    public class AuNguoiDungNhomQuyenController : ApiController
    {
        private readonly IAuNguoiDungNhomQuyenService _service;
        public AuNguoiDungNhomQuyenController(IAuNguoiDungNhomQuyenService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("GetByUsername/{username}")]
        public IHttpActionResult GetByUsername(string username)
        {
            var _unitcode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(username)) return BadRequest();
            var result = new TransferObj<List<AuNguoiDungNhomQuyenVm.ViewModel>>();
            try
            {
                List<AuNguoiDungNhomQuyenVm.ViewModel> lst =new List<AuNguoiDungNhomQuyenVm.ViewModel>();
                using (var connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            @"SELECT NN.ID,NN.USERNAME,NN.MANHOMQUYEN,NQ.TENNHOMQUYEN FROM AU_NGUOIDUNG_NHOMQUYEN NN INNER JOIN AU_NHOMQUYEN NQ ON NN.MANHOMQUYEN=NQ.MANHOMQUYEN 
                            WHERE NN.UNITCODE='" + _unitcode + "' AND NQ.UNITCODE='" + _unitcode + "' AND NN.USERNAME='"+username+"'";
                        using (OracleDataReader oracleDataReader =
                            command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows)
                            {
                                lst=new List<AuNguoiDungNhomQuyenVm.ViewModel>();
                            }
                            else
                            {
                                while (oracleDataReader.Read())
                                {
                                    AuNguoiDungNhomQuyenVm.ViewModel item =new AuNguoiDungNhomQuyenVm.ViewModel()
                                    {
                                        Id = oracleDataReader["ID"].ToString(),
                                        USERNAME = oracleDataReader["USERNAME"].ToString(),
                                        MANHOMQUYEN = oracleDataReader["MANHOMQUYEN"].ToString(),
                                        TENNHOMQUYEN = oracleDataReader["TENNHOMQUYEN"].ToString()
                                    };
                                    lst.Add(item);
                                }
                            }
                        }
                    }
                }
                result.Status = true;
                result.Data = lst;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("Config")]
        public IHttpActionResult Config(AuNguoiDungNhomQuyenVm.ConfigModel model)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(model.USERNAME)) return BadRequest();
            var result=new TransferObj();
            try
            {
                if (model.LstDelete != null && model.LstDelete.Count > 0)
                {
                    foreach (var item in model.LstDelete)
                    {
                        _service.Delete(item.Id);
                    }
                }
                if (model.LstAdd != null && model.LstAdd.Count > 0)
                {
                    foreach (var item in model.LstAdd)
                    {
                        AU_NGUOIDUNG_NHOMQUYEN obj = new AU_NGUOIDUNG_NHOMQUYEN()
                        {
                            ObjectState = ObjectState.Added,
                            MANHOMQUYEN = item.MANHOMQUYEN,
                            UnitCode = _unitCode,
                            USERNAME = item.USERNAME,
                        };
                        _service.Insert(obj, false);
                    }
                }
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Cập nhật thành công.";
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
    }
}
