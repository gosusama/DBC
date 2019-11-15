using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/XuatBanLe")]
    [Route("{id?}")]
    [Authorize]
    public class XuatBanLeController : ApiController
    {
        private readonly INvXuatBanLeService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        public XuatBanLeController(INvXuatBanLeService service, IDclGeneralLedgerService service2)
        {
            _service = service;
            _serviceGeneral = service2;
        }


        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvBanLe")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvXuatBanLeVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanLeVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            filtered.OrderBy = "NgayCT";
            filtered.OrderType = "DESC";
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.XBANLE.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
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
                        Field = ClassHelper.GetPropertyName(() => new NvVatTuChungTu().ICreateDate),
                        Method = OrderMethod.DESC

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
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvXuatBanLeVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [CustomAuthorize(Method = "THEM", State = "nvBanLe")]
        public async Task<IHttpActionResult> Post(NvXuatBanLeVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();

            try
            {
                var item = _service.InsertPhieu(instance);
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
        public NvXuatBanLeVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        [CustomAuthorize(Method = "SUA", State = "nvBanLe")]
        public async Task<IHttpActionResult> Put(string id, NvXuatBanLeVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return BadRequest();
            }
            try
            {
                var item = _service.UpdatePhieu(instance);
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

        [CustomAuthorize(Method = "DUYET", State = "nvBanLe")]
        [Route("PostApproval")]
        public async Task<IHttpActionResult> PostApproval(NvVatTuChungTu instance)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chungTu.TrangThai = (int)ApprovalState.IsComplete;
            chungTu.NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode);
            chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
            chungTu.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            switch (_service.Approval(chungTu.Id))
            {
                case StateProcessApproval.NoPeriod:

                    try
                    {
                        await _service.UnitOfWork.SaveAsync();
                        return Ok(true);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Success:
                    try
                    {
                        await _service.UnitOfWork.SaveAsync();
                        return Ok(true);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Failed:
                    break;
                default:
                    break;
            }

            return BadRequest("Không thể duyệt phiếu này");
        }

        [CustomAuthorize(Method = "XOA", State = "nvBanLe")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvVatTuChungTu instance = await _service.Repository.FindAsync(id);
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
                return null;
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
        [Route("GetReport/{id}")]
        public TransferObj<NvXuatBanLeVm.ReportModel> GetReport(string id)
        {
            var result = new TransferObj<NvXuatBanLeVm.ReportModel>();
            var data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
            }
            return result;
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvBanLe")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvXuatBanLeVm.Dto>();
            var temp = new NvXuatBanLeVm.Dto();
            var phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvXuatBanLeVm.Dto>(phieu);
                var chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                var chiTietSoCai = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanLeVm.DtoDetail>>(chiTietPhieu);
                temp.DataDetails.ForEach(x => x.CalcResult());
                temp.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvXuatBanLeVm.DtoClauseDetail>>(chiTietSoCai);
                temp.CalcResult();
                result.Data = temp;
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
