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

        public static bool Cast(JsonNode token, out object? ret, Type type)
        {
            if (token is JsonObject jobj)
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

            if (token.GetType().IsAssignableTo(type))
            {
                ret = token;
                return true;
            }
            else if (token is JsonNull)
            {
                ret = null;
                return true;
            }
            else if (token is JsonValue value)
            {
                try
                {
                    ret = value.Cast(type);
                    return true;
                } catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && token is JsonArray arr)
            {
                Type listType = type.GetGenericArguments()[0];
                IList? list = (IList?)Activator.CreateInstance(type);
                if (list != null)
                {
                    foreach (JsonNode item in arr)
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
                itemType == typeof(Guid))
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
