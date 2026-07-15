using CorpseLib.Logging;
using System.Diagnostics;

namespace CorpseLib
{
    public static class Log
    {
        private static readonly string LOG_FORMAT = "[${d}-${M}-${y} ${h}:${m}:${s}.${ms}] ${log}";
        public static readonly Logger DEBUG = new(LOG_FORMAT) { (string log) => Console.WriteLine(log), (string log) => Debug.WriteLine(log) };
        public static readonly Logger WARNING = new(LOG_FORMAT);
        public static readonly Logger ERROR = new(LOG_FORMAT);
        public static readonly Logger STANDARD = new(LOG_FORMAT) { (string log) => Console.WriteLine(log) };

        public static void Start()
        {
            WARNING.Start();
            ERROR.Start();
            STANDARD.Start();
            #if DEBUG
                DEBUG.Start();
            #endif
        }
    }
}
