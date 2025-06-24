namespace CorpseLib
{
    public class OperationResult
    {
        protected readonly string m_Error;
        protected readonly string m_Description;
        protected readonly bool m_Success;

        public string Error => m_Error;
        public string Description => m_Description;

        public OperationResult(string error, string description)
        {
            m_Error = error;
            m_Description = description;
            m_Success = false;
        }

        public OperationResult()
        {
            m_Error = string.Empty;
            m_Description = string.Empty;
            m_Success = true;
        }

        public static implicit operator bool(OperationResult result) => result.m_Success;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(m_Error))
            {
                if (!string.IsNullOrEmpty(m_Description))
                    return $"{m_Error}: {m_Description}";
                return m_Error;
            }
            return string.Empty;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        private readonly T? m_Result;

        public T? Result => m_Result;

        public OperationResult(string error, string description): base(error, description) => m_Result = default;
        public OperationResult(T? result): base() => m_Result = result;

        public OperationResult<U> Cast<U>()
        {
            if (m_Success)
            {
                if (m_Result == null)
                    return new OperationResult<U>(default);
                else if (m_Result is U?)
                    return new OperationResult<U>((U?)(object?)m_Result);
                return new OperationResult<U>("Cast error", $"Cannot cast from {typeof(T).Name} to {typeof(U).Name}");
            }
            return new OperationResult<U>(m_Error, m_Description);
        }

        public override string ToString()
        {
            if (m_Success)
            {
                if (m_Result == null)
                    return "null";
                else
                    return m_Result.ToString() ?? string.Empty;
            }
            return base.ToString();
        }
    }
}
