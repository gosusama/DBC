using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Oracle.ManagedDataAccess.Client;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuNguoiDungQuyen;
namespace BTS.SP.API.Api.Authorize.AuNguoiDungQuyen
{
    [RoutePrefix("api/Authorize/AuNguoiDungQuyen")]
    [Route("{id?}")]
    [Authorize]
    public class AuNguoiDungQuyenController : ApiController
    {
        private readonly IAuNguoiDungQuyenService _service;
        public AuNguoiDungQuyenController(IAuNguoiDungQuyenService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetByUsername/{username}")]
        public IHttpActionResult GetByUsername(string username)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(username)) return BadRequest();
            var result = new TransferObj<List<AuNguoiDungQuyenVm.ViewModel>>();
            try
            {
                List<AuNguoiDungQuyenVm.ViewModel> lst = new List<AuNguoiDungQuyenVm.ViewModel>();
                using (var connection = new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            @"SELECT NQ.ID,NQ.USERNAME,NQ.MACHUCNANG,SYSC.TITLE AS TENCHUCNANG,SYSC.SORT AS SOTHUTU,NQ.XEM,NQ.THEM,NQ.SUA,NQ.XOA,NQ.DUYET,NQ.GIAMUA,NQ.GIABAN,NQ.GIAVON,NQ.TYLELAI,NQ.BANCHIETKHAU,NQ.BANBUON,NQ.BANTRALAI
                            FROM AU_NGUOIDUNG_QUYEN NQ INNER JOIN AU_MENU SYSC ON NQ.MACHUCNANG = SYSC.MENUID WHERE NQ.UNITCODE='" + _unitCode + "' " +
                            "AND SYSC.UNITCODE='" + _unitCode + "' AND NQ.USERNAME='" + username + "' ORDER BY SYSC.SORT";
                        using (OracleDataReader oracleDataReader =
                            command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows)
                            {
                                lst = new List<AuNguoiDungQuyenVm.ViewModel>();
                            }
                            else
                            {
                                while (oracleDataReader.Read())
                                {
                                    AuNguoiDungQuyenVm.ViewModel item = new AuNguoiDungQuyenVm.ViewModel()
                                    {
                                        Id = oracleDataReader["ID"].ToString(),
                                        USERNAME = oracleDataReader["USERNAME"].ToString(),
                                        MACHUCNANG = oracleDataReader["MACHUCNANG"].ToString(),
                                        TENCHUCNANG = oracleDataReader["TENCHUCNANG"].ToString(),
                                        SOTHUTU = oracleDataReader["SOTHUTU"].ToString(),
                                        XEM = oracleDataReader["XEM"].ToString().Equals("1"),
                                        THEM = oracleDataReader["THEM"].ToString().Equals("1"),
                                        SUA = oracleDataReader["SUA"].ToString().Equals("1"),
                                        XOA = oracleDataReader["XOA"].ToString().Equals("1"),
                                        DUYET = oracleDataReader["DUYET"].ToString().Equals("1"),
                                        GIAMUA = oracleDataReader["GIAMUA"].ToString().Equals("1"),
                                        GIABAN = oracleDataReader["GIABAN"].ToString().Equals("1"),
                                        GIAVON = oracleDataReader["GIAVON"].ToString().Equals("1"),
                                        TYLELAI = oracleDataReader["TYLELAI"].ToString().Equals("1"),
                                        BANCHIETKHAU = oracleDataReader["BANCHIETKHAU"].ToString().Equals("1"),
                                        BANBUON = oracleDataReader["BANBUON"].ToString().Equals("1"),
                                        BANTRALAI = oracleDataReader["BANTRALAI"].ToString().Equals("1"),
                                        UnitCode = _unitCode
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
        public IHttpActionResult Config(AuNguoiDungQuyenVm.ConfigModel model)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(model.USERNAME)) return BadRequest();
            var result = new TransferObj();
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
                        AU_NGUOIDUNG_QUYEN obj = new AU_NGUOIDUNG_QUYEN()
                        {
                            ObjectState = ObjectState.Added,
                            USERNAME = item.USERNAME,
                            MACHUCNANG = item.MACHUCNANG,
                            XOA = item.XOA,
                            DUYET = item.DUYET,
                            SUA = item.SUA,
                            THEM = item.THEM,
                            GIAMUA = item.GIAMUA,
                            GIABAN = item.GIABAN,
                            GIAVON = item.GIAVON,
                            TYLELAI = item.TYLELAI,
                            BANCHIETKHAU = item.BANCHIETKHAU,
                            BANBUON = item.BANBUON,
                            BANTRALAI = item.BANTRALAI,
                            TRANGTHAI = 1,
                            XEM = item.XEM,
                            UnitCode = _unitCode
                        };
                        _service.Insert(obj, false);
                    }
                }
                if (model.LstEdit != null && model.LstEdit.Count > 0)
                {
                    foreach (var item in model.LstEdit)
                    {
                        AU_NGUOIDUNG_QUYEN obj = new AU_NGUOIDUNG_QUYEN()
                        {
                            Id = item.Id,
                            UnitCode = _unitCode,
                            USERNAME = item.USERNAME,
                            MACHUCNANG = item.MACHUCNANG,
                            ObjectState = ObjectState.Modified,
                            TRANGTHAI = 1,
                            XEM = item.XEM,
                            THEM = item.THEM,
                            SUA = item.SUA,
                            XOA = item.XOA,
                            DUYET = item.DUYET,
                            GIAMUA = item.GIAMUA,
                            GIABAN = item.GIABAN,
                            GIAVON = item.GIAVON,
                            TYLELAI = item.TYLELAI,
                            BANCHIETKHAU = item.BANCHIETKHAU,
                            BANBUON = item.BANBUON,
                            BANTRALAI = item.BANTRALAI
                        };
                        _service.Update(obj);
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
