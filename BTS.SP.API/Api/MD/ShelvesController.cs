using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ASYNC.DatabaseContext;
using BTS.API.ENTITY;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Shelves")]
    [Route("{id?}")]
    [Authorize]
    public class ShelvesController : ApiController
    {
        protected readonly IMdShelvesService _service;
        protected readonly IMdMerchandiseService _serviceMerchandise;
        public ShelvesController(IMdShelvesService service, IMdMerchandiseService serviceMerchandise)
        {
            _service = service;
            _serviceMerchandise = serviceMerchandise;
        }
        [Route("GetSelectDataFromSQL")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataFromSQL()
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmkehang;
                return data.Select(x => new ChoiceObj
                {
                    Value = x.Makehang,
                    Text = x.Tenkehang
                }).ToList();
            }
        }
        [Route("GetAll_Shelves")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_Shelves()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaKeHang, Text = x.MaKeHang + "|" + x.TenKeHang, Description = x.TenKeHang, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }

        [Route("GetAll_ShelvesRoot")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_ShelvesRoot()
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
                        cmd.CommandText = "SELECT MAKEHANG,TENKEHANG,UNITCODE FROM DM_KEHANG WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MAKEHANG = dataReader["MAKEHANG"] != null ? dataReader["MAKEHANG"].ToString() : "";
                                string TENKEHANG = dataReader["TENKEHANG"] != null ? dataReader["TENKEHANG"].ToString() : "";
                                _DTO.Value = MAKEHANG;
                                _DTO.Text = MAKEHANG + "|" + TENKEHANG;
                                _DTO.Description = TENKEHANG;
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

        /// <returns></returns>
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var parentUnitCode = _service.GetParentUnitCode();
            return data.Where(x=>x.UnitCode.StartsWith(parentUnitCode)).Select(x => new ChoiceObj { Value = x.MaKeHang, Text = x.TenKeHang, Id = x.Id }).ToList();
        }


        [Route("PostListHangHoa")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PostListHangHoa(List<MdMerchandise> instance)
        {
            var result = new TransferObj<MdMerchandise>();
            foreach (var value in instance)
            {
                try
                {
                    var item = _serviceMerchandise.Update(value);
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                    return Ok(result);
                }
            }
            _serviceMerchandise.UnitOfWork.Save();
            result.Status = true;
            return Ok(result);
        }


        [Route("UploadFile")]
        [AllowAnonymous]
        [HttpPost]
        [CustomAuthorize(Method = "SUA", State = "shelves")]
        public async Task<IHttpActionResult> UploadFile()
        {
            var ctx = new ERPContext();
            var result = new List<MdMerchandise>();
            string path = HttpContext.Current.Server.MapPath("/Upload/KiemKe/");
            HttpRequest request = HttpContext.Current.Request;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (request.Files.Count > 0)
            {
                try
                {

                    HttpPostedFile file = request.Files[0];
                    file.SaveAs(path + file.FileName);
                    //kiem tra ton tai file
                    if (File.Exists(path + file.FileName))
                    {
                        if (file.FileName.Contains(".txt"))
                        {
                            //doc du lieu tu file text
                            string[] lines = System.IO.File.ReadAllLines(path + file.FileName);
                            foreach (string line in lines)
                            {
                                string[] data = line.Split(',');
                                string MaVatTu = data[0].ToString();
                                var vatTu = ctx.MdMerchandises.Where(x => x.MaVatTu == MaVatTu).FirstOrDefault();
                                if (vatTu != null)
                                {
                                    result.Add(vatTu);
                                }                          
                            }
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception ex)
                {
                    return InternalServerError();
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "shelves")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdShelvesVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdShelves>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdShelves().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdShelves().MaKeHang),
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

                result.Data = Mapper.Map<PagedObj<MdShelves>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "shelves")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdShelvesVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdShelves>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdShelves().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdShelves().MaKeHang),
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
        [ResponseType(typeof(MdShelves))]
        [CustomAuthorize(Method = "THEM", State = "shelves")]
        public async Task<IHttpActionResult> Post(MdShelves instance)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj<MdShelves>();
            try
            {
                var item = _service.Insert(instance);
                item.MaKeHang = _service.BuildCode_DM(TypeMasterData.KEHANG.ToString(), _unitCode, true);
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

        [ResponseType(typeof(MdShelves))]
        [CustomAuthorize(Method = "XOA", State = "shelves")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdShelves instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdShelves))]
        [CustomAuthorize(Method = "XEM", State = "shelves")]
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
        [CustomAuthorize(Method = "SUA", State = "shelves")]
        public async Task<IHttpActionResult> Put(string id, MdShelves instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdShelves>();
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

        [Route("GetByCode/{code}")]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaKeHang == code && x.UnitCode== _unitCode);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [Route("GetNewInstance")]
        public MdShelves GetNewInstance()
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
