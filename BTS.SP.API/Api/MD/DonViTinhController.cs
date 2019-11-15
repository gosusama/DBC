using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Authorize.Utils;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/DonViTinh")]
    [Route("{id?}")]
    [Authorize]
    public class DonViTinhController : ApiController
    {
        private readonly IMdDonViTinhService _service;
        public DonViTinhController(IMdDonViTinhService service)
        {
            _service = service;
        }
        [Route("GetAll_DonViTinh")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "donViTinh")]
        public async Task<IHttpActionResult> GetAll_DonViTinh()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaDVT, Text = x.MaDVT + "|" + x.TenDVT, Description = x.TenDVT, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }
        [Route("GetAll_DonViTinhRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "donViTinh")]
        public IHttpActionResult GetAll_DonViTinhRoot()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            List<ChoiceObj> lstResult = new List<ChoiceObj>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings["LTT.Connection"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = connection;
                        cmd.InitialLONGFetchSize = 1000;
                        cmd.CommandText = "SELECT MADVT,TENDVT,UNITCODE FROM DM_DONVITINH WHERE TRANGTHAI = 10 AND UNITCODE = '"+ rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MADVT = dataReader["MADVT"] != null ? dataReader["MADVT"].ToString() : "";
                                string TENDVT = dataReader["TENDVT"] != null ? dataReader["TENDVT"].ToString() : "";
                                _DTO.Value = MADVT;
                                _DTO.Text = MADVT + "|" + TENDVT;
                                _DTO.Description = TENDVT;
                                _DTO.ExtendValue = dataReader["UNITCODE"] != null ? dataReader["UNITCODE"].ToString() : "";
                                lstResult.Add(_DTO);
                            }
                        }
                        if (lstResult.Count > 0)
                        {
                            result.Status = true;
                            result.Data = lstResult;
                        }
                        else
                        {
                            result.Status = false;
                            result.Message = "NotFound";
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "NotFound";
                    }
                }
                catch
                {
                    result.Status = false;
                    result.Message = "NotFound";
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return Ok(result);
        }
        
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "donViTinh")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdDonViTinhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdDonViTinh>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdDonViTinh().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdDonViTinh>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewInstance")]
        public MdDonViTinh GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "donViTinh")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdDonViTinhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdDonViTinh>>();
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdDonViTinh().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
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
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [ResponseType(typeof(MdDonViTinh))]
        [CustomAuthorize(Method = "THEM", State = "donViTinh")]
        public async Task<IHttpActionResult> Post(MdDonViTinhVm.Dto instance)
        {
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<MdDonViTinh>();
            if (instance.IsGenCode)
                instance.MaDVT = _service.SaveCode();
            else
            {
                if (instance.MaDVT == "")
                {
                    result.Status = false;
                    result.Message = "Mã không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaDVT == instance.MaDVT && x.UnitCode.StartsWith(_parentUnitCode));
                    if (exist != null)
                    {
                        result.Status = false;
                        result.Message = "Đã tồn tại mã loại này";
                        return Ok(result);
                    }
                }
            }
            try
            {
                var data = Mapper.Map<MdDonViTinhVm.Dto, MdDonViTinh>(instance);
                //instance.MaDVT = _service.SaveCode();
                var item = _service.Insert(data);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Thêm mới thành công";
                result.Data = item;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "donViTinh")]
        public async Task<IHttpActionResult> Put(string id, MdDonViTinh instance)
        {
            var result = new TransferObj<MdDonViTinh>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                var item = _service.Update(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                result.Message = "Cập nhật thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [CustomAuthorize(Method = "XOA", State = "donViTinh")]
        [ResponseType(typeof(MdDonViTinh))]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdDonViTinh instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                _service.Delete(instance.Id);
                await _service.UnitOfWork.SaveAsync();
                return Ok(instance);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [ResponseType(typeof(MdDonViTinh))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [ResponseType(typeof(MdDonViTinh))]
        [Route("GetByCode/{code}")]
        public IHttpActionResult GetByCode(string code)
        {
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaDVT == code);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        private bool IsExists(string id)
        {
            return _service.Repository.Find(id) != null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Repository.DataContext.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
