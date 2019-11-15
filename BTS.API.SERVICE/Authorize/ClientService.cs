using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Authorize
{
    public interface IClientService : IDataInfoService<Client>
    {
    }
    public class ClientService : DataInfoServiceBase<Client>, IClientService
    {
        public ClientService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
