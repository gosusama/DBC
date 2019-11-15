namespace BTS.API.SERVICE
{
    public abstract class ServiceBase : IService
    {
        private IUnitOfWork _unitOfWork;

        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}