namespace CorpseLib.Json
{
    public class JNull : JNode
    {
        public override void ToJson(ref JBuilder builder) => builder.AppendValue("null");
    }
}
