using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdHangKhService : IDataInfoService<MdHangKH>
    {
        //Add function here
    }
    public class MdHangKhService : DataInfoServiceBase<MdHangKH>, IMdHangKhService
    {
        public MdHangKhService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdHangKH, bool>> GetKeyFilter(MdHangKH instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.MaHangKh == instance.MaHangKh && x.UnitCode.StartsWith(_parent);
        }
    }
}
