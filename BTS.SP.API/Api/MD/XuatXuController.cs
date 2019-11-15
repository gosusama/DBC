using AutoMapper;

using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE;
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

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/XuatXu")]
    [Route("{id?}")]
    [Authorize]
    public class XuatXuController : ApiController
    {
        protected readonly IMdXuatXuService _service;
        public XuatXuController(IMdXuatXuService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x=>x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaXuatXu, Text = x.TenXuatXu, Id = x.Id,Description = x.GhiChu}).ToList();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "xuatXu")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var paged = new PagedObj<MdXuatXu>();
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdXuatXuVm.Search>>();
            paged = ((JObject)postData.paged).ToObject<PagedObj<MdXuatXu>>();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdXuatXu().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdXuatXu().MaXuatXu),
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
        [Route("GetNewCode")]
        public string GetNewCode()
        {
            return _service.BuildCode();
        }
        [ResponseType(typeof(MdXuatXu))]
        [CustomAuthorize(Method = "THEM", State = "xuatXu")]
        public async Task<IHttpActionResult> Post(MdXuatXuVm.Dto instance)
        {
            var result = new TransferObj<MdXuatXu>();
            var _parentUnitCode = _service.GetParentUnitCode();
            if (instance.IsGenCode)
                instance.MaXuatXu = _service.SaveCode();
            else
            {
                if (instance.MaXuatXu == "")
                {
                    result.Status = false;
                    result.Message = "Mã không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaXuatXu == instance.MaXuatXu && x.UnitCode.StartsWith(_parentUnitCode));
                    if (exist != null)
                    {
                        result.Status = false;
                        result.Message = "Đã tồn tại mã này";
                        return Ok(result);
                    }
                }
            }
            try
            {
                var data = Mapper.Map<MdXuatXuVm.Dto, MdXuatXu>(instance);
                var item = _service.Insert(data);
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
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "xuatXu")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdXuatXuVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdXuatXu>>();
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdXuatXu().UnitCode),
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

                result.Data = Mapper.Map<PagedObj<MdXuatXu>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [ResponseType(typeof(MdXuatXu))]
        [CustomAuthorize(Method = "XOA", State = "xuatXu")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdXuatXu instance = await _service.Repository.FindAsync(id);
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
    }
}
