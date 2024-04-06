namespace CorpseLib.Serialize
{
    public class StringNativeSerializer : ANativeSerializer<string>
    {
        protected override string SerializeBool(bool value) => value.ToString();
        protected override bool DeserializeBool(string serial) => bool.Parse(serial);

        protected override string SerializeByte(byte value) => value.ToString();
        protected override byte DeserializeByte(string serial) => byte.Parse(serial);

        protected override string SerializeSByte(sbyte value) => value.ToString();
        protected override sbyte DeserializeSByte(string serial) => sbyte.Parse(serial);

        protected override string SerializeChar(char value) => value.ToString();
        protected override char DeserializeChar(string serial) => char.Parse(serial);

        protected override string SerializeShort(short value) => value.ToString();
        protected override short DeserializeShort(string serial) => short.Parse(serial);

        protected override string SerializeUShort(ushort value) => value.ToString();
        protected override ushort DeserializeUShort(string serial) => ushort.Parse(serial);

        protected override string SerializeInt(int value) => value.ToString();
        protected override int DeserializeInt(string serial) => int.Parse(serial);

        protected override string SerializeUInt(uint value) => value.ToString();
        protected override uint DeserializeUInt(string serial) => uint.Parse(serial);

        protected override string SerializeLong(long value) => value.ToString();
        protected override long DeserializeLong(string serial) => long.Parse(serial);

        protected override string SerializeULong(ulong value) => value.ToString();
        protected override ulong DeserializeULong(string serial) => ulong.Parse(serial);

        protected override string SerializeFloat(float value) => value.ToString();
        protected override float DeserializeFloat(string serial) => float.Parse(serial);

        protected override string SerializeDouble(double value) => value.ToString();
        protected override double DeserializeDouble(string serial) => double.Parse(serial);

        protected override string SerializeDecimal(decimal value) => value.ToString();
        protected override decimal DeserializeDecimal(string serial) => decimal.Parse(serial);

        protected override string SerializeString(string value) => value.ToString();
        protected override string DeserializeString(string serial) => serial;
    }
}
