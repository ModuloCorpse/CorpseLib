using System.Collections;

namespace CorpseLib.Json
{
    public class JHelper
    {
        public static readonly JFormat NETWORK_FORMAT = new()
        {
            InlineScope = true,
            DoLineBreak = false,
            DoIndent = false,
        };

        public static bool Cast(JNode token, out object? ret, Type type)
        {
            if (token is JObject jobj)
            {
                JSerializer jSerializer = new();
                AJSerializer? serializer = jSerializer.GetSerializerFor(type);
                if (serializer != null)
                {
                    OperationResult<object?> result = serializer.DeserializeObj(jobj);
                    if (result)
                    {
                        ret = result.Result;
                        return true;
                    }
                    ret = null;
                    return false;
                }
            }

            if (token.GetType() == type)
            {
                ret = token;
                return true;
            }
            else if (token is JNull)
            {
                ret = null;
                return true;
            }
            else if (token is JValue value)
            {
                try
                {
                    ret = value.Cast(type);
                    return true;
                } catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && token is JArray arr)
            {
                Type listType = type.GetGenericArguments()[0];
                IList? list = (IList?)Activator.CreateInstance(type);
                if (list != null)
                {
                    foreach (JNode item in arr)
                    {
                        if (Cast(item, out object? listRet, listType))
                            list.Add(listRet);
                    }
                }
                ret = list;
                return true;
            }
            ret = null;
            return false;
        }

        public static bool Cast<T>(JNode token, out T? ret)
        {
            if (Cast(token, out object? tmp, typeof(T)))
            {
                ret = (T?)tmp;
                return true;
            }
            ret = default;
            return false;
        }

        public static JNode Cast(object? item)
        {
            if (item == null)
                return new JNull();
            JSerializer jSerializer = new();
            AJSerializer? serializer = jSerializer.GetSerializerFor(item.GetType());
            if (serializer != null)
            {
                JObject ret = new();
                serializer.SerializeObj(item, ret);
                return ret;
            }
            if (item is JNode node)
                return node;
            else
                return new JValue(item);
        }
    }
}
