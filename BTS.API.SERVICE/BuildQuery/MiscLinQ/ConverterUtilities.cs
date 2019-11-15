using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTS.API.SERVICE.BuildQuery.Query.MiscLinQ
{
    public static class ConverterUtilities
    {
        static ConverterUtilities()
        {
            Converters = new Dictionary<Type, IConverter>();

            AddConverter<Guid>(new GuidConverter());

            AddConverter<string>(new TextConverter());
            AddConverter<char>(new TextConverter());

            AddConverter<decimal>(new NumberConverter());
            AddConverter<int>(new NumberConverter());
            AddConverter<double>(new NumberConverter());

            AddConverter<bool>(new BooleanConverter());

            AddConverter<DateTime>(new DateTimeConverter());

            AddConverter(typeof (IEnumerable<>), new ArrayConverter());
            AddConverter(typeof (IList<>), new ArrayConverter());
            AddConverter(typeof (Array), new ArrayConverter());
        }

        public static IDictionary<Type, IConverter> Converters { get; set; }

        public static void AddConverter<T>(IConverter converter)
        {
            AddConverter(typeof (T), converter);
        }

        public static void AddConverter(Type type, IConverter converter)
        {
            if (Converters.ContainsKey(type))
            {
                if (converter != null)
                    Converters[type] = converter;
                else
                    Converters.Remove(type);
            }
            else
            {
                if (converter != null)
                    Converters.Add(type, converter);
            }
        }

        public static IConverter GetConverter<T>()
        {
            return GetConverter(typeof (T));
        }

        public static IConverter GetConverter(Type type)
        {
            IConverter result = null;
            if (Converters.ContainsKey(type))
                result = Converters[type];
            return result;
        }

        public static string MapTo<T>(T value)
        {
            var result = "";
            var converter = GetConverter<T>() ?? new DefaultConverter();
            result = converter.MapTo(value);
            return result;
        }
    }
}