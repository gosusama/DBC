using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BTS.API.SERVICE.BuildQuery
{
    [DataContract]
    [Serializable]
    public static class LogState
    {
        [DataMember] public static string Default = "";

        [DataMember] public static string Message = "message";

        [DataMember] public static string Log = "log";
        [DataMember] public static string Action = "action";

        [DataMember] public static string Report = "report";
        [DataMember] public static string Test = "test";

        [DataMember] public static string Success = "success";
        [DataMember] public static string NotSuccess = "notSuccess";

        [DataMember] public static string Error = "error";
        [DataMember] public static string Exception = "exception";

        [DataMember] public static string Alert = "alert";
        [DataMember] public static string Warning = "warning";

        [DataMember] public static string Importance = "importance";
    }
}