using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using BTS.SP.API.Utils;
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/KhuyenMaiVoucher")]
    [Route("{id?}")]
    [Authorize]
    public class KhuyenMaiVoucherController : ApiController
    {
        private readonly INvKhuyenMaiVoucherService _service;
        private readonly INvGiaoDichQuayService _serviceGdq;

        public KhuyenMaiVoucherController(INvKhuyenMaiVoucherService service, INvGiaoDichQuayService serviceGdq)
        {
            _service = service;
            _serviceGdq = serviceGdq;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode == unitCode).Select(x => new ChoiceObj() { Value = x.MaChuongTrinh, Text = x.NoiDung, Id = x.Id }).ToList();
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<ChoiceObj>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiVoucherVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvChuongTrinhKhuyenMai>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvKhuyenMaiVoucherVm.Dto>> result = new TransferObj<PagedObj<NvKhuyenMaiVoucherVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvKhuyenMaiVoucherVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiVoucherVm.Search>>();
            PagedObj<NvChuongTrinhKhuyenMai> paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
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
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().LoaiKhuyenMai),
                            Method = FilterMethod.EqualTo,
                            Value = LoaiKhuyenMai.Voucher
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvChuongTrinhKhuyenMai().ICreateDate),
                        Method = OrderMethod.DESC

                    }
                }
            };
            try
            {
                ResultObj<PagedObj<NvChuongTrinhKhuyenMai>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvChuongTrinhKhuyenMai>, PagedObj<NvKhuyenMaiVoucherVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [ResponseType(typeof(NvChuongTrinhKhuyenMai))]
        [CustomAuthorize(Method = "THEM", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> Post(NvKhuyenMaiVoucherVm.Dto instance)
        {
            TransferObj<NvChuongTrinhKhuyenMai> result = new TransferObj<NvChuongTrinhKhuyenMai>();
            try
            {
                NvChuongTrinhKhuyenMai item = _service.InsertPhieu(instance);
                _service.UnitOfWork.Save();
                result.Data = item;
                result.Status = true;
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> Put(string id, NvKhuyenMaiVoucherVm.Dto instance)
        {
            TransferObj<NvChuongTrinhKhuyenMai> result = new TransferObj<NvChuongTrinhKhuyenMai>();
            NvChuongTrinhKhuyenMai check = _service.FindById(instance.Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (check == null)
            {
                return NotFound();
            }
            //if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            //{
            //    return BadRequest();
            //}

            try
            {
                NvChuongTrinhKhuyenMai item = _service.UpdatePhieu(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [ResponseType(typeof(NvChuongTrinhKhuyenMai))]
        [CustomAuthorize(Method = "XOA", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvChuongTrinhKhuyenMai instance = await _service.Repository.FindAsync(id);
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
        [Route("PostPrintDetail")]
        public async Task<IHttpActionResult> PostPrintDetail(JObject jsonData)
        {
            var result = new List<NvKhuyenMaiVoucherVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiVoucherVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvChuongTrinhKhuyenMai>>();
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
                            Method = FilterMethod.EqualTo,
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvChuongTrinhKhuyenMai().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    },
                    Method = FilterMethod.And,
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvChuongTrinhKhuyenMai().ICreateDate),
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

                result = Mapper.Map<List<NvChuongTrinhKhuyenMai>, List<NvKhuyenMaiVoucherVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(u => u.MaChuongTrinh == x.MaChuongTrinh);
                        x.DataDetails = Mapper.Map<List<NvChuongTrinhKhuyenMaiChiTiet>, List<NvKhuyenMaiVoucherVm.DtoDetail>>(details.ToList());
                    }
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("PostApproval/{id}")]
        [CustomAuthorize(Method = "DUYET", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> PostApproval(string id)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvChuongTrinhKhuyenMai chuongTrinh = _service.FindById(id);

            if (chuongTrinh == null || chuongTrinh.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chuongTrinh.TrangThai = (int)ApprovalState.IsComplete;
            chuongTrinh.TrangThaiSuDung = (int)ApprovalState.IsComplete;
            chuongTrinh.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            return Ok(true);
        }
        [Route("PostUnApprove/{id}")]
        [CustomAuthorize(Method = "DUYET", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> PostUnApprove(string id)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvChuongTrinhKhuyenMai chuongTrinh = _service.FindById(id);

            if (chuongTrinh == null || chuongTrinh.TrangThai == (int)ApprovalState.IsNotApproval)
            {
                return NotFound();
            }
            chuongTrinh.TrangThai = (int)ApprovalState.IsNotApproval;
            chuongTrinh.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            return Ok(true);
        }

        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvKMVoucher")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvKhuyenMaiVoucherVm.Dto> result = new TransferObj<NvKhuyenMaiVoucherVm.Dto>();
            NvKhuyenMaiVoucherVm.Dto temp = new NvKhuyenMaiVoucherVm.Dto();

            NvChuongTrinhKhuyenMai phieu = _service.FindById(id);

            if (phieu != null)
            {
                temp = Mapper.Map<NvChuongTrinhKhuyenMai, NvKhuyenMaiVoucherVm.Dto>(phieu);
                DbSet<NvChuongTrinhKhuyenMaiChiTiet> tb_ChuongTrinhKhuyenMaiChiTiet = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet;
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetDisCountVoucher/{voucher}")]
        public async Task<IHttpActionResult> GetDisCountVoucher(string voucher)
        {
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<NvKhuyenMaiVoucherVm.Dto>();
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var phieu = _service.Repository.DbSet.FirstOrDefault(x => x.MaGiamGia == voucher && x.TuNgay <= date && x.DenNgay >= date && x.UnitCode.StartsWith(_parentUnitCode));
            if (phieu != null)
            {
                if (phieu.TrangThai == 10 && phieu.TrangThaiSuDung == (int)ApprovalState.IsComplete)
                {
                    result.Data = Mapper.Map<NvChuongTrinhKhuyenMai, NvKhuyenMaiVoucherVm.Dto>(phieu);
                    result.Message = "Khuyến mại voucher";
                    result.Status = true;
                }
                else if (phieu.TrangThai == 10 && phieu.TrangThaiSuDung == (int)ApprovalState.IsExpired)
                {
                    result.Data = Mapper.Map<NvChuongTrinhKhuyenMai, NvKhuyenMaiVoucherVm.Dto>(phieu);
                    result.Status = false;
                    result.Message = "Mã voucher không còn hiệu lực";
                }
            }
            else
            {
                result.Data = new NvKhuyenMaiVoucherVm.Dto();
                result.Message = "Khuyến mại voucher";
                result.Status = false;
            }
            return Ok(result);
        }

        [Route("GetGiaoDichByVoucher/{maGiamGia}")]
        public async Task<IHttpActionResult> GetGiaoDichByVoucher(string maGiamGia)
        {
            var temp = new NvGiaoDichQuayVm.Dto();
            var _parentUnitCode = _service.GetParentUnitCode();
            var result = new TransferObj<NvGiaoDichQuayVm.Dto>();
            NvGiaoDichQuayVm.DataDto instance = new NvGiaoDichQuayVm.DataDto();
            var phieu = _serviceGdq.Repository.DbSet.FirstOrDefault(x => x.MaVoucher == maGiamGia && x.UnitCode.StartsWith(_parentUnitCode));
            if (phieu != null)
            {
                instance = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.DataDto>(phieu);
                temp = Mapper.Map<NvGiaoDichQuay, NvGiaoDichQuayVm.Dto>(phieu);
                var chiTietPhieu =
                    _service.UnitOfWork.Repository<NvGiaoDichQuayChiTiet>()
                        .DbSet.Where(x => x.MaGDQuayPK == phieu.MaGiaoDichQuayPK)
                        .ToList();
                temp.DataDetails =
                    Mapper.Map<List<NvGiaoDichQuayChiTiet>, List<NvGiaoDichQuayVm.DtoDetail>>(chiTietPhieu).ToList();
                if (!string.IsNullOrEmpty(instance.MaKhachHang))
                {
                    var customer = _service.UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.MaKH == instance.MaKhachHang && x.UnitCode.StartsWith(_parentUnitCode));
                    if (customer != null)
                    {
                        temp.DiaChi = customer.DiaChi;
                        temp.DienThoai = customer.DienThoai;
                        temp.NgaySinh = customer.NgaySinh;
                        temp.NgayDacBiet = customer.NgayDacBiet;
                        temp.Email = customer.Email;
                        temp.TenKhachHang = customer.TenKH;
                        temp.MaThe = customer.MaThe;
                        temp.QuenThe = customer.QuenThe;
                        temp.TongTien = customer.TongTien;
                        temp.TienNguyenGia = customer.TienNguyenGia;
                        temp.TienSale = customer.TienSale;
                        temp.HangKhachHang = customer.HangKhachHang;
                    }
                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            else
            {
                result.Data = null;
                result.Message = "Không tìm thấy bản ghi";
                result.Status = false;
            }
            return Ok(result);
        }

        [Route("GetNewInstance")]
        public NvKhuyenMaiVoucherVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();

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
