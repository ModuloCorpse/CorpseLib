using CorpseLib.Datafile;

namespace CorpseLib.Json
{
    public class JsonParser : DataFileParser<JsonObject, JsonWriter, JsonReader>
    {
        public static T? LoadFromFile<T>(string filePath)
        {
            try
            {
                JsonObject obj = LoadFromFile(filePath);
                JsonHelper.Cast(obj, out T? ret);
                return ret;
            }
            catch
            {
                return default;
            }
        }

        public static void WriteToFile<T>(string filePath, object value)
        {
            if (JsonHelper.Cast(value) is JsonObject obj)
                WriteToFile(filePath, obj);
        }
    }
}
