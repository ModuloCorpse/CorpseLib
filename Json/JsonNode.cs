using CorpseLib.Datafile;

namespace CorpseLib.Json
{
    public abstract class JsonNode : DataFileFormatedObject<JsonWriter, JsonFormat>
    {
        public string ToNetworkString() => ToString(JsonHelper.NETWORK_FORMAT);

        public T? Cast<T>()
        {
            JsonHelper.Cast(this, out T? ret);
            return ret;
        }
    }
}
