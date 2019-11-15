using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
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
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/KhuyenMaiTichDiem")]
    [Route("{id?}")]
    [Authorize]
    public class KhuyenMaiTichDiemController : ApiController
    {
        private readonly INvKhuyenMaiTichDiemService _service;

        public KhuyenMaiTichDiemController(INvKhuyenMaiTichDiemService service)
        {
            _service = service;
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
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiTichDiemVm.Search>>();
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
        [CustomAuthorize(Method = "XEM", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            TransferObj<PagedObj<NvKhuyenMaiTichDiemVm.Dto>> result = new TransferObj<PagedObj<NvKhuyenMaiTichDiemVm.Dto>>();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<NvKhuyenMaiTichDiemVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiTichDiemVm.Search>>();
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
                            Value = LoaiKhuyenMai.TichDiem
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

                result.Data = Mapper.Map<PagedObj<NvChuongTrinhKhuyenMai>, PagedObj<NvKhuyenMaiTichDiemVm.Dto>>(filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }


        [ResponseType(typeof(NvChuongTrinhKhuyenMai))]
        [CustomAuthorize(Method = "THEM", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> Post(NvKhuyenMaiTichDiemVm.Dto instance)
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
        [CustomAuthorize(Method = "SUA", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> Put(string id, NvKhuyenMaiTichDiemVm.Dto instance)
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
        [CustomAuthorize(Method = "XOA", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            NvChuongTrinhKhuyenMai instance = await _service.Repository.FindAsync(id);
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
            var result = new List<NvKhuyenMaiTichDiemVm.Dto>();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<NvKhuyenMaiTichDiemVm.Search>>();
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

                result = Mapper.Map<List<NvChuongTrinhKhuyenMai>, List<NvKhuyenMaiTichDiemVm.Dto>>(filterResult.Value.Data);
                result.ForEach(x =>
                {
                    {
                        var details = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet.Where(u => u.MaChuongTrinh == x.MaChuongTrinh);
                        x.DataDetails = Mapper.Map<List<NvChuongTrinhKhuyenMaiChiTiet>, List<NvKhuyenMaiTichDiemVm.DtoDetail>>(details.ToList());
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
        [CustomAuthorize(Method = "DUYET", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> PostApproval(string id)
        {
            string unitCode = _service.GetCurrentUnitCode();
            NvChuongTrinhKhuyenMai chuongTrinh = _service.FindById(id);

            if (chuongTrinh == null || chuongTrinh.TrangThai == (int)ApprovalState.IsComplete)
            {
                return NotFound();
            }
            chuongTrinh.TrangThai = (int)ApprovalState.IsComplete;
            chuongTrinh.ObjectState = ObjectState.Modified;
            await _service.UnitOfWork.SaveAsync();
            return Ok(true);
        }
        [Route("PostUnApprove/{id}")]
        [CustomAuthorize(Method = "DUYET", State = "nvKMTichDiem")]
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
        [CustomAuthorize(Method = "XEM", State = "nvKMTichDiem")]
        public async Task<IHttpActionResult> GetDetails(string id)
        {
            TransferObj<NvKhuyenMaiTichDiemVm.Dto> result = new TransferObj<NvKhuyenMaiTichDiemVm.Dto>();
            NvKhuyenMaiTichDiemVm.Dto temp = new NvKhuyenMaiTichDiemVm.Dto();

            NvChuongTrinhKhuyenMai phieu = _service.FindById(id);

            if (phieu != null)
            {
                temp = Mapper.Map<NvChuongTrinhKhuyenMai, NvKhuyenMaiTichDiemVm.Dto>(phieu);
                DbSet<NvChuongTrinhKhuyenMaiChiTiet> tb_ChuongTrinhKhuyenMaiChiTiet = _service.UnitOfWork.Repository<NvChuongTrinhKhuyenMaiChiTiet>().DbSet;

                List<NvChuongTrinhKhuyenMaiChiTiet> chiTietChuongTrinhKhuyenMai = tb_ChuongTrinhKhuyenMaiChiTiet.Where(x => x.MaChuongTrinh == phieu.MaChuongTrinh).ToList();
                temp.DataDetails = Mapper.Map<List<NvChuongTrinhKhuyenMaiChiTiet>, List<NvKhuyenMaiTichDiemVm.DtoDetail>>(chiTietChuongTrinhKhuyenMai);

                result.Data = temp;
                result.Status = true;
                return Ok(result);
            }
            return NotFound();
        }
        [Route("GetNewInstance")]
        public NvKhuyenMaiTichDiemVm.Dto GetNewInstance()
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
