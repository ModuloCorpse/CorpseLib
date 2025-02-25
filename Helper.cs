using System.Collections;
using System.Text;

namespace CorpseLib
{
    public static class Helper
    {
        public static bool ChangeType<T>(object? value, out T? ret)
        {
            object? tmp = ChangeType(value, typeof(T));
            if (tmp == null)
            {
                ret = default;
                return false;
            }
            ret = (T)tmp;
            return true;
        }

        public static object? ChangeType(object? value, Type type)
        {
            try
            {
                return Convert.ChangeType(value, type);
            } catch
            {
                return null;
            }
        }

        public static T? Cast<T>(object? value) => (T?)Cast(value, typeof(T));

        public static object? Cast(object? value, Type type)
        {
            if (value == null)
                return null;
            if (value.GetType().IsAssignableTo(type))
                return value;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return default;
                type = Nullable.GetUnderlyingType(type)!;
            }
            if (type == typeof(Guid) && value is string str)
                return Guid.Parse(str);
            if (type == typeof(DateTime))
            {
                if (value is string dateStr)
                    return DateTime.Parse(dateStr);
                else if (long.TryParse(value.ToString(), out long ticks))
                    return new DateTime(ticks);

            }
            if (type.IsEnum)
            {
                int? enumValue = (int?)ChangeType(value, typeof(int));
                if (enumValue == null)
                    return null;
                return Enum.ToObject(type, enumValue);
            }
            return ChangeType(value, type);
        }

        public static string ToString(IEnumerable enumerable, Func<object, string> elementConverter)
        {
            StringBuilder sb = new("{ ");
            int i = 0;
            foreach (object elem in enumerable)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(elementConverter(elem));
                i++;
            }
            sb.Append(" }");
            return sb.ToString();
        }
        public static string ToString(IEnumerable enumerable) => ToString(enumerable, (elem) => elem.ToString() ?? string.Empty);

        public static string ToString(IDictionary dict, Func<object, string> keyConverter, Func<object?, string> valueConverter)
        {
            StringBuilder sb = new("{ ");
            int i = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append("{ key=");
                sb.Append(keyConverter(entry.Key));
                sb.Append(", value=");
                sb.Append(valueConverter(entry.Value));
                sb.Append(" }");
                i++;
            }
            sb.Append(" }");
            return sb.ToString();
        }
        public static string ToString(IDictionary dict) => ToString(dict, (key) => key.ToString() ?? string.Empty, (value) => value?.ToString() ?? string.Empty);
        public static string ToString(IDictionary dict, Func<object?, string> valueConverter) => ToString(dict, (key) => key.ToString() ?? string.Empty, valueConverter);
    }
}
