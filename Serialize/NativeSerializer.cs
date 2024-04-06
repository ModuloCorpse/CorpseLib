using CorpseLib.Network;

namespace CorpseLib.Serialize
{
    public abstract class ANativeSerializer<TSerial>
    {
        public bool IsNative<T>() => IsNative(typeof(T));
        public bool IsNative(Type type) => (type.IsEnum ||
            type == typeof(bool) ||
            type == typeof(byte) ||
            type == typeof(sbyte) ||
            type == typeof(char) ||
            type == typeof(short) ||
            type == typeof(ushort) ||
            type == typeof(int) ||
            type == typeof(uint) ||
            type == typeof(long) ||
            type == typeof(ulong) ||
            type == typeof(float) ||
            type == typeof(double) ||
            type == typeof(decimal) ||
            type == typeof(string) ||
            type == typeof(Guid) ||
            type == typeof(DateTime) ||
            type == typeof(TimeSpan) ||
            type == typeof(URI));

        public T? Deserialize<T>(TSerial serial) => (T?)Deserialize(serial, typeof(T));
        public object? Deserialize(TSerial serial, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            try
            {
                if (type.IsEnum)
                {
                    Type enumType = Enum.GetUnderlyingType(type);
                    object? enumValue = Deserialize(serial, enumType);
                    if (enumValue != null)
                        return Enum.ToObject(type, enumValue);
                    return null;
                }
                if (type == typeof(bool))
                    return DeserializeBool(serial);
                if (type == typeof(byte))
                    return DeserializeByte(serial);
                if (type == typeof(sbyte))
                    return DeserializeSByte(serial);
                if (type == typeof(char))
                    return DeserializeChar(serial);
                if (type == typeof(short))
                    return DeserializeShort(serial);
                if (type == typeof(ushort))
                    return DeserializeUShort(serial);
                if (type == typeof(int))
                    return DeserializeInt(serial);
                if (type == typeof(uint))
                    return DeserializeUInt(serial);
                if (type == typeof(long))
                    return DeserializeLong(serial);
                if (type == typeof(ulong))
                    return DeserializeULong(serial);
                if (type == typeof(float))
                    return DeserializeFloat(serial);
                if (type == typeof(double))
                    return DeserializeDouble(serial);
                if (type == typeof(decimal))
                    return DeserializeDecimal(serial);
                if (type == typeof(string))
                    return DeserializeString(serial);
                if (type == typeof(Guid))
                    return Guid.Parse(DeserializeString(serial));
                if (type == typeof(DateTime))
                    return new DateTime(DeserializeLong(serial));
                if (type == typeof(TimeSpan))
                    return new TimeSpan(DeserializeLong(serial));
                if (type == typeof(URI))
                    return URI.Parse(DeserializeString(serial));
            } catch { }
            return null;
        }

        public TSerial? Serialize(object obj)
        {
            try
            {
                Type type = obj.GetType();
                type = Nullable.GetUnderlyingType(type) ?? type;
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);
                if (type == typeof(bool))
                    return SerializeBool((bool)obj);
                if (type == typeof(byte))
                    return SerializeByte((byte)obj);
                if (type == typeof(sbyte))
                    return SerializeSByte((sbyte)obj);
                if (type == typeof(char))
                    return SerializeChar((char)obj);
                if (type == typeof(short))
                    return SerializeShort((short)obj);
                if (type == typeof(ushort))
                    return SerializeUShort((ushort)obj);
                if (type.IsEnum || type == typeof(int))
                    return SerializeInt((int)obj);
                if (type == typeof(uint))
                    return SerializeUInt((uint)obj);
                if (type == typeof(long))
                    return SerializeLong((long)obj);
                if (type == typeof(ulong))
                    return SerializeULong((ulong)obj);
                if (type == typeof(float))
                    return SerializeFloat((float)obj);
                if (type == typeof(double))
                    return SerializeDouble((double)obj);
                if (type == typeof(decimal))
                    return SerializeDecimal((decimal)obj);
                if (type == typeof(string))
                    return SerializeString((string)obj);
                if (type == typeof(Guid))
                    return SerializeString(((Guid)obj).ToString());
                if (type == typeof(DateTime))
                    return SerializeLong(((DateTime)obj).Ticks);
                if (type == typeof(TimeSpan))
                    return SerializeLong(((TimeSpan)obj).Ticks);
                if (type == typeof(URI))
                    return SerializeString(((URI)obj).ToString());
            } catch { }
            return default;
        }

        protected abstract TSerial SerializeBool(bool value);
        protected abstract bool DeserializeBool(TSerial serial);

        protected abstract TSerial SerializeByte(byte value);
        protected abstract byte DeserializeByte(TSerial serial);

        protected abstract TSerial SerializeSByte(sbyte value);
        protected abstract sbyte DeserializeSByte(TSerial serial);

        protected abstract TSerial SerializeChar(char value);
        protected abstract char DeserializeChar(TSerial serial);

        protected abstract TSerial SerializeShort(short value);
        protected abstract short DeserializeShort(TSerial serial);

        protected abstract TSerial SerializeUShort(ushort value);
        protected abstract ushort DeserializeUShort(TSerial serial);

        protected abstract TSerial SerializeInt(int value);
        protected abstract int DeserializeInt(TSerial serial);

        protected abstract TSerial SerializeUInt(uint value);
        protected abstract uint DeserializeUInt(TSerial serial);

        protected abstract TSerial SerializeLong(long value);
        protected abstract long DeserializeLong(TSerial serial);

        protected abstract TSerial SerializeULong(ulong value);
        protected abstract ulong DeserializeULong(TSerial serial);

        protected abstract TSerial SerializeFloat(float value);
        protected abstract float DeserializeFloat(TSerial serial);

        protected abstract TSerial SerializeDouble(double value);
        protected abstract double DeserializeDouble(TSerial serial);

        protected abstract TSerial SerializeDecimal(decimal value);
        protected abstract decimal DeserializeDecimal(TSerial serial);

        protected abstract TSerial SerializeString(string value);
        protected abstract string DeserializeString(TSerial serial);
    }
}
