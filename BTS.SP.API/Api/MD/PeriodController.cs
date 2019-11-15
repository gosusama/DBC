using AutoMapper;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.DCL;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.SERVICE.NV;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Period")]
    [Route("{id?}")]
    [Authorize]
    public class PeriodController : ApiController
    {
        private readonly IMdPeriodService _service;
        public readonly IDclCloseoutService _close;
        public PeriodController(IMdPeriodService service, IDclCloseoutService close)
        {
            _service = service;
            _close = close;
        }
        [Route("GetTableNameByDate")]
        [HttpPost]
        public IHttpActionResult GetTableNameByDate(ParameterInventory param)
        {
            if (param.FromDate != null || param.ToDate != null)
            {
                var result = new TransferObj();
                DateTime day = new DateTime(param.FromDate.Year, param.FromDate.Month, param.FromDate.Day, 0, 0, 0);
                var data = _service.Repository.DbSet.FirstOrDefault(x => x.FromDate == day);
                if (data != null)
                {
                    string tableName = ProcedureCollection.GetTableName(data.Year, data.Period);
                    result.Data = tableName;
                    result.Status = true;
                    result.Message = "Ok";
                    return Ok(result);
                }
                else
                {
                    result.Message = "Chưa khởi tạo kỳ kế toán";
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
            
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode == unitCode).Select(x => new ChoiceObj { Value = x.Id, Text = x.Name, Id = x.Id }).ToList();
        }
        [Route("CheckUnClosingOut")]
        [HttpGet]
        public IHttpActionResult CheckUnClosingOut()
        {
            var result = new TransferObj<List<MdPeriod>>();
            var _unitCode = _service.GetCurrentUnitCode();
            //trạng thái bỏ khóa sổ
            var check = _service.Repository.DbSet.Where(x => x.UnitCode == _unitCode && x.TrangThai == (int)ProcessState.IsUnClosingOut).ToList();
            if (check.Count > 0)
            {
                result.Status = true;
                result.Message = "Mở khóa sổ";
                result.Data = check;
            }
            else
            {
                result.Status = false;
                result.Message = "Không tồn tại mở khóa sổ";
                result.Data = null;
            }
            return Ok(result);
        }
        [Route("GetKyKeToan")]
        public async Task<IHttpActionResult> GetKyKeToan()
        {
            var result = new TransferObj<MdPeriod>();
            var _unitCode = _service.GetCurrentUnitCode();
            var periodCollection = _service.Repository.DbSet.Where(x => x.UnitCode == _unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
            if (periodCollection != null)
            {
                var lastPeriod = periodCollection.OrderByDescending(x => new { x.Year,x.Period }).FirstOrDefault();
                if (lastPeriod != null)
                {
                    result.Status = true;
                    result.Data = lastPeriod;
                }
                else
                {
                    result.Status = false;
                    result.Data = new MdPeriod();
                }
            }
            return Ok(result);
        }

        [Route("GetNewParameter")]
        public async Task<IHttpActionResult> GetNewParameter()
        {
            var unitCode = _service.GetCurrentUnitCode();
            var currentDate = DateTime.Now.Date;
            var result = new ParameterInventory()
            {
                ToDate = currentDate,
                FromDate = currentDate,
                MinDate = currentDate,
                MaxDate = currentDate,
                UnitCode = unitCode,
            };
            var periodCollection = _service.Repository.DbSet.Where(x => x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete);
            if (periodCollection != null && periodCollection.Count() > 0)
            {
                var lastPeriod = periodCollection.OrderByDescending(x => new { x.Year, x.Period }).FirstOrDefault();
                var originalPeriod = periodCollection.OrderBy(x => x.Period).FirstOrDefault();
                result.MaxDate = lastPeriod.ToDate;
                result.MinDate = originalPeriod.FromDate;
                result.ToDate = lastPeriod.ToDate;
                if (lastPeriod.ToDate.AddMonths(-1) >= originalPeriod.FromDate)
                {
                    result.FromDate = lastPeriod.ToDate.AddMonths(-1);
                }
                else
                {
                    result.FromDate = originalPeriod.FromDate;
                }
                result.Year = lastPeriod.Year;
            }
            return Ok(result);
        }
        
        [Route("GetNextPeriod")]
        [HttpPost]
        public async Task<IHttpActionResult> GetNextPeriod(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            var unitCode = _service.GetCurrentUnitCode();
            DateTime nDay = instance.FromDate.AddDays(+1);
            DateTime nextDay = new DateTime(nDay.Year, nDay.Month, nDay.Day, 0, 0, 0);
            var kyKeToan = _service.Repository.DbSet.FirstOrDefault(x => x.ToDate == nextDay && x.FromDate == nextDay && x.UnitCode == unitCode);
            if (kyKeToan != null)
            {
                result.Status = true;
                result.Message = "Oke";
                result.Data = kyKeToan;
            }
            else
            {
                result.Status = true;
                result.Message = "NotFound";
                result.Data = kyKeToan;
            }
            return Ok(result);
        }

        [Route("GetCurrentPeriod")]
        [CustomAuthorize(Method = "XEM", State = "period")]
        public IHttpActionResult GetCurrentPeriod()
        {
            var result = new TransferObj<List<MdPeriod>>();
            var unitCode = _service.GetCurrentUnitCode();
            List<MdPeriod> listData = new List<MdPeriod>();
            var periods = _service.Repository.DbSet
                .Where(x => x.UnitCode == unitCode && x.TrangThai == (int)ApprovalState.IsComplete)
                .OrderByDescending(x => new {x.Year,x.Period});
            if (periods.Count() > 0)
            {
                var currentPeriods = periods.First();
                DateTime bDay =  currentPeriods.FromDate.AddDays(-6);
                DateTime beginDay = new DateTime(bDay.Year, bDay.Month, bDay.Day, 0, 0, 0);
                DateTime nDay = currentPeriods.FromDate.AddDays(+5);
                DateTime nowDay = new DateTime(nDay.Year, nDay.Month, nDay.Day, 0, 0, 0);
                var kyKeToan = _service.Repository.DbSet.Where(x => x.ToDate < nowDay && x.FromDate > beginDay  && x.UnitCode == unitCode).ToList();
                if (kyKeToan.Count > 0)
                {
                    listData = kyKeToan;
                }
                result.Status = true;
                result.Data = listData;
            }
            else
            {
                result.Status = false;
                result.Data = null;
            }
            return Ok(result);
        }

        [Route("PostSelectData")]
        [CustomAuthorize(Method = "XEM", State = "period")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdPeriodVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdPeriod>>();
            var unitCode = _service.GetCurrentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdPeriod().UnitCode),
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

                result.Data = Mapper.Map<PagedObj<MdPeriod>, PagedObj<ChoiceObj>>
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
        [CustomAuthorize(Method = "XEM", State = "period")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<MdPeriodVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<MdPeriod>>();
            var unitCode = _service.GetCurrentUnitCode();
            filtered.OrderBy = "ToDate";
            filtered.OrderType = "DESC";
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new MdPeriod().UnitCode),
                    Method = FilterMethod.EqualTo,
                    Value = unitCode
                },
                Orders = new List<IQueryOrder>()
                {
                    new QueryOrder()
                    {
                        Field = ClassHelper.GetPropertyName(() => new MdPeriod().Period),
                        Method = OrderMethod.ASC
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

                result.Data = filterResult.Value;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("PostUpdateGiaVon")]
        public async Task<IHttpActionResult> PostUpdateGiaVon(MdPeriod instance)
        {

            try
            {
                var exsist = _service.Find(instance);
                if (exsist == null)
                {
                    return NotFound();
                }
                var processName = CodeProcess.CAPNHATGIAVON.ToString();
                var unitCode = _service.GetCurrentUnitCode();
                var process = _service.UnitOfWork.Repository<MdMonitorProcess>().DbSet.FirstOrDefault(x => x.ProcessCode == processName && x.UnitCode == unitCode && x.State == ProcessState.IsRunning);
                if (process != null)
                {
                    process.State = ProcessState.IsRunning;
                    return BadRequest("Đang cập nhật giá vốn");
                }
                if (!_service.UpDateGiaVon(instance))
                {
                    return BadRequest("Chưa thể cập nhật được giá vốn");
                }

                return Ok();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("Post")]
        [ResponseType(typeof(MdPeriod))]
        [CustomAuthorize(Method = "THEM", State = "period")]
        public async Task<IHttpActionResult> Post(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            try
            {
                instance.ICreateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.ICreateDate = DateTime.Now;
                var item = _service.Insert(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Thêm mới thành công";
                result.Data = item;
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }

        [Route("PostCurrentPeriod")]
        [HttpPost]
        public async Task<IHttpActionResult> PostCurrentPeriod(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            try
            {
                var item = _service.InitializePeriod(instance);
                _service.UnitOfWork.Save();
                var exist = _service.Find(instance);
                if (exist != null)
                {
                    if (instance.TrangThai == (int)ApprovalState.IsComplete)
                    {
                        return BadRequest("Kỳ này đã được duyệt!");
                    }
                    var unitCode = _service.GetCurrentUnitCode();
                    var tableNameKyTruoc = item.GetTableName();
                    var talbeName = instance.GetTableName();
                    var preTalbeName = _service.GetPreTableName(instance);
                    ProcessState stateOfPeriod = _service.CheckProcess(instance);
                    var checkExist = string.Format("SELECT TABLE_NAME FROM DBA_TABLES where TABLE_NAME = '{0}'", tableNameKyTruoc);
                    using (var ctx = new ERPContext())
                    {
                        var existKyTruoc = ctx.Database.SqlQuery<InventoryExpImp>(checkExist).ToList();
                        if (existKyTruoc.Count == 0)
                        {
                            //tạo mới bảng XNT kỳ trước
                            _close.CreateTableXNT_KhoaSo(null, tableNameKyTruoc, instance.UnitCode, instance.Year,
                                instance.Period);
                        }
                        else
                        {
                            stateOfPeriod = ProcessState.IsPending;
                        }
                        switch (stateOfPeriod)
                        {
                            case ProcessState.IsPending:
                                try
                                {
                                    _close.ProcedureCloseInventory(preTalbeName, talbeName, instance.UnitCode, instance.Year, instance.Period);
                                    exist.TrangThai = (int)ApprovalState.IsComplete;
                                    exist.ObjectState = BTS.API.ENTITY.ObjectState.Modified;
                                    result.Status = true;
                                    result.Message = "Khóa sổ thành công";
                                    _service.UnitOfWork.Save();
                                }
                                catch (Exception e)
                                {
                                    return InternalServerError(e);
                                }
                                break;
                            case ProcessState.IsComplete:
                                break;
                            case ProcessState.IsRunning:
                                return BadRequest("Đang trong quá trình khóa");
                            case ProcessState.IsError:
                                break;
                            default:
                                break;
                        }
                    }

                }
                result.Status = true;
                result.Data = item;
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [HttpPut]
        [CustomAuthorize(Method = "SUA", State = "period")]
        public async Task<IHttpActionResult> Put(string id, MdPeriod instance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != instance.Id)
            {
                return BadRequest();
            }
            var result = new TransferObj<MdPeriod>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                instance.IUpdateBy = _service.GetClaimsPrincipal().Identity.Name;
                instance.IUpdateDate = DateTime.Now;
                var item = _service.Update(instance);
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Data = item;
                result.Message = "Cập nhật thành công";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = false;
                result.Message = e.Message;
                return Ok(result);
            }
        }

        [HttpDelete]
        [CustomAuthorize(Method = "XOA", State = "period")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            MdPeriod instance = await _service.Repository.FindAsync(id);
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

        [ResponseType(typeof(MdPeriod))]
        [CustomAuthorize(Method = "XEM", State = "period")]

        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }

        [Route("GetStatus")]
        public IHttpActionResult GetStatus()
        {
            var processName = CodeProcess.KHOASO.ToString();
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.UnitOfWork.Repository<MdMonitorProcess>().DbSet.FirstOrDefault(x => x.ProcessCode == processName && x.UnitCode == unitCode);
            return Ok(instance);
        }

        [Route("GetUpdateGiaVonStatus")]
        public IHttpActionResult GetUpdateGiaVonStatus()
        {
            var processName = CodeProcess.CAPNHATGIAVON.ToString();
            var unitCode = _service.GetCurrentUnitCode();
            var instance = _service.UnitOfWork.Repository<MdMonitorProcess>().DbSet.FirstOrDefault(x => x.ProcessCode == processName && x.UnitCode == unitCode);
            return Ok(instance);
        }

        [Route("GetByUnit/{maDonVi}")]
        public List<ChoiceObj> GetByUnit(string maDonVi)
        {
            if (string.IsNullOrEmpty(maDonVi)) return null;
            var data = _service.Repository.DbSet;
            return data.Where(x => x.UnitCode == maDonVi && x.TrangThai == 10).OrderBy(x => x.Period).Select(x => new ChoiceObj { Value = x.Period.ToString(), Text = x.Name, Id = x.Id }).ToList();
        }

        [Route("PostCapNhatSoXuat")]
        public async Task<IHttpActionResult> PostCapNhatSoXuat(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            var exist = _service.Find(instance);
            if (exist != null)
            {
                instance.TrangThai = (int)ApprovalState.IsNotApproval;
                var talbeName = instance.GetTableName();
                var preTalbeName = _service.GetPreTableName(instance);
                ProcessState stateOfPeriod = _service.CheckProcess(instance);
                switch (stateOfPeriod)
                {
                    case ProcessState.IsPending:
                        try
                        {
                            _close.ProcedureCloseInventory(preTalbeName, talbeName, instance.UnitCode, instance.Year, instance.Period);

                            exist.TrangThai = (int)ApprovalState.IsComplete;
                            exist.ObjectState = BTS.API.ENTITY.ObjectState.Modified;
                            result.Status = true;
                            result.Message = "Cập nhật thành công";
                            _service.UnitOfWork.Save();
                        }
                        catch (Exception e)
                        {
                            return InternalServerError(e);
                        }
                        break;
                    case ProcessState.IsComplete:
                        break;
                    case ProcessState.IsRunning:
                        return BadRequest("Đang trong quá trình khóa");
                    case ProcessState.IsError:
                        break;
                    default:
                        break;
                }
            }
            return Ok(result);

        }

        [Route("OpenApproval")]
        public IHttpActionResult OpenApproval(MdPeriod instance)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj();
            DateTime temp;
            if (DateTime.TryParse(instance.ToDate.ToString(), out temp))
            {
                try
                {
                    var listOpen = _service.Repository.DbSet.Where(x => x.FromDate == instance.FromDate && x.ToDate == instance.ToDate && x.TrangThai == 10 && x.UnitCode == _unitCode).ToList();
                    if (listOpen.Count > 0)
                    {
                        foreach (var data in listOpen)
                        {
                            data.TrangThai = (int) ProcessState.IsUnClosingOut;
                            _service.Update(data);
                        }
                        _service.UnitOfWork.SaveAsync();
                        result.Status = true;
                        result.Message = "Mở khóa sổ thành công";
                        result.Data = listOpen;
                    }
                    else
                    {
                        result.Status = false;
                        result.Message = "Kỳ này đang mở";
                    }
                }
                catch (Exception ex)
                {
                    result.Status = false;
                    result.Message = "Lỗi: "+ex;
                }
            }
            else
            {
                result.Status = false;
                result.Message = "Không có dữ liệu truyền lên ! Kiểm tra lại";
            }
            return Ok(result);
        }
        [Route("PostMultiplePeriod")]
        public async Task<IHttpActionResult> PostMultiplePeriod(MdPeriod instance)
        {
            var result = new TransferObj<List<MdPeriod>>();
            result.Data = new List<MdPeriod>();
            var exist = _service.Find(instance);
            if (exist != null)
            {
                if (instance.TrangThai == (int)ApprovalState.IsComplete)
                {
                    return BadRequest("Kỳ này đã được duyệt!");
                }
                //tìm kỳ cuối cùng khóa
                var unitCode = _service.GetCurrentUnitCode();
                var periodUnlock =
                    _service.Repository.DbSet.OrderBy(x=>x.Period).FirstOrDefault(
                        x => x.Year == exist.Year && x.UnitCode == unitCode && x.Period < exist.Period &&
                             (x.TrangThai == (int) ApprovalState.IsNotApproval ||
                              x.TrangThai == (int) ApprovalState.IsUnClosingOut));
                if (periodUnlock != null)
                {
                    var beforePeriodNotClose = _service.Repository.DbSet.Any(x => x.Year == periodUnlock.Year && x.Period < periodUnlock.Period && x.TrangThai != (int)ApprovalState.IsComplete && x.UnitCode == unitCode);
                    if (beforePeriodNotClose)
                    {
                        return BadRequest("Kỳ trước ngày này đang mở!");
                    }
                    else
                    {
                        var listPeriod =
                            _service.Repository.DbSet.Where(
                                x =>
                                    x.FromDate >= periodUnlock.FromDate && x.FromDate <= exist.FromDate &&
                                    x.UnitCode == unitCode).OrderBy(x=>x.Period).ToList();
                        if (listPeriod.Count > 0)
                        {
                            for (int a = 0; a < listPeriod.Count; a++)
                            {
                                var talbeName = listPeriod[a].GetTableName();
                                var preTalbeName = _service.GetPreTableName(listPeriod[a]);
                                ProcessState stateOfPeriod = _service.CheckProcess(listPeriod[a]);
                                switch (stateOfPeriod)
                                {
                                    case ProcessState.IsPending:
                                        try
                                        {
                                            _close.ProcedureCloseInventory(preTalbeName, talbeName, listPeriod[a].UnitCode, listPeriod[a].Year, listPeriod[a].Period);
                                            listPeriod[a].TrangThai = (int)ApprovalState.IsComplete;
                                            listPeriod[a].ObjectState = BTS.API.ENTITY.ObjectState.Modified;
                                            _service.UnitOfWork.Save();
                                            result.Data.Add(listPeriod[a]);
                                            result.Status = true;
                                            result.Message = "Khóa sổ thành công";
                                        }
                                        catch (Exception e)
                                        {
                                            return InternalServerError(e);
                                        }
                                        break;
                                    case ProcessState.IsComplete:
                                        break;
                                    case ProcessState.IsRunning:
                                        return BadRequest("Đang trong quá trình khóa");
                                    case ProcessState.IsError:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return Ok(result);
        }

        [Route("PostAppoval")]
        public async Task<IHttpActionResult> PostAppoval(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            var exist = _service.Find(instance);
            if (exist != null)
            {
                if (instance.TrangThai == (int)ApprovalState.IsComplete)
                {
                    return BadRequest("Kỳ này đã được duyệt!");
                }
                var unitCode = _service.GetCurrentUnitCode();
                var exsitPeriodNotClose = _service.Repository.DbSet.Any(x => x.Year == exist.Year && x.Period < exist.Period && x.TrangThai != (int)ApprovalState.IsComplete && x.UnitCode == unitCode);
                if (exsitPeriodNotClose)
                {
                    return BadRequest("Chưa khóa kỳ trước");
                }

                var talbeName = instance.GetTableName();
                var preTalbeName = _service.GetPreTableName(instance);
                ProcessState stateOfPeriod = _service.CheckProcess(instance);
                switch (stateOfPeriod)
                {
                    case ProcessState.IsPending:
                        try
                        {
                            _close.ProcedureCloseInventory(preTalbeName, talbeName, instance.UnitCode, instance.Year, instance.Period);
                            exist.TrangThai = (int)ApprovalState.IsComplete;
                            exist.ObjectState = ObjectState.Modified;
                            result.Status = true;
                            result.Message = "Khóa sổ thành công";
                            _service.UnitOfWork.Save();
                        }
                        catch (Exception e)
                        {
                            return InternalServerError(e);
                        }
                        break;
                    case ProcessState.IsComplete:
                        break;
                    case ProcessState.IsRunning:
                        return BadRequest("Đang trong quá trình khóa");
                    case ProcessState.IsError:
                        break;
                    default:
                        break;
                }

            }
            return Ok(result);
        }

        [Route("PostUpdateGiaoDich")]
        public async Task<IHttpActionResult> PostUpdateGiaoDich(MdPeriod instance)
        {
            var result = new TransferObj<MdPeriod>();
            var exist = _service.Find(instance);
            if (exist != null)
            {
                var unitCode = _service.GetCurrentUnitCode();
                var exsitPeriodNotClose = _service.Repository.DbSet.Any(x => x.Year == exist.Year && x.Period < exist.Period && x.TrangThai != (int)ApprovalState.IsComplete && x.UnitCode == unitCode);
                if (exsitPeriodNotClose)
                {
                    return BadRequest("Chưa khóa kỳ trước");
                }

                var talbeName = instance.GetTableName();
                var preTalbeName = _service.GetPreTableName(instance);
                ProcessState stateOfPeriod = _service.CheckProcess(instance);
                switch (stateOfPeriod)
                {
                    case ProcessState.IsPending:
                        try
                        {
                            _close.ProcedureCloseInventory(preTalbeName, talbeName, instance.UnitCode, instance.Year, instance.Period);

                            exist.TrangThai = (int)ApprovalState.IsComplete;
                            exist.ObjectState = BTS.API.ENTITY.ObjectState.Modified;
                            result.Status = true;
                            result.Message = "Khóa sổ thành công";
                            _service.UnitOfWork.Save();
                        }
                        catch (Exception e)
                        {
                            return InternalServerError(e);
                        }
                        break;
                    case ProcessState.IsComplete:
                        break;
                    case ProcessState.IsRunning:
                        return BadRequest("Đang trong quá trình khóa");
                    case ProcessState.IsError:
                        break;
                    default:
                        break;
                }

            }
            return Ok(result);
        }
        [Route("PostCreateNewPeriod")]
        public async Task<IHttpActionResult> PostCreateNewPeriod(MdPeriodVm.RequestCreatePeriod request)
        {
            var result = new TransferObj();
            try
            {
                var isSuccess = _service.CreateNewPeriodByDay(request.Year);
                if (isSuccess)
                {
                    await _service.UnitOfWork.SaveAsync();
                    result.Status = true;
                    result.Message = "Tạo mới kỳ kế toán năm "+ request.Year +" thành công";
                    return Ok(result);
                }
                return BadRequest("Kỳ trong năm nay đã được tạo!");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
        /// <summary>
        /// UpdatePrice
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("UpdatePrice")]
        public IHttpActionResult UpdatePrice(MdPeriodVm.Dto param)
        {
            var result = new TransferObj();
            try
            {
                bool data = _service.UpdatePrice(_service.GetCurrentUnitCode(), param.FromDate, param.ToDate);
                if (data)
                {
                    result.Status = true;
                    result.Message = "Success";
                    result.Data = data;
                }
                else
                {
                    result.Status = false;
                    result.Message = "Failed";
                    result.Data = null;
                }
                
            }
            catch
            {
                result.Status = false;
                result.Message = "Failed";
                result.Data = null;
            }
            return Ok(result);
        }

        [Route("GetSoLuongTonByDate")]
        [HttpPost]
        public IHttpActionResult GetSoLuongTonByDate(ParameterInventory param)
        {
            if (param.FromDate != null || param.ToDate != null)
            {
                var result = new TransferObj();
                DateTime day = new DateTime(param.FromDate.Year, param.FromDate.Month, param.FromDate.Day, 0, 0, 0);
                var data = _service.Repository.DbSet.FirstOrDefault(x => x.FromDate == day);
                if (data != null)
                {
                    MdPeriodVm.ResponseData dataResult = new MdPeriodVm.ResponseData();
                    string tableName = ProcedureCollection.GetTableName(data.Year, data.Period);
                    var getTonKhoXuat =  string.Format("SELECT TONCUOIKYSL FROM {0} where MAVATTU = '{1}' and MAKHO = '{2}'", tableName , param.MerchandiseCodes.ToUpper() , param.WareHouseCodes);
                    var getTonKhoNhap = string.Format("SELECT TONCUOIKYSL FROM {0} where MAVATTU = '{1}' and MAKHO = '{2}'", tableName, param.MerchandiseCodes.ToUpper(), param.WareHouseRecieveCode);
                    using (var ctx = new ERPContext())
                    {
                        decimal soLuongTonKhoXuat = ctx.Database.SqlQuery<decimal> (getTonKhoXuat).SingleOrDefault();
                        decimal soLuongTonKhoNhap = ctx.Database.SqlQuery<decimal>(getTonKhoNhap).SingleOrDefault();
                        dataResult.SoLuongTonKhoNhap = soLuongTonKhoNhap;
                        dataResult.SoLuongTonKhoXuat = soLuongTonKhoXuat;
                        result.Data = dataResult;
                    }
                    result.Status = true;
                    result.Message = "Ok";
                    return Ok(result);
                }
                else
                {
                    result.Message = "Chưa khởi tạo kỳ kế toán";
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
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
