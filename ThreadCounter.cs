using System.Collections.Concurrent;

namespace CorpseLib
{
    public class ThreadCounter
    {
        private ulong m_Counter;
        private ThreadCounter() => m_Counter = 0;
        private void IncreaseCounter() => Interlocked.Increment(ref m_Counter);
        private void DecreaseCounter() => Interlocked.Decrement(ref m_Counter);
        private bool IsOver() => Interlocked.Read(ref m_Counter) == 0;
        private void Wait(ulong value, int delay)
        {
            while (true)
            {
                if (Interlocked.Read(ref m_Counter) == value)
                    return;
                Thread.Sleep(delay);
            }
        }

        private static readonly ConcurrentDictionary<Guid, ThreadCounter> ms_Counters = new();

        public static Guid NewCounter()
        {
            Guid counterGuid = Guid.NewGuid();
            ThreadCounter counter = new();
            ms_Counters[counterGuid] = counter;
            return counterGuid;
        }

        public static void IncreaseCounter(Guid counterGuid) => ms_Counters[counterGuid].IncreaseCounter();
        public static void DecreaseCounter(Guid counterGuid) => ms_Counters[counterGuid].DecreaseCounter();
        public static bool IsCounterOver(Guid counterGuid) => ms_Counters[counterGuid].IsOver();
        public static void Wait(Guid counterGuid, ulong value, int delay) => ms_Counters[counterGuid].Wait(value, delay);
    }
}
