using CorpseLib.Network;
using System.Collections;

namespace CorpseLib.DataNotation
{
    public class DataHelper
    {
        private static readonly Dictionary<Type, ADataSerializer> ms_RegisteredSerializers = [];
        private static readonly DataNativeSerializer ms_NativeSerializer = new();

        public static void RegisterSerializer<T>(ADataSerializer<T> serializer) => ms_RegisteredSerializers[typeof(T)] = serializer;
        public static void UnregisterSerializer<T>() => ms_RegisteredSerializers.Remove(typeof(T));

        private static DataSerializer NewSerializer()
        {
            DataSerializer serializer = new();
            foreach (ADataSerializer registeredSerializer in ms_RegisteredSerializers.Values)
                serializer.Register(registeredSerializer);
            return serializer;
        }

        private static bool CastToDictionary(DataObject token, out object? ret, Type type)
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

        private static bool CastFromCmonValue(DataValue token, out object? ret, Type type)
        {
            if (type == typeof(DataValue))
            {
                ret = token;
                return true;
            }
            else if (type == typeof(object))
            {
                ret = token.Value;
                return true;
            }

            try
            {
                ret = ms_NativeSerializer.Deserialize(token, type);
                return true;
            }
            catch
            {
                ret = null;
                return false;
            }
        }

        private static bool CastToArray(DataArray token, out object? ret, Type type)
        {
            Type arrType = type.GetElementType()!;
            Type genericListType = typeof(List<>).MakeGenericType(arrType);
            IList? list = (IList?)Activator.CreateInstance(genericListType);
            if (list != null)
            {
                foreach (DataNode item in token)
                {
                    if (Cast(item, out object? listRet, arrType))
                        list.Add(listRet);
                }
                System.Array arr = System.Array.CreateInstance(arrType, list.Count);
                int i = 0;
                foreach (object? item in list)
                    arr.SetValue(item, i++);
                ret = arr;
                return true;
            }
            ret = null;
            return false;
        }

        private static bool CastToList(DataArray token, out object? ret, Type type)
        {
            Type listType = type.GetGenericArguments()[0];
            IList? list = (IList?)Activator.CreateInstance(type);
            if (list != null)
            {
                foreach (DataNode item in token)
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

        public static bool Cast(DataNode token, out object? ret, Type type)
        {
            if (type.IsAssignableTo(typeof(DataNode)) && token.GetType().IsAssignableTo(type))
            {
                ret = token;
                return true;
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                type.GetGenericArguments()[0] == typeof(string) &&
                token is DataObject dictObj)
                return CastToDictionary(dictObj, out ret, type);
            else if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(List<>) &&
                token is DataArray list)
                return CastToList(list, out ret, type);
            else if (type.IsArray &&
                token is DataArray arr)
                return CastToArray(arr, out ret, type);

            if (token is DataObject jobj)
            {
                DataSerializer serializer = NewSerializer();
                OperationResult<object?> result = serializer.Deserialize(jobj, type);
                if (result)
                {
                    ret = result.Result;
                    return true;
                }
            }
            else if (token is DataValue value)
                return CastFromCmonValue(value, out ret, type);
            else if (token.GetType().IsAssignableTo(type))
            {
                ret = token;
                return true;
            }
            ret = null;
            return false;
        }

        public static bool Cast<T>(DataNode token, out T? ret)
        {
            if (Cast(token, out object? tmp, typeof(T)))
            {
                ret = (T?)tmp;
                return true;
            }
            ret = default;
            return false;
        }

        public static DataNode Cast(object? item)
        {
            if (item == null)
                return new DataValue();
            if (item is DataNode node)
                return node;
            Type itemType = item.GetType();
            if (ms_NativeSerializer.IsNative(itemType))
            {
                DataValue? value = ms_NativeSerializer.Serialize(item);
                if (value != null)
                    return value;
            }
            if (item is IDictionary dict && itemType.GetGenericArguments()[0] == typeof(string))
            {
                DataObject obj = [];
                foreach (DictionaryEntry pair in dict)
                    obj[(string)pair.Key] = pair.Value;
                return obj;
            }
            else if ((itemType.IsArray || item is IList) && item is IEnumerable enumerable)
            {
                DataArray arr = [];
                foreach (object elem in enumerable)
                    arr.Add(Cast(elem));
                return arr;
            }
            else if (item is URI uri)
                return new DataValue(uri.ToString());
            DataSerializer serializer = NewSerializer();
            DataObject ret = [];
            if (serializer.Serialize(item, ret))
                return ret;
            throw new DataException(string.Format("Cannot cast item : No know conversion from '{0}' to data node", itemType.Name));
        }

        public static object? Flatten(DataNode node)
        {
            if (node is DataValue val)
                return val.Value;
            else if (node is DataObject obj)
            {
                Dictionary<string, object?> ret = [];
                foreach (KeyValuePair<string, DataNode> pair in obj)
                    ret[pair.Key] = Flatten(pair.Value);
                return ret;
            }
            else if (node is DataArray arr)
            {
                List<object?> ret = [];
                foreach (DataNode elem in arr)
                    ret.Add(Flatten(elem));
                return ret;
            }
            else
                return null;
        }
    }
}
