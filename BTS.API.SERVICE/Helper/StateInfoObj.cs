using System.Runtime.Serialization;
namespace BTS.API.SERVICE.Helper
{
    [DataContract]
    public class StateInfoObj
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string ExtendValue { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public bool Selected { get; set; }
        [DataMember]
        public string Infomation { get; set; }
    }
}