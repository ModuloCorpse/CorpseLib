using CorpseLib.Placeholder;
using System.Collections;
using System.Text;

namespace CorpseLib.Logging
{
    public class Logger(string format) : IEnumerable<IExtension>
    {
        private class LambdaExtension(Action<string> lambda) : IExtension
        {
            private readonly Action<string> m_Lambda = lambda;
            public void Log(string message) => m_Lambda(message);
        }

        private readonly List<IExtension> m_Extensions = [];
        private readonly string m_Format = format;
        private volatile bool m_Started = false;

        public void Start() => m_Started = true;
        public void Stop() => m_Started = false;

        public void Add(IExtension extension) => m_Extensions.Add(extension);
        public void Add(Action<string> extension) => m_Extensions.Add(new LambdaExtension(extension));

        public void Log(string logContent, Context context)
        {
            if (m_Started)
            {
                Context logContext = new();
                DateTime now = DateTime.Now;
                logContext.AddVariable("d", now.Day);
                logContext.AddVariable("M", now.Month);
                logContext.AddVariable("y", now.Year);
                logContext.AddVariable("h", now.Hour);
                logContext.AddVariable("m", now.Minute);
                logContext.AddVariable("s", now.Second);
                logContext.AddVariable("ms", now.Millisecond);
                logContext.AddVariable("log", logContent);

                StringBuilder builder = new();
                foreach (string traceLine in Environment.StackTrace.Split('\n'))
                {
                    if (!traceLine.Contains("CorpseLib.Logging.Logger") && !traceLine.Contains("at System.Environment.get_StackTrace()"))
                        builder.AppendLine(traceLine);
                }
                logContext.AddVariable("St", builder);

                string log = Converter.Convert(m_Format, [logContext, context]);
                foreach (IExtension extension in m_Extensions)
                    extension.Log(log);
            }
        }

        public void Log(string logContent) => Log(logContent, new());

        //Logger are not real Enumerator, they implement the interface to allow the list init when creating new Logger
        public IEnumerator<IExtension> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
