using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuNguoiDung;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using Newtonsoft.Json.Linq;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.API.SERVICE.BuildQuery.Query.Types;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace BTS.SP.API.Api.Authorize.AuNguoiDung
{
    [RoutePrefix("api/Authorize/AuNguoiDung")]
    [Route("{id?}")]
    [Authorize]
    public class AuNguoiDungController : ApiController
    {
        private IAuNguoiDungService _service;
        public AuNguoiDungController(IAuNguoiDungService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            var unitCode = _service.GetCurrentUnitCode();
            return data.Where(x => x.UnitCode == unitCode).Select(x => new ChoiceObj() { Value = x.MaNhanVien, Text = x.TenNhanVien, Id = x.Id }).ToList();
        }
        [Route("GetUserByProfile/{parameter}")]
        public async Task<IHttpActionResult> GetUserByProfile(string parameter)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj<AU_NGUOIDUNG>();
            if (parameter != null && parameter != "")
            {
                var user = _service.Repository.DbSet.FirstOrDefault(x => x.MaNhanVien == parameter && x.UnitCode.StartsWith(unitCode));
                if (user != null)
                {
                    result.Status = true;
                    result.Data = user;
                    result.Message = "Ok";
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                    result.Message = "NotFound";
                }
            }
            return Ok(result);
        }
        [Route("GetUserByUsername/{parameter}")]
        public async Task<IHttpActionResult> GetUserByUsername(string parameter)
        {
            var unitCode = _service.GetCurrentUnitCode();
            var result = new TransferObj<AU_NGUOIDUNG>();
            if (parameter != null && parameter != "")
            {
                var user = _service.Repository.DbSet.FirstOrDefault(x => x.Username == parameter && x.UnitCode.StartsWith(unitCode));
                if (user != null)
                {
                    result.Status = true;
                    result.Data = user;
                    result.Message = "Ok";
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                    result.Message = "NotFound";
                }
            }
            return Ok(result);
        }
        [Route("Register")]
        public async Task<IHttpActionResult> Register(AuNguoiDungVm.ModelRegister userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _service.CreateNewUser(userModel);
            return Ok();
        }
        [Route("GetCurrentUser")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var unitCode = _service.GetCurrentUnitCode();
            var currentUser = _service.Repository.DbSet.FirstOrDefault(x => x.Username == userName);
            var result = new AuNguoiDungVm.CurrentUser()
            {
                UserName = userName,
                MaNhanVien = currentUser.MaNhanVien,
                TenNhanVien = currentUser.TenNhanVien,
                SoDienThoai = currentUser.SoDienThoai,
                ChungMinhThu = currentUser.ChungMinhThu,
                GioiTinh = currentUser.GioiTinh.ToString(),
                ChucVu = currentUser.ChucVu,
                UnitUser = unitCode,
            };
            return Ok(result);
        }
        [Route("PostQuery")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuNguoiDungVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<AU_NGUOIDUNG>>();
            var unitCode = _service.GetCurrentUnitCode();
            var maDonViCha = _service.GetParentUnitCode();
            var query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new AU_NGUOIDUNG().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
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
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [Route("GetNewInstance/{unitCode}")]
        public string GetNewInstance(string unitCode)
        {
            return _service.BuildCodeByParent(unitCode);
        }
        [Route("CheckUserNameExist/{userName}")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckUserNameExist(string userName)
        {
            var result = new TransferObj();
            var exist = _service.Repository.DbSet.Where(x => x.Username == userName).ToList();
            if (exist.Count > 0)
            {
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }
            return Ok(result);
        }
        [Route("PostSelectData")]
        public async Task<IHttpActionResult> PostSelectData(JObject jsonData)
        {
            TransferObj result = new TransferObj();
            dynamic postData = ((dynamic)jsonData);
            FilterObj<AuNguoiDungVm.Search> filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuNguoiDungVm.Search>>();
            PagedObj<AU_NGUOIDUNG> paged = ((JObject)postData.paged).ToObject<PagedObj<AU_NGUOIDUNG>>();
            string unitCode = _service.GetCurrentUnitCode();
            string maDonViCha = _service.GetParentUnitCode();
            QueryBuilder query = new QueryBuilder
            {
                Take = paged.ItemsPerPage,
                Skip = paged.FromItem - 1,
                Filter = new QueryFilterLinQ()
                {
                    Property = ClassHelper.GetProperty(() => new AU_NGUOIDUNG().UnitCode),
                    Method = FilterMethod.StartsWith,
                    Value = maDonViCha
                }
            };
            try
            {
                ResultObj<PagedObj<AU_NGUOIDUNG>> filterResult = _service.Filter(filtered, query);
                if (!filterResult.WasSuccessful)
                {
                    return NotFound();
                }

                result.Data = Mapper.Map<PagedObj<AU_NGUOIDUNG>, PagedObj<ChoiceObj>>
                    (filterResult.Value);
                result.Status = true;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
        [HttpPost]
        [ResponseType(typeof(AU_NGUOIDUNG))]
        [CustomAuthorize(Method = "THEM", State = "sys_User")]
        public async Task<IHttpActionResult> Post(AU_NGUOIDUNG instance)
        {
            var result = new TransferObj<AU_NGUOIDUNG>();
            var unitCode = _service.GetCurrentUnitCode();
            var parentUnitCode = _service.GetParentUnitCode();
            var exist = _service.Repository.DbSet.FirstOrDefault(x=>x.Username == instance.Username);
            if (exist != null)
            {
                result.Status = false;
                return Ok(result);
            }
            else
            {
              
                try
                {
                    instance.Password = MD5Encrypt.MD5Hash(instance.Password);
                    instance.MaNhanVien = _service.SaveCodeByParent(parentUnitCode);
                    instance.ParentUnitcode = parentUnitCode;
                    
                    var item = _service.Insert(instance);
                    _service.UnitOfWork.Save();
                    result.Status = true;
                    result.Data = item;
                }
                catch (Exception e)
                {
                    result.Status = false;
                    result.Message = e.Message;
                    return Ok(result);
                }
            }
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }
        [HttpPut]
        [ResponseType(typeof(void))]
        //[CustomAuthorize(Method = "SUA", State = "sys_User")]
        public async Task<IHttpActionResult> Put(string id, AU_NGUOIDUNG instance)
        {
            var result = new TransferObj<AU_NGUOIDUNG>();
            if (id != instance.Id)
            {
                result.Status = false;
                result.Message = "Id không hợp lệ";
                return Ok(result);
            }

            try
            {
                var item = _service.Update(instance);
                _service.UnitOfWork.Save();
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
        [HttpDelete]
        [ResponseType(typeof(AU_NGUOIDUNG))]
        [CustomAuthorize(Method = "XOA", State = "sys_User")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            AU_NGUOIDUNG instance = await _service.Repository.FindAsync(id);
            if (instance == null)
            {
                return NotFound();
            }
            try
            {
                if (_service.DeleteUser(id))
                {
                    _service.Delete(instance.Id);
                    _service.UnitOfWork.Save();
                    return Ok(instance);
                }
                return InternalServerError();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IList<AU_NGUOIDUNG> Get()
        {
            var data = _service.Repository.DbSet;
            return data.ToList();
        }
        [HttpGet]
        [ResponseType(typeof(AU_NGUOIDUNG))]
        [CustomAuthorize(Method = "XEM", State = "sys_User")]
        public IHttpActionResult Get(string id)
        {
            var instance = _service.FindById(id);
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        [Route("GetTargetUser/{id}")]
        [ResponseType(typeof(AU_NGUOIDUNG))]
        public IHttpActionResult GetTargetUser(string id)
        {
            var instance = _service.Repository.DbSet.Where(x=>x.Id == id).Select(x => new { x.MaNhanVien , x.SoDienThoai ,x.TenNhanVien }).FirstOrDefault();
            if (instance == null)
            {
                return NotFound();
            }
            return Ok(instance);
        }
        //----------
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
