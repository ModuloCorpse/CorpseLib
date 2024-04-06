using CorpseLib.Network;
using CorpseLib.Serialize;

namespace CorpseLib.Json
{
    public class JsonNativeSerializer : ANativeSerializer<JsonValue>
    {
        protected override JsonValue SerializeBool(bool value) => new(value);
        protected override bool DeserializeBool(JsonValue serial) => (bool)Convert.ChangeType(serial.Value, typeof(bool));

        protected override JsonValue SerializeByte(byte value) => new(value);
        protected override byte DeserializeByte(JsonValue serial) => (byte)Convert.ChangeType(serial.Value, typeof(byte));

        protected override JsonValue SerializeSByte(sbyte value) => new(value);
        protected override sbyte DeserializeSByte(JsonValue serial) => (sbyte)Convert.ChangeType(serial.Value, typeof(sbyte));

        protected override JsonValue SerializeChar(char value) => new(value);
        protected override char DeserializeChar(JsonValue serial) => (char)Convert.ChangeType(serial.Value, typeof(char));

        protected override JsonValue SerializeShort(short value) => new(value);
        protected override short DeserializeShort(JsonValue serial) => (short)Convert.ChangeType(serial.Value, typeof(short));

        protected override JsonValue SerializeUShort(ushort value) => new(value);
        protected override ushort DeserializeUShort(JsonValue serial) => (ushort)Convert.ChangeType(serial.Value, typeof(ushort));

        protected override JsonValue SerializeInt(int value) => new(value);
        protected override int DeserializeInt(JsonValue serial) => (int)Convert.ChangeType(serial.Value, typeof(int));

        protected override JsonValue SerializeUInt(uint value) => new(value);
        protected override uint DeserializeUInt(JsonValue serial) => (uint)Convert.ChangeType(serial.Value, typeof(uint));

        protected override JsonValue SerializeLong(long value) => new(value);
        protected override long DeserializeLong(JsonValue serial) => (long)Convert.ChangeType(serial.Value, typeof(long));

        protected override JsonValue SerializeULong(ulong value) => new(value);
        protected override ulong DeserializeULong(JsonValue serial) => (ulong)Convert.ChangeType(serial.Value, typeof(ulong));

        protected override JsonValue SerializeFloat(float value) => new(value);
        protected override float DeserializeFloat(JsonValue serial) => (float)Convert.ChangeType(serial.Value, typeof(float));

        protected override JsonValue SerializeDouble(double value) => new(value);
        protected override double DeserializeDouble(JsonValue serial) => (double)Convert.ChangeType(serial.Value, typeof(double));

        protected override JsonValue SerializeDecimal(decimal value) => new(value);
        protected override decimal DeserializeDecimal(JsonValue serial) => (decimal)Convert.ChangeType(serial.Value, typeof(decimal));

        protected override JsonValue SerializeString(string value) => new(value);
        protected override string DeserializeString(JsonValue serial) => serial.Value.ToString()!;
    }
}
