using CorpseLib.Placeholder;

namespace CorpseLib.Logging
{
    public class LogInFile : IExtension
    {
        private DateTime m_LastLogTime = DateTime.Now;
        private readonly TimeSpan m_TimeBeforeFileNameReset;
        private readonly string m_FilePathFormat;
        private string m_FileName = string.Empty;
        private readonly object m_Lock = new();

        public LogInFile(string filePathFormat, TimeSpan timeBeforeFileNameReset)
        {
            m_TimeBeforeFileNameReset = timeBeforeFileNameReset;
            m_FilePathFormat = filePathFormat;
            LoadLastLogTime();
        }

        public LogInFile(string filePathFormat) : this(filePathFormat, TimeSpan.FromDays(1)) { }

        private void LoadLastLogTime()
        {
            string? pathToDir = Path.GetDirectoryName(Path.GetFullPath(GenerateFileName(m_LastLogTime)));
            if (string.IsNullOrEmpty(pathToDir))
                pathToDir = ".";
            string fileNameToUse = string.Empty;
            DateTime dateTimeToUse = DateTime.MinValue;
            if (!Directory.Exists(pathToDir))
                Directory.CreateDirectory(pathToDir);
            foreach (string file in Directory.GetFiles(pathToDir))
            {
                string firstLine = File.ReadLines(file).First();
                if (firstLine.StartsWith(m_FilePathFormat))
                {
                    DateTime fileDateTime = DateTime.Parse(firstLine[m_FilePathFormat.Length..]);
                    if (fileDateTime > dateTimeToUse)
                    {
                        dateTimeToUse = fileDateTime;
                        fileNameToUse = file;
                    }
                }
            }
            if (!string.IsNullOrEmpty(fileNameToUse))
            {
                m_LastLogTime = dateTimeToUse;
                m_FileName = fileNameToUse;
            }
        }

        private string GenerateFileName(DateTime now)
        {
            Context context = new();
            context.AddVariable("d", now.Day);
            context.AddVariable("M", now.Month);
            context.AddVariable("y", now.Year);
            context.AddVariable("h", now.Hour);
            context.AddVariable("m", now.Minute);
            context.AddVariable("s", now.Second);
            context.AddVariable("ms", now.Millisecond);
            return Converter.Convert(m_FilePathFormat, context);
        }

        public void Log(string message)
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastLog = now - m_LastLogTime;
            if (string.IsNullOrEmpty(m_FileName) || timeSinceLastLog > m_TimeBeforeFileNameReset)
            {
                m_FileName = GenerateFileName(now);
                m_LastLogTime = now;
            }
            lock (m_Lock)
            {
                StreamWriter fileStream;
                if (!File.Exists(m_FileName))
                {
                    string? directoryPath = Path.GetDirectoryName(m_FileName);
                    if (!string.IsNullOrEmpty(directoryPath))
                        Directory.CreateDirectory(directoryPath);
                    fileStream = new(File.Create(m_FileName));
                    fileStream.Write(m_FilePathFormat);
                    fileStream.WriteLine(m_LastLogTime);
                }
                else
                    fileStream = File.AppendText(m_FileName);
                fileStream.WriteLine(message);
                fileStream.Flush();
                fileStream.Close();
            }
        }
    }
}
