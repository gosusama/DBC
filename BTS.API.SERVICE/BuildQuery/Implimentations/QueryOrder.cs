using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BTS.API.SERVICE.BuildQuery.Query.Types;

namespace BTS.API.SERVICE.BuildQuery
{
    [Serializable]
    [DataContract]
    public class QueryOrder : IQueryOrder
    {
        public QueryOrder()
        {
            IsUseProperty = false;
            Method = OrderMethod.ASC;
        }

        private string _field;

        [DataMember]
        public string Field
        {
            get
            {
                var result = _field;
                if (IsUseProperty)
                {
                    result = Property.Name;
                }
                return result;
            }
            set
            {
                _field = value;
                if (!string.IsNullOrEmpty(_field))
                {
                    var itemp = _field.IndexOf(" ");
                    if (itemp > 0)
                    {
                        var prefix = _field.Substring(0, itemp);
                        var suffix = _field.Substring(itemp);
                        _field = prefix;
                        MethodName = suffix;
                    }
                }
            }
        }

        private PropertyInfo _property;

        [XmlIgnore, IgnoreDataMember]
        public PropertyInfo Property
        {
            get { return _property; }
            set
            {
                _property = value;
                IsUseProperty = _property != null;
            }
        }

        public bool IsUseProperty { get; set; }


        [DataMember]
        public OrderMethod Method { get; set; }

        [DataMember]
        public string MethodName
        {
            get { return Method.ToString(); }
            set
            {
                Method = (value ?? "").Trim().ToUpper() == OrderMethod.DESC.ToString()
                    ? OrderMethod.DESC
                    : OrderMethod.ASC;
            }
        }


        public string Build()
        {
            var result = string.Format("{0} {1}", Field, MethodName);
            return result;
        }

        public T ConvertTo<T>() where T : IQueryOrder
        {
            var result = Activator.CreateInstance<T>();
            MapTo(result);
            return result;
        }

        public void MapTo<T>(T order) where T : IQueryOrder
        {
            if (order != null)
            {
                order.Field = Field;
                order.Property = Property;
                order.Method = Method;
            }
        }

        public override string ToString()
        {
            return Build();
        }
    }
}