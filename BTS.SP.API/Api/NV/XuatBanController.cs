using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY.DCL;
using BTS.API.ENTITY.NV;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BTS.SP.API.Utils;
using System.Data.Entity;
using BTS.API.ENTITY.Md;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/XuatBan")]
    [Route("{id?}")]
    [Authorize]
    public class XuatBanController : ApiController
    {
        private readonly INvXuatBanService _service;
        private readonly INvPhieuDatHangService _serviceDatHang;

        private readonly INvCongNoService _serviceCongNo;
        private readonly IMdPeriodService _servicePeriod;
        public XuatBanController(INvXuatBanService service, INvCongNoService service2, IMdPeriodService servicePeriod, INvPhieuDatHangService serviceDatHang)
        {
            _service = service;
            _serviceCongNo = service2;
            _servicePeriod = servicePeriod;
            _serviceDatHang = serviceDatHang;
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "phieuXuatBan")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvXuatBanVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvXuatBanVm.Dto>>(filterResult.Value);
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
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvXuatBanVm.Dto>>(filterResult.Value.Data);
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
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
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
                            Value = TypeVoucher.XBAN.ToString()
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

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvXuatBanVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanVm.DtoDetail>>(details.ToList());
                    }
                    {
                        var details = _service.UnitOfWork.Repository<DclGeneralLedger>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataClauseDetails = Mapper.Map<List<DclGeneralLedger>, List<NvXuatBanVm.DtoClauseDetail>>(details.ToList());
                    }

                });
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            string unitCode = _service.GetCurrentUnitCode();
            DateTime currentDate = DateTime.Now.Date;
            DateTime datelock = CurrentSetting.GetNgayKhoaSo(unitCode);
            ParameterXuatBan result = new ParameterXuatBan()
            {
                ToDate = datelock,
                FromDate = datelock,
                MaxDate = currentDate,
                UnitCode = unitCode,
                GroupBy = TypeGroupXuatBan.MAKHO,
                ReportType = TypeReportSelling.XUATBANBUON
            };
            return Ok(result);
        }
        [CustomAuthorize(Method = "THEM", State = "phieuXuatBan")]
        public async Task<IHttpActionResult> Post(NvXuatBanVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            try
            {
                var item = _service.InsertPhieu(instance);
                if (!string.IsNullOrEmpty(instance.SoPhieuDatHang))
                {
                    if (_serviceDatHang.UpdateXuatBan(instance.SoPhieuDatHang) == null)
                    {
                        return InternalServerError();
                    }
                    await _serviceDatHang.UnitOfWork.SaveAsync();
                }
                if (item != null)
                {
                    if (await _service.UnitOfWork.SaveAsync() > 0)
                    {
                        result.Data = item;
                        result.Status = true;
                        result.Message = "Thêm mới thành công";
                    }
                    else
                    {
                        result.Data = null;
                        result.Status = false;
                        result.Message = "Xảy ra lỗi khi thêm mới dữ liệu";
                        return BadRequest(result.Message);
                    }
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                    result.Message = "Không có dòng chi tiết phiếu";
                    return BadRequest(result.Message);
                }
                return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                result.Message = e.Message.ToString();
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("GetNewInstance")]
        public NvXuatBanVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();
        }

        [Route("UpdateStatusPhieu")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateStatusPhieu(NvXuatBanVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            var check = _service.FindById(instance.Id);
            if (check == null)
            {
                return BadRequest("Không tìm thấy phiếu cha");
            }
            if (check.TrangThaiThanhToan == (int)TrangThaiThanhToan.DaThanhToan)
            {
                result.Status = false;
                result.Data = new NvVatTuChungTu();
                result.Message = "Hóa đơn này đã thanh toán !";
                return Ok(result);
            }
            try
            {
                #region For Only ILOVEVN - B&T
                //if ((!check.TrangThaiThanhToan.HasValue || (check.TrangThaiThanhToan.HasValue && check.TrangThaiThanhToan.Value == (int)TrangThaiThanhToan.ChuaThanhToan)) && instance.TrangThaiThanhToan == (int)TrangThaiThanhToan.DaThanhToan)
                //{
                //    _InsertPhieuCongNo(check);
                //}
                #endregion  
                var item = _service.UpdateStatus(instance);
                if (item != null)
                {
                    await _service.UnitOfWork.SaveAsync();
                    result.Status = true;
                    result.Message = "Thanh toán thành công! Sinh phiếu công nợ thành công !";
                    result.Data = item;
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                    result.Message = "Cập nhật thanh toán không thành công !";
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex.Message.ToString();
            }
            return Ok(result);
        }
        [CustomAuthorize(Method = "SUA", State = "phieuXuatBan")]
        public async Task<IHttpActionResult> Put(string id, NvXuatBanVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            var check = _service.FindById(instance.Id);
            if (id != instance.Id || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return BadRequest("Không tìm thấy phiếu cha");
            }
            try
            {
                var item = _service.UpdatePhieu(instance);
                if (item != null)
                {
                    if (await _service.UnitOfWork.SaveAsync() > 0)
                    {
                        result.Status = true;
                        result.Data = item;
                        result.Message = "Cập nhật thành công";
                    }
                    else
                    {
                        result.Status = false;
                        result.Data = null;
                        result.Message = "Xảy ra lỗi khi cập nhật";
                        return BadRequest(result.Message);
                    }
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                    result.Message = "Không tìm thấy dòng chi tiết";
                    return BadRequest(result.Message);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                WriteLogs.LogError(e);
                return InternalServerError();
            }
        }

        [Route("PostApproval")]
        [CustomAuthorize(Method = "DUYET", State = "phieuXuatBan")]
        public async Task<IHttpActionResult> PostApproval(NvXuatBanVm.Dto instance)
        {
            var check = _service.FindById(instance.Id);
            var unitCode = _service.GetCurrentUnitCode();
            if (check == null || check.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            check.TrangThai = (int)ApprovalState.IsComplete;
            check.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
            check.ThoiGianDuyetPhieu = int.Parse(DateTime.Now.ToString("HHmmssfff"));
            #region For Only ILOVEVN - B&T
            //NvCongNoVm.Dto obj = _serviceCongNo.GetAmmountCustomerBorrowed(check.MaKhachHang, check.NgayDuyetPhieu.Value);
            //if (check.TrangThaiThanhToan == (int)TrangThaiThanhToan.DaThanhToan)
            //{
            //    check.TienThanhToan = check.TienThanhToan.HasValue ? check.TienThanhToan.Value : 0;
            //    check.TienNoCu = obj.ThanhTienCanTra.Value;
            //}
            //else
            //{
            //    check.TienThanhToan = _serviceCongNo.GetTienThanhToan(check.MaKhachHang, check.NgayDuyetPhieu.Value);
            //    check.TienNoCu = obj.ThanhTienCanTra.Value + check.TienThanhToan.Value;
            //}
            //string ammountMsg = "Nợ cũ " + Math.Round(check.TienNoCu.Value / 1000, 1, MidpointRounding.AwayFromZero) + "k, Thanh toán " + Math.Round(check.TienThanhToan.Value / 1000, 1, MidpointRounding.AwayFromZero)
            //                    + "k, Tổng nợ " + Math.Round((check.TienNoCu.Value - check.TienThanhToan.Value + check.ThanhTienSauVat.Value) / 1000, 1, MidpointRounding.AwayFromZero) + "k";
            //check.NoiDung = ammountMsg;
            #endregion
            check.ObjectState = ObjectState.Modified;
            _service.UnitOfWork.Save();
            #region For Only ILOVEVN - B&T
            //if (check.TrangThaiThanhToan == (int)TrangThaiThanhToan.DaThanhToan)
            //{
            //    _InsertPhieuCongNo(check);
            //}
            #endregion
            switch (_service.Approval(check))
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
        [CustomAuthorize(Method = "XOA", State = "phieuXuatBan")]
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
        #region Excel Old Version
        [Route("PostExportExcel")]
        public HttpResponseMessage PostExportExcel(JObject jsonData)
        {
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                    return null;
                }
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvXuatBanVm.Dto>>(filterResult.Value.Data);
                var streamData = _service.ExportExcel(result, filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuXuatBan.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByMerchandise")]
        public HttpResponseMessage PostExportExcelByMerchandise(JObject jsonData)
        {
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 1, 1);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 1, 1);
                var streamData = _service.ExportExcelByMerchandise(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuXuatBanTheoHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByNhaCungCap")]
        public HttpResponseMessage PostExportExcelByNhaCungCap(JObject jsonData)
        {
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);
                var streamData = _service.ExportExcelByNhaCungCap(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuXuatBanTheoNCC.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByMerchandiseType")]
        public HttpResponseMessage PostExportExcelByMerchandiseType(JObject jsonData)
        {
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);

                var streamData = _service.ExportExcelByMerchandiseType(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuXuatBanTheoLoaiHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }

        [Route("PostExportExcelByMerchandiseGroup")]
        public HttpResponseMessage PostExportExcelByMerchandiseGroup(JObject jsonData)
        {
            var result = new List<NvXuatBanVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvXuatBanVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
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
                            Value = TypeVoucher.XBAN.ToString()
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
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 2, 2);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 2, 2);
                var streamData = _service.ExportExcelByMerchandiseGroup(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuXuatBanTheoNhomHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        #endregion
        #region Excel Lastest Version
        [Route("PostReportXBTongHop")]
        public async Task<IHttpActionResult> PostReportXBTongHop(ParameterXuatBan para)
        {
            NvGiaoDichQuayVm.ReportGDQTongHopNew reporter = new NvGiaoDichQuayVm.ReportGDQTongHopNew();
            List<NvGiaoDichQuayVm.ReportGDQTongHopNew> result = new List<NvGiaoDichQuayVm.ReportGDQTongHopNew>();
            try
            {
                var unitCode = _service.GetCurrentUnitCode();
                reporter.CreateDateNow();
                reporter.FromDay = para.FromDate.Day;
                reporter.FromMonth = para.FromDate.Month;
                reporter.FromYear = para.FromDate.Year;
                reporter.ToDay = para.ToDate.Day;
                reporter.ToMonth = para.ToDate.Month;
                reporter.ToYear = para.ToDate.Year;
                reporter.ToDate = para.ToDate;
                reporter.FromDate = para.FromDate;
                reporter.IsPay = para.IsPay;
                reporter.TenDonVi = CurrentSetting.GetUnitName(unitCode);
                reporter.DiaChiDonVi = CurrentSetting.GetUnitAddress(unitCode);
                //GetNhanVien
                if (HttpContext.Current != null && HttpContext.Current.User is ClaimsPrincipal)
                {
                    var currentUser = (HttpContext.Current.User as ClaimsPrincipal);
                    var name = currentUser.Identity.Name;
                    var nhanVien = _service.UnitOfWork.Repository<AU_NGUOIDUNG>().DbSet.Where(x => x.Username == name).FirstOrDefault();
                    if (nhanVien != null)
                    {
                        reporter.Username = nhanVien.TenNhanVien;
                    }
                    else
                    {
                        reporter.Username = "Administrator";
                    }
                }
                switch (para.ReportType)
                {
                    case TypeReportSelling.XUATBANBUON:
                        reporter.DataDetails.AddRange(_service.ReportXuatBan(para));
                        break;
                    case TypeReportSelling.XUATKHAC:
                        reporter.DataDetails.AddRange(_service.ReportXuatKhac(para));
                        break;
                    case TypeReportSelling.XUATDIEUCHUYEN:
                        reporter.DataDetails.AddRange(_service.ReportDieuChuyenXuat(para));
                        break;
                    default:
                        reporter.DataDetails.AddRange(_service.ReportXuatBan(para));
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return Ok(reporter);
        }
        [Route("PostExportExcelTongHop")]
        public HttpResponseMessage PostExportExcelTongHop(ParameterXuatBan para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.ReportType)
                {
                    case TypeReportSelling.XUATBANBUON:
                        streamData = _service.ExportExcelXBTongHop(para);
                        break;
                    case TypeReportSelling.XUATKHAC:
                        streamData = _service.ExportExcelXKTongHop(para);
                        break;
                    case TypeReportSelling.XUATDIEUCHUYEN:
                        streamData = _service.ExportExcelDCXTongHop(para);
                        break;
                    default:
                        streamData = _service.ExportExcelXBTongHop(para);
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.ReportType)
                {
                    case TypeReportSelling.XUATKHAC:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_TongHop.xlsx" };
                                break;
                        }
                        break;
                    case TypeReportSelling.XUATDIEUCHUYEN:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_TongHop.xlsx" };
                                break;
                        }
                        break;
                    default:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoKho.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHACHHANG:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoKhachHang.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop.xlsx" };
                                break;
                        }
                        break;
                }
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelChiTiet")]
        public HttpResponseMessage PostExportExcelChiTiet(ParameterXuatBan para)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                MemoryStream streamData = new MemoryStream();
                switch (para.ReportType)
                {
                    case TypeReportSelling.XUATBANBUON:
                        streamData = _service.ExportExcelDetail(para);
                        break;
                    case TypeReportSelling.XUATKHAC:
                        streamData = _service.ExportExcelXKDetail(para);
                        break;
                    case TypeReportSelling.XUATDIEUCHUYEN:
                        streamData = _service.ExportExcelDCXDetail(para);
                        break;
                    default:
                        streamData = _service.ExportExcelDetail(para);
                        break;
                }
                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                switch (para.ReportType)
                {
                    case TypeReportSelling.XUATKHAC:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCXuatKhac_ChiTiet.xlsx" };
                                break;
                        }
                        break;
                    case TypeReportSelling.XUATDIEUCHUYEN:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet_TheoKho.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BCDieuChuyenXuat_ChiTiet.xlsx" };
                                break;
                        }
                        break;
                    default:
                        switch (para.GroupBy)
                        {
                            case TypeGroupXuatBan.MANHOMVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet_TheoNhomVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MALOAIVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet_TheoLoaiVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MANHACUNGCAP:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet_TheoNhaCungCap.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAVATTU:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_TongHop_TheoVatTu.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHO:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet_TheoKho.xlsx" };
                                break;
                            case TypeGroupXuatBan.MAKHACHHANG:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet_TheoKhachHang.xlsx" };
                                break;
                            default:
                                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "BaoCao_XB_ChiTiet.xlsx" };
                                break;
                        }
                        break;
                }
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        #endregion
        [Route("GetReport/{id}")]
        public TransferObj<NvXuatBanVm.ReportModel> GetReport(string id)
        {
            var result = new TransferObj<NvXuatBanVm.ReportModel>();
            try
            {
                var data = _service.CreateReport(id);
                if (data != null)
                {
                    NvCongNoVm.Dto obj = _serviceCongNo.GetAmmountCustomerBorrowed(data.MaKhachHang,DateTime.Now.Date);
                    if (data.TrangThai != (int)OrderState.IsComplete)
                    {
                        data.TienTongNo = obj.ThanhTienCanTra.Value - data.TienThanhToan + data.ThanhTienSauVat;
                    }
                    else
                    {
                        data.TienTongNo = data.TienNoCu - data.TienThanhToan + data.ThanhTienSauVat;
                    }
                    result.Data = data;
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message.ToString();
            }
            return result;
        }
        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "phieuXuatBan")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvXuatBanVm.Dto> result = new TransferObj<NvXuatBanVm.Dto>();
            NvXuatBanVm.Dto temp = new NvXuatBanVm.Dto();
            string _ParentUnitCode = _service.GetParentUnitCode();
            try
            {
                NvVatTuChungTu phieu = _service.FindById(id);
                if (phieu != null)
                {
                    temp = Mapper.Map<NvVatTuChungTu, NvXuatBanVm.Dto>(phieu);
                    List<NvVatTuChungTuChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                    if (temp.SoPhieuDatHang != null)
                    {
                        var donDatHang = _service.UnitOfWork.Repository<NvDatHang>().DbSet.FirstOrDefault(x => x.SoPhieuPk == phieu.SoPhieuDatHang);
                        donDatHang = _service.UnitOfWork.Repository<NvDatHang>().DbSet.FirstOrDefault(x => x.SoPhieuPk == phieu.SoPhieuDatHang);
                        if (donDatHang != null)
                        {
                            temp.IdPhieuDatHang = donDatHang.Id;
                        }
                    }
                    temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanVm.DtoDetail>>(chiTietPhieu);
                    temp.DataDetails.ForEach(x => x.CalcResult());
                    List<NvXuatBanVm.DtoDetail> listDetails = new List<NvXuatBanVm.DtoDetail>();
                    string unitCode = _service.GetCurrentUnitCode();
                    string MaKho = temp.MaKhoXuat;
                    MdPeriod ky = CurrentSetting.GetKhoaSo(unitCode);
                    string tableName = ProcedureCollection.GetTableName(ky.Year, ky.Period);
                    //string kyKeToan = _servicePeriod.GetKyKeToan((DateTime)phieu.NgayCT);
                    foreach (NvXuatBanVm.DtoDetail value in temp.DataDetails)
                    {
                        List<MdMerchandiseVm.DataXNT> data = ProcedureCollection.GetDataInventoryByCondition(unitCode, MaKho, value.MaHang, tableName, _ParentUnitCode);
                        if (data.Count == 1)
                        {
                            value.GiaVon = data[0].GiaVon;
                            value.GiaVonVat = value.GiaVon * (1 + data[0].TyLeVATVao / 100);
                            value.TyLeVatRa = data[0].TyLeVATRa;
                            value.TyLeVatVao = data[0].TyLeVATVao;
                        }
                        listDetails.Add(value);
                    }
                    temp.DataDetails = listDetails;

                    result.Data = temp;
                    result.Status = true;
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }

        }

        [HttpGet]
        [Route("CheckExist/{MaPhieu}")]
        public async Task<IHttpActionResult> CheckExist(string MaPhieu)
        {
            NvVatTuChungTu result = new NvVatTuChungTu();
            try
            {
                result = _service.Repository.DbSet.Where(x => x.MaChungTu == MaPhieu && x.LoaiPhieu == "XBAN").FirstOrDefault();
            }
            catch (Exception)
            {
            }
            if (result == null)
                return Ok();
            return Ok(result.Id);
        }

        private bool IsExists(string id)
        {
            return _service.Repository.Find(id) != null;
        }
        private void _InsertPhieuCongNo(NvVatTuChungTu chungTu)
        {
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


        //[Route("GetDetailsData/{id}")]
        //public async Task<IHttpActionResult> GetDetailsData(string id)
        //{

        //    var result = new TransferObj<NvXuatBanVm.Dto>();
        //    var temp = new NvXuatBanVm.Dto();
        //    var phieu = _service.FindById(id);
        //    if (phieu != null)
        //    {
        //        temp = Mapper.Map<NvVatTuChungTu, NvXuatBanVm.Dto>(phieu);
        //        var chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
        //        temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvXuatBanVm.DtoDetail>>(chiTietPhieu);
        //        List<NvXuatBanVm.DtoDetail> listDetails = new List<NvXuatBanVm.DtoDetail>();
        //        var unitCode = _service.GetCurrentUnitCode();
        //        var MaKho = temp.MaKhoXuat;
        //        foreach (var value in temp.DataDetails)
        //        {
        //            value.GiaVon = ProcedureCollection.GetDataInventoryByCondition(unitCode, MaKho, value.MaHang);
        //            listDetails.Add(value);
        //        }
        //        temp.DataDetails = listDetails;
        //        result.Data = temp;
        //        result.Status = true;
        //        return Ok(result);
        //    }
        //    return NotFound();
        //}
    }
}
