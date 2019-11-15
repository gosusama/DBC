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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using System.Data.Entity.Core.Metadata.Edm;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/TypeReason")]
    [Route("{id?}")]
    [Authorize]
    public class TypeReasonController : ApiController
    {
        private readonly IMdTypeReasonService _service;
        public TypeReasonController(IMdTypeReasonService service)
        {
            _service = service;
        }

        [Route("GetAll_TypeReason")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "typeReason")]
        public async Task<IHttpActionResult> GetAll_TypeReason()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var maDonViCha = _service.GetParentUnitCode();
            var lst = _service.Repository.DbSet.Where(x => x.UnitCode.StartsWith(maDonViCha)).ToList();
            result.Data = Mapper.Map<List<MdTypeReason>,List<ChoiceObj>>(lst);
            return Ok(result);
        }

        [Route("GetAll_TypeReasonByParent/{parent}")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "typeReason")]
        public async Task<IHttpActionResult> GetAll_TypeReasonByParent(TypeXN parent)
        {
            try
            {
                var result = new TransferObj<List<ChoiceObj>>();
                var maDonViCha = _service.GetParentUnitCode();
                var lst = _service.Repository.DbSet.Where(x => x.UnitCode.StartsWith(maDonViCha) && x.Loai == parent).ToList();
                result.Data = Mapper.Map<List<MdTypeReason>, List<ChoiceObj>>(lst);
                return Ok(result);
            }
           catch(Exception ex)
            {

            }
            return null;
        }

        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            return _service.GetSelectSort();
        }
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "typeReason")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdTypeReasonVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdTypeReason>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdTypeReason().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdTypeReason().MaLyDo),
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

                result.Data = Mapper.Map<PagedObj<MdTypeReason>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "typeReason")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdTypeReasonVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdTypeReason>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdTypeReason().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdTypeReason().MaLyDo),
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
        [ResponseType(typeof(MdTypeReason))]
        [CustomAuthorize(Method = "THEM", State = "typeReason")]
        public async Task<IHttpActionResult> Post(MdTypeReason instance)
        {
            var result = new TransferObj<MdTypeReason>();

            try
            {
                var item = _service.Insert(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                result.Message = "Thêm mới thành công";
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
        [CustomAuthorize(Method = "SUA", State = "typeReason")]
        public async Task<IHttpActionResult> Put(string id, MdTypeReason instance)
        {
            var result = new TransferObj<MdTypeReason>();
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
        [ResponseType(typeof(MdTypeReason))]
        [CustomAuthorize(Method = "XOA", State = "typeReason")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdTypeReason instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdTypeReason))]
        [CustomAuthorize(Method = "XEM", State = "typeReason")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        public IList<MdTypeReason> Get()
        {
            var data = _service.Repository.DbSet;
            return data.ToList();
        }
        private bool IsExists(string id)
        {
            return _service.Repository.Find(id) != null;
        }
        [Route("GetSelectDataType/{type}")]
        public IList<ChoiceObj> GetSelectDataType(string type)
        {
            TypeXN pType;
            if (!Enum.TryParse<TypeXN>(type, out pType))
            {
                return null;
            }
            return _service.GetSelectSort(pType);
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
