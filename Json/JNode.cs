namespace CorpseLib.Json
{
    public abstract class JNode
    {
        public abstract void ToJson(ref JBuilder builder);

        public string ToString(JBuilder builder)
        {
            ToJson(ref builder);
            return builder.ToString();
        }
        public string ToString(JFormat format) => ToString(new JBuilder(format));
        public override string ToString() => ToString(new JBuilder());
        public string ToNetworkString() => ToString(new JBuilder(JHelper.NETWORK_FORMAT));
    }
}
