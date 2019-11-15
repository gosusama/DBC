using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.API.ENTITY.Md;
using BTS.SP.API.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/NhapKhac")]
    [Route("{id?}")]
    [Authorize]
    public class NhapKhacController : ApiController
    {
        private readonly INvNhapKhacService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        public NhapKhacController(INvNhapKhacService service, IDclGeneralLedgerService service2)
        {
            _service = service;
            _serviceGeneral = service2;
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvNhapKhacVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapKhacVm.Search>>();
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
                            Value = TypeVoucher.NKHAC.ToString()
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

                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvNhapKhacVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [Route("PostPrint")]
        public async Task<IHttpActionResult> PostPrint(JObject jsonData)
        {
            var result = new List<NvNhapKhacVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapKhacVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NKHAC.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapKhacVm.Dto>>(filterResult.Value.Data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("PostPrintDetail")]
        public async Task<IHttpActionResult> PostPrintDetail(JObject jsonData)
        {
            var result = new List<NvNhapKhacVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapKhacVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.NKHAC.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And
                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapKhacVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapKhacVm.DtoDetail>>(details.ToList());
                    }
                    {
                        var details = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvNhapKhacVm.DtoClauseDetail>>(details.ToList());
                    }

                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [CustomAuthorize(Method = "THEM", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> Post(NvNhapKhacVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();

            try
            {
                if (_service.ValidateNgayCT(instance.NgayCT.Value))
                {
                    var item = _service.InsertPhieu(instance);
                    await _service.UnitOfWork.SaveAsync();
                    result.Data = item;
                    result.Status = true;
                    return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
                }
                return InternalServerError();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [Route("GetNewInstance")]
        public NvNhapKhacVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        [CustomAuthorize(Method = "SUA", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> Put(string id, NvNhapKhacVm.Dto instance)
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
        [Route("PostApproval")]
        [CustomAuthorize(Method = "DUYET", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> PostApproval(NvVatTuChungTu instance)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chungTu.TrangThai = (int)ApprovalState.IsComplete;
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
        [CustomAuthorize(Method = "XOA", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvVatTuChungTu instance = await _service.Repository.FindAsync(id);
            var chitietinstance = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(o => o.MaChungTuPk == instance.MaChungTuPk).ToList();
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
        public async Task<IHttpActionResult> GetReport(string id)
        {
            var result = new TransferObj<NvNhapKhacVm.ReportModel>();
            var data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "phieuNhapKhac")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvNhapKhacVm.Dto>();
            var temp = new NvNhapKhacVm.Dto();
            var _ParentUnitCode = _service.GetParentUnitCode();
            var phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvNhapKhacVm.Dto>(phieu);
                var chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapKhacVm.DtoDetail>>(chiTietPhieu);
                string unitcode = _service.GetCurrentUnitCode();
                foreach (NvNhapKhacVm.DtoDetail dt in temp.DataDetails)
                {
                    var sp = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu.Equals(dt.MaHang) && x.MaDonVi.StartsWith(_ParentUnitCode)).FirstOrDefault();
                    var item = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.Where(x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode)).FirstOrDefault();

                    if (sp != null)
                    {
                        dt.TyLeVatVao = sp.TyLeVatVao;
                    }
                    if (item != null) dt.TenHang = item.TenHang;
                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }

        [Route("PostXuLyHangAm")]
        public async Task<IHttpActionResult> PostXuLyHangAm(NvNhapKhacVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            try
            {
                if (_service.ValidateNgayCT(instance.NgayCT.Value))
                {
                    var item = _service.InsertPhieuXuLyAm(instance);
                    await _service.UnitOfWork.SaveAsync();
                    result.Data = item;
                    result.Status = true;
                    return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
                }
                return InternalServerError();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
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
