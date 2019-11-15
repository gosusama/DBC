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
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Size")]
    [Route("{id?}")]
    [Authorize]
    public class SizeController : ApiController
    {
        private readonly IMdSizeService _service;
        public SizeController(IMdSizeService service)
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
            return data.Where(x=>x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaSize, Text = x.TenSize, Id = x.Id,Description=x.TenSize }).ToList();
        }
        [Route("GetAll_Sizes")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "size")]
        public async Task<IHttpActionResult> GetAll_Sizes()
        {
            var result = new TransferObj<List<ChoiceObj>>();
            var data = _service.Repository.DbSet;
            var maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaSize, Text = x.MaSize + "|" + x.TenSize, Description = x.TenSize, ExtendValue = x.UnitCode, Id = x.Id }).ToList();
            return Ok(result);
        }

        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "size")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSizeVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSize>>();
            var maDonViCha = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdSize().UnitCode),
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

                result.Data = Mapper.Map<PagedObj<MdSize>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("CheckExist/{maSize}")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckExist(string maSize)
        {
            var result = new TransferObj();
            var exist = _service.Repository.DbSet.Where(x => x.MaSize == maSize).ToList();
            if (exist.Count > 0)
            {
                result.Data = 1;
                result.Status = true;
            }
            else
            {
                result.Data = 0;
                result.Status = false;
            }
            return Ok(result);
        }

        [Route("GetNewInstance")]
        public MdSize GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "size")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdSizeVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdSize>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdSize().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdSize().MaSize),
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
        [ResponseType(typeof(MdSize))]
        [CustomAuthorize(Method = "THEM", State = "size")]
        public async Task<IHttpActionResult> Post(MdSizeVm.Dto instance)
        {
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<MdSize>();
            if (instance.IsGenCode)
                instance.MaSize = _service.SaveCode();
            else
            {
                if (instance.MaSize == "")
                {
                    result.Status = false;
                    result.Message = "Mã không hợp lệ";
                    return Ok(result);
                }
                else
                {
                    var exist = _service.Repository.DbSet.FirstOrDefault(x => x.MaSize == instance.MaSize && x.UnitCode.StartsWith(_parentUnitCode));
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
                var data = Mapper.Map<MdSizeVm.Dto, MdSize>(instance);
                //instance.MaSize = _service.SaveCode();
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
        [CustomAuthorize(Method = "SUA", State = "size")]
        public async Task<IHttpActionResult> Put(string id, MdSize instance)
        {
            var result = new TransferObj<MdSize>();
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
        [ResponseType(typeof(MdSize))]
        [CustomAuthorize(Method = "XOA", State = "size")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdSize instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdSize))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }

        [ResponseType(typeof(MdSize))]
        [Route("GetByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "size")]
        public IHttpActionResult GetByCode(string code)
        {
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaSize == code);
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
