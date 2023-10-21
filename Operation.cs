namespace CorpseLib
{
    public abstract class AOperation
    {
        private readonly TaskCompletionSource<bool> m_Task;
        protected volatile bool m_HaveResult = false;

        protected AOperation() => m_Task = new TaskCompletionSource<bool>();

        protected void Success()
        {
            if (!m_HaveResult)
            {
                m_Task.SetResult(true);
                m_HaveResult = true;
            }
        }

        protected void Failure()
        {
            if (!m_HaveResult)
            {
                m_Task.SetResult(false);
                m_HaveResult = true;
            }
        }

        public abstract void SetError(string error, string description);

        public bool Wait()
        {
            m_Task.Task.Wait();
            return m_Task.Task.Result;
        }
    }

    public class Operation : AOperation
    {
        private OperationResult? m_Result = null;

        public OperationResult Result => m_Result!;

        public Operation() : base() { }

        public void SetResult()
        {
            if (!m_HaveResult)
            {
                m_Result = new();
                Success();
            }
        }

        public override void SetError(string error, string description)
        {
            if (!m_HaveResult)
            {
                m_Result = new(error, description);
                Failure();
            }
        }
    }

    public class Operation<TResult> : AOperation
    {
        private OperationResult<TResult>? m_Result = null;

        public OperationResult<TResult> Result => m_Result!;

        public Operation() : base() { }

        public void SetResult(TResult value)
        {
            if (!m_HaveResult)
            {
                m_Result = new(value);
                Success();
            }
        }

        public override void SetError(string error, string description)
        {
            if (!m_HaveResult)
            {
                m_Result = new(error, description);
                Failure();
            }
        }
    }
}
