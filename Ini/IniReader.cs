using CorpseLib.Datafile;

namespace CorpseLib.Ini
{
    public class IniReader : DataFileReader<IniFile, IniWriter>
    {
        public override IniFile Read()
        {
            IniFile ret = new();
            IniSection section = [];
            bool hasRead = false;
            int i = 0;
            while (CanRead)
            {
                string line = NextLine();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line[0] == '[' && line[^1] == ']')
                    {
                        if (hasRead)
                            ret.Add(section);
                        section = new(line[1..^1]);
                        hasRead = false;
                    }
                    else
                    {
                        int keyPos = line.IndexOf('=');
                        if (keyPos != -1)
                        {
                            section.Add(line[..keyPos].Trim(), line[(keyPos + 1)..].Trim());
                            hasRead = true;
                        }
                        else
                            throw new IniException(string.Format("Invalid ini file: Invalid line {0}", i));
                    }
                }
            }
            if (hasRead)
                ret.Add(section);
            return ret;
        }
    }
}
