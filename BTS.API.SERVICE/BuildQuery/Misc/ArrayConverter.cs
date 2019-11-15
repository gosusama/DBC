using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.Misc
{
    public class ArrayConverter : IConverter
    {
        public string MapTo(dynamic value)
        {
            var result = "";
            var type = value.GetType();
            if (type.IsArray || (value is IList && type.IsGenericType))
            {
                Type subType = GetEnumerableType(type);
                var converter = ConverterUtilities.GetConverter(subType);
                var childs = new List<string>();
                foreach (var item in value)
                {
                    var childValue = converter.MapTo(item);
                    childs.Add(childValue);
                }
                result = string.Join(@", ", childs);
            }
            return result;
        }

        public static bool IsArrayText(dynamic value)
        {
            var result = false;
            var type = value.GetType();
            if (type.IsArray || (value is IList && type.IsGenericType))
            {
                Type subType = GetEnumerableType(type);
                result = subType == typeof (string) || subType == typeof (char);
            }
            return result;
        }

        internal static Type GetEnumerableType(Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }
            return null;
        }
    }
}