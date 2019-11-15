using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/Districts")]
    [Route("{id?}")]
    [Authorize]
    public class DistrictsController : ApiController
    {
        protected readonly IMdDistrictsService _service;
        public DistrictsController(IMdDistrictsService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            return data.Select(x => new ChoiceObj { Value = x.DistrictsId, Text = x.DistrictsName, Id = x.Id,Description = x.DistrictsName, Parent = x.CityId }).ToList();
        }
    }
}
