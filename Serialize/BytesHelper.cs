namespace CorpseLib.Serialize
{
    public static class BytesHelper
    {
        public static decimal ToDecimal(byte[] value, int startIndex)
        {
            int[] bits = [
                ((value[0 + startIndex] | (value[1 + startIndex] << 8)) | (value[2 + startIndex] << 0x10)) | (value[3 + startIndex] << 0x18), //lo
                ((value[4 + startIndex] | (value[5 + startIndex] << 8)) | (value[6 + startIndex] << 0x10)) | (value[7 + startIndex] << 0x18), //mid
                ((value[8 + startIndex] | (value[9 + startIndex] << 8)) | (value[10 + startIndex] << 0x10)) | (value[11 + startIndex] << 0x18), //hi
                ((value[12 + startIndex] | (value[13 + startIndex] << 8)) | (value[14 + startIndex] << 0x10)) | (value[15 + startIndex] << 0x18), //flags
            ];
            return new decimal(bits);
        }

        public static byte[] GetBytes(decimal d)
        {
            int[] bits = decimal.GetBits(d);
            byte[] bytes = [
                (byte)bits[0], (byte)(bits[0] >> 8), (byte)(bits[0] >> 0x10), (byte)(bits[0] >> 0x18),
                (byte)bits[1], (byte)(bits[1] >> 8), (byte)(bits[1] >> 0x10), (byte)(bits[1] >> 0x18),
                (byte)bits[2], (byte)(bits[2] >> 8), (byte)(bits[2] >> 0x10), (byte)(bits[2] >> 0x18),
                (byte)bits[3], (byte)(bits[3] >> 8), (byte)(bits[3] >> 0x10), (byte)(bits[3] >> 0x18),
            ];
            return bytes;
        }
    }
}
