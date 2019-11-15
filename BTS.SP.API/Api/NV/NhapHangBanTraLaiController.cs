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
using BTS.SP.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/NhapHangBanTraLai")]
    [Route("{id?}")]
    [Authorize]
    public class NhapHangBanTraLaiController : ApiController
    {
        private readonly INvNhapHangBanTraLaiService _service;
        private readonly IDclGeneralLedgerService _serviceGeneral;
        private readonly INvCongNoService _serviceCongNo;
        public NhapHangBanTraLaiController(INvNhapHangBanTraLaiService service, IDclGeneralLedgerService service2, INvCongNoService serviceCongNo)
        {
            _service = service;
            _serviceGeneral = service2;
            _serviceCongNo = serviceCongNo;
        }
        [Route("GetProductCost/{id}")]
        public decimal GetProductCost(string id)
        {
            //var item = function -- check hang hoa ton tai
            //neu co => tinh don gia ton dau kyf/soluongdau ky
            // ve data
            var data = 1000000;
            return data;
        }

        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvNhapHangBanTraLaiVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangBanTraLaiVm.Search>>();
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
                            Value = TypeVoucher.NHBANTL.ToString()
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

                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvNhapHangBanTraLaiVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostPrint")]
        public async Task<IHttpActionResult> PostPrint(JObject jsonData)
        {
            var result = new List<NvNhapHangBanTraLaiVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangBanTraLaiVm.Search>>();
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
                            Value = TypeVoucher.NHBANTL.ToString()
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangBanTraLaiVm.Dto>>(filterResult.Value.Data);
                foreach (var item in result)
                {
                    var detail = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == item.MaChungTuPk);
                    item.ThanhTienSauVat = 0;
                    foreach (var itemDetail in detail)
                    {
                        item.ThanhTienSauVat += itemDetail.ThanhTien.HasValue ? itemDetail.ThanhTien.Value : 0;
                    }
                }
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
            var result = new List<NvNhapHangBanTraLaiVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvNhapHangBanTraLaiVm.Search>>();
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
                            Value = TypeVoucher.NHBANTL.ToString()
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

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvNhapHangBanTraLaiVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangBanTraLaiVm.DtoDetail>>(details.ToList());
                    }
                    {
                        var details = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvNhapHangBanTraLaiVm.DtoClauseDetail>>(details.ToList());
                    }

                });
                foreach (var item in result)
                {
                    var detail = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == item.MaChungTuPk);
                    item.ThanhTienSauVat = 0;
                    foreach (var itemDetail in detail)
                    {
                        item.ThanhTienSauVat += itemDetail.ThanhTien.HasValue ? itemDetail.ThanhTien.Value : 0;
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [CustomAuthorize(Method = "THEM", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> Post(NvNhapHangBanTraLaiVm.Dto instance)
        {
            TransferObj<NvVatTuChungTu> result = new TransferObj<NvVatTuChungTu>();
            try
            {
                for (int i = 0; i < instance.DataDetails.Count; i++)
                {
                    if (string.IsNullOrEmpty(instance.DataDetails[i].MaHang))
                    {
                        instance.DataDetails.RemoveAt(i);
                    }
                }
                NvVatTuChungTu item = _service.InsertPhieu(instance);
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
        public NvNhapHangBanTraLaiVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        [CustomAuthorize(Method = "SUA", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> Put(string id, NvNhapHangBanTraLaiVm.Dto instance)
        {
            TransferObj<NvVatTuChungTu> result = new TransferObj<NvVatTuChungTu>();
            for (int i = 0; i < instance.DataDetails.Count; i++)
            {
                if (instance.DataDetails[i].MaHang == null)
                    instance.DataDetails.RemoveAt(i);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            NvVatTuChungTu check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return BadRequest();
            }
            try
            {
                NvVatTuChungTu item = _service.UpdatePhieu(instance);
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
        [CustomAuthorize(Method = "DUYET", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> PostApproval(NvVatTuChungTu instance)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvVatTuChungTu chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            #region For Only ILOVEVN - B&T
            //chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
            //_InsertPhieuCongNo(chungTu);
            #endregion
            //chungTu = _service.FindById(instance.Id);
            chungTu.TrangThai = (int)ApprovalState.IsComplete;
            chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
            chungTu.ThoiGianDuyetPhieu = int.Parse(DateTime.Now.ToString("HHmmssfff"));
            chungTu.ObjectState = ObjectState.Modified;
            _service.UnitOfWork.Save();
            switch (_service.Approval(chungTu))
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

        [CustomAuthorize(Method = "XOA", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvVatTuChungTu instance = await _service.Repository.FindAsync(id);
            List<NvVatTuChungTuChiTiet> chitietinstance = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(o => o.MaChungTuPk == instance.MaChungTuPk).ToList();
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
            var result = new TransferObj<NvNhapHangBanTraLaiVm.ReportModel>();
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
        [CustomAuthorize(Method = "XEM", State = "phieuNhapHangBanTraLai")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvNhapHangBanTraLaiVm.Dto> result = new TransferObj<NvNhapHangBanTraLaiVm.Dto>();
            NvNhapHangBanTraLaiVm.Dto temp = new NvNhapHangBanTraLaiVm.Dto();

            NvVatTuChungTu phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvNhapHangBanTraLaiVm.Dto>(phieu);
                List<NvVatTuChungTuChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                List<DclGeneralLedger> chiTietSoCai = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvNhapHangBanTraLaiVm.DtoDetail>>(chiTietPhieu);
                //temp.DataDetails.ForEach(x => x.CalcResult());
                temp.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvNhapHangBanTraLaiVm.DtoClauseDetail>>(chiTietSoCai);
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
        private void _InsertPhieuCongNo(NvVatTuChungTu chungTu)
        {
            //Cos nhieu phieu trong ngay phat sinh no.
            var unitCode = _service.GetCurrentUnitCode();

            var _thanhTienCanTra = _serviceCongNo.GetAmmountCustomerBorrowed(chungTu.MaKhachHang, chungTu.NgayDuyetPhieu.Value).ThanhTienCanTra;
            _serviceCongNo.InsertPhieu(new NvCongNoVm.Dto()
            {
                Id = Guid.NewGuid().ToString(),
                LoaiChungTu = LoaiCongNo.CNKH.ToString(),
                NgayCT = CurrentSetting.GetNgayKhoaSo(unitCode),
                MaKhachHang = chungTu.MaKhachHang,
                GhiChu = "[" + chungTu.MaChungTu + "]",
                ThanhTien = chungTu.ThanhTienSauVat,
                ThanhTienCanTra = _thanhTienCanTra,

            });
            _serviceCongNo.UnitOfWork.Save();
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
