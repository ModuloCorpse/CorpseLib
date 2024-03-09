namespace CorpseLib.Datafile
{
    public abstract class DataFileFormatedObject<TWriter, TFormat> : DataFileObject<TWriter> where TWriter : DataFileFormatedWriter<TFormat>, new() where TFormat : DataFileFormat, new()
    {
        public string ToString(TFormat format)
        {
            TWriter builder = new();
            builder.SetFormat(format);
            return ToString(builder);
        }
    }
}
