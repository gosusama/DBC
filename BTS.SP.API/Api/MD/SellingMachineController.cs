using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using AutoMapper;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/SellingMachine")]
    [Route("{id?}")]
    [Authorize]
    public class SellingMachineController : ApiController
    {
        private readonly IMdSellingMachineService _service;
        public SellingMachineController(IMdSellingMachineService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x => x.TrangThai == 10).OrderBy(x=>x.Code).Select(x => new ChoiceObj { Value = x.Code, Text = x.Name, Id = x.Id }).ToList();
        }
        [Route("CheckStatusSelling")]
        [CustomAuthorize(Method = "XEM", State = "sellingMachine")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckStatusSelling()
        {
            var result = new TransferObj();
            var unitCode = _service.GetCurrentUnitCode();
            try
            {
                var log = _service.UnitOfWork.Repository<AU_LOG>()
                    .DbSet.Where(x => x.UnitCode == unitCode && x.NGAY.Year == DateTime.Now.Year && x.NGAY.Month == DateTime.Now.Month && x.NGAY.Day == DateTime.Now.Day)
                    .OrderByDescending(x => new {x.UnitCode, x.NGAY, x.ThoiGian});
                if (log != null)
                {
                    result.Data = log.ToList();
                }
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "sellingMachine")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSellingMachineVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSellingMachine>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdSellingMachine().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdSellingMachine().Code),
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

                result.Data = Mapper.Map<PagedObj<MdSellingMachine>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "sellingMachine")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSellingMachineVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSellingMachine>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdSellingMachine().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdSellingMachine().Code),
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

        [ResponseType(typeof(MdSellingMachine))]
        [CustomAuthorize(Method = "THEM", State = "sellingMachine")]
        public async Task<IHttpActionResult> Post(MdSellingMachine instance)
        {
            var result = new TransferObj<MdSellingMachine>();
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
        [CustomAuthorize(Method = "SUA", State = "sellingMachine")]
        public async Task<IHttpActionResult> Put(string id, MdSellingMachine instance)
        {
            var result = new TransferObj<MdSellingMachine>();
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
        [ResponseType(typeof(MdSellingMachine))]
        [CustomAuthorize(Method = "XOA", State = "sellingMachine")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdSellingMachine instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdSellingMachine))]
        [CustomAuthorize(Method = "XEM", State = "sellingMachine")]
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
