using CorpseLib.Placeholder;
using System.Collections;
using System.Text;

namespace CorpseLib.Logging
{
    public class Logger : IEnumerable<IExtension>
    {
        private class LambdaExtension : IExtension
        {
            private readonly Action<string> m_Lambda;
            public LambdaExtension(Action<string> lambda) => m_Lambda = lambda;
            public void Log(string message) => m_Lambda(message);
        }

        private readonly List<IExtension> m_Extensions = new();
        private readonly string m_Format;
        private volatile bool m_Started = false;

        public Logger(string format) => m_Format = format;

        public void Start() => m_Started = true;
        public void Stop() => m_Started = false;

        public void Add(IExtension extension) => m_Extensions.Add(extension);
        public void Add(Action<string> extension) => m_Extensions.Add(new LambdaExtension(extension));

        public void Log(string logContent)
        {
            if (m_Started)
            {
                Context context = new();
                DateTime now = DateTime.Now;
                context.AddVariable("d", now.Day);
                context.AddVariable("M", now.Month);
                context.AddVariable("y", now.Year);
                context.AddVariable("h", now.Hour);
                context.AddVariable("m", now.Minute);
                context.AddVariable("s", now.Second);
                context.AddVariable("ms", now.Millisecond);
                context.AddVariable("log", logContent);

                StringBuilder builder = new();
                foreach (string traceLine in Environment.StackTrace.Split('\n'))
                {
                    if (!traceLine.Contains("CorpseLib.Logging.Logger") && !traceLine.Contains("at System.Environment.get_StackTrace()"))
                        builder.AppendLine(traceLine);
                }
                context.AddVariable("St", builder);

                string log = Converter.Convert(m_Format, context);
                foreach (IExtension extension in m_Extensions)
                    extension.Log(log);
            }
        }

        //Logger are not real Enumerator, they implement the interface to allow the list init when creating new Logger
        public IEnumerator<IExtension> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
