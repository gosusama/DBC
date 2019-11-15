using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BTS.API.SERVICE.BuildQuery
{
    public class ResultObj : ResultObj<object>
    {
    }

    [Serializable]
    [DataContract]
    public class ResultObj<T>
    {
        public ResultObj()
        {
            Logs = new List<LogObj>();
            State = ResultState.NotSuccess;
            WasSuccessful = false;
        }

        [DataMember]
        public bool WasSuccessful { get; set; }

        [DataMember]
        public ResultState State
        {
            get { return _state; }
            set
            {
                _state = value;
                switch (_state)
                {
                    case ResultState.NotSuccess:
                        WasSuccessful = false;
                        break;
                    case ResultState.Error:
                        WasSuccessful = false;
                        break;
                    case ResultState.Exception:
                        WasSuccessful = false;
                        break;
                    case ResultState.Success:
                        WasSuccessful = true;
                        break;
                    case ResultState.SuccessButSomeInnerErrors:
                        WasSuccessful = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private ResultState _state;

        [DataMember]
        public T Value { get; set; }

        public string Message { get; set; }

        public string ResultType
        {
            get
            {
                string result = typeof (T).FullName;
                return result;
            }
        }

        public LogObj SetLog
        {
            set
            {
                if (value != null)
                {
                    Logs.Add(value);
                }
            }
        }

        [DataMember]
        public List<LogObj> Logs { get; set; }

        public MessageObj SetMessage
        {
            set
            {
                if (value != null)
                {
                    SetLog = new LogObj {Message = value};
                }
            }
        }

        public Exception SetException
        {
            set
            {
                if (value != null)
                {
                    SetLog = new LogObj {Exception = value};
                    State = ResultState.Exception;
                }
            }
        }


        public bool MergeResult<TExt>(ResultObj<TExt> extResult)
        {
            bool result = false;
            if (MergeResultDetails(extResult))
            {
                State = extResult.State;
                WasSuccessful = extResult.WasSuccessful;
                result = true;
            }
            return result;
        }

        public bool MergeResultDetails<TExt>(ResultObj<TExt> extResult)
        {
            bool result = false;
            if (extResult != null)
            {
                foreach (LogObj item in extResult.Logs)
                {
                    SetLog = item;
                }
                result = true;
            }
            return result;
        }

        public bool MergeResultIfFail<TExt>(ResultObj<TExt> extResult)
        {
            bool result = false;
            if (extResult != null && !extResult.WasSuccessful)
            {
                foreach (LogObj item in extResult.Logs)
                {
                    SetLog = item;
                }
                State = ResultState.SuccessButSomeInnerErrors;
                result = true;
            }
            return result;
        }

        public ResultObj<TExt> ToResult<TExt>()
        {
            var result = new ResultObj<TExt>();
            result.MergeResultDetails(this);
            result.MergeResult(this);
            return result;
        }
    }

    [Serializable]
    [DataContract]
    public class ResultObjTranf<T>
    {
        [DataMember]
        public bool WasSuccessful { get; set; }

        [DataMember]
        public T Value { get; set; }

        public string Message { get; set; }

        public string ResultType
        {
            get;
            set;
        }
    }

}