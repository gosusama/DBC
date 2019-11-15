using AutoMapper;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.API.SERVICE.Authorize.AuThamSoHeThong;
namespace BTS.SP.API.Api.Authorize.AuThamSoHeThong
{
    [RoutePrefix("api/Authorize/AuThamSoHeThong")]
    [Route("{id?}")]
    [Authorize]
    public class AuThamSoHeThongController : ApiController
    {
        private readonly IAuThamSoHeThongService _service;
        public AuThamSoHeThongController(IAuThamSoHeThongService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;

            return data.Select(x => new ChoiceObj() { Value = x.MaThamSo, Text = x.MaThamSo + "|" + x.TenThamSo + "|" + x.GiaTriThamSo, Id = x.Id }).ToList();
        }
        [Route("GetSelectDataIsComplete")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            //var jOject = JObject.FromObject(jsonData);
            var result = new TransferObj<PagedObj<ChoiceObj>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuThamSoHeThongVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<AU_THAMSOHETHONG>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<AU_THAMSOHETHONG>, PagedObj<ChoiceObj>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("PostQuery")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<AU_THAMSOHETHONG>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuThamSoHeThongVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<AU_THAMSOHETHONG>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
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
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        public async Task<IHttpActionResult> Post(AU_THAMSOHETHONG instance)
        {
            var result = new TransferObj<AU_THAMSOHETHONG>();

            try
            {
                var item = _service.Insert(instance);
                await _service.UnitOfWork.SaveAsync();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        public async Task<IHttpActionResult> Put(string id, AU_THAMSOHETHONG instance)
        {
            var result = new TransferObj<AU_THAMSOHETHONG>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != instance.Id)
            {
                return BadRequest();
            }
            try
            {
                var item = _service.Update(instance);
                await _service.UnitOfWork.SaveAsync();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        public async Task<IHttpActionResult> Delete(string id)
        {
            AU_THAMSOHETHONG instance = await _service.Repository.FindAsync(id);
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
        public async Task<IHttpActionResult> Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
    }
}
