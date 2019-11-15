using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.NV;
using BTS.SP.API.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/DatHangNCC")]
    [Route("{id?}")]
    [Authorize]
    public class DatHangNCCController : ApiController
    {
        private readonly INvPhieuDatHangNCCService _service;
        public DatHangNCCController(INvPhieuDatHangNCCService service)
        {
            _service = service;
        }
        [Route("GetSelectDataApprovalBySupplierCode/{code}")]
        [HttpGet]
        public IList<ChoiceObj> GetSelectDataApprovalBySupplierCode(string code)
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            var parentUnitCode = _service.GetParentUnitCode();
            return data.Where(u => u.TrangThai == (int)OrderState.IsApproval && u.Loai == (int)LoaiDatHang.NHACUNGCAP && u.MaNhaCungCap == code && u.UnitCode.StartsWith(parentUnitCode))
                       .Select(x => new ChoiceObj() { Value = x.SoPhieu, Text = x.SoPhieu + "|" + x.NoiDung, Id = x.Id }).ToList();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvPhieuDatHangNCCVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangNCCVm.Search>>();
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
                            Value = (int)LoaiDatHang.NHACUNGCAP
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().SoPhieuCon),
                            Method = FilterMethod.Null,
                        }
                    },
                    Method = FilterMethod.And
                },
            };
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvDatHang>, PagedObj<NvPhieuDatHangNCCVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }

        [Route("PostPrint")]
        public async Task<IHttpActionResult> PostPrint(JObject jsonData)
        {
            var result = new List<NvPhieuDatHangNCCVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangNCCVm.Search>>();
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
                result = Mapper.Map<List<NvDatHang>, List<NvPhieuDatHangNCCVm.Dto>>(filterResult.Value.Data);
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
            var result = new List<NvPhieuDatHangNCCVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangNCCVm.Search>>();
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
                result = Mapper.Map<List<NvDatHang>, List<NvPhieuDatHangNCCVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet.Where(u => u.SoPhieu == x.SoPhieu);
                        x.DataDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangNCCVm.DtoDetail>>(details.ToList());
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
        [CustomAuthorize(Method = "THEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> Post(NvPhieuDatHangNCCVm.Dto instance)
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
        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            TransferObj<PagedObj<NvDatHang>> result = new TransferObj<PagedObj<NvDatHang>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvPhieuDatHangNCCVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangNCCVm.Search>>();
            filtered.OrderBy = "Ngay";
            filtered.OrderType = "DESC";
            PagedObj<NvDatHang> paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
            string parentUnitCode = _service.GetParentUnitCode();
            QueryBuilder query = new QueryBuilder
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
                            Method = FilterMethod.StartsWith,
                            Value = parentUnitCode
                        },
                        new QueryFilterLinQ
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().TrangThai),
                            Method = FilterMethod.EqualTo,
                            Value = (int) OrderState.IsApproval
                        },
                        new QueryFilterLinQ
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().MaNhaCungCap),
                            Method = FilterMethod.EqualTo,
                            Value = filtered.AdvanceData.MaNhaCungCap
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().Loai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)LoaiDatHang.NHACUNGCAP
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().SoPhieuCon),
                            Method = FilterMethod.Null,
                        }
                    },
                    Method = FilterMethod.And
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new NvDatHang().ICreateDate),
                        Method = OrderMethod.DESC
                    }

                }
            };
            try
            {
                ResultObj<PagedObj<NvDatHang>> filterResult = _service.Filter(filtered, query);
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
        [Route("GetNewInstance")]
        public NvPhieuDatHangNCCVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> Put(string id, NvPhieuDatHangNCCVm.Dto instance)
        {
            var result = new TransferObj<NvDatHang>();
            NvDatHang check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return BadRequest();
            }
            if (check == null)
            {
                return NotFound();
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
        [CustomAuthorize(Method = "DUYET", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> PostApproval(NvPhieuDatHangNCCVm.Dto instance)
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

        [ResponseType(typeof(NvDatHang))]
        [CustomAuthorize(Method = "XOA", State = "nvDatHangNCC")]
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
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
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
            TransferObj<NvPhieuDatHangNCCVm.ReportModel> result = new TransferObj<NvPhieuDatHangNCCVm.ReportModel>();
            NvPhieuDatHangNCCVm.ReportModel data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvPhieuDatHangNCCVm.Dto>();
            var temp = new NvPhieuDatHangNCCVm.Dto();

            var phieu = _service.FindById(id);

            if (phieu != null)
            {
                temp = Mapper.Map<NvDatHang, NvPhieuDatHangNCCVm.Dto>(phieu);
                var tb_datHangChiTiet = _service.UnitOfWork.Repository<NvDatHangChiTiet>().DbSet;

                var chiTietDatHangNCC = tb_datHangChiTiet.Where(x => x.SoPhieu == phieu.SoPhieu).ToList();
                temp.DataDetails = Mapper.Map<List<NvDatHangChiTiet>, List<NvPhieuDatHangNCCVm.DtoDetail>>(chiTietDatHangNCC);
                foreach (var item in temp.DataDetails)
                {
                    item.DefaultApproval();
                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetChild/{id}")]
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> GetChild(string id)
        {
            TransferObj<List<NvDatHang>> result = new TransferObj<List<NvDatHang>>();
            List<NvDatHang> temp = new List<NvDatHang>();

            NvDatHang phieu = _service.FindById(id);

            if (phieu != null)
            {
                if (!string.IsNullOrEmpty(phieu.SoPhieuCon))
                {
                    string[] phieus = phieu.SoPhieuCon.Split(',');
                    if (phieus.Length > 0)
                    {
                        foreach (var str in phieus)
                        {
                            NvDatHang obj = _service.UnitOfWork.Repository<NvDatHang>().DbSet.FirstOrDefault(x => x.SoPhieuPk == str);
                            if (obj != null)
                            {
                                temp.Add(obj);
                            }
                        }
                    }
                }

                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("PostQuerySummary")]
        [CustomAuthorize(Method = "XEM", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> PostQuerySummary(JObject jsonData)
        {
            TransferObj<PagedObj<NvPhieuDatHangNCCVm.Dto>> result = new TransferObj<PagedObj<NvPhieuDatHangNCCVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvPhieuDatHangNCCVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDatHangNCCVm.Search>>();
            filtered.OrderType = "DESC";
            filtered.OrderBy = "Ngay";
            PagedObj<NvDatHang> paged = ((JObject)postData.paged).ToObject<PagedObj<NvDatHang>>();
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
                            Property = ClassHelper.GetProperty(() => new NvDatHang().SoPhieuCon),
                            Method = FilterMethod.NotNull,
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvDatHang().Loai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)LoaiDatHang.NHACUNGCAP
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
            };
            try
            {
                ResultObj<PagedObj<NvDatHang>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<NvDatHang>, PagedObj<NvPhieuDatHangNCCVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostMerge")]
        public async Task<IHttpActionResult> PostMerge(List<string> soPhieus)
        {
            TransferObj<List<NvPhieuDatHangNCCVm.DtoDetail>> result = new TransferObj<List<NvPhieuDatHangNCCVm.DtoDetail>>();
            try
            {
                List<NvPhieuDatHangNCCVm.DtoDetail> listPhieu = _service.MergePhieu(soPhieus);
                result.Data = listPhieu;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostAddNewSummary")]
        [ResponseType(typeof(NvDatHang))]
        public async Task<IHttpActionResult> PostAddNewSummary(NvPhieuDatHangNCCVm.Dto instance)
        {
            TransferObj<NvDatHang> result = new TransferObj<NvDatHang>();
            try
            {
                NvDatHang item = _service.InsertSummary(instance);
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
        [Route("PostReceiveSummary")]
        [ResponseType(typeof(NvDatHang))]
        public async Task<IHttpActionResult> PostReceiveSummary(NvPhieuDatHangNCCVm.Dto instance)
        {
            TransferObj<NvDatHang> result = new TransferObj<NvDatHang>();
            try
            {
                NvDatHang item = _service.ReceiveSummary(instance);
                _service.UnitOfWork.Save();
                result.Data = item;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }
        [ResponseType(typeof(NvDatHang))]
        [Route("PostDeleteSummary")]
        [CustomAuthorize(Method = "XOA", State = "nvDatHangNCC")]
        public async Task<IHttpActionResult> PostDeleteSummary(NvDatHang item)
        {
            NvDatHang instance = await _service.Repository.FindAsync(item.Id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                _service.DeleteSummary(instance);
                await _service.UnitOfWork.SaveAsync();
                return Ok(instance);
            }
            catch (Exception)
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
