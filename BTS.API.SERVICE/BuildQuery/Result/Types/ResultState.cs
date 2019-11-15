using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery
{
    [Serializable]
    [DataContract]
    public enum ResultState
    {
        [DataMember] NotSuccess = 0,
        [DataMember] Error = 1,
        [DataMember] Exception = 2,
        [DataMember] Success = 3,
        [DataMember] SuccessButSomeInnerErrors = 4,
    }
}