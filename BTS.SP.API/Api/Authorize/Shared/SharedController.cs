using BTS.API.SERVICE.Authorize;
using BTS.API.SERVICE.Helper;
using System.Web.Http;
namespace BTS.SP.API.Authorize.Shared
{
    [RoutePrefix("api/Authorize/Shared")]
    [Route("{id?}")]
    [Authorize]
    public class SharedController : ApiController
    {
        private readonly ISharedService _service;

        public SharedController(ISharedService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("GetAccesslist/{machucnang}")]
        public RoleState GetAccesslist(string machucnang)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            RoleState roleState = _service.GetRoleStateByMaChucNang(_unitCode,RequestContext.Principal.Identity.Name, machucnang);
            return roleState;
        }
    }
}
