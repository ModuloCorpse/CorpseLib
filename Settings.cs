using System.Text;

namespace CorpseLib
{
    public class Settings
    {
        private readonly Dictionary<string, string> m_Settings = new();

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                string file = File.ReadAllText(path);
                string[] lines = file.Split('\n');
                foreach (string line in lines)
                {
                    int separatorIdx = line.IndexOf(':');
                    if (separatorIdx >= 0)
                        m_Settings[(separatorIdx == 0) ? "" : line[..separatorIdx]] = (separatorIdx != (line.Length - 1)) ? line[(separatorIdx + 1)..] : "";
                }
            }
        }

        public void Save(string path)
        {
            StringBuilder sb = new();
            foreach (var pair in m_Settings)
            {
                sb.Append(pair.Key);
                sb.Append(':');
                sb.AppendLine(pair.Value);
            }
            File.WriteAllText(path, sb.ToString());
        }

        private bool CheckAdd(string key, string value)
        {
            if (key.Contains(':'))
                return false;
            m_Settings[key] = value;
            return true;
        }

        public bool Add(string key, bool value) => CheckAdd(key, (value) ? "true" : "false");
        public bool Add(string key, byte value) => CheckAdd(key, value.ToString());
        public bool Add(string key, sbyte value) => CheckAdd(key, value.ToString());
        public bool Add(string key, char value) => CheckAdd(key, value.ToString());
        public bool Add(string key, short value) => CheckAdd(key, value.ToString());
        public bool Add(string key, ushort value) => CheckAdd(key, value.ToString());
        public bool Add(string key, int value) => CheckAdd(key, value.ToString());
        public bool Add(string key, uint value) => CheckAdd(key, value.ToString());
        public bool Add(string key, long value) => CheckAdd(key, value.ToString());
        public bool Add(string key, ulong value) => CheckAdd(key, value.ToString());
        public bool Add(string key, float value) => CheckAdd(key, value.ToString());
        public bool Add(string key, double value) => CheckAdd(key, value.ToString());
        public bool Add(string key, string value) => CheckAdd(key, value);

        public bool GetBool(string key) => m_Settings[key] == "true";
        public byte GetByte(string key) => byte.Parse(m_Settings[key]);
        public sbyte GetSByte(string key) => sbyte.Parse(m_Settings[key]);
        public char GetChar(string key) => char.Parse(m_Settings[key]);
        public short GetShort(string key) => short.Parse(m_Settings[key]);
        public ushort GetUShort(string key) => ushort.Parse(m_Settings[key]);
        public int GetInt(string key) => int.Parse(m_Settings[key]);
        public uint GetUInt(string key) => uint.Parse(m_Settings[key]);
        public long GetLong(string key) => long.Parse(m_Settings[key]);
        public ulong GetULong(string key) => ulong.Parse(m_Settings[key]);
        public float GetFloat(string key) => float.Parse(m_Settings[key]);
        public double GetDouble(string key) => double.Parse(m_Settings[key]);
        public string GetString(string key) => m_Settings[key];
    }
}
