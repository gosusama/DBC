using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize.AuMenu;
using BTS.API.SERVICE.BuildQuery;
using BTS.API.SERVICE.Helper;
using Newtonsoft.Json.Linq;
using BTS.API.SERVICE.Authorize.Utils;
using BTS.SP.API.Utils;

namespace BTS.SP.API.Api.Authorize.AuMenu
{
    [RoutePrefix("api/Authorize/AuMenu")]
    [Route("{id?}")]
    [Authorize]
    public class AuMenuController : ApiController
    {
        protected readonly IAuMenuService _service;
        public AuMenuController(IAuMenuService service)
        {
            _service = service;
        }
        [Route("GetByUnitCode")]
        [HttpGet]
        public IList<ChoiceObj> GetByUnitCode()
        {
            var unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(unitCode)) return new List<ChoiceObj>();
            try
            {

                var data = _service.Repository.DbSet.Where(x => x.MenuId.Equals(unitCode) && x.TrangThai == 10)
                    .OrderBy(x => x.Sort).Select(x => new ChoiceObj()
                    {
                        Id = x.Id,
                        Value = x.MenuId,
                        Text = x.Title,
                        Parent = x.MenuIdCha
                    });
                return data.ToList();
            }
            catch (Exception ex)
            {
                return new List<ChoiceObj>();
            }
        }
        [HttpGet]
        [Route("GetAllForConfigNhomQuyen/{manhomquyen}")]
        public IHttpActionResult GetAllForConfig(string manhomquyen)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(manhomquyen)) return BadRequest();
            var result = new TransferObj<List<AU_MENU>>();
            try
            {
                var data = _service.GetAllForConfigNhomQuyen(manhomquyen, _unitCode);
                result.Status = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("GetAllForConfigQuyen/{username}")]
        public IHttpActionResult GetAllForConfigQuyen(string username)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(username)) return BadRequest();
            var result = new TransferObj<List<AU_MENU>>();
            try
            {
                var data = _service.GetAllForConfigQuyen(username, _unitCode);
                result.Status = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
        [Route("GetSelectData")]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            //var maDonViCha = _service.GetParentUnitCode();
            return data.Select(x => new ChoiceObj { Value = x.MenuId, Text = x.Title, Id = x.Id ,Parent = x.MenuIdCha}).ToList();
        }
        [Route("PostQuery")]
        [CustomAuthorize(Method = "XEM", State = "auMenu")]
        public async Task<IHttpActionResult> PostQuery(JObject jsonData)
        {
            var result = new TransferObj();
            var postData = ((dynamic)jsonData);
            var filtered = ((JObject)postData.filtered).ToObject<FilterObj<AuMenuVm.Search>>();
            var paged = ((JObject)postData.paged).ToObject<PagedObj<AU_MENU>>();
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
                result.Data = filterResult.Value;
                result.Status = true;
                return Ok(result);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
        [HttpPost]
        [ResponseType(typeof(AU_MENU))]
        [CustomAuthorize(Method = "THEM", State = "auMenu")]
        public async Task<IHttpActionResult> Post(AU_MENU instance)
        {
            var result = new TransferObj<AU_MENU>();
            try
            {
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
            return CreatedAtRoute("DefaultApi", new { controller = this, id = instance.Id }, result);
        }
        [HttpPut]
        [ResponseType(typeof(void))]
        [CustomAuthorize(Method = "SUA", State = "auMenu")]
        public async Task<IHttpActionResult> Put(string id, AU_MENU instance)
        {
            var result = new TransferObj<AU_MENU>();
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
        [ResponseType(typeof(AU_MENU))]
        [CustomAuthorize(Method = "XOA", State = "auMenu")]
        public async Task<IHttpActionResult> Delete(string id)
        {
            AU_MENU instance = await _service.Repository.FindAsync(id);
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
        [Route("Get")]
        public async Task<IHttpActionResult> Get()
        { 
            var result = new TransferObj<List<AuMenuVm.Menu>>();
            List<AuMenuVm.Menu> listMenu = new List<AuMenuVm.Menu>();
            var data = _service.Repository.DbSet;
            if (data != null)
            {
                List<AU_MENU> listData = data.ToList();
                if (listData.Count > 0)
                {
                    foreach (var value in listData)
                    {
                        AuMenuVm.Menu menu = new AuMenuVm.Menu();
                        menu.Id = value.MenuId;
                        menu.Title = value.Title;
                        menu.Parent = value.MenuIdCha;
                        listMenu.Add(menu);
                    }
                }
                result.Status = true;
                result.Data = listMenu;
            }
            else
            {
                result.Status = false;
                result.Data = new List<AuMenuVm.Menu>();
                result.Message = "Không tìm thấy !";
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("GetMenu/{username}")]
        public IHttpActionResult GetMenu(string username)
        {
            var result = new TransferObj<List<ChoiceObj>>();
            try
            {
                var _unitCode = _service.GetCurrentUnitCode();
                List<AU_MENU> lstMenu = new List<AU_MENU>();
                if (username.Equals("admin"))
                {
                    lstMenu = _service.Repository.DbSet.Where(x => x.TrangThai == 10 && x.UnitCode == _unitCode).OrderBy(x => x.Sort).ToList();
                }
                else
                {
                    lstMenu = _service.GetAllForStarting(username, _unitCode);
                }
                result.Data = new List<ChoiceObj>();
                if (lstMenu != null)
                {
                    lstMenu.ForEach(x =>
                    {
                        ChoiceObj obj = new ChoiceObj()
                        {
                            Id = x.Id,
                            Text = x.Title,
                            Parent = x.MenuIdCha,
                            Value = x.MenuId,
                        };
                        result.Data.Add(obj);
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLogs.LogError(ex);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByMenuId/{menuId}")]
        [ResponseType(typeof(AU_MENU))]
        public AU_MENU GetByMenuId(string menuId)
        {
            var menu = new AU_MENU();
            menu = string.IsNullOrEmpty(menuId) ? null : menu = _service.Repository.DbSet.FirstOrDefault(x => x.MenuId == menuId);
            return menu;
        }
    }
}
