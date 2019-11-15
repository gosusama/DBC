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

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Department")]
    [Route("{id?}")]
    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IMdDepartmentService _service;
        public DepartmentController(IMdDepartmentService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var _parent = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var temp = data.Where(x=>x.UnitCode.StartsWith(_parent)).ToList();
            var result = new List<ChoiceObj>();
            foreach (var item in temp)
            {
                result.Add(new ChoiceObj()
                {
                    Id = item.Id,
                    //ExtendValue = item.TyLeGiamGia.ToString(),
                    Text = item.TenPhong,
                    Value = item.MaPhong,
                    //Description = string.Format("{0}-{1}%", item.TenPhong)
                });
            }
            return result;
        }
        
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdDepartmentVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdDepartment>>();
            var _parentUnitCode = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdDepartment().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = _parentUnitCode
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdDepartment>, PagedObj<ChoiceObj>>
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
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdDepartmentVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdDepartment>>();
            var _parent = _service.GetParentUnitCode();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdDepartment().UnitCode),
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
        [HttpPost]
        [ResponseType(typeof(MdDepartment))]
        [CustomAuthorize(Method = "THEM", State = "department")]
        public async Task<IHttpActionResult> Post(MdDepartment instance)
        {
            var result = new TransferObj<MdDepartment>();

            try
            {
                var _unitCode = _service.GetParentUnitCode();    
                string MA_DM = _unitCode + "-P";
                                     
                instance.MaPhong = _service.BuildCode_DM(MA_DM, _unitCode, true);

                var item = _service.Insert(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
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
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "department")]
        public async Task<IHttpActionResult> Put(string id, MdDepartment instance)
        {
            var result = new TransferObj<MdDepartment>();
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
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [HttpDelete]
        [ResponseType(typeof(MdDepartment))]
        [CustomAuthorize(Method = "XOA", State = "department")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdDepartment instance = await _service.Repository.FindAsync(id);
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

        [Route("GetNewInstance")]
        public MdDepartment GetNewInstance()
        {
            var _unitCode = _service.GetParentUnitCode();
            string maDanhMuc = _unitCode + "-P";
            return new MdDepartment()
            {
                MaPhong = _service.BuildCode_DM(maDanhMuc, _unitCode, false)
            };
        }
        
        [ResponseType(typeof(MdDepartment))]
        [CustomAuthorize(Method = "XEM", State = "department")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        //[ResponseType(typeof(MdDepartment))]
        //[Route("GetByCode/{code}")]
        //public IHttpActionResult GetByCode(string code)
        //{
        //    var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaDepartment == code);
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