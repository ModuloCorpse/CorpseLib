﻿using CorpseLib.DataNotation;
using CorpseLib.Serialize;

namespace CorpseLib.Json
{
    public class JsonObjectSerializer : ABytesSerializer<DataObject>
    {
        protected override OperationResult<DataObject> Deserialize(ABytesReader reader)
        {
            try
            {
                return new(JsonParser.Parse(reader.Read<string>()));
            } catch (JsonException e)
            {
                return new("Deserialization error", e.Message);
            }
        }

        protected override void Serialize(DataObject obj, ABytesWriter writer)
        {
            string str = JsonParser.NetStr(obj);
            writer.Write(str);
        }
    }
}
