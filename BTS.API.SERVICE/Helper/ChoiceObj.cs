using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace BTS.API.SERVICE.Helper
{
    [DataContract]
    public class ChoiceObj : StateInfoObj
    {
        [DataMember]
        public string Parent { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool OldSelected { get; set; }
        [DataMember]
        public string ReferenceDataId { get; set; }
    }
}