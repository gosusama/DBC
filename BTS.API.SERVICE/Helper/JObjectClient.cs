using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.Helper
{
    [DataContract]
    public class JObjectClient<T>
    {
        [DataMember]
        public Paged paged { get; set; }
        [DataMember]
        public Filtered<T> filtered { get; set; }
    }

    [DataContract]
    public class Paged
    {
        [DataMember]
        public int TotalItems { get; set; }
        [DataMember]
        public int ItemsPerPage { get; set; }
        [DataMember]
        public int CurrentPage { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int TotalPage { get; set; }

    }
    [DataContract]

    public class Filtered<T>
    {
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public bool IsAdvance { get; set; }
        [DataMember]
        public T AdvanceData { get; set; }
        [DataMember]
        public string OrderBy { get; set; }
        [DataMember]
        public string OrderType { get; set; }
    }
}
