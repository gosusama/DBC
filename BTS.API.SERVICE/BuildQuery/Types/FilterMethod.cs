using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.Types
{
    [Serializable]
    [DataContract]
    public enum FilterMethod
    {
        [DataMember] All,

        [DataMember] EqualTo,
        [DataMember] NotEqualTo,
        [DataMember] LessThan,
        [DataMember] NotLessThan,
        [DataMember] GreaterThan,
        [DataMember] NotGreaterThan,
        [DataMember] LessThanOrEqualTo,
        [DataMember] GreaterThanOrEqualTo,


        [DataMember] StartsWith,
        [DataMember] Like,
        [DataMember] NotLike,
        [DataMember] In,
        [DataMember] NotIn,


        [DataMember] Null,
        [DataMember] NotNull,


        [DataMember] Not,


        [DataMember] And,
        [DataMember] Or
    }
}