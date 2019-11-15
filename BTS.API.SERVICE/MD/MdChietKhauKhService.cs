using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;

namespace BTS.API.SERVICE.MD
{
    public interface IMdChietKhauKhService : IDataInfoService<MdChietKhauKH>
    {
        //Add function here
    }
    public class MdChietKhauKhService : DataInfoServiceBase<MdChietKhauKH>, IMdChietKhauKhService
    {
        public MdChietKhauKhService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdChietKhauKH, bool>> GetKeyFilter(MdChietKhauKH instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.MaChietKhau == instance.MaChietKhau && x.UnitCode.StartsWith(_parent);
        }
    }
}
