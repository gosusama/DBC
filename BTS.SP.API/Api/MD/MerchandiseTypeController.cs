using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/MerchandiseType")]
    [Route("{id?}")]
    [Authorize]
    
    public class MerchandiseTypeController : ApiController
    {
        protected readonly IMdMerchandiseTypeService _service;
        public MerchandiseTypeController(IMdMerchandiseTypeService service)
        {
            _service = service;
        }

        [Route("GetSelectAll")]
        public IList<ChoiceObj> GetSelectAll()
        {
            return _service.GetSelectSort();
        }
        [Route("GetSelectDataFromSQL")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataFromSQL()
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmnganhhang;
                return data.Select(x => new ChoiceObj
                {
                    Value = x.Manganh,
                    Text = x.Tennganh
                }).ToList();
            }
        }
        [Route("GetAll_MerchandiseType")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "merchandiseType")]
        public async Task<IHttpActionResult> GetAll_MerchandiseType()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaLoaiVatTu, Text = x.MaLoaiVatTu + "|" + x.TenLoaiVatTu, Description = x.TenLoaiVatTu, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }
        [Route("GetAll_MerchandiseTypeRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "merchandiseType")]
        public async Task<IHttpActionResult> GetAll_MerchandiseTypeRoot()
        {
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
            var result = new TransferObj<List<ChoiceObj>>();
            List<ChoiceObj> lstResult = new List<ChoiceObj>();
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
                        cmd.CommandText = "SELECT MALOAIVATTU,TENLOAIVT,UNITCODE FROM DM_LOAIVATTU WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MALOAIVATTU = dataReader["MALOAIVATTU"] != null ? dataReader["MALOAIVATTU"].ToString() : "";
                                string TENLOAIVT = dataReader["TENLOAIVT"] != null ? dataReader["TENLOAIVT"].ToString() : "";
                                _DTO.Value = MALOAIVATTU;
                                _DTO.Text = MALOAIVATTU + "|" + TENLOAIVT;
                                _DTO.Description = TENLOAIVT;
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
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            DbSet<MdMerchandiseType> data = _service.Repository.DbSet;
            string maDonViCha = _service.GetParentUnitCode();
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaLoaiVatTu, Text = x.TenLoaiVatTu, Id = x.Id }).ToList();
        }

        [Route("GetSelectDataByUnitCode")]
        public IList<ChoiceObj> GetSelectDataByUnitCode()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var data = _service.Repository.DbSet;
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaLoaiVatTu, Text = x.TenLoaiVatTu, Id = x.Id }).ToList();
        }
        [Route("GetNewInstance")]
        public MdMerchandiseType GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
               /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "merchandiseType")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseTypeVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandiseType>>();
            var maDonViCha = _service.GetParentUnitCode();
            //var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdMerchandiseType().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdMerchandiseType().MaLoaiVatTu),
                        Method = OrderMethod.ASC
                    }
                }
            };
            try
            {
                if (filtered.IsAdvance)
                {
                    filtered.AdvanceData.LoadGeneralParam(filtered.Summary);
                }
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdMerchandiseType>, PagedObj<ChoiceObj>>
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
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "merchandiseType")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdMerchandiseTypeVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdMerchandiseType>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdMerchandiseType().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdMerchandiseType().MaLoaiVatTu),
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
        [ResponseType(typeof(MdMerchandiseType))]
        [CustomAuthorize(Method = "THEM", State = "merchandiseType")]
        public async Task<IHttpActionResult> Post(MdMerchandiseTypeVm.Dto instance)
        {
            var result = new TransferObj<MdMerchandiseType>();
            var _parentUnitCode = _service.GetParentUnitCode();
            if (instance.IsGenCode) instance.MaLoaiVatTu = _service.SaveCode();
            else
            {
                if (instance.MaLoaiVatTu == "")
                {
                    result.Status = false;
                    result.Message = "Mã không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaLoaiVatTu == instance.MaLoaiVatTu && x.UnitCode.StartsWith(_parentUnitCode));
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
                var data = Mapper.Map<MdMerchandiseTypeVm.Dto, MdMerchandiseType>(instance);
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
        [Route("GetForNvByCode/{code}")]
        public async Task<IHttpActionResult> GetForNvByCode(string code)
        {
            MdMerchandiseVm.Dto result = null;
            var unitCode = _service.GetCurrentUnitCode();
            //var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu.ToUpper() == code.ToUpper());//&& x.UnitCode == unitCode);
            var service = new ProcedureService<MdMerchandiseTypeVm.Dto>();
            var data = ProcedureCollection.GetMerchandiseType(new BTS.API.ENTITY.ERPContext(), code, unitCode);
            if (data != null && data.Count() >= 1)
            {
                var items = data.ToList();


                return Ok(items[0]);
            }
            else
            {
                return NotFound();
            }

        }

        [ResponseType(typeof(MdMerchandiseType))]
        [CustomAuthorize(Method = "XOA", State = "merchandiseType")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdMerchandiseType instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdMerchandiseType))]
        [CustomAuthorize(Method = "XEM", State = "merchandiseType")]
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
        [CustomAuthorize(Method = "SUA", State = "merchandiseType")]
        public async Task<IHttpActionResult> Put(string id, MdMerchandiseType instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdMerchandiseType>();
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

        [Route("FilterTypeMerchandiseCodes/{maLoai}")]
        [ResponseType(typeof(MdMerchandiseType))]
        public MdMerchandiseType FilterTypeMerchandiseCodes(string maLoai)
        {
            MdMerchandiseType typeMer = new MdMerchandiseType();
            if (string.IsNullOrEmpty(maLoai))
            {
                typeMer = null;
            }
            else
            {
                maLoai = maLoai.ToUpper();
                maLoai = maLoai.Trim();
                string unitCode = _service.GetCurrentUnitCode();
                typeMer = _service.Repository.DbSet.Where(x => x.MaLoaiVatTu == maLoai).FirstOrDefault(x => x.UnitCode == unitCode);
                if (typeMer != null)
                {
                    return typeMer;
                }
                else
                {
                    typeMer = null;
                }
            }
            return typeMer;
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
