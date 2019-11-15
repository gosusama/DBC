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
    [RoutePrefix("api/Md/NhomVatTu")]
    [Route("{id?}")]
    [Authorize]
    public class NhomVatTuController : ApiController
    {
        protected readonly IMdNhomVatTuService _service;
        public NhomVatTuController(IMdNhomVatTuService service)
        {
            _service = service;
        }
        [Route("GetSelectDataFromSQLByMaLoai/{code}")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataFromSQLByMaLoai(string code)
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmnhomhang;
                return data.Where(x => x.Manganh == code).Select(x => new ChoiceObj { Value = x.Manhomhang, Text = x.Tennhomhang }).ToList();
            }

        }
        
        [Route("GetAll_NhomVatTu")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> GetAll_NhomVatTu()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaNhom, Text = x.MaNhom + "|" + x.TenNhom, Description = x.TenNhom, ExtendValue = x.UnitCode,Parent = x.MaLoaiVatTu, Id = x.Id }).ToList();
            return Ok(result);
        }

        [Route("GetAll_NhomVatTuRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> GetAll_NhomVatTuRoot()
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
                        cmd.CommandText = "SELECT MANHOMVTU,TENNHOMVT,UNITCODE FROM DM_NHOMVATTU WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MANHOMVTU = dataReader["MANHOMVTU"] != null ? dataReader["MANHOMVTU"].ToString() : "";
                                string TENNHOMVT = dataReader["TENNHOMVT"] != null ? dataReader["TENNHOMVT"].ToString() : "";
                                _DTO.Value = MANHOMVTU;
                                _DTO.Text = MANHOMVTU + "|" + TENNHOMVT;
                                _DTO.Description = TENNHOMVT;
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

        [Route("GetSelectDataByUnitCode")]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public IList<ChoiceObj> GetSelectDataByUnitCode()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaNhom, Text = x.TenNhom, Id = x.Id }).ToList();
        }

        [Route("GetForNvByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> GetForNvByCode(string code)
        {
            MdMerchandiseVm.Dto result = null;
            var unitCode = _service.GetCurrentUnitCode();
            //var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaVatTu.ToUpper() == code.ToUpper());//&& x.UnitCode == unitCode);
            var service = new ProcedureService<MdNhomVatTuVm.Dto>();
            var data = ProcedureCollection.GetNhomVatTu(new BTS.API.ENTITY.ERPContext(), code, unitCode);
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
        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdNhomVatTuVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdNhomVatTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdNhomVatTu().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
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

                result.Data = Mapper.Map<PagedObj<MdNhomVatTu>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdNhomVatTuVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdNhomVatTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdNhomVatTu().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdNhomVatTu().MaNhom),
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
        [ResponseType(typeof(MdNhomVatTu))]
        [CustomAuthorize(Method = "THEM", State = "nhomVatTu")]
        public async Task<IHttpActionResult> Post(MdNhomVatTuVm.Dto instance)
        {
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<MdNhomVatTu>();
            if (instance.IsGenCode)
                instance.MaNhom = _service.SaveCode();
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
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaNhom == instance.MaNhom && x.UnitCode.StartsWith(_parentUnitCode));
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
                var data = Mapper.Map<MdNhomVatTuVm.Dto, MdNhomVatTu>(instance);
                data.ICreateBy = _service.GetClaimsPrincipal().Identity.Name;
                data.ICreateDate = DateTime.Now;
                var item = _service.Insert(data);
                //_service.SaveCode();
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

        [ResponseType(typeof(MdNhomVatTu))]
        [CustomAuthorize(Method = "XOA", State = "nhomVatTu")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdNhomVatTu instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdNhomVatTu))]
        [CustomAuthorize(Method = "XEM", State = "nhomVatTu")]
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
        [CustomAuthorize(Method = "SUA", State = "nhomVatTu")]
        public async Task<IHttpActionResult> Put(string id, MdNhomVatTu instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdNhomVatTu>();
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

        [Route("GetByCode/{code}")]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaNhom == code);// && x.UnitCode == unitCode);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }

        [Route("GetSelectByMaLoai/{maloai}")]
        public List<ChoiceObj> GetSelectByMaLoai(string maloai)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var instance = _service.Repository.DbSet.Where(x => x.MaLoaiVatTu == maloai && x.UnitCode.StartsWith(maDonViCha));//&& x.UnitCode == unitCode);
            if (instance == null)
            {
                return null;
            }
            return instance.Select(x => new ChoiceObj { Value = x.MaNhom, Text = x.MaNhom + " | " + x.TenNhom, Id = x.Id }).ToList();
        }

        [Route("GetSelectByMaLoaiRoot/{maloai}")]
        public IHttpActionResult GetSelectByMaLoaiRoot(string maloai)
        {
            var result = new TransferObj<List<ChoiceObj>>();
            string rootUnitcode = ConfigurationManager.AppSettings["rootUnitCode"];
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
                        cmd.CommandText = "SELECT MALOAIVATTU,MANHOMVTU,TENNHOMVT,UNITCODE FROM DM_NHOMVATTU WHERE TRANGTHAI = 10 AND MALOAIVATTU = '"+ maloai + "' AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MALOAIVATTU = dataReader["MALOAIVATTU"] != null ? dataReader["MALOAIVATTU"].ToString() : "";
                                string MANHOMVTU = dataReader["MANHOMVTU"] != null ? dataReader["MANHOMVTU"].ToString() : "";
                                string TENNHOMVT = dataReader["TENNHOMVT"] != null ? dataReader["TENNHOMVT"].ToString() : "";
                                _DTO.Value = MANHOMVTU;
                                _DTO.Text = MANHOMVTU + " | " + TENNHOMVT;
                                _DTO.Description = TENNHOMVT;
                                _DTO.ExtendValue = dataReader["UNITCODE"] != null ? dataReader["UNITCODE"].ToString() : "";
                                _DTO.Parent = MALOAIVATTU;
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

        [Route("FilterGroupMerchandiseCodes/{maNhom}")]
        [ResponseType(typeof(MdNhomVatTu))]
        public MdNhomVatTu FilterGroupMerchandiseCodes(string maNhom)
        {
            var typeMer = new MdNhomVatTu();
            if (string.IsNullOrEmpty(maNhom))
            {
                typeMer = null;
            }
            else
            {
                maNhom = maNhom.ToUpper();
                maNhom = maNhom.Trim();
                var unitCode = _service.GetCurrentUnitCode();
                typeMer = _service.Repository.DbSet.Where(x => x.MaNhom == maNhom).FirstOrDefault(x => x.UnitCode == unitCode);
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

        [Route("GetNewCode")]
        public string GetNewCode()
        {
            return _service.BuildCode();
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
