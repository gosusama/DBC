using AutoMapper;
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
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Tax")]
    [Route("{id?}")]
    [Authorize]
    public class TaxController : ApiController
    {
        private readonly IMdTaxService _service;
        public TaxController(IMdTaxService service)
        {
            _service = service;
        }
        [Route("GetSelectDataFromSQL")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public IList<ChoiceObj> GetSelectDataFromSQL()
        {
            using (var ctx = new DBCSQL())
            {
                var data = ctx.TDS_Dmvat;
                return data.Select(x => new ChoiceObj
                {
                    Value = x.Mavat,
                    Text = x.Tenvat,
                    ExtendValue = "" + x.Vat + ""
                }).ToList();
            }
        }
        [Route("GetTaxFromSQL/{maVat}")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public TDS_VatVm GetTaxFromSQL(string maVat)
        {
            using (var ctx = new DBCSQL())
            {
                var item = ctx.TDS_Dmvat.FirstOrDefault(x => x.Mavat == maVat);
                var result = Mapper.Map<TDS_Dmvat, TDS_VatVm>(item);
                return result;
            }
        }
        [Route("GetAll_TaxRoot")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public IHttpActionResult GetAll_TaxRoot()
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
                        cmd.CommandText = "SELECT MALOAITHUE,LOAITHUE,TYGIA,TAIKHOANKT,TRANGTHAI,UNITCODE FROM DM_LOAITHUE WHERE TRANGTHAI = 10 AND UNITCODE = '" + rootUnitcode + "'";
                        cmd.CommandType = CommandType.Text;
                        OracleDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                int TRANGTHAI = 0;
                                decimal TYGIA = 0;
                                ChoiceObj _DTO = new ChoiceObj();
                                string MALOAITHUE = dataReader["MALOAITHUE"] != null ? dataReader["MALOAITHUE"].ToString() : "";
                                string LOAITHUE = dataReader["LOAITHUE"] != null ? dataReader["LOAITHUE"].ToString() : "";
                                int.TryParse(dataReader["TRANGTHAI"] != null ? dataReader["TRANGTHAI"].ToString() : "", out TRANGTHAI);
                                decimal.TryParse(dataReader["TYGIA"] != null ? dataReader["TYGIA"].ToString() : "", out TYGIA);
                                _DTO.Value = MALOAITHUE;
                                _DTO.Text = MALOAITHUE + "|" + LOAITHUE;
                                _DTO.Description = LOAITHUE;
                                _DTO.ExtendValue = TYGIA.ToString();
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

        [Route("GetAll_Tax")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public async Task<IHttpActionResult> GetAll_Tax()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaLoaiThue, Text = x.MaLoaiThue + "|" + x.LoaiThue, Description = x.LoaiThue, ExtendValue = x.TaxRate.ToString(), Id = x.Id }).ToList();
            return Ok(result);
        }



        [Route("GetAll_TyGia")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public async Task<IHttpActionResult> GetAll_TyGia()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaLoaiThue, Text = x.TaxRate.ToString(), Description = x.LoaiThue, ExtendValue = x.TaxRate.ToString(), Id = x.Id }).ToList();
            return Ok(result);
        }

        [Route("GetTaxByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public async Task<IHttpActionResult> GetTaxByCode(string code)
        {
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaLoaiThue == code);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }

        [Route("GetSelectDataByUnitCode")]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public IList<ChoiceObj> GetSelectDataByUnitCode()
        {
            var data = _service.Repository.DbSet;
            var _ParentUnitCode = _service.GetParentUnitCode();
            var temp = data.Where(x => x.UnitCode.StartsWith(_ParentUnitCode)).ToList();
            var result = new List<ChoiceObj>();
            foreach (var item in temp)
            {
                result.Add(new ChoiceObj()
                {
                    Id = item.Id,
                    ExtendValue = item.TaxRate.ToString(),
                    Text = string.Format("{0}-{1}%", item.LoaiThue, item.TaxRate),
                    Value = item.MaLoaiThue,
                    Description = item.TaiKhoanKt
                });
            }
            return result;
        }
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdTaxVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdTax>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdTax().UnitCode),
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

                result.Data = Mapper.Map<PagedObj<MdTax>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdTaxVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdTax>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdTax().UnitCode),
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

        [ResponseType(typeof(MdTax))]
        [CustomAuthorize(Method = "THEM", State = "tax")]
        public async Task<IHttpActionResult> Post(MdTax instance)
        {
            var result = new TransferObj<MdTax>();

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
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "tax")]
        public async Task<IHttpActionResult> Put(string id, MdTax instance)
        {
            var result = new TransferObj<MdTax>();
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
        [ResponseType(typeof(MdTax))]
        [CustomAuthorize(Method = "XOA", State = "tax")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdTax instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdTax))]
        [Route("Get")]
        [CustomAuthorize(Method = "XEM", State = "tax")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
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
