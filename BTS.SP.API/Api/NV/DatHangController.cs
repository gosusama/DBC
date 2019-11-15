using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
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
using BTS.SP.API.Utils;
using BTS.API.SERVICE.Authorize.Utils;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/DatHang")]
    [Route("{id?}")]
    [Authorize]
    public class DatHangController : ApiController
    {
        private readonly INvPhieuDatHangService _service;
        public DatHangController(INvPhieuDatHangService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode == unitCode && x.Loai == (int)LoaiDatHang.KHACHHANG).Select(x => new ChoiceObj() { Value = x.SoPhieu, Text = x.NoiDung, Id = x.Id }).ToList();
        }
        [Route("GetSelectDataIsComplete")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataIsComplete()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(u => u.TrangThai == (int)OrderState.IsComplete && u.UnitCode == unitCode && u.Loai == (int)LoaiDatHang.KHACHHANG).Select(x => new ChoiceObj() { Value = x.SoPhieu, Text = x.SoPhieu + "|" + x.NoiDung, Id = x.Id }).ToList();
        }
        [Route("GetSelectDataIsCompleteByCustomerCode/{code}")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataIsCompleteByCustomerCode(string code)
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(u => u.TrangThai == (int)OrderState.IsComplete && u.MaKhachHang == code && u.UnitCode == unitCode && u.Loai == (int)LoaiDatHang.KHACHHANG).Select(x => new ChoiceObj() { Value = x.SoPhieu, Text = x.SoPhieu + "|" + x.NoiDung, Id = x.Id }).ToList();
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<ChoiceObj>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangVm.Search>>();
            filtered.OrderBy = "Ngay";
            filtered.OrderType = "DESC";
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ
                {
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().TrangThai),
                            Method = FilterMethod.EqualTo,
                            Value = (int) TrangThaiDonHang.DAXACNHAN
                        },
                        new QueryFilterLinQ
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().IsBanBuon),
                            Method = FilterMethod.EqualTo,
                            Value = (int) LoaiDonDatHang.BANBUON
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().Loai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)LoaiDatHang.KHACHHANG
                        },

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

                result.Data = Mapper.Map<PagedObj<NvDatHang>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostMerger")]
        public async Task<IHttpActionResult> PostMerger(List<string> soPhieus)
        {
            List<NvPhieuDatHangVm.DtoDetail> result = null;
            try
            {
                result = _service.MergerPhieu(soPhieus);
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuDatHang")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvPhieuDatHangVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangVm.Search>>();
            filtered.OrderType = "DESC";
            filtered.OrderBy = "Ngay";
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
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
                            Property = ClassHelper.GetProperty(() => new NvDatHang().Loai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)LoaiDatHang.KHACHHANG
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
                        Property = ClassHelper.GetProperty(() => new NvDatHang().TrangThaiTt),
                        Method = OrderMethod.DESC
                    },
                    new QueryOrder()
                    {
                        Property = ClassHelper.GetProperty(() => new NvDatHang().TrangThai),
                        Method = OrderMethod.ASC
                    },

                }
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvDatHang>, PagedObj<NvPhieuDatHangVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostQueryApproval")]
        public async Task<IHttpActionResult> PostQueryApproval(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvDatHang>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
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
                            Property = ClassHelper.GetProperty(() => new NvDatHang().Loai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)LoaiDatHang.KHACHHANG
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().TrangThai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)OrderState.IsApproval,
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new MdContract().UnitCode),
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

                result.Data = filterResult.Value;

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
            var result = new List<NvPhieuDatHangVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new NvDatHang().UnitCode),
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
                result = Mapper.Map<List<NvDatHang>, List<NvPhieuDatHangVm.Dto>>(filterResult.Value.Data);
                foreach (var item in result)
                {
                    var detail = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == item.SoPhieu);
                    item.ThanhTien = 0;
                    foreach (var itemDetail in detail)
                    {
                        item.ThanhTien += itemDetail.ThanhTien.HasValue ? itemDetail.ThanhTien.Value : 0;
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
            var result = new List<NvPhieuDatHangVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                TakeAll = true,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new NvDatHang().UnitCode),
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
                result = Mapper.Map<List<NvDatHang>, List<NvPhieuDatHangVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(u => u.SoPhieu == x.SoPhieu);
                        x.DataDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangVm.DtoDetail>>(details.ToList());
                    }
                });
                foreach (var item in result)
                {
                    var detail = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == item.SoPhieu);
                    item.ThanhTien = 0;
                    foreach (var itemDetail in detail)
                    {
                        item.ThanhTien += itemDetail.ThanhTien.HasValue ? itemDetail.ThanhTien.Value : 0;
                    }
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [ResponseType(typeof(NvDatHang))]
        [CustomAuthorize(Method = "THEM", State = "phieuDatHang")]
        public async Task<IHttpActionResult> Post(NvPhieuDatHangVm.Dto instance)
        {
            var result = new TransferObj<NvDatHang>();
            try
            {
                var item = _service.InsertPhieu(instance);
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
        [Route("GetNewInstance")]
        public NvPhieuDatHangVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "phieuDatHang")]
        public async Task<IHttpActionResult> Put(string id, NvPhieuDatHangVm.Dto instance)
        {
            var result = new TransferObj<NvDatHang>();
            var check = _service.FindById(instance.Id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (check == null)
            {
                return NotFound();
            }
            if (id != instance.Id || check.TrangThai == (int)OrderState.IsComplete)
            {
                return BadRequest();
            }

            try
            {
                var item = _service.UpdatePhieu(instance);
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
        [Route("PostApproval")]
        [ResponseType(typeof(bool))]
        [CustomAuthorize(Method = "DUYET", State = "phieuDatHang")]
        public async Task<IHttpActionResult> PostApproval(NvPhieuDatHangVm.Dto instance)
        {
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null)
            {
                return NotFound();
            }
            if (chungTu.TrangThai == (int)OrderState.IsApproval)
            {
                return BadRequest("Đơn này đã được duyệt!");
            }
            chungTu.TrangThai = (int)OrderState.IsApproval;
            chungTu.Ngay = DateTime.Now;
            {
                var details = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == chungTu.SoPhieu).ToList();
                if (details.Count != instance.DataDetails.Count) return Ok(false); // trường hợp cố tình sửa đổi không giống như hợp đồng

                foreach (var item in instance.DataDetails)
                {
                    var itemNew = details.FirstOrDefault(u => u.Id == item.Id);
                    if (itemNew == null) return Ok(false); //trường hợp trong danh sách không hợp lệ
                    itemNew.SoLuong = itemNew.SoLuongDuyet = item.SoLuongDuyet;
                    itemNew.DonGia = itemNew.DonGiaDuyet = item.DonGiaDuyet;
                    itemNew.ThanhTien = item.SoLuongDuyet * item.DonGiaDuyet;
                    itemNew.ObjectState = ObjectState.Modified;
                }
            }
            chungTu.ObjectState = ObjectState.Modified;
            try
            {
                _service.UnitOfWork.Save();
                return Ok(true);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("PostComplete")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> PostComplete(NvPhieuDatHangVm.Dto instance)
        {
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null || chungTu.TrangThai == (int)OrderState.IsComplete)
            {
                return NotFound();
            }
            chungTu.TrangThai = (int)OrderState.IsComplete;
            chungTu.Ngay = DateTime.Now;
            {
                var details = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == chungTu.SoPhieu).ToList();
                if (details.Count != instance.DataDetails.Count) return Ok(false); // trường hợp cố tình sửa đổi không giống như hợp đồng

                foreach (var item in instance.DataDetails)
                {
                    var itemNew = details.FirstOrDefault(u => u.Id == item.Id);
                    if (itemNew == null) return Ok(false); //trường hợp trong danh sách không hợp lệ
                    itemNew.SoLuongDuyet = item.SoLuongDuyet;
                    itemNew.DonGiaDuyet = item.DonGiaDuyet;
                    itemNew.SoLuongBaoDuyet = item.SoLuongBaoDuyet;
                    itemNew.SoLuongLeDuyet = item.SoLuongLeDuyet;
                    itemNew.ThanhTien = item.SoLuongDuyet * item.DonGiaDuyet;
                    itemNew.ObjectState = ObjectState.Modified;
                }
            }
            chungTu.ObjectState = ObjectState.Modified;
            try
            {
                _service.UnitOfWork.Save();
                return Ok(true);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("PostCompletes")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> PostCompletes(List<string> instance)
        {
            foreach (var sp in instance)
            {
                var chungTu = _service.Repository.DbSet.FirstOrDefault(x => x.SoPhieu == sp);

                if (chungTu == null || chungTu.TrangThai == (int)OrderState.IsComplete)
                {
                    return NotFound();
                }
                chungTu.TrangThai = (int)OrderState.IsComplete;
                chungTu.ObjectState = ObjectState.Modified;
            }
            try
            {
                _service.UnitOfWork.Save();
                return Ok(true);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [ResponseType(typeof(NvDatHang))]
        [CustomAuthorize(Method = "XOA", State = "phieuDatHang")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvDatHang instance = await _service.Repository.FindAsync(id);
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
        [CustomAuthorize(Method = "XEM", State = "phieuDatHang")]
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
            var result = new TransferObj<NvPhieuDatHangVm.ReportModel>();
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
        [CustomAuthorize(Method = "XEM", State = "phieuDatHang")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvPhieuDatHangVm.Dto>();
            var temp = new NvPhieuDatHangVm.Dto();

            var phieu = _service.FindById(id);

            if (phieu != null)
            {
                temp = Mapper.Map<NvDatHang, NvPhieuDatHangVm.Dto>(phieu);
                var khachhang = _service.UnitOfWork.Repository<MdCustomer>().DbSet.FirstOrDefault(x => x.DienThoai == phieu.MaKhachHang);
                if (khachhang != null)
                {
                    temp.SdtKhachHang = khachhang.DienThoai;
                    temp.DiaChiKH = khachhang.DiaChi;
                    temp.EmailKH = khachhang.Email;
                }
                var nhanvien = _service.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.FirstOrDefault(x => x.MaNhanVien == phieu.MaNhanVien);
                if (nhanvien != null)
                {
                    temp.MaNhanVien = nhanvien.MaNhanVien;
                    temp.TenNql = nhanvien.MaNhanVien + " - " + nhanvien.TenNhanVien;
                    temp.SdtNql = nhanvien.SoDienThoai;
                }

                var chiTietDatHang = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieuPk == phieu.SoPhieuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangVm.DtoDetail>>(chiTietDatHang);
                foreach (var item in temp.DataDetails)
                {
                    item.DefaultApproval();
                }
                //temp.SoPhieuDatHang = temp.SoPhieuPk;
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetDetailComplete/{id}")]
        public async Task<IHttpActionResult> GetDetailComplete(string id)
        {
            var result = new TransferObj<NvPhieuDatHangVm.Dto>();
            var temp = new NvPhieuDatHangVm.Dto();
            var phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvDatHang, NvPhieuDatHangVm.Dto>(phieu);
                var hopDong = _service.UnitOfWork.Repository<MdContract>().DbSet.FirstOrDefault(x => x.MaHd == phieu.MaHd);
                if (hopDong != null)
                {
                    temp.MaKhachHang = hopDong.KhachHang;
                }
                var tb_datHangChiTiet = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet;

                var chiTietDatHang = tb_datHangChiTiet.Where(x => x.SoPhieu == phieu.SoPhieu).ToList();
                temp.DataDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangVm.DtoDetail>>(chiTietDatHang);
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("CountOrderNew")]
        public IHttpActionResult CountOrderNew()
        {
            int count = _service.UnitOfWork.Repository<NvDatHang>().DbSet.Count();
            return Ok(count);
        }

        // lam moi

        [Route("PostConfirm")]
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> PostConfirm(NvPhieuDatHangVm.Dto instance)
        {
            var chungTu = _service.FindById(instance.Id);

            if (chungTu == null)
            {
                return NotFound();
            }
            if (chungTu.TrangThai == (int)TrangThaiDonHang.DAXACNHAN)
            {
                return BadRequest("Đơn này đã được duyệt!");
            }
            chungTu.TrangThai = (int)TrangThaiDonHang.DAXACNHAN;
            chungTu.Ngay = DateTime.Now;
            {
                var details = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(x => x.SoPhieu == chungTu.SoPhieu).ToList();
                if (details.Count != instance.DataDetails.Count) return Ok(false); // trường hợp cố tình sửa đổi không giống như hợp đồng

                foreach (var item in instance.DataDetails)
                {
                    var itemNew = details.FirstOrDefault(u => u.Id == item.Id);
                    if (itemNew == null) return Ok(false); //trường hợp trong danh sách không hợp lệ
                    //itemNew.SoLuong = itemNew.SoLuongDuyet = item.SoLuongDuyet;
                    //itemNew.DonGia = itemNew.DonGiaDuyet = item.DonGiaDuyet;
                    //itemNew.ThanhTien = item.SoLuongDuyet * item.DonGiaDuyet;
                    //itemNew.ObjectState = ObjectState.Modified;
                }
            }
            chungTu.ObjectState = ObjectState.Modified;
            try
            {
                _service.UnitOfWork.Save();
                return Ok(true);
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
