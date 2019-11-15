using BTS.API.SERVICE.Authorize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.API.SERVICE.Authorize.AuNguoiDung;

namespace BTS.SP.API.Api.Authorize
{
    [RoutePrefix("api/RefreshTokens")]
    [Authorize]
    public class RefreshTokensController : ApiController
    {

        private IAuNguoiDungService _service;

        public RefreshTokensController(IAuNguoiDungService service)
        {
            _service = service;
        }

        [Authorize(Users = "Admin")]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_service.GetAllRefreshTokens());
        }

        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _service.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");

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
