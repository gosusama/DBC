using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using AutoMapper;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Packaging")]
    [Route("{id?}")]
    [Authorize]
    public class PackagingController : ApiController
    {
        protected readonly IMdPackagingService _service;
        public PackagingController(IMdPackagingService service)
        {
            _service = service;
        }
        [Route("GetSelectDataFromSQL")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataFromSQL()
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmdonvitinh;
                return data.Select(x => new ChoiceObj
                {
                    Value = x.Madvtinh,
                    Text = "Đv Lẻ: " + x.Donvile + "-Đv lớn: " + x.Donvilon,
                    ExtendValue = x.Donvilon
                }).ToList();
            }
        }
        [Route("GetAll_Packaging")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "packaging")]
        public async Task<IHttpActionResult> GetAll_Packaging()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaBaoBi, Text = x.MaBaoBi + "|" + x.TenBaoBi, Description = x.TenBaoBi, ExtendValue = x.SoLuong.ToString(), Id = x.Id }).ToList();
            return Ok(result);
        }


        [Route("GetAll_PackagingRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "packaging")]
        public async Task<IHttpActionResult> GetAll_PackagingRoot()
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
                        cmd.CommandText = "SELECT MABAOBI,TENBAOBI,UNITCODE FROM DM_BAOBI WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MABAOBI = dataReader["MABAOBI"] != null ? dataReader["MABAOBI"].ToString() : "";
                                string TENBAOBI = dataReader["TENBAOBI"] != null ? dataReader["TENBAOBI"].ToString() : "";
                                _DTO.Value = MABAOBI;
                                _DTO.Text = MABAOBI + "|" + TENBAOBI;
                                _DTO.Description = TENBAOBI;
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

        [Route("GetNewInstance")]
        public MdPackaging GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        [Route("PackagingCtl_GetSelectDataByUnitCode")]
        [HttpGet]
        public IList<ChoiceObj> PackagingCtl_GetSelectDataByUnitCode()
        {
            var result = new List<ChoiceObj>();
            var _ParentUnitCode = _service.GetParentUnitCode();
            var tempData = _service.Repository.DbSet.Where(x => x.UnitCode.StartsWith(_ParentUnitCode)).ToList();
            foreach (var item in tempData)
            {
                result.Add(new ChoiceObj()
                {
                    Value = item.MaBaoBi,
                    Text = item.TenBaoBi,
                    ExtendValue = item.SoLuong.ToString(),
                    Id = item.Id
                });
            }
            return result;
        }

        [Route("GetByCode/{code}")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "packaging")]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var result = new TransferObj<MdPackaging>();
            var _unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaBaoBi == code && x.UnitCode.Equals(_unitCode));
            if (instance == null)
            {
                result.Data = null;
                result.Status = false;
            }
            else
            {
                result.Data = instance;
                result.Status = true;
            }
            return Ok(result);
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "packaging")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdPackagingVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdPackaging>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdPackaging().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdPackaging().MaBaoBi),
                        Method = OrderMethod.ASC
                    }
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdPackaging>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        /// <summary>
        /// Query entity
        /// POST
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PackagingCtl_GetSelectDataByUnitCode_page")]
        public async Task<IHttpActionResult> PackagingCtl_GetSelectDataByUnitCode_page(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdPackagingVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdPackaging>>();
            var _ParentUnitCode = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdPackaging().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = _ParentUnitCode
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
        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdPackaging))]
        [CustomAuthorize(Method = "THEM", State = "packaging")]
        public async Task<IHttpActionResult> Post(MdPackaging instance)
        {
            var result = new TransferObj<MdPackaging>();
            try
            {
                instance.ICreateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.ICreateDate = DateTime.Now;
                var item = _service.Insert(instance);
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

        [ResponseType(typeof(MdPackaging))]
        [CustomAuthorize(Method = "XOA", State = "packaging")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdPackaging instance = await _service.Repository.FindAsync(id);
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
        /// <summary>
        /// Get by id
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdPackaging))]
        [CustomAuthorize(Method = "XEM", State = "packaging")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        /// <summary>
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "packaging")]
        public async Task<IHttpActionResult> Put(string id, MdPackaging instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdPackaging>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                instance.IUpdateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.IUpdateDate = DateTime.Now;
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
        //[Route("GetByCode/{code}")]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetByCode(string code)
        //{
        //    var _ParentUnitCode = _service.GetParentUnitCode();
        //    var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaBaoBi == code && x.UnitCode.StartsWith(_ParentUnitCode));// && x.UnitCode == unitCode);
        //    if (instance == null) return NotFound();
        //    return Ok(instance);
        //}

        [Route("PackagingCtl_addNew")]
        [HttpGet]
        public MdPackaging PackagingCtl_addNew()
        {
            return _service.CreateNewInstance();
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
