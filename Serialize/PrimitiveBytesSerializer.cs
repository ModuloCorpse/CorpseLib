using System.Text;

namespace CorpseLib.Serialize
{
    internal abstract class PrimitiveBytesSerializer<T> : ABytesSerializer<T>
    {
        protected override OperationResult<T> Deserialize(ABytesReader reader)
        {
            int size = Size();
            byte[] bytes = reader.ReadBytes(Size());
            if (bytes.Length != size)
                return new("Deserialization error", "Not enough bytes");
            if (bytes.Length > 1 && BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new(Convert(bytes));
        }

        protected abstract int Size();
        protected abstract T Convert(byte[] bytes);

        protected override void Serialize(T obj, ABytesWriter writer)
        {
            byte[] bytes = BitConverter.GetBytes((dynamic)obj!);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            writer.Write(bytes);
        }
    }

    internal class BoolBytesSerializer : PrimitiveBytesSerializer<bool>
    {
        protected override bool Convert(byte[] bytes) => BitConverter.ToBoolean(bytes);
        protected override int Size() => sizeof(bool);
    }

    internal class CharBytesSerializer : PrimitiveBytesSerializer<char>
    {
        protected override char Convert(byte[] bytes) => BitConverter.ToChar(bytes);
        protected override int Size() => sizeof(char);
    }

    internal class IntBytesSerializer : PrimitiveBytesSerializer<int>
    {
        protected override int Convert(byte[] bytes) => BitConverter.ToInt32(bytes);
        protected override int Size() => sizeof(int);
    }

    internal class UIntBytesSerializer : PrimitiveBytesSerializer<uint>
    {
        protected override uint Convert(byte[] bytes) => BitConverter.ToUInt32(bytes);
        protected override int Size() => sizeof(uint);
    }

    internal class ShortBytesSerializer : PrimitiveBytesSerializer<short>
    {
        protected override short Convert(byte[] bytes) => BitConverter.ToInt16(bytes);
        protected override int Size() => sizeof(short);
    }

    internal class UShortBytesSerializer : PrimitiveBytesSerializer<ushort>
    {
        protected override ushort Convert(byte[] bytes) => BitConverter.ToUInt16(bytes);
        protected override int Size() => sizeof(ushort);
    }

    internal class LongBytesSerializer : PrimitiveBytesSerializer<long>
    {
        protected override long Convert(byte[] bytes) => BitConverter.ToInt64(bytes);
        protected override int Size() => sizeof(long);
    }

    internal class ULongBytesSerializer : PrimitiveBytesSerializer<ulong>
    {
        protected override ulong Convert(byte[] bytes) => BitConverter.ToUInt64(bytes);
        protected override int Size() => sizeof(ulong);
    }

    internal class FloatBytesSerializer : PrimitiveBytesSerializer<float>
    {
        protected override float Convert(byte[] bytes) => BitConverter.ToSingle(bytes);
        protected override int Size() => sizeof(float);
    }

    internal class DoubleBytesSerializer : PrimitiveBytesSerializer<double>
    {
        protected override double Convert(byte[] bytes) => BitConverter.ToDouble(bytes);
        protected override int Size() => sizeof(double);
    }

    internal class ByteBytesSerializer : ABytesSerializer<byte>
    {
        protected override OperationResult<byte> Deserialize(ABytesReader reader) => new(reader.ReadBytes(1)[0]);
        protected override void Serialize(byte obj, ABytesWriter writer) => writer.Write([obj]);
    }

    internal class SByteBytesSerializer : ABytesSerializer<sbyte>
    {
        protected override OperationResult<sbyte> Deserialize(ABytesReader reader) => new((sbyte)reader.ReadBytes(1)[0]);
        protected override void Serialize(sbyte obj, ABytesWriter writer) => writer.Write([(byte)obj]);
    }

    internal class GuidBytesSerializer : ABytesSerializer<Guid>
    {
        protected override OperationResult<Guid> Deserialize(ABytesReader reader) => new(new(reader.ReadBytes(16)));
        protected override void Serialize(Guid obj, ABytesWriter writer) => writer.Write(obj.ToByteArray());
    }

    internal class DateTimeBytesSerializer : ABytesSerializer<DateTime>
    {
        protected override OperationResult<DateTime> Deserialize(ABytesReader reader) => new(DateTime.FromBinary(reader.Read<long>()));
        protected override void Serialize(DateTime obj, ABytesWriter writer) => writer.Write(obj.Ticks);
    }

    internal class StringTimeBytesSerializer : ABytesSerializer<string>
    {
        protected override OperationResult<string> Deserialize(ABytesReader reader)
        {
            List<byte> bytes = [];
            byte read;
            do
            {
                read = reader.ReadBytes(1)[0];
                if (read != 0)
                    bytes.Add(read);
            } while (read != 0);
            return new(Encoding.UTF8.GetString([..bytes]));
        }
        protected override void Serialize(string obj, ABytesWriter writer)
        {
            writer.Write(Encoding.UTF8.GetBytes(obj));
            writer.Write([0]);
        }
    }

    internal class DecimalTimeBytesSerializer : ABytesSerializer<decimal>
    {
        protected override OperationResult<decimal> Deserialize(ABytesReader reader)
        {
            int lo = reader.Read<int>();
            int mid = reader.Read<int>();
            int hi = reader.Read<int>();
            int flags = reader.Read<int>();
            return new(new([lo, mid, hi, flags]));
        }

        protected override void Serialize(decimal obj, ABytesWriter writer)
        {
            int[] bytes = decimal.GetBits(obj);
            writer.Write(bytes[0]); //lo
            writer.Write(bytes[1]); //mid
            writer.Write(bytes[2]); //hi
            writer.Write(bytes[3]); //flags
        }
    }
}
