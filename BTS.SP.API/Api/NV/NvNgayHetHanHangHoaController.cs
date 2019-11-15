using AutoMapper;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.SP.API.Utils;
using static BTS.API.SERVICE.NV.NvNgayHetHanHangHoaVm;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/NgayHetHanHangHoa")]
    [Route("{id?}")]
    [Authorize]
    public class NvNgayHetHanHangHoaController : ApiController
    {
        private readonly INvNgayHetHanHangHoaService _service;
        public NvNgayHetHanHangHoaController(INvNgayHetHanHangHoaService service)
        {
            _service = service;
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuNgayHetHanHangHoa")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvNgayHetHanHangHoaVm.Dto>> result = new TransferObj<PagedObj<NvNgayHetHanHangHoaVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvNgayHetHanHangHoaVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNgayHetHanHangHoaVm.Search>>();
            filtered.OrderBy = "NgayBaoDate";
            filtered.OrderType = "DESC";
            PagedObj<NvNgayHetHanHangHoa> paged = ((JObject)postData.paged).ToObject<PagedObj<NvNgayHetHanHangHoa>>();
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,

                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvNgayHetHanHangHoa().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvNgayHetHanHangHoa().ICreateDate),
                        Method = OrderMethod.DESC
                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvNgayHetHanHangHoa>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvNgayHetHanHangHoa>, PagedObj<NvNgayHetHanHangHoaVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [CustomAuthorize(Method = "THEM", State = "phieuNgayHetHanHangHoa")]
        public async Task<IHttpActionResult> Post(NvNgayHetHanHangHoaVm.Dto instance)
        {
            TransferObj<NvNgayHetHanHangHoa> result = new TransferObj<NvNgayHetHanHangHoa>();
            try
            {
                NvNgayHetHanHangHoa item = _service.InsertPhieu(instance);
                await _service.UnitOfWork.SaveAsync();
                result.Data = item;
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewInstance")]
        public NvNgayHetHanHangHoaVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            string unitCode = _service.GetCurrentUnitCode();
            DateTime currentDate = DateTime.Now.Date;
            DateTime datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            ParameterNgayHetHanHangHoa result = new ParameterNgayHetHanHangHoa()
            {
                ToDate = datelock,
                FromDate = datelock,
                MaxDate = currentDate,
                UnitCode = unitCode,
                GroupBy = "MADONVI",
            };
            return Ok(result);
        }
        [CustomAuthorize(Method = "SUA", State = "phieuNgayHetHanHangHoa")]
        public async Task<IHttpActionResult> Put(string id, NvNgayHetHanHangHoaVm.Dto instance)
        {
            TransferObj<NvNgayHetHanHangHoa> result = new TransferObj<NvNgayHetHanHangHoa>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            NvNgayHetHanHangHoa check = _service.FindById(instance.Id);
            try
            {
                NvNgayHetHanHangHoa item = _service.UpdatePhieu(instance);
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

        [CustomAuthorize(Method = "XOA", State = "phieuNgayHetHanHangHoa")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvNgayHetHanHangHoa instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                if (_service.DeletePhieu(id))
                {
                    _service.Delete(instance.Id);
                    _service.UnitOfWork.Save();
                    return Ok(instance);
                }
                return InternalServerError();
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "phieuNgayHetHanHangHoa")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            string _ParentUnitCode = _service.GetParentUnitCode();
            TransferObj<NvNgayHetHanHangHoaVm.Dto> result = new TransferObj<NvNgayHetHanHangHoaVm.Dto>();
            NvNgayHetHanHangHoaVm.Dto temp = new NvNgayHetHanHangHoaVm.Dto();
            NvNgayHetHanHangHoa phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvNgayHetHanHangHoa, NvNgayHetHanHangHoaVm.Dto>(phieu);
                List<NvNgayHetHanHangHoaChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvNgayHetHanHangHoaChiTiet>().DbSet.Where(x => x.MaPhieuPk == phieu.MaPhieuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvNgayHetHanHangHoaChiTiet>, List<NvNgayHetHanHangHoaVm.DtoDetail>>(chiTietPhieu);
            }
            result.Data = temp;
            result.Status = true;
            return Ok(result);
        }

        [Route("GetReport/{id}")]
        public async Task<IHttpActionResult> GetReport(string id)
        {
            TransferObj<NvNgayHetHanHangHoaVm.ReportModel> result = new TransferObj<NvNgayHetHanHangHoaVm.ReportModel>();
            NvNgayHetHanHangHoaVm.ReportModel data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
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
