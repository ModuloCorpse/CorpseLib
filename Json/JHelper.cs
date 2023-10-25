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

        private static readonly Dictionary<Type, AJSerializer> ms_RegisteredSerializers = new();

        public static void RegisterSerializer<T>(AJSerializer<T> serializer) => ms_RegisteredSerializers[typeof(T)] = serializer;
        public static void UnregisterSerializer<T>() => ms_RegisteredSerializers.Remove(typeof(T));

        public static JSerializer NewSerializer()
        {
            JSerializer serializer = new();
            foreach (AJSerializer registeredSerializer in ms_RegisteredSerializers.Values)
                serializer.Register(registeredSerializer);
            return serializer;
        }

        public static bool Cast(JNode token, out object? ret, Type type)
        {
            if (token is JObject jobj)
            {
                JSerializer jSerializer = NewSerializer();
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

            if (token.GetType().IsAssignableTo(type))
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
            if (item is JNode node)
                return node;
            Type itemType = item.GetType();
            if (itemType.IsPrimitive || itemType.IsEnum || itemType == typeof(decimal) || itemType == typeof(string))
                return new JValue(item);
            if (item is IDictionary dict && itemType.GetGenericArguments()[0] == typeof(string))
            {
                JObject obj = new();
                foreach (DictionaryEntry pair in dict)
                    obj[(string)pair.Key] = pair.Value;
                return obj;
            }
            else if ((itemType.IsArray || item is IList) && item is IEnumerable enumerable)
            {
                JArray arr = new();
                foreach (object elem in enumerable)
                    arr.Add(Cast(elem));
                return arr;
            }
            JSerializer jSerializer = NewSerializer();
            AJSerializer? serializer = jSerializer.GetSerializerFor(itemType);
            if (serializer != null)
            {
                JObject ret = new();
                serializer.SerializeObj(item, ret);
                return ret;
            }
            throw new JException(string.Format("Cannot cast item : No know conversion from '{0}' to json node", itemType.Name));
        }

        public static object? Flatten(JNode node)
        {
            if (node is JValue val)
                return val.Value;
            else if (node is JObject obj)
            {
                Dictionary<string, object?> ret = new();
                foreach (KeyValuePair<string, JNode> pair in obj)
                    ret[pair.Key] = Flatten(pair.Value);
                return ret;
            }
            else if (node is JArray arr)
            {
                List<object?> ret = new();
                foreach (JNode elem in arr)
                    ret.Add(Flatten(elem));
                return ret;
            }
            else
                return null;
        }
    }
}
