using AutoMapper;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Authorize.Utils;
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

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/ChietKhauKh")]
    [Route("{id?}")]
    [Authorize]
    public class ChietKhauKhController:ApiController
    {
        private readonly IMdChietKhauKhService _service;
        public ChietKhauKhController(IMdChietKhauKhService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var temp = data.ToList();
            var result = new List<ChoiceObj>();
            foreach (var item in temp)
            {
                result.Add(new ChoiceObj()
                {
                    Id = item.Id,
                    ExtendValue = item.TyLeChietKhau.ToString(),
                    Text = string.Format("{0}-{1}%", item.MaChietKhau, item.TyLeChietKhau),
                    Value = item.MaChietKhau,
                    // Description = item.TenChietKhauKh
                });
            }
            return result;
        }
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "chietKhauKh")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdChietKhauKhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdChietKhauKH>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                //Filter = new QueryFilterLinQ()
                //{
                //    Property = ClassHelper.GetProperty(() => new MdChietKhauKH().UnitCode),
                //    Method = FilterMethod.EqualTo,
                //    Value = unitCode
                //}
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdChietKhauKH>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "chietKhauKh")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdChietKhauKhVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdChietKhauKH>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                //Filter = new QueryFilterLinQ()
                //{
                //    Property = ClassHelper.GetProperty(() => new MdChietKhauKH().UnitCode),
                //    Method = FilterMethod.EqualTo,
                //    Value = unitCode
                //}
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
        [ResponseType(typeof(MdChietKhauKH))]
        [CustomAuthorize(Method = "THEM", State = "chietKhauKh")]
        public async Task<IHttpActionResult> Post(MdChietKhauKH instance)
        {
            var result = new TransferObj<MdChietKhauKH>();

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
        [CustomAuthorize(Method = "SUA", State = "chietKhauKh")]
        public async Task<IHttpActionResult> Put(string id, MdChietKhauKH instance)
        {
            var result = new TransferObj<MdChietKhauKH>();
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
        [ResponseType(typeof(MdChietKhauKH))]
        [CustomAuthorize(Method = "XOA", State = "chietKhauKh")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdChietKhauKH instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdChietKhauKH))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        //[ResponseType(typeof(MdChietKhauKH))]
        //[Route("GetByCode/{code}")]
        //public IHttpActionResult GetByCode(string code)
        //{
        //    var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaChietKhauKh == code);
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