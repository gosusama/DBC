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
using BTS.API.ENTITY.Common;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Contract")]
    [Route("{id?}")]
    [Authorize]
    public class ContractController : ApiController
    {
        private readonly IMdContractService _service;
        public ContractController(IMdContractService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data/*.Where(x => x.UnitCode == unitCode)*/.Select(x => new ChoiceObj() { Value = x.MaHd, Text = x.TenHd, Id = x.Id }).ToList();
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdContractVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdContract>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdContract().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<MdContract>, PagedObj<ChoiceObj>>
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
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdContractVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdContract>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdContract().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
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
        [ResponseType(typeof(MdContract))]
        public async Task<IHttpActionResult> Post(MdContractVm.Dto instance)
        {
            var result = new TransferObj<MdContract>();
            try
            {
                var item = _service.InsertDto(instance);
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

        /// <summary>
        /// Update entity
        /// PUT
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(string id, MdContractVm.Dto instance)
        {
            var result = new TransferObj<MdContract>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                var item = _service.UpdateDto(instance);
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

        [ResponseType(typeof(MdContract))]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdContract instance = await _service.Repository.FindAsync(id);
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
        [ResponseType(typeof(MdContract))]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);


        }
        [Route("GetReport/{id}")]
        public TransferObj<MdContractVm.ReportModel> GetReport(string id)
        {
            var result = new TransferObj<MdContractVm.ReportModel>();
            var data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return result;
            }
            return result;
        }
        [Route("GetDetails/{id}")]
        public TransferObj<MdContractVm.Dto> GetDetails(string id)
        {
            var result = new TransferObj<MdContractVm.Dto>();
            var temp = new MdContractVm.Dto();
            var hopDong = _service.FindById(id);
            var chitietHopDong = _service.UnitOfWork.Repository<MdDetailContract>().DbSet;
            if (hopDong != null)
            {
                temp = Mapper.Map<MdContract, MdContractVm.Dto>(hopDong);
                var chiTietHopDong = chitietHopDong.Where(x => x.MaHd == hopDong.MaHd).ToList();
                temp.DataDetails = Mapper.Map<List<MdDetailContract>, List<MdContractVm.Detail>>(chiTietHopDong);
                result.Data = temp;
                result.Status = true;
            }
            return result;
        }
        [Route("GetContractByCustomerId/{id}")]
        public IList<ChoiceObj> GetContractByCustomerId(string id)
        {
            var khachHang = _service.UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.Id == id);
            if (khachHang != null)
            {
                var listContract = _service.Repository.DbSet.Where(x => x.KhachHang == khachHang.MaKH);
                return listContract.Select(x => new ChoiceObj() { Value = x.MaHd, Text = x.MaHd + " | " + x.TenHd, Id = x.Id }).ToList();
            }
            return null;
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

        [HttpGet]
        [Route("GetProfile")]
        public UserInfo GetProfile()
        {
            return _service.GetDoProfile();
        }
    }
}
