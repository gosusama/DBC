using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Services;

namespace BTS.API.SERVICE.Authorize.AuNguoiDungNhomQuyen
{
    public interface IAuNguoiDungNhomQuyenService : IDataInfoService<AU_NGUOIDUNG_NHOMQUYEN>
    {
    }
    public class AuNguoiDungNhomQuyenService : DataInfoServiceBase<AU_NGUOIDUNG_NHOMQUYEN>, IAuNguoiDungNhomQuyenService
    {
        public AuNguoiDungNhomQuyenService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

    }
}
