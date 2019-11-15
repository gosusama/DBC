using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
using System;
using System.Linq.Expressions;
namespace BTS.API.SERVICE.MD
{
    public interface IMdDepartmentService : IDataInfoService<MdDepartment>
    {
        //Add function here
        
    }
    public class MdDepartmentService:  DataInfoServiceBase<MdDepartment>, IMdDepartmentService
    {
        public MdDepartmentService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
        protected override Expression<Func<MdDepartment, bool>> GetKeyFilter(MdDepartment instance)
        {
            var _parent = GetParentUnitCode();
            return x => x.MaPhong == instance.MaPhong && x.UnitCode.StartsWith(_parent);
        }
        
    }
}
