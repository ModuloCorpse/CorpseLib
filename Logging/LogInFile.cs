using CorpseLib.Placeholder;

namespace CorpseLib.Logging
{
    public class LogInFile : IExtension
    {
        private DateTime m_LastLogTime = DateTime.Now;
        private readonly TimeSpan m_TimeBeforeFileNameReset;
        private readonly string m_FilePathFormat;
        private string m_FileName = string.Empty;

        public LogInFile(string filePathFormat, TimeSpan timeBeforeFileNameReset)
        {
            m_TimeBeforeFileNameReset = timeBeforeFileNameReset;
            m_FilePathFormat = filePathFormat;
        }

        public LogInFile(string filePathFormat) : this(filePathFormat, TimeSpan.FromDays(1)) { }

        public void Log(string message)
        {
            Context context = new();
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastLog = now - m_LastLogTime;
            context.AddVariable("d", now.Day);
            context.AddVariable("M", now.Month);
            context.AddVariable("y", now.Year);
            context.AddVariable("h", now.Hour);
            context.AddVariable("m", now.Minute);
            context.AddVariable("s", now.Second);
            context.AddVariable("ms", now.Millisecond);
            if (string.IsNullOrEmpty(m_FileName) || timeSinceLastLog > m_TimeBeforeFileNameReset)
            {
                m_FileName = Converter.Convert(m_FilePathFormat, context);
                m_LastLogTime = now;
            }
            StreamWriter fileStream;
            if (!File.Exists(m_FileName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(m_FileName)!);
                fileStream = new(File.Create(m_FileName));
            }
            else
                fileStream = File.AppendText(m_FileName);
            fileStream.WriteLine(message);
            fileStream.Flush();
            fileStream.Close();
        }
    }
}
