using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace BTS.SP.API.Api.MD
{
    [RoutePrefix("api/Md/City")]
    [Route("{id?}")]
    [Authorize]
    public class CityController : ApiController
    {
        protected readonly IMdCityService _service;
        public CityController(IMdCityService service)
        {
            _service = service;
        }
        [Route("GetSelectData")]
        [AllowAnonymous]
        public IList<ChoiceObj> GetSelectData()
        {
            var data = _service.Repository.DbSet;
            return data.Select(x => new ChoiceObj { Value = x.CityId, Text = x.CityName, Id = x.Id,Description = x.CityName}).ToList();
        }
    }
}
