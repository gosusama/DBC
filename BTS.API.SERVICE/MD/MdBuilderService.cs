using BTS.API.ENTITY.Md;
using BTS.API.SERVICE.Services;
namespace BTS.API.SERVICE.MD
{
    public interface IMdIdBuilderService : IEntityService<MdIdBuilder>
    {
        //Add function here
    }
    public class MdIdBuilderService : EntityServiceBase<MdIdBuilder>, IMdIdBuilderService
    {
        public MdIdBuilderService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


    }
}
