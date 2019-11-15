using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
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
using BTS.SP.API.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/XuatKhac")]
    [Route("{id?}")]
    [Authorize]
    public class XuatKhacController : ApiController
    {
        private readonly INvXuatKhacService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        private readonly IMdPeriodService _servicePeriod;
        public XuatKhacController(INvXuatKhacService service, IDclGeneralLedgerService service2)
        {
            _service = service;
            _serviceGeneral = service2;
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvXuatKhacVm.Dto>> result = new TransferObj<PagedObj<NvXuatKhacVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvXuatKhacVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatKhacVm.Search>>();
            filtered.OrderBy = "NgayCT";
            filtered.OrderType = "DESC";
            PagedObj<NvVatTuChungTu> paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
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
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                            Method = FilterMethod.EqualTo,
                            Value = TypeVoucher.XKHAC.ToString()
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
                ResultObj<PagedObj<NvVatTuChungTu>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvXuatKhacVm.Dto>>(filterResult.Value);
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
            List<NvXuatKhacVm.Dto> result = new List<NvXuatKhacVm.Dto>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvXuatKhacVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatKhacVm.Search>>();
            PagedObj<NvVatTuChungTu> paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            string unitCode = _service.GetCurrentUnitCode();
            QueryBuilder query = new QueryBuilder
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
                            Value = TypeVoucher.XKHAC.ToString()
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
                ResultObj<PagedObj<NvVatTuChungTu>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvXuatKhacVm.Dto>>(filterResult.Value.Data);
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
            var result = new List<NvXuatKhacVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatKhacVm.Search>>();
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
                            Value = TypeVoucher.XKHAC.ToString()
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

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvXuatKhacVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatKhacVm.DtoDetail>>(details.ToList());
                    }
                    {
                        var details = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvXuatKhacVm.DtoClauseDetail>>(details.ToList());
                    }

                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [CustomAuthorize(Method = "THEM", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> Post(NvXuatKhacVm.Dto instance)
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
        public NvXuatKhacVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [CustomAuthorize(Method = "SUA", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> Put(string id, NvXuatKhacVm.Dto instance)
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
        [CustomAuthorize(Method = "DUYET", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> PostApproval(NvXuatKhacVm.Dto instance)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            _service.UpdatePhieu(instance);
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
        [CustomAuthorize(Method = "XOA", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvVatTuChungTu instance = await _service.Repository.FindAsync(id);
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
            var result = new TransferObj<NvXuatKhacVm.ReportModel>();
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
        [CustomAuthorize(Method = "XEM", State = "phieuXuatKhac")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var _ParentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<NvXuatKhacVm.Dto>();
            var temp = new NvXuatKhacVm.Dto();
            var phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvXuatKhacVm.Dto>(phieu);
                var chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatKhacVm.DtoDetail>>(chiTietPhieu);
                temp.DataDetails.ForEach(
                    x =>
                    {
                        x.CalcResult();
                        x.GiaVon = x.GiaVon;
                    }
                    );
                if (phieu.TrangThai != 10)
                {
                    decimal sum = 0;
                    List<NvXuatKhacVm.DtoDetail> listDetails = new List<NvXuatKhacVm.DtoDetail>();
                    var unitCode = _service.GetCurrentUnitCode();
                    var ky = CurrentSetting.GetKhoaSo(unitCode);
                    var tableName = ProcedureCollection.GetTableName(ky.Year, ky.Period);
                    var MaKho = temp.MaKhoXuat;
                    decimal giaVon = 0;
                    //string kyKeToan = _servicePeriod.GetKyKeToan((DateTime)phieu.NgayCT);
                    foreach (var value in temp.DataDetails)
                    {
                        var sp = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu.Equals(value.MaHang) && x.MaDonVi.StartsWith(_ParentUnitCode)).FirstOrDefault();
                        var item = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.Where(x => x.MaVatTu.Equals(value.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode)).FirstOrDefault();

                        if (sp != null)
                        {
                            value.TyLeVATVao = sp.TyLeVatVao;
                        }
                        if (item != null) value.TenHang = item.TenHang;
                        List<MdMerchandiseVm.DataXNT> data = ProcedureCollection.GetDataInventoryByCondition(unitCode, MaKho, value.MaHang, tableName, _ParentUnitCode);
                        if (data.Count > 0)
                        {
                            decimal.TryParse(value.GiaVon.ToString(), out giaVon);
                            value.DonGia = giaVon;
                            value.TyLeVATRa = data[0].TyLeVATRa;
                            value.TyLeVATVao = data[0].TyLeVATVao;
                            value.ThanhTien = value.DonGia * value.SoLuong;
                            sum += (decimal)value.ThanhTien;
                        }
                        listDetails.Add(value);
                    }
                    temp.DataDetails = listDetails;
                    var tyLe = _service.UnitOfWork.Repository<MdTax>().DbSet.Where(x => x.MaLoaiThue == temp.VAT).Select(x => x.TaxRate).FirstOrDefault();
                    if (tyLe != null)
                    {
                        temp.TienVat = sum * (tyLe / 100);

                    }
                    else
                    {
                        temp.TienVat = 0;
                    }

                    temp.ThanhTienTruocVat = sum;
                    temp.ThanhTienSauVat = temp.ThanhTienTruocVat + temp.TienVat;
                }
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
