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

        private void InternalLog(string logContent, Context context)
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

                StringBuilder builder = new();
                foreach (string traceLine in Environment.StackTrace.Split('\n'))
                {
                    string trace = traceLine.Trim();
                    if (!string.IsNullOrEmpty(trace) && !trace.Contains("CorpseLib.Logging.Logger") && !trace.Contains("at System.Environment.get_StackTrace()"))
                        builder.AppendLine(trace);
                }
                logContext.AddVariable("St", builder);

                logContext.AddVariable("log", Converter.Convert(logContent, [logContext, context]));

                string log = Converter.Convert(m_Format, [logContext, context]);
                foreach (IExtension extension in m_Extensions)
                    extension.Log(log);
            }
        }

        public void Log(string logContent, Context context) => InternalLog(logContent, context);
        public void Log(string logContent) => InternalLog(logContent, new());
        public void Log(string logContent, params object[] args)
        {
            Context logContext = new();
            int i = 0;
            foreach (object arg in args)
            {
                if (arg is ILoggable loggable)
                    logContext.AddVariable(i.ToString(), loggable.ToLog());
                else
                    logContext.AddVariable(i.ToString(), arg.ToString() ?? string.Empty);
                ++i;
            }
            Log(logContent, logContext);
        }

        //Logger are not real Enumerator, they implement the interface to allow the list init when creating new Logger
        public IEnumerator<IExtension> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
