using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
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

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Color")]
    [Route("{id?}")]
    [Authorize]
    public class ColorController : ApiController
    {
        private readonly IMdColorService _service;
        public ColorController(IMdColorService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            return data.Where(x=>x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaColor, Text = x.TenColor, Id = x.Id }).ToList();
        }
        [Route("GetAll_Colors")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll_Colors()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaColor, Text = x.MaColor + "|" + x.TenColor, Description = x.TenColor, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdColorVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdColor>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdColor().UnitCode),
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

                result.Data = Mapper.Map<PagedObj<MdColor>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewInstance")]
        public MdColor GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [Route("PostQuery")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdColorVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdColor>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdColor().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdColor().MaColor),
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
        [ResponseType(typeof(MdColor))]
        public async Task<IHttpActionResult> Post(MdColorVm.Dto instance)
        {
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<MdColor>();
            if (instance.IsGenCode)
                instance.MaColor = _service.SaveCode();
            else
            {
                if (instance.MaColor == "")
                {
                    result.Status = false;
                    result.Message = "Mã không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaColor == instance.MaColor && x.UnitCode.StartsWith(_parentUnitCode));
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
                var data = Mapper.Map<MdColorVm.Dto, MdColor>(instance);
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
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(string id, MdColor instance)
        {
            var result = new TransferObj<MdColor>();
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
        [ResponseType(typeof(MdColor))]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdColor instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdColor))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [ResponseType(typeof(MdColor))]
        [Route("GetByCode/{code}")]
        public IHttpActionResult GetByCode(string code)
        {
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaColor == code);
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
