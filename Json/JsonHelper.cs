using CorpseLib.Network;
using System.Collections;

namespace CorpseLib.Json
{
    public class JsonHelper
    {
        public static readonly JsonFormat NETWORK_FORMAT = new()
        {
            InlineScope = true,
            DoLineBreak = false,
            DoIndent = false,
        };

        private static readonly Dictionary<Type, AJsonSerializer> ms_RegisteredSerializers = [];

        public static void RegisterSerializer<T>(AJsonSerializer<T> serializer) => ms_RegisteredSerializers[typeof(T)] = serializer;
        public static void UnregisterSerializer<T>() => ms_RegisteredSerializers.Remove(typeof(T));

        public static JsonSerializer NewSerializer()
        {
            JsonSerializer serializer = new();
            foreach (AJsonSerializer registeredSerializer in ms_RegisteredSerializers.Values)
                serializer.Register(registeredSerializer);
            return serializer;
        }

        private static bool CastToDictionary(JsonObject token, out object? ret, Type type)
        {
            Type valueType = type.GetGenericArguments()[1];
            IDictionary? dict = (IDictionary?)Activator.CreateInstance(type);
            if (dict != null)
            {
                foreach (var pair in token)
                {
                    string key = pair.Key;
                    if (Cast(pair.Value, out object? dictValue, valueType))
                    {
                        try
                        {
                            dict.Add(key, dictValue);
                        }
                        catch
                        {
                            ret = null;
                            return false;
                        }
                    }
                }
                ret = dict;
                return true;
            }
            ret = null;
            return false;
        }

        private static bool CastFromJsonValue(JsonValue token, out object? ret, Type type)
        {
            if (type == typeof(JsonValue))
            {
                ret = token;
                return true;
            }

            try
            {
                object obj = token.Value;
                if (type == typeof(URI) && obj is string uriStr)
                {
                    ret = URI.Parse(uriStr);
                    return true;
                }
                if (type == typeof(Guid))
                {
                    if (obj is string guidStr)
                    {
                        ret = Guid.Parse(guidStr);
                        return true;
                    }
                    else
                    {
                        ret = null;
                        return false;
                    }
                }
                else if (type == typeof(TimeSpan))
                {
                    if (long.TryParse(obj.ToString(), out long ticks))
                    {
                        ret = new TimeSpan(ticks);
                        return true;
                    }
                    else
                    {
                        ret = null;
                        return false;
                    }
                }
                else if (type == typeof(DateTime))
                {
                    if (obj is string dateStr)
                    {
                        ret = DateTime.Parse(dateStr);
                        return true;
                    }
                    else if (long.TryParse(obj.ToString(), out long ticks))
                    {
                        ret = new DateTime(ticks);
                        return true;
                    }
                    else
                    {
                        ret = null;
                        return false;
                    }
                }
                ret = Helper.Cast(obj, type);
                return true;
            }
            catch
            {
                ret = null;
                return false;
            }
        }

        private static bool CastToArray(JsonArray token, out object? ret, Type type)
        {
            Type arrType = type.GetElementType()!;
            Type genericListType = typeof(List<>).MakeGenericType(arrType);
            IList? list = (IList?)Activator.CreateInstance(genericListType);
            if (list != null)
            {
                foreach (JsonNode item in token)
                {
                    if (Cast(item, out object? listRet, arrType))
                        list.Add(listRet);
                }
                Array arr = Array.CreateInstance(arrType, list.Count);
                int i = 0;
                foreach (object? item in list)
                    arr.SetValue(item, i++);
                ret = arr;
                return true;
            }
            ret = null;
            return false;
        }

        private static bool CastToList(JsonArray token, out object? ret, Type type)
        {
            Type listType = type.GetGenericArguments()[0];
            IList? list = (IList?)Activator.CreateInstance(type);
            if (list != null)
            {
                foreach (JsonNode item in token)
                {
                    if (Cast(item, out object? listRet, listType))
                        list.Add(listRet);
                }
                ret = list;
                return true;
            }
            ret = null;
            return false;
        }

        public static bool Cast(JsonNode token, out object? ret, Type type)
        {
            if (type.IsAssignableTo(typeof(JsonNode)) && token.GetType().IsAssignableTo(type))
            {
                ret = token;
                return true;
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                type.GetGenericArguments()[0] == typeof(string) &&
                token is JsonObject dictObj)
                return CastToDictionary(dictObj, out ret, type);
            else if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(List<>) &&
                token is JsonArray list)
                return CastToList(list, out ret, type);
            else if (type.IsArray &&
                token is JsonArray arr)
                return CastToArray(arr, out ret, type);

            if (token is JsonNull)
            {
                ret = null;
                return true;
            }
            else if (token is JsonObject jobj)
            {
                JsonSerializer jSerializer = NewSerializer();
                AJsonSerializer? serializer = jSerializer.GetSerializerFor(type);
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
            else if (token is JsonValue value)
                return CastFromJsonValue(value, out ret, type);
            else if (token.GetType().IsAssignableTo(type))
            {
                ret = token;
                return true;
            }
            ret = null;
            return false;
        }

        public static bool Cast<T>(JsonNode token, out T? ret)
        {
            if (Cast(token, out object? tmp, typeof(T)))
            {
                ret = (T?)tmp;
                return true;
            }
            ret = default;
            return false;
        }

        public static JsonNode Cast(object? item)
        {
            if (item == null)
                return new JsonNull();
            if (item is JsonNode node)
                return node;
            Type itemType = item.GetType();
            if (itemType.IsPrimitive ||
                itemType.IsEnum ||
                itemType == typeof(decimal) ||
                itemType == typeof(string) ||
                itemType == typeof(Guid) ||
                itemType == typeof(DateTime) ||
                itemType == typeof(TimeSpan))
                return new JsonValue(item);
            if (item is IDictionary dict && itemType.GetGenericArguments()[0] == typeof(string))
            {
                JsonObject obj = [];
                foreach (DictionaryEntry pair in dict)
                    obj[(string)pair.Key] = pair.Value;
                return obj;
            }
            else if ((itemType.IsArray || item is IList) && item is IEnumerable enumerable)
            {
                JsonArray arr = [];
                foreach (object elem in enumerable)
                    arr.Add(Cast(elem));
                return arr;
            }
            else if (item is URI uri)
                return new JsonValue(uri.ToString());
            JsonSerializer jSerializer = NewSerializer();
            AJsonSerializer? serializer = jSerializer.GetSerializerFor(itemType);
            if (serializer != null)
            {
                JsonObject ret = [];
                serializer.SerializeObj(item, ret);
                return ret;
            }
            throw new JsonException(string.Format("Cannot cast item : No know conversion from '{0}' to json node", itemType.Name));
        }

        public static object? Flatten(JsonNode node)
        {
            if (node is JsonValue val)
                return val.Value;
            else if (node is JsonObject obj)
            {
                Dictionary<string, object?> ret = [];
                foreach (KeyValuePair<string, JsonNode> pair in obj)
                    ret[pair.Key] = Flatten(pair.Value);
                return ret;
            }
            else if (node is JsonArray arr)
            {
                List<object?> ret = [];
                foreach (JsonNode elem in arr)
                    ret.Add(Flatten(elem));
                return ret;
            }
            else
                return null;
        }
    }
}
