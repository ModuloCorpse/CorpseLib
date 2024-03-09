namespace CorpseLib.Datafile
{
    public abstract class DataFileObject<TWriter> where TWriter : DataFileWriter, new()
    {
        public override string ToString() => ToString(new TWriter());

        protected string ToString(TWriter writer)
        {
            AppendToWriter(ref writer);
            return writer.ToString();
        }

        protected void AppendObject(ref TWriter writer, DataFileObject<TWriter> obj) => obj.AppendToWriter(ref writer);

        protected abstract void AppendToWriter(ref TWriter writer);
    }
}
