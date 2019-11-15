using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace BTS.API.SERVICE.BuildQuery
{
    [Serializable]
    [DataContract]
    public class LogObj
    {
        public LogObj()
        {
            Message = new MessageObj();
            LogTime = DateTime.Now;
            State = LogState.Default;

            var stackTrace = new StackTrace(true);
            CallStack = stackTrace.ToString();

            var stackFrame = stackTrace.GetFrame(1);
            var currentMethod = stackFrame.GetMethod();
            Location = String.Format("{0}.{1}", currentMethod.ReflectedType.FullName, currentMethod.Name);
        }

        private DateTime? _logTime;

        [DataMember]
        public DateTime LogTime
        {
            get
            {
                if (_logTime == null)
                {
                    _logTime = DateTime.Now;
                }
                return _logTime.Value;
            }
            set { _logTime = value; }
        }

        private string _state;

        [DataMember]
        public string State
        {
            get { return _state; }
            set
            {
                _state = value;
                if (OnStateChange != null)
                    OnStateChange(this);
            }
        }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string CallStack { get; set; }

        [XmlIgnore, IgnoreDataMember]
        public dynamic ExtData { get; set; }

        [DataMember]
        public MessageObj Message { get; set; }

        private Exception _exception;

        [XmlIgnore, IgnoreDataMember]
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                _exception = value;
                if (_exception != null && State == LogState.Default)
                {
                    State = LogState.Exception;
                }
            }
        }

        private string exceptionDetails
        {
            get
            {
                var result = "";
                if (Exception != null)
                {
                    result = String.Format("{0}, {1}, {2}, {3}, {4}",
                                           Exception.Message,
                                           Exception.Source,
                                           Exception.Data,
                                           Exception.InnerException,
                                           Exception.StackTrace);
                }
                return result;
            }
        }

        public string ShortDetails
        {
            get
            {
                var result = String.Format("【{0}】 {1}-{2}",
                                           State,
                                           ((Message == null || Message.Code == "") ? "" : Message.LongDetails),
                                           Exception == null ? "" : Exception.Message);
                return result;
            }
        }

        public string LongDetails
        {
            get
            {
                var result = String.Format("【{0}】【{1}】{2} {3}-{4}",
                                           State,
                                           LogTime.ToString("yyyy/MM/dd hh:mm:ss"),
                                           ((Message == null || Message.Code == "") ? "" : Message.LongDetails),
                                           Location,
                                           Exception == null ? "" : Exception.Message);
                return result;
            }
        }

        #region Overrides of Object

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            var result = String.Format(@"【{0}】【{1}】{2} {3}-{4}-{5}-{6}-{7}",
                                       State,
                                       LogTime.ToString("yyyy/MM/dd hh:mm:ss"),
                                       String.IsNullOrEmpty(User) ? "" : String.Format("【{0}】", User),
                                       Message,
                                       Location,
                                       CallStack,
                                       ExtData == null ? "" : String.Format("【{0}】", ExtData),
                                       exceptionDetails);
            return result;
        }

        #endregion

        #region Events

        public delegate void LogDelegate(LogObj _log);

        public static event LogDelegate OnStateChange;

        #endregion
    }
}