using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [RoutePrefix("api/Md/HangKh")]
    [Route("{id?}")]
    [Authorize]
    public class HangKhController:ApiController
    {
        private readonly IMdHangKhService _service;
        public HangKhController(IMdHangKhService service)
        {
            _service = service;
        }
        [Route("GetAll_HangKh")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_HangKh()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaHangKh, Text = x.MaHangKh + "|" + x.TenHangKh, Description = x.TenHangKh, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }
        /// <summary>
        /// GetAll_HangKhachHangRoot
        /// </summary>
        /// <returns></returns>
        [Route("GetAll_HangKhachHangRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "hangKh")]
        public IHttpActionResult GetAll_HangKhachHangRoot()
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
                        cmd.CommandText = "SELECT MAHANGKH,TENHANGKH FROM DM_HANGKHACHHANG WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                ChoiceObj _DTO = new ChoiceObj();
                                string MAHANGKH = dataReader["MAHANGKH"] != null ? dataReader["MAHANGKH"].ToString() : "";
                                string TENHANGKH = dataReader["TENHANGKH"] != null ? dataReader["TENHANGKH"].ToString() : "";
                                _DTO.Value = MAHANGKH;
                                _DTO.Text = MAHANGKH + "|" + TENHANGKH;
                                _DTO.Description = MAHANGKH;
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

        /// <summary>
        /// GetSelectData
        /// </summary>
        /// <returns></returns>
        [Route("GetSelectData")]
        [CustomAuthorize(Method = "XEM", State = "hangKh")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var _parent = _service.GetParentUnitCode();
            var temp = data.Where(x=>x.UnitCode.StartsWith(_parent)).ToList();
            var result = new List<ChoiceObj>();
            foreach (var item in temp)
            {
                result.Add(new ChoiceObj()
                {
                    Id = item.Id,
                    ExtendValue = item.TyLeGiamGia.ToString(),
                    Text = item.TenHangKh,
                    Value = item.MaHangKh,
                    Description = string.Format("{0}-{1}%", item.TenHangKh, item.TyLeGiamGia)
                });
            }
            return result;
        }
        //[Route("GetSelectDataFromSQL")]
        //[AllowAnonymous]
        //public IList<ChoiceObj> GetSelectDataFromSQL()
        //{
        //    using (var ctx = new DBCSQL())
        //    {
        //        var data = ctx.TDS_Dmvat;
        //        return data.Select(x => new ChoiceObj
        //        {
        //            Value = x.Mavat,
        //            Text = x.Tenvat,
        //            ExtendValue = "" + x.Vat + ""
        //        }).ToList();
        //    }
        //}
        //[Route("GetTaxFromSQL/{maVat}")]
        //[AllowAnonymous]
        //public TDS_VatVm GetTaxFromSQL(string maVat)
        //{
        //    using (var ctx = new DBCSQL())
        //    {
        //        Mapper.CreateMap<TDS_Dmvat, TDS_VatVm>();

        //        var item = ctx.TDS_Dmvat.FirstOrDefault(x => x.Mavat == maVat);
        //        var result = Mapper.Map<TDS_Dmvat, TDS_VatVm>(item);
        //        return result;
        //    }
        //}
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "hangKh")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdHangKhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdHangKH>>();
            var unitCode = _service.GetCurrentUnitCode();
            var _parent = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdHangKH().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = _parent
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdHangKH>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "hangKh")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdHangKhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdHangKH>>();
            var unitCode = _service.GetCurrentUnitCode();
            var _parent = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdHangKH().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = _parent
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
        [ResponseType(typeof(MdHangKH))]
        [CustomAuthorize(Method = "THEM", State = "hangKh")]
        public async Task<IHttpActionResult> Post(MdHangKH instance)
        {
            var result = new TransferObj<MdHangKH>();

            try
            {
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
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "hangKh")]
        public async Task<IHttpActionResult> Put(string id, MdHangKH instance)
        {
            var result = new TransferObj<MdHangKH>();
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
        [ResponseType(typeof(MdHangKH))]
        [CustomAuthorize(Method = "XOA", State = "hangKh")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdHangKH instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdHangKH))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        //[ResponseType(typeof(MdHangKH))]
        //[Route("GetByCode/{code}")]
        //public IHttpActionResult GetByCode(string code)
        //{
        //    var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaHangKh == code);
        //    if (instance == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(instance);
        //}
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