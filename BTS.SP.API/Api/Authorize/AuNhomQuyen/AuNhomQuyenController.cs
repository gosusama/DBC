using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuNhomQuyen;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.Authorize.AuNhomQuyen
{
    [RoutePrefix("api/Authorize/AuNhomQuyen")]
    [Route("{id?}")]
    [Authorize]
    public class AuNhomQuyenController : ApiController
    {
        private readonly IAuNhomQuyenService _service;
        public AuNhomQuyenController(IAuNhomQuyenService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("GetNhomQuyenConfig/{username}")]
        public async Task<IHttpActionResult> GetNhomQuyenConfig(string username)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var result = await _service.GetNhomQuyenConfigByUsername(_unitCode, username);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetAllForConfigNhomQuyen/{username}")]
        public IHttpActionResult GetAllForConfigNhomQuyen(string username)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(username)) return BadRequest();
            var result = new TransferObj<List<ChoiceObj>>();
            try
            {
                List<ChoiceObj> lst = new List<ChoiceObj>();
                using (
                    OracleConnection connection =
                        new OracleConnection(new ERPContext().Database.Connection.ConnectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText =
                            @"SELECT MANHOMQUYEN,TENNHOMQUYEN FROM AU_NHOMQUYEN WHERE TRANGTHAI=1 AND UNITCODE='"+_unitCode+"' " +
                            "AND MANHOMQUYEN NOT IN(SELECT MANHOMQUYEN FROM AU_NGUOIDUNG_NHOMQUYEN WHERE AU_NGUOIDUNG_NHOMQUYEN.USERNAME='" + username + "')";
                        using (
                            OracleDataReader oracleDataReader =
                                command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!oracleDataReader.HasRows)
                            {
                                lst = new List<ChoiceObj>();
                            }
                            else
                            {
                                while (oracleDataReader.Read())
                                {
                                    lst.Add(new ChoiceObj()
                                    {
                                        Text = oracleDataReader["TENNHOMQUYEN"].ToString(),
                                        Value = oracleDataReader["MANHOMQUYEN"].ToString(),
                                        Selected = false
                                    });
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

        [Route("Select_Page")]
        [HttpPost]
        [CustomAuthorize(Method = "XEM", State = "auGroup")]
        public IHttpActionResult Select_Page(JObject jsonData)
        {

            var _unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuNhomQuyenVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<AU_NHOMQUYEN>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new AU_NHOMQUYEN().UnitCode),
                    Value = _unitCode,
                    Method = FilterMethod.EqualTo
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = filterResult.Value;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [Route("Add")]
        [HttpPost]
        [ResponseType(typeof(AU_NHOMQUYEN))]
        [CustomAuthorize(Method = "THEM", State = "auGroup")]
        public IHttpActionResult Add(AU_NHOMQUYEN instance)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj<AU_NHOMQUYEN>();
            try
            {
                instance.UnitCode = _unitCode;
                var item = _service.Insert(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Cập nhật thành công.";
                result.Data = item;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
            }
            return Ok(result);
        }

        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("Update/{id}")]
        [CustomAuthorize(Method = "SUA", State = "auGroup")]
        public IHttpActionResult Update(string id, AU_NHOMQUYEN instance)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj<AU_NHOMQUYEN>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
            }
            else
            {
                try
                {
                    instance.UnitCode = _unitCode;
                    var item = _service.Update(instance);
                    _service.UnitOfWork.Save();
                    result.Status = true;
                    result.Message = "Cập nhật thành công.";
                    result.Data = item;
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                }
            }
            return Ok(result);
        }

        [ResponseType(typeof(AU_NHOMQUYEN))]
        [HttpDelete]
        [Route("DeleteItem/{id}")]
        [CustomAuthorize(Method = "XOA", State = "auGroup")]
        public IHttpActionResult DeleteItem(string Id)
        {
            AU_NHOMQUYEN instance = _service.FindById(Id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                if (_service.DeleteNhomQuyen(Id))
                {
                    _service.Delete(instance.Id);
                    _service.UnitOfWork.Save();
                    return Ok(instance);
                }
                return InternalServerError();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
