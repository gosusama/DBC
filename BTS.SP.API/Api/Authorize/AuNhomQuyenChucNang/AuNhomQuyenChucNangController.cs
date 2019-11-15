using System;
using System.Collections.Generic;
using System.Web.Http;
using BTS.API.SERVICE.Helper;
using BTS.API.ENTITY.Authorize;
using BTS.API.ENTITY;
using BTS.API.SERVICE.Authorize.AuNhomQuyenChucNang;
namespace BTS.SP.API.Api.Authorize.AuNhomQuyenChucNang
{
    [RoutePrefix("api/Authorize/AuNhomQuyenChucNang")]
    [Route("{id?}")]
    [Authorize]
    public class AuNhomQuyenChucNangController : ApiController
    {
        private readonly IAuNhomQuyenChucNangService _service;

        public AuNhomQuyenChucNangController(IAuNhomQuyenChucNangService service)
        {
            _service = service;
        }
        [Route("GetByMaNhomQuyen/{manhomquyen}")]
        public IHttpActionResult GetByMaNhomQuyen(string manhomquyen)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(manhomquyen)) return BadRequest();
            var result = new TransferObj<List<AuNhomQuyenChucNangVm.ViewModel>>();
            try
            {
                var data = _service.GetByMaNhomQuyen(_unitCode, manhomquyen);
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

        [Route("Config")]
        [HttpPost]
        public IHttpActionResult Config(AuNhomQuyenChucNangVm.ConfigModel model)
        {
            var _unitCode = _service.GetCurrentUnitCode();
            if (string.IsNullOrEmpty(model.MANHOMQUYEN)) return BadRequest();
            var result=new TransferObj();
            try
            {
                if (model.LstDelete != null && model.LstDelete.Count > 0)
                {
                    foreach (var item in model.LstDelete)
                    {
                        _service.Delete(item.Id);
                    }
                }
                if (model.LstAdd != null && model.LstAdd.Count > 0)
                {
                    foreach (var item in model.LstAdd)
                    {
                        AU_NHOMQUYEN_CHUCNANG obj = new AU_NHOMQUYEN_CHUCNANG()
                        {
                            ObjectState = ObjectState.Added,
                            MANHOMQUYEN = item.MANHOMQUYEN,
                            MACHUCNANG = item.MACHUCNANG,
                            XOA = item.XOA,
                            DUYET = item.DUYET,
                            SUA = item.SUA,
                            THEM = item.THEM,
                            TRANGTHAI = 10,
                            XEM = item.XEM,
                            GIAMUA = item.GIAMUA,
                            GIABAN = item.GIABAN,
                            GIAVON = item.GIAVON,
                            TYLELAI = item.TYLELAI,
                            BANCHIETKHAU = item.BANCHIETKHAU,
                            BANBUON = item.BANBUON,
                            BANTRALAI = item.BANTRALAI,
                            UnitCode = _unitCode
                            
                        };
                        _service.Insert(obj, false);
                    }
                }
                if (model.LstEdit != null && model.LstEdit.Count > 0)
                {
                    foreach (var item in model.LstEdit)
                    {
                        AU_NHOMQUYEN_CHUCNANG obj = new AU_NHOMQUYEN_CHUCNANG()
                        {
                            Id = item.Id,
                            MACHUCNANG = item.MACHUCNANG,
                            MANHOMQUYEN = item.MANHOMQUYEN,
                            UnitCode = _unitCode,
                            XEM = item.XEM,
                            THEM = item.THEM,
                            SUA = item.SUA,
                            XOA = item.XOA,
                            DUYET = item.DUYET,
                            GIAMUA = item.GIAMUA,
                            GIABAN = item.GIABAN,
                            GIAVON = item.GIAVON,
                            TYLELAI = item.TYLELAI,
                            BANCHIETKHAU = item.BANCHIETKHAU,
                            BANBUON = item.BANBUON,
                            BANTRALAI = item.BANTRALAI,
                            TRANGTHAI = 10,
                            ObjectState = ObjectState.Modified
                        };
                        _service.Update(obj);
                    }
                }
                _service.UnitOfWork.Save();
                result.Status = true;
                result.Message = "Cập nhật thành công.";
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = ex.Message;
            }
            return Ok(result);
        }
    }
}
