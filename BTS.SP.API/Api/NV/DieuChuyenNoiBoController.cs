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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.IO;
using System.Web.Hosting;
using OfficeOpenXml;
using System.Net.Http.Headers;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.SP.API.Utils;
using System.Text;
using System.Globalization;
using System.Web;
using System.Drawing;
using OfficeOpenXml.Style;
using BTS.API.SERVICE;

//using Respone = System.Web.HttpResponse;
namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/DieuChuyenNoiBo")]
    [Route("{id?}")]
    [Authorize]
    public class DieuChuyenNoiBoController : ApiController
    {
        private readonly INvPhieuDieuChuyenNoiBoService _service;
        private bool loadding;
        public DieuChuyenNoiBoController(INvPhieuDieuChuyenNoiBoService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.LoaiPhieu == TypeVoucher.DCX.ToString() && x.UnitCode == unitCode).Select(x => new ChoiceObj { Value = x.MaChungTu, Text = x.MaKhachHang }).ToList();
        }
        [Route("PostQuery")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            //filtered.OrderType = "DESC";
            //filtered.OrderBy = "I_CREATE_DATE";
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
                            Value = TypeVoucher.DCX.ToString()
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
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [Route("PostRecieveQuery")]
        public async Task<IHttpActionResult> PostRecieveQuery(JObject jsonData)
        {
            var result = new TransferObj<PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
            string loaiDCN = ((string)postData.loaiDCN);
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            //var loaiDCN = (((JObject)postData.paged))
            //filtered.OrderBy = "MaChungTu";
            //filtered.OrderType = "DESC";
            var query = new QueryBuilder();
            var unitCode = _service.GetCurrentUnitCode();
            if (loaiDCN == null)
            {
                query = new QueryBuilder
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
                            Value = TypeVoucher.DCN.ToString()
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ()
                        {
                                Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaKhoNhap),
                            Method = FilterMethod.StartsWith,
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
            }
            else
            {
                if (loaiDCN == "1")
                {
                    query = new QueryBuilder
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
                                    Value = TypeVoucher.DCN.ToString()
                                },
                                new QueryFilterLinQ()
                                {
                                    Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                                    Method = FilterMethod.EqualTo,
                                    Value = unitCode
                                },
                                 new QueryFilterLinQ()
                                {
                                     Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaDonViXuat),
                                    Method = FilterMethod.EqualTo,
                                    Value = unitCode
                                },
                                new QueryFilterLinQ()
                                {
                                    Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaKhoNhap),
                                    Method = FilterMethod.StartsWith,
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
                }
                else
                {
                    query = new QueryBuilder
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
                                    Value = TypeVoucher.DCN.ToString()
                                },
                                new QueryFilterLinQ()
                                {
                                    Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().UnitCode),
                                    Method = FilterMethod.EqualTo,
                                    Value = unitCode
                                },
                                new QueryFilterLinQ()
                                {
                                     Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaDonViXuat),
                                    Method = FilterMethod.NotEqualTo,
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
                }

            }
            try
            {
                var filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value);
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
            var result = new TransferObj<PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Method = FilterMethod.And,
                    Value = TypeVoucher.DCX.ToString(),
                    SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                        Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                        Method = FilterMethod.EqualTo,
                        Value = TypeVoucher.DCX.ToString(),
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().TrangThai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)ApprovalState.IsComplete
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaDonViNhan),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().IState),
                            Method = FilterMethod.NotEqualTo,
                            Value = StateTranform.IsComplete.ToString()
                        }
                    }
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
                result.Data = Mapper.Map<PagedObj<NvVatTuChungTu>, PagedObj<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
        [CustomAuthorize(Method = "THEM", State = "phieuDieuChuyenNoiBo")]
        public async Task<IHttpActionResult> Post(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            instance.TrangThai = 0;
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
                WriteLogs.LogError(e);
                return InternalServerError();
            }


        }
        [Route("PostRecieve")]
        [CustomAuthorize(Method = "THEM", State = "nvPhieuDieuChuyenNoiBoNhan")]
        public async Task<IHttpActionResult> PostRecieve(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            var newone = new NvPhieuDieuChuyenNoiBoVm.Dto();
            NvVatTuChungTu item = null;
            var phieuDieuChuyen = _service.FindById(instance.Id);
            if (phieuDieuChuyen != null)
            {
                var exsist = _service.Repository.DbSet.FirstOrDefault(x => x.LenhDieuDong == phieuDieuChuyen.MaChungTu);
                if (exsist == null)
                {
                    newone = _service.MapDtoRecieve(phieuDieuChuyen, instance);
                    newone.NgayCT = DateTime.Now;
                    phieuDieuChuyen.IState = StateTranform.IsComplete.ToString();
                    phieuDieuChuyen.ObjectState = ObjectState.Modified;
                    item = _service.InsertPhieuNhan(newone);
                }
                else
                {
                    return BadRequest("Phiếu này đã được nhập");
                }

            }
            else
            {
                item = _service.InsertPhieuNhan(instance);
            }
            try
            {

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
        [Route("PostNewRecieve")]
        public async Task<IHttpActionResult> PostNewRecieve(NvPhieuDieuChuyenNoiBoVm.Dto instance)
        {
            var result = new TransferObj<NvVatTuChungTu>();
            var newone = new NvPhieuDieuChuyenNoiBoVm.Dto();
            NvVatTuChungTu item = null;
            var phieuDieuChuyen = _service.FindById(instance.Id);

            if (phieuDieuChuyen != null)
            {
                newone = _service.MapDtoRecieve(phieuDieuChuyen, instance);
                phieuDieuChuyen.IState = StateTranform.IsRecievedButNotApproval.ToString();
                phieuDieuChuyen.ObjectState = ObjectState.Modified;
                item = _service.InsertPhieuNhan(instance);
            }
            else
            {
                instance.NgayCT = DateTime.Now;
                instance.NgayDieuDong = DateTime.Now;
                item = _service.InsertPhieuNhan(instance);
            }
            try
            {
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
        [Route("PostPrint")]
        public async Task<IHttpActionResult> PostPrint(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value.Data);
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
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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

                result = Mapper.Map<List<NvVatTuChungTu>, List<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(u => u.MaChungTuPk == x.MaChungTuPk);
                        x.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>>(details.ToList());
                    }

                });
                return Ok(result);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [Route("PostExportExcel")]
        public HttpResponseMessage PostExportExcel(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value.Data);
                var streamData = _service.ExportExcel(result, filtered, TypeVoucher.DCX);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenXuat.xlsx" };
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
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenXuatTheoHang.xlsx" };
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
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenXuatTheoNCC.xlsx" };
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
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenXuatTheoLoaiHang.xlsx" };
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
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCX.ToString()
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
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenXuatTheoNhomHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("postExportExcelReceive")]
        public HttpResponseMessage PostExportExcelReceive(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCN.ToString()
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
                result = Mapper.Map<List<NvVatTuChungTu>, List<NvPhieuDieuChuyenNoiBoVm.Dto>>(filterResult.Value.Data);
                var streamData = _service.ExportExcel(result, filtered, TypeVoucher.DCN);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenNhan.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByMerchandiseReceive")]
        public HttpResponseMessage PostExportExcelByMerchandiseReceive(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<NvVatTuChungTu>>();
            var unitCode = _service.GetCurrentUnitCode();
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                filtered.AdvanceData.TuNgay = filtered.AdvanceData.TuNgay.HasValue ? filtered.AdvanceData.TuNgay.Value : new DateTime(2015, 1, 1);
                filtered.AdvanceData.DenNgay = filtered.AdvanceData.DenNgay.HasValue ? filtered.AdvanceData.DenNgay.Value : new DateTime(2018, 1, 1);
                var streamData = _service.ExportExcelByMerchandiseReceive(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenNhanTheoHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByNhaCungCapReceive")]
        public HttpResponseMessage PostExportExcelByNhaCungCapReceive(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCN.ToString()
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
                var streamData = _service.ExportExcelByNhaCungCapReceive(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenNhanTheoNCC.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByMerchandiseTypeReceive")]
        public HttpResponseMessage PostExportExcelByMerchandiseTypeReceive(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCN.ToString()
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
                var streamData = _service.ExportExcelByMerchandiseTypeReceive(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenNhanTheoLoaiHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("PostExportExcelByMerchandiseGroupReceive")]
        public HttpResponseMessage PostExportExcelByMerchandiseGroupReceive(JObject jsonData)
        {
            var result = new List<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvPhieuDieuChuyenNoiBoVm.Search>>();
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
                            Value = TypeVoucher.DCN.ToString()
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
                var streamData = _service.ExportExcelByMerchandiseGroupReceive(filtered);

                response.StatusCode = HttpStatusCode.OK;
                streamData.Seek(0, SeekOrigin.Begin);
                response.Content = new StreamContent(streamData);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "PhieuDieuChuyenNhanTheoNhomHang.xlsx" };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return response;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
        }
        [Route("GetNewInstance")]
        public NvPhieuDieuChuyenNoiBoVm.Dto GetNewInstance()
        {
            return _service.CreateNewInstance();

        }
        [Route("GetNewReciveInstance")]
        public NvPhieuDieuChuyenNoiBoVm.Dto GetNewReciveInstance()
        {
            return _service.CreateNewReciveInstance();

        }
        [Route("GetNewInstanceFrom/{maChungTu}")]
        public NvPhieuDieuChuyenNoiBoVm.Dto GetNewInstanceFrom(string maChungTu)
        {
            return _service.CreateNewInstance(maChungTu);

        }
        [Route("GetInfoItemDetails/{id}")]
        public async Task<IHttpActionResult> GetInfoItemDetails(string id)
        {
            TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto> result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto>();
            NvPhieuDieuChuyenNoiBoVm.Dto temp = new NvPhieuDieuChuyenNoiBoVm.Dto();
            string _ParentUnitCode = _service.GetParentUnitCode();
            NvVatTuChungTu phieu = _service.FindById(id);
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>(phieu);
                List<NvVatTuChungTuChiTiet> chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>>(chiTietPhieu.OrderBy(x => x.Index).ToList());
                string unitcode = _service.GetCurrentUnitCode();
                foreach (NvPhieuDieuChuyenNoiBoVm.DtoDetail dt in temp.DataDetails)
                {
                    MdMerchandisePrice sp = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu.Equals(dt.MaHang) && x.MaDonVi.Equals(unitcode)).FirstOrDefault();
                    MdMerchandise item = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.FirstOrDefault(x => x.MaVatTu.Equals(dt.MaHang) && x.UnitCode.StartsWith(_ParentUnitCode));

                    if (sp != null)
                    {
                        dt.GiaBanLeVat = sp.GiaBanLeVat;
                    }
                    if (item != null) dt.TenHang = item.TenHang;

                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("WriteDataToExcel")]
        public async Task<IHttpActionResult> WriteDataToExcel(NvPhieuDieuChuyenNoiBoVm.Dto data)
        {
            var result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto>();
            try
            {
                var filenameTemp = "TemPlateNhapMua";
                var pathRelaTemp = string.Format(@"~/Upload/Barcode/");
                var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                var tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                var pathRela = string.Format(@"~/Upload/Barcode/");
                var pathAbs = HostingEnvironment.MapPath(pathRela);
                var getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                getAbsoluteDirectoryInfo.Create();
                var filenameNew = @"Barcode.xls";
                FileInfo newFile = new FileInfo(pathAbs + filenameNew);
                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    //newFile = new FileInfo(pathAbs + @"Barcode.xls");
                    newFile = new FileInfo(pathAbs + filenameNew);
                }
                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    int index = 0;

                    for (int i = 0; i < data.DataDetails.Count; i++)
                    {
                        for (int j = 0; j < data.DataDetails[i].SoLuong; j++)
                        {
                            worksheet.Cells[index + 2, 1].Value = index + 1;
                            worksheet.Cells[index + 2, 2].Value = data.DataDetails[i].MaHang;
                            worksheet.Cells[index + 2, 3].Value = UnicodetoTCVN222(data.DataDetails[i].TenHang);
                            worksheet.Cells[index + 2, 4].Value = data.DataDetails[i].Barcode;
                            worksheet.Cells[index + 2, 5].Value = Formattienviet(data.DataDetails[i].GiaBanLeVat.ToString()) + " VND";
                            worksheet.Cells[index + 2, 6].Value = Formattienviet(data.DataDetails[i].GiaBanLeVat.ToString()) + " VND";
                            worksheet.Cells[index + 2, 7].Value = data.MaKhachHang;
                            worksheet.Cells[index + 2, 8].Value = "1";
                            index++;
                        }
                    }
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    var dataCells = worksheet.Cells[2, 1, totalRows, totalCols];
                    var dataFont = dataCells.Style.Font;
                    dataFont.SetFromFont(new Font(".VnTime", 10));
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    HttpContext.Current.Response.Charset = "UTF-8";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    package.SaveAs(newFile);

                }
                result.Status = true;
                result.Message = filenameNew;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex + "";
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }
        [Route("WriteDataToExcelByShelves")]
        public async Task<IHttpActionResult> WriteDataToExcelByShelves(NvPhieuDieuChuyenNoiBoVm.Dto data)
        {
            TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto> result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto>();
            try
            {
                string filenameTemp = "TemPlateNhapMua";
                string pathRelaTemp = string.Format(@"~/Upload/Barcode/");
                string pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    DirectoryInfo getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                FileInfo tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                string pathRela = string.Format(@"~/Upload/Barcode/");
                string pathAbs = HostingEnvironment.MapPath(pathRela);
                DirectoryInfo getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                getAbsoluteDirectoryInfo.Create();
                string filenameNew = @"Barcode.xls";
                FileInfo newFile = new FileInfo(pathAbs + filenameNew);
                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    //newFile = new FileInfo(pathAbs + @"Barcode.xls");
                    newFile = new FileInfo(pathAbs + filenameNew);
                }
                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int startColumn = 1;
                    int currentRow = 2;
                    int index = 1;
                    for (int i = 0; i < data.DataDetails.Count; i++)
                    {
                        worksheet.Cells[currentRow, startColumn].Value = index;
                        worksheet.Cells[currentRow, startColumn + 1].Value = data.DataDetails[i].MaHang;
                        worksheet.Cells[currentRow, startColumn + 2].Value = UnicodetoTCVN222(data.DataDetails[i].TenHang);
                        worksheet.Cells[currentRow, startColumn + 3].Value = data.DataDetails[i].Barcode;
                        worksheet.Cells[currentRow, startColumn + 4].Value = Formattienviet(data.DataDetails[i].GiaBanLeVat.ToString()) + " VND";
                        worksheet.Cells[currentRow, startColumn + 5].Value = Formattienviet(data.DataDetails[i].GiaBanLeVat.ToString()) + " VND";
                        worksheet.Cells[currentRow, startColumn + 6].Value = data.MaKhachHang;
                        worksheet.Cells[currentRow, startColumn + 7].Value = 1;
                        currentRow++;
                        index++;
                    }
                    int totalRows = worksheet.Dimension.End.Row;
                    int totalCols = worksheet.Dimension.End.Column;
                    ExcelRange dataCells = worksheet.Cells[2, 1, totalRows, totalCols];
                    ExcelFont dataFont = dataCells.Style.Font;
                    dataFont.SetFromFont(new Font(".VnTime", 10));
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                    HttpContext.Current.Response.Charset = "UTF-8";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    package.SaveAs(newFile);

                }
                result.Status = true;
                result.Message = filenameNew;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex + "";
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }
        public static string Formattienviet(string _string)
        {
            string str;
            try
            {
                str = ConvertVNCurencyFormat(decimal.Parse(_string));
            }
            catch
            {
                str = "0";
            }
            return str;
        }
        public static string ConvertVNCurencyFormat(decimal number)
        {
            return number.ToString("C", GetVNeseCultureInfo());
        }
        public static CultureInfo GetVNeseCultureInfo()
        {
            int[] numArray = new int[] { 3 };
            int[] numArray1 = numArray;
            numArray = new int[1];
            int[] numArray2 = numArray;
            CultureInfo cultureInfo = new CultureInfo("vi-VN", true);
            cultureInfo.NumberFormat.CurrencyDecimalDigits = 0;
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyGroupSeparator = ",";
            cultureInfo.NumberFormat.CurrencyGroupSizes = numArray1;
            cultureInfo.NumberFormat.CurrencySymbol = "";
            cultureInfo.NumberFormat.NumberDecimalDigits = 2;
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.NumberGroupSeparator = ",";
            cultureInfo.NumberFormat.NumberGroupSizes = numArray1;
            cultureInfo.NumberFormat.PercentDecimalDigits = 1;
            cultureInfo.NumberFormat.PercentGroupSizes = numArray2;
            cultureInfo.NumberFormat.PercentDecimalSeparator = ".";
            cultureInfo.NumberFormat.PercentGroupSeparator = ",";
            cultureInfo.NumberFormat.PercentSymbol = "%";
            return cultureInfo;
        }
        public String UnicodetoTCVN222(String strUnicode)
        {
            string strReturn = string.Empty;
            string strTest = "a,à,á,ả,ã,ạ,â,ầ,ấ,ẩ,ẫ,ậ,ă,ằ,ắ,ẳ,ẵ,ặ,e,è,é,ẻ,ẽ,ẹ,ê,ề,ế,ể,ễ,ệ,i,ì,í,ỉ,ĩ,ị,o,ò,ó,ỏ,õ,ọ,ơ,ờ,ớ,ở,ỡ,ợ,ô,ồ,ố,ổ,ỗ,ộ,u,ù,ú,ủ,ũ,ụ,ư,ừ,ứ,ử,ữ,ự,y,ỳ,ý,ỷ,ỹ,ỵ,đ";
            for (int j = 0; j < strUnicode.Length; j++)
            {
                if (strTest.Contains(strUnicode[j].ToString().ToLower()))
                {
                    //convert sang TCVN
                    StringBuilder strB = new StringBuilder(strUnicode[j].ToString().ToLower());
                    StringBuilder strtemp = new StringBuilder(strUnicode[j].ToString().ToLower());
                    #region chuyển mã kí tự unicode thường sang TCVN


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] Unicode_char = {             '\u1EF3','\u00FD','\u1EF7','\u1EF9','\u1EF5',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u01B0','\u1EEB','\u1EE9','\u1EED','\u1EEF','\u1EF1',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00F2','\u00F3','\u1ECF','\u00F5','\u1ECD',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u01A1','\u1EDD','\u1EDB','\u1EDF','\u1EE1','\u1EE3',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00F4','\u1ED3','\u1ED1','\u1ED5','\u1ED7','\u1ED9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00EC','\u00ED','\u1EC9','\u0129','\u1ECB',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00EA','\u1EC1','\u1EBF','\u1EC3','\u1EC5','\u1EC7',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00E8','\u00E9','\u1EBB','\u1EBD','\u1EB9',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u0103','\u1EB1','\u1EAF','\u1EB3','\u1EB5','\u1EB7',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00E0','\u00E1','\u1EA3','\u00E3','\u1EA1',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00E2','\u1EA7','\u1EA5','\u1EA9','\u1EAB','\u1EAD',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00F9','\u00FA','\u1EE7','\u0169','\u1EE5',
                //                            đ
                                            '\u0111'};


                    //                            y          ỳ      ý       ỷ           ỹ       ỵ               
                    char[] TCVN_char = {                '\u00FA', '\u00FD','\u00FB','\u00FC','\u00FE',
                //                            ư          ừ       ứ      ử           ữ      ự
                                            '\u00AD','\u00F5','\u00F8','\u00F6','\u00F7','\u00F9',
                //                            o          ò       ó      ỏ           õ      ọ
                                                     '\u00DF','\u00E3','\u00E1','\u00E2','\u00E4',
                //                            ơ          ờ       ớ      ở           ỡ      ợ
                                            '\u00AC','\u00EA','\u00ED','\u00EB','\u00EC','\u00EE',
                //                            ô          ồ       ố      ổ           ỗ      ộ
                                            '\u00AB','\u00E5','\u00E8','\u00E6','\u00E7','\u00E9',
                //                            i          ì       í      ỉ           ĩ      ị
                                                     '\u00D7','\u00DD','\u00D8','\u00DC','\u00DE',
                //                            ê          ề       ế      ể           ễ      ệ
                                            '\u00AA','\u00D2','\u00D5','\u00D3','\u00D4','\u00D6',
                //                            e          è       é      ẻ           ẽ      ẹ
                                                     '\u00CC','\u00D0','\u00CE','\u00CF','\u00D1',
                //                            ă          ằ       ắ      ẳ           ẵ      ặ
                                            '\u00A8','\u00BB','\u00BE','\u00BC','\u00BD','\u00C6',
                //                            a          à       á      ả           ã      ạ
                                                     '\u00B5','\u00B8','\u00B6','\u00B7','\u00B9',
                //                            â          ầ       ấ      ẩ           ẫ      ậ
                                            '\u00A9','\u00C7','\u00CA','\u00C8','\u00C9','\u00CB',
                //                            u          ù       ú      ủ           ũ      ụ
                                                     '\u00EF','\u00F3','\u00F1','\u00F2','\u00F4',
                //                            đ
                                            '\u00AE'};

                    for (int i = 0; i < Unicode_char.Length; i++)
                    {
                        char a = Unicode_char[i];
                        char b = TCVN_char[i];
                        strB.Replace(a, b);
                        if (strtemp.ToString() != strB.ToString())
                        {
                            break;
                        }
                    }
                    strReturn = strReturn + strB.ToString();
                    #endregion
                }
                else
                {
                    //ko convert
                    strReturn = strReturn + strUnicode[j].ToString();
                }

            }
            return strReturn;
        }
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "phieuDieuChuyenNoiBo")]
        public async Task<IHttpActionResult> Put(string id, NvPhieuDieuChuyenNoiBoVm.Dto instance)
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

        [HttpGet]
        [Route("CheckConnectServer/{unitCode}")]
        public IHttpActionResult CheckConnectServer(string unitCode)
        {
            return Ok(_service.CheckConnectServer(unitCode));
        }

        [Route("PostApproval")]
        [CustomAuthorize(Method = "DUYET", State = "phieuDieuChuyenNoiBo")]
        public async Task<IHttpActionResult> PostApproval(NvVatTuChungTu instance)
        {
            var result = new TransferObj<string>();
            string unitCode = _service.GetCurrentUnitCode();
            NvVatTuChungTu chungTu = _service.FindById(instance.Id);
            if (chungTu == null || chungTu.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            if (chungTu.LoaiPhieu == TypeVoucher.DCN.ToString())
            {
                if (!string.IsNullOrEmpty(chungTu.LenhDieuDong))
                {
                    NvVatTuChungTu phieuDCX = _service.Repository.DbSet.FirstOrDefault(x => x.MaChungTu == chungTu.LenhDieuDong);
                    if (phieuDCX != null)
                    {
                        phieuDCX.IState = StateTranform.IsComplete.ToString();
                        phieuDCX.ObjectState = ObjectState.Modified;
                    }
                }
                chungTu.TrangThai = (int)ApprovalState.IsComplete;
                chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
                chungTu.IState = StateTranform.IsComplete.ToString();
                chungTu.ObjectState = ObjectState.Modified;
                await _service.UnitOfWork.SaveAsync();
            }
            else
            {
                chungTu.TrangThai = (int)ApprovalState.IsComplete;
                chungTu.NgayDuyetPhieu = CurrentSetting.GetNgayKhoaSo(unitCode);
                chungTu.IState = StateTranform.IsRecievedButNotApproval.ToString();
                chungTu.ObjectState = ObjectState.Modified;
                await _service.UnitOfWork.SaveAsync();
                // cùng đơn vị
                if (chungTu.MaDonViNhan == unitCode)
                {
                    NvPhieuDieuChuyenNoiBoVm.Dto temp = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>(chungTu);
                    await PostRecieve(temp);
                    chungTu.IState = StateTranform.IsComplete.ToString();
                    chungTu.ObjectState = ObjectState.Modified;
                    await _service.UnitOfWork.SaveAsync();
                }
                else  // khác đơn vị
                {
                    // điều chuyển xuất khác đơn vị sẽ lưu phiếu tới đơn vị theo API
                    if (!_service.InsertVoucherToUnitCode(instance))
                    {
                        return BadRequest("Xảy ra lỗi");
                    }
                    else
                    {
                        result.Message = "Điều chuyển phiếu thành công";
                        result.Status = true;
                        result.Data = "Success";
                    }
                }
            }
            switch (_service.Approval(chungTu.Id, chungTu.LoaiPhieu))
            {
                case StateProcessApproval.NoPeriod:
                    try
                    {
                        await _service.UnitOfWork.SaveAsync();
                        result.Message = "Kỳ kế toán không đúng";
                        result.Status = true;
                        result.Data = "NoPeriod";
                        return Ok(result);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Success:
                    try
                    {
                        await _service.UnitOfWork.SaveAsync();
                        result.Message = "Điều chuyển đến siêu thị và duyệt phiếu thành công";
                        result.Status = true;
                        result.Data = "Complete";
                        return Ok(result);
                    }
                    catch (Exception e)
                    {
                        return InternalServerError();
                    }
                case StateProcessApproval.Failed:
                    result.Message = "Lỗi không duyệt được phiếu";
                    result.Status = true;
                    result.Data = "Failed";
                    return Ok(result);
                default:
                    break;
            }
            return BadRequest("Không thể duyệt phiếu này");
        }



        [CustomAuthorize(Method = "XOA", State = "phieuDieuChuyenNoiBo")]
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
        [Route("CountItem")]
        public async Task<IHttpActionResult> CountItem()
        {
            try
            {
                var unitCode = _service.GetCurrentUnitCode();
                var query = new QueryBuilder
                {
                    Filter = new QueryFilterLinQ()
                    {
                        Method = FilterMethod.And,
                        Value = TypeVoucher.DCX.ToString(),
                        SubFilters = new List<IQueryFilter>()
                    {
                        new QueryFilterLinQ()
                        {
                        Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().LoaiPhieu),
                        Method = FilterMethod.EqualTo,
                        Value = TypeVoucher.DCX.ToString(),
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().TrangThai),
                            Method = FilterMethod.EqualTo,
                            Value = (int)ApprovalState.IsComplete
                        },
                        new QueryFilterLinQ()
                        {
                            Property = ClassHelper.GetProperty(() => new NvVatTuChungTu().MaDonViNhan),
                            Method = FilterMethod.EqualTo,
                            Value = unitCode
                        }
                    }
                    }
                };
                var result = _service.Repository.Query(query).Count;
                return Ok(result);
            }
            catch (Exception e)
            {

                return InternalServerError();
            }

        }
        [Route("GetReport/{id}")]
        public async Task<IHttpActionResult> GetReport(string id)
        {
            TransferObj<NvPhieuDieuChuyenNoiBoVm.ReportModel> result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.ReportModel>();
            NvPhieuDieuChuyenNoiBoVm.ReportModel data = _service.CreateReport(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetDetails/{id}")]
        [CustomAuthorize(Method = "XEM", State = "phieuDieuChuyenNoiBo")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            var result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto>();
            var temp = new NvPhieuDieuChuyenNoiBoVm.Dto();
            var phieu = _service.FindById(id);
            var unitCode = _service.GetCurrentUnitCode();
            if (phieu != null)
            {
                temp = Mapper.Map<NvVatTuChungTu, NvPhieuDieuChuyenNoiBoVm.Dto>(phieu);
                var chiTietPhieu = _service.UnitOfWork.Repository<NvVatTuChungTuChiTiet>().DbSet.Where(x => x.MaChungTuPk == phieu.MaChungTuPk).ToList();
                temp.DataDetails = Mapper.Map<List<NvVatTuChungTuChiTiet>, List<NvPhieuDieuChuyenNoiBoVm.DtoDetail>>(chiTietPhieu);
                foreach (NvPhieuDieuChuyenNoiBoVm.DtoDetail dt in temp.DataDetails)
                {
                    var maKho = unitCode != temp.MaDonViNhan ? temp.MaKhoXuat : temp.MaKhoNhap;
                    var invetory = ProcedureCollection.GetInventoryItem(dt.MaHang, maKho, unitCode);
                    var item = _service.UnitOfWork.Repository<MdMerchandise>().DbSet.Where(x => x.MaVatTu.Equals(dt.MaHang)).FirstOrDefault();
                    dt.SoLuongTon = invetory != null ? invetory.ClosingQuantity : 0;
                    if (item != null) dt.TenHang = item.TenHang;
                    var price = _service.UnitOfWork.Repository<MdMerchandisePrice>().DbSet.Where(x => x.MaVatTu == dt.MaHang).FirstOrDefault();
                    if (price != null) dt.GiaBanLeVat = price.GiaBanLeVat;
                }
                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();

        }
        [Route("ImportExcel")]
        public async Task<IHttpActionResult> ImportExcel(NvPhieuDieuChuyenNoiBoVm.Dto data)
        {
            var result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.Dto>();
            try
            {
                var filenameTemp = "TemPlateNhapMua";
                var pathRelaTemp = string.Format(@"~/Upload/Barcode/");

                var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                var tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xls");
                var pathRela = string.Format(@"~/Upload/Barcode/");
                var pathAbs = HostingEnvironment.MapPath(pathRela);
                var getAbsoluteDirectoryInfo = new DirectoryInfo(pathAbs);
                getAbsoluteDirectoryInfo.Create();
                FileInfo newFile =
                    new FileInfo(pathAbs + "Barcode" + @".xls");

                if (newFile.Exists) //Kiểm tra file mới này đã tồn tại chưa
                {
                    newFile.Delete();
                    newFile = new FileInfo(pathAbs + @"Barcode.xls");
                }

                using (ExcelPackage package = new ExcelPackage(newFile, tempFile))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    int index = 0;
                    for (int i = 0; i < data.DataDetails.Count; i++)
                    {
                        for (int j = 0; j < data.DataDetails[i].SoLuong; j++)
                        {
                            index++;
                        }
                    }

                    package.SaveAs(newFile);
                }
                result.Status = true;
                result.Data = data;
                result.Message = "Success";

            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = null;
                result.Message = ex + "";

            }
            return Ok(result);
        }
        [Route("GetDataExcel")]
        public NvPhieuDieuChuyenNoiBoVm.Dto GetDataExcel()
        {
            var result = new NvPhieuDieuChuyenNoiBoVm.Dto();
            try
            {
                var filenameTemp = "Test";
                var pathRelaTemp = string.Format(@"~/Upload/Barcode/");

                var pathAbsTemp = HostingEnvironment.MapPath(pathRelaTemp);
                if (pathAbsTemp != null)
                {
                    var getAbsoluteDirectoryInfoReport = new DirectoryInfo(pathAbsTemp);
                    getAbsoluteDirectoryInfoReport.Create();
                }
                var tempFile = new FileInfo(pathAbsTemp + filenameTemp + ".xlsx");
                result.NgayCT = DateTime.Now;
                result.NgayDieuDong = DateTime.Now;
                using (ExcelPackage package = new ExcelPackage(tempFile))
                {
                    var worksheet = package.Workbook.Worksheets[1];
                    var rowCount = worksheet.Dimension.End.Row;
                    for (int i = 13; i <= rowCount - 9; i++) // vì theo template excel
                    {
                        var data = new NvPhieuDieuChuyenNoiBoVm.DtoDetail();
                        data.MaHang = worksheet.Cells[i, 2].Value.ToString();
                        data.Barcode = worksheet.Cells[i, 3].Value.ToString();
                        data.TenHang = worksheet.Cells[i, 5].Value.ToString();
                        data.DonViTinh = worksheet.Cells[i, 8].Value.ToString();
                        data.SoLuong = Decimal.Parse(worksheet.Cells[i, 9].Value.ToString());
                        data.SoLuongCT = Decimal.Parse(worksheet.Cells[i, 9].Value.ToString());
                        data.DonGia = Decimal.Parse(worksheet.Cells[i, 11].Value.ToString());
                        data.ThanhTien = Decimal.Parse(worksheet.Cells[i, 21].Value.ToString());
                        result.DataDetails.Add(data);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [Route("GetPhieuByLenhDieuDong/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPhieuByLenhDieuDong(string code)
        {
            string result = _service.Repository.DbSet.Where(x => x.LenhDieuDong.Equals(code)).Select(x => x.MaChungTu).FirstOrDefault();
            return Ok(result);
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
        [Route("GetReportReceive/{id}")]
        public async Task<IHttpActionResult> GetReportReceive(string id)
        {
            TransferObj<NvPhieuDieuChuyenNoiBoVm.ReportModel> result = new TransferObj<NvPhieuDieuChuyenNoiBoVm.ReportModel>();
            NvPhieuDieuChuyenNoiBoVm.ReportModel data = _service.CreateReportReceive(id);
            if (data != null)
            {
                result.Data = data;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        public NvPhieuDieuChuyenNoiBoVm.DtoDetail data { get; set; }
    }
}