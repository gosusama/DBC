using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Helper;
using BTS.API.SERVICE.MD;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using BTS.API.SERVICE;
using BTS.API.SERVICE.NV;
namespace BTS.SP.API.Api.NV
{
    [RoutePrefix("api/Nv/TraTon")]
    [Route("{id?}")]
    [Authorize]
    public class TraTonController : ApiController
    {
        private readonly IMdPeriodService _servicePeriod;
        private readonly INvRetailsService _serviceRetails;
        public TraTonController(INvRetailsService serviceRetails,IMdPeriodService service)
        {
            _servicePeriod = service;
            _serviceRetails = serviceRetails;
        }
        [Route("GetPeriodDate")]
        [HttpGet]
        public MdPeriod GetPeriodDate()
        {
            var unitCode = _servicePeriod.GetCurrentUnitCode();
            var period = new MdPeriod();
            period = CurrentSetting.GetKhoaSo(unitCode);
            if (period != null)
            {
                return period;
            }
            else
            {
                return null;
            }
        }

        [Route("BindingDataHangHoa/{strKey}")]
        [HttpGet]
        public async Task<IHttpActionResult> BindingDataHangHoa(string strKey)
        {
            var result = new TransferObj();
            var unitCode = _serviceRetails.GetCurrentUnitCode();
            try
            {
                var serviceProcedure = new ProcedureService<MdMerchandiseVm.FilterData>();
                var data = ProcedureCollection.QueryFilterMerchandise(unitCode, strKey);
                if (data.Count > 0)
                {
                    result.Data = data;
                    result.Status = true;
                    result.Message = "Truy vấn thành công";
                }
                else
                {
                    result.Data = null;
                    result.Status = false;
                    result.Message = "Không tìm thấy";
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Status = false;
                result.Message = "Không tìm thấy";
            }
            return Ok(result);
        }

        [Route("CheckInventory/{code}")]
        [HttpGet]
        public async Task<IHttpActionResult> CheckInventory(string code)
        {
            List<string[]> lstData = new List<string[]>();
            var result = new TransferObj<List<string[]>>();
            try
            {
                lstData = _serviceRetails.GetInventory(code);
                if (lstData.Count > 0)
                {
                    result.Status = true;
                    result.Data = lstData;
                }
                else
                {
                    result.Status = false;
                    result.Data = null;
                }
            }
            catch (Exception)
            {
                result.Status = false;
                result.Data = null;
            }
            return Ok(result);
        }
    }
}