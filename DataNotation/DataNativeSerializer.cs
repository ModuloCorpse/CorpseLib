using CorpseLib.Serialize;

namespace CorpseLib.DataNotation
{
    public class DataNativeSerializer : ANativeSerializer<DataValue>
    {
        protected override DataValue SerializeBool(bool value) => new(value);
        protected override bool DeserializeBool(DataValue serial) => (bool?)Convert.ChangeType(serial.Value, typeof(bool)) ?? false;

        protected override DataValue SerializeByte(byte value) => new(value);
        protected override byte DeserializeByte(DataValue serial) => (byte?)Convert.ChangeType(serial.Value, typeof(byte)) ?? 0;

        protected override DataValue SerializeSByte(sbyte value) => new(value);
        protected override sbyte DeserializeSByte(DataValue serial) => (sbyte?)Convert.ChangeType(serial.Value, typeof(sbyte)) ?? 0;

        protected override DataValue SerializeChar(char value) => new(value);
        protected override char DeserializeChar(DataValue serial) => (char?)Convert.ChangeType(serial.Value, typeof(char)) ?? '\0';

        protected override DataValue SerializeShort(short value) => new(value);
        protected override short DeserializeShort(DataValue serial) => (short?)Convert.ChangeType(serial.Value, typeof(short)) ?? 0;

        protected override DataValue SerializeUShort(ushort value) => new(value);
        protected override ushort DeserializeUShort(DataValue serial) => (ushort?)Convert.ChangeType(serial.Value, typeof(ushort)) ?? 0;

        protected override DataValue SerializeInt(int value) => new(value);
        protected override int DeserializeInt(DataValue serial) => (int?)Convert.ChangeType(serial.Value, typeof(int)) ?? 0;

        protected override DataValue SerializeUInt(uint value) => new(value);
        protected override uint DeserializeUInt(DataValue serial) => (uint?)Convert.ChangeType(serial.Value, typeof(uint)) ?? 0;

        protected override DataValue SerializeLong(long value) => new(value);
        protected override long DeserializeLong(DataValue serial) => (long?)Convert.ChangeType(serial.Value, typeof(long)) ?? 0;

        protected override DataValue SerializeULong(ulong value) => new(value);
        protected override ulong DeserializeULong(DataValue serial) => (ulong?)Convert.ChangeType(serial.Value, typeof(ulong)) ?? 0;

        protected override DataValue SerializeFloat(float value) => new(value);
        protected override float DeserializeFloat(DataValue serial) => (float?)Convert.ChangeType(serial.Value, typeof(float)) ?? 0;

        protected override DataValue SerializeDouble(double value) => new(value);
        protected override double DeserializeDouble(DataValue serial) => (double?)Convert.ChangeType(serial.Value, typeof(double)) ?? 0;

        protected override DataValue SerializeDecimal(decimal value) => new(value);
        protected override decimal DeserializeDecimal(DataValue serial) => (decimal?)Convert.ChangeType(serial.Value, typeof(decimal)) ?? 0;

        protected override DataValue SerializeString(string value) => new(value);
        protected override string DeserializeString(DataValue serial) => serial.ToVariableString();
    }
}
