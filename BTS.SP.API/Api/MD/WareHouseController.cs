using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using AutoMapper;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/WareHouse")]
    [Route("{id?}")]
    [Authorize]
    public class WareHouseController : ApiController
    {
        protected readonly IMdWareHouseService _service;
        public WareHouseController(IMdWareHouseService service)
        {
            _service = service;
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <returns></returns>
        [Route("GetAll_WareHouse")]
        [HttpGet]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public async Task<IHttpActionResult> GetAll_WareHouse()
        {
            TransferObj<List<ChoiceObj>> result = new TransferObj<List<ChoiceObj>>();
            DbSet<MdWareHouse> data = _service.Repository.DbSet;
            string maDonViCha = _service.GetParentUnitCode();
            result.Data = data.Where(x => x.UnitCode.StartsWith(maDonViCha)).Select(x => new ChoiceObj { Value = x.MaKho, Text = x.MaKho + "|" + x.TenKho, Description = x.TenKho, ExtendValue = x.ThongTinBoSung, Id = x.Id }).ToList();
            return Ok(result);
        }
      
        [Route("GetByUnit/{maDonVi}")]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public IHttpActionResult GetByUnit(string maDonVi)
        {
            TransferObj<List<ChoiceObj>> result = new TransferObj<List<ChoiceObj>>();
            DbSet<MdWareHouse> data = _service.Repository.DbSet;
            result.Data = data.Where(x => x.MaKho.StartsWith(maDonVi)).Select(x => new ChoiceObj { Value = x.MaKho, Text = x.MaKho + "|" + x.TenKho, Description = x.TenKho, ExtendValue = x.ThongTinBoSung, Id = x.Id }).ToList();
            return Ok(result);
        }
        [Route("GetCurrentWareHouse")]
        [AllowAnonymous]
        public TransferObj<MdWareHouse> GetCurrentWareHouse()
        {
            var result = new TransferObj<MdWareHouse>();
            var maDonVi = _service.GetCurrentUnitCode();

            try
            {
                var data = _service.Repository.DbSet.FirstOrDefault(x => x.UnitCode == maDonVi);
                if (data != null)
                {
                    result.Status = true;
                    result.Data = data;
                }
                return result;

            }
            catch (Exception)
            {
                result.Status = false;
                result.Message = "Cửa hàng hiện tại chưa có kho hợp lệ!";
                return result;
            }
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <returns></returns>
        [Route("wareHouseCtl_GetSelectDataByUnitCode_page")]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public async Task<IHttpActionResult> wareHouseCtl_GetSelectDataByUnitCode_page(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdWareHouseVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdWareHouse>>();
            var _ParentUnitCode = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdWareHouse().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = _ParentUnitCode
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<MdWareHouse>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        /// <summary>
        /// Query entity
        /// POST
        /// </summary>
        /// <param name="jsonData">complex data : jsonData.filtered & jsonData.paged</param>
        /// <returns></returns>
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdWareHouseVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdWareHouse>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdWareHouse().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdWareHouse().MaKho),
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
        /// <summary>
        /// Create entity
        /// POST
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdWareHouse))]
        [CustomAuthorize(Method = "THEM", State = "wareHouse")]
        public async Task<IHttpActionResult> Post(MdWareHouseVm.Dto instance)
        {
            var result = new TransferObj<MdWareHouse>();
            try
            {
                var _unitCode = _service.GetCurrentUnitCode();
                string MA_DM = _unitCode + "-K";
                if (!instance.IsKhoBanLe && !instance.IsKhoKM)
                {
                    instance.MaKho =  _service.BuildCode_DM(MA_DM, _unitCode, true);
                }
                var wareHouse = Mapper.Map<MdWareHouseVm.Dto, MdWareHouse>(instance);
                var item = _service.Insert(wareHouse);
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

        [Route("GetNewInstance")]
        public MdWareHouseVm.Dto GetNewInstance()
        {
            return _service.GetNewInstance();
        }
        [ResponseType(typeof(MdWareHouse))]
        [CustomAuthorize(Method = "XOA", State = "wareHouse")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdWareHouse instance = await _service.Repository.FindAsync(id);
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
        /// <summary>
        /// Get by id
        /// GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MdWareHouse))]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        /// <summary>
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "wareHouse")]
        public async Task<IHttpActionResult> Put(string id, MdWareHouse instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdWareHouse>();
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

        [HttpGet]
        [Route("FilterWareHouse/{maKho}")]
        [ResponseType(typeof(MdWareHouse))]
        public MdWareHouse FilterWareHouse(string maKho)
        {
            var wareHouse = new MdWareHouse();
            if (string.IsNullOrEmpty(maKho))
            {
                wareHouse = null;
            }
            else
            {
                maKho = maKho.ToUpper();
                maKho = maKho.Trim();
                var unitCode = _service.GetCurrentUnitCode();
                wareHouse = _service.Repository.DbSet.Where(x => x.MaKho == maKho).FirstOrDefault(x => x.UnitCode == unitCode);
            }
            return wareHouse;
        }

        [Route("GetByCode/{code}")]
        [CustomAuthorize(Method = "XEM", State = "wareHouse")]
        public async Task<IHttpActionResult> GetByCode(string code)
        {
            var instance = _service.Repository.DbSet.FirstOrDefault(x => x.MaKho == code);
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
