using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdDistrictsService: IDataInfoService<MdDistricts>
    {
    }
    public class MdDistrictsService : DataInfoServiceBase<MdDistricts>, IMdDistrictsService
    {
        public MdDistrictsService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdDistricts, bool>> GetKeyFilter(MdDistricts instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.CityId == instance.CityId && x.UnitCode.StartsWith(_parent);
        }
    }
}
