using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using BTS.SP.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/CongNo")]
    [Route("{id?}")]
    [Authorize]
    public class NvCongNoController : ApiController
    {
        // GET: NvCongNo
        private readonly INvCongNoService _service;

        public NvCongNoController(INvCongNoService service)
        {
            _service = service;
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvCongNoNCC")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvCongNoVm.Dto>> result = new TransferObj<PagedObj<NvCongNoVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvCongNoVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvCongNoVm.Search>>();
            PagedObj<NvCongNo> paged = ((JObject)postData.paged).ToObject<PagedObj<NvCongNo>>();
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvCongNo().LoaiChungTu),
                            Method = FilterMethod.EqualTo,
                            Value = filtered.AdvanceData.LoaiChungTu
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvCongNo().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvCongNo().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvCongNo>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvCongNo>, PagedObj<NvCongNoVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Status = false;
                result.Message = e.Message.ToString();
                return Ok(result);
            }
        }

        [Route("GetNewInstance/{loaiChungTu}")]
        public NvCongNoVm.Dto GetNewInstance(string LoaiChungTu)
        {
            return _service.CreateNewInstance(LoaiChungTu);
        }
        [HttpPost]
        [CustomAuthorize(Method = "THEM", State = "nvCongNoNCC")]
        public async Task<IHttpActionResult> Post(NvCongNoVm.Dto instance)
        {
            TransferObj<NvCongNo> result = new TransferObj<NvCongNo>();

            try
            {
                NvCongNo item = _service.InsertPhieu(instance);
                await _service.UnitOfWork.SaveAsync();
                result.Data = item;
                result.Message = "Thêm mới thành công";
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }

            return Ok(result);
        }

        [CustomAuthorize(Method = "SUA", State = "nvCongNoNCC")]
        public async Task<IHttpActionResult> Put(string id, NvCongNoVm.Dto instance)
        {
            TransferObj<NvCongNo> result = new TransferObj<NvCongNo>();
            NvCongNo check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                result.Status = false;
                result.Message = "Dữ liệu không đúng!";
                return Ok(result);
            }
            try
            {
                NvCongNo item = _service.UpdatePhieu(instance);
                await _service.UnitOfWork.SaveAsync();
                result.Status = true;
                result.Message = "Update thành công!";
                result.Data = item;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Status = false;
                result.Message = e.Message.ToString();
            }
            return Ok(result);
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvCongNoNCC")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvCongNoVm.Dto> result = new TransferObj<NvCongNoVm.Dto>();
            NvCongNoVm.Dto temp = new NvCongNoVm.Dto();
            NvCongNo phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvCongNo, NvCongNoVm.Dto>(phieu);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetAmmountCustomerBorrowed/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAmmountCustomerBorrowed(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest();
            }
            var result = new TransferObj<NvCongNoVm.Dto>();
            try
            {
                var data = _service.GetAmmountCustomerBorrowed(code,DateTime.Now.Date);
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetAmmountSupplierLend/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAmmountSupplierLend(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest();
            }
            var result = new TransferObj<NvCongNoVm.Dto>();
            try
            {
                var data = _service.GetAmmountSupplierLend(code);
                result.Data = data;
                result.Status = true;
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [CustomAuthorize(Method = "XOA", State = "nvCongNoNCC")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvCongNo instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                _service.Delete(instance.Id);
                _service.UnitOfWork.Save();
                return Ok(instance);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            var result = new ParameterCongNo()
            {
                ToDate = datelock,
                FromDate = datelock.AddDays(-1),
                UnitCode = unitCode,
                LoaiBaoCao = "0",
            };
            return Ok(result);
        }
    }
}