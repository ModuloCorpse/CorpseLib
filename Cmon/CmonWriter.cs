using CorpseLib.DataNotation;
using System.Text.RegularExpressions;

namespace CorpseLib.Cmon
{
    public partial class CmonWriter() : DataWriter<CmonFormat>()
    {
        [GeneratedRegex(@"^[a-zA-Z\-_][a-zA-Z0-9\-_]*$")]
        private static partial Regex ObjectKeyRegex();

        private void OpenScope(char scope)
        {
            if (!m_Format.InlineScope && m_Builder.Length != 0)
                LineBreak();
            m_Builder.Append(scope);
            Indent();
            LineBreak();
        }

        private void CloseScope(char scope)
        {
            Unindent();
            LineBreak();
            m_Builder.Append(scope);
        }

        private void AppendObject(DataObject obj)
        {
            bool hasAppend = false;
            foreach (var child in obj)
            {
                if (ObjectKeyRegex().IsMatch(child.Key))
                {
                    if (hasAppend)
                        LineBreak();
                    m_Builder.Append(child.Key);
                    DataNode childObj = child.Value;
                    if (childObj is DataValue)
                    {
                        Append("=");
                        AppendNextNode(childObj);
                    }
                    else if (childObj is DataObject)
                    {
                        OpenScope('{');
                        AppendNextNode(childObj);
                        CloseScope('}');
                    }
                    else if (childObj is DataArray)
                    {
                        OpenScope('[');
                        AppendNextNode(childObj);
                        CloseScope(']');
                    }
                    hasAppend = true;
                }
            }
        }

        private void AppendArray(DataArray arr)
        {
            bool hasAppend = false;
            foreach (var child in arr)
            {
                if (child is DataValue)
                {
                    if (hasAppend)
                    {
                        if (arr.ArrayType != typeof(string))
                            m_Builder.Append(CmonReader.ARRAY_SEPARATOR);
                        LineBreak();
                    }
                    AppendNextNode(child);
                    hasAppend = true;
                }
                else if (child is DataObject)
                {
                    if (hasAppend)
                        LineBreak();
                    OpenScope('{');
                    AppendNextNode(child);
                    CloseScope('}');
                    hasAppend = true;
                }
                else if (child is DataArray)
                {
                    if (hasAppend)
                        LineBreak();
                    OpenScope('[');
                    AppendNextNode(child);
                    CloseScope(']');
                    hasAppend = true;
                }
            }
        }

        private void AppendValue(DataValue value)
        {
            if (value.Value == null)
            {
                Append("null");
                return;
            }
            else if (value.Value is string str)
            {
                Append(string.Format("\"{0}\"", str
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")));
            }
            else if (value.Value is Guid guid)
                Append(string.Format("\"{0}\"", guid));
            else if (value.Value is DateTime date)
                Append(date.Ticks.ToString());
            else if (value.Value is TimeSpan timeSpan)
                Append(timeSpan.Ticks.ToString());
            else if (value.Value is bool b)
                Append(b ? "true" : "false");
            else if (value.Value is char c)
            {
                if (c == '"')
                    Append("\"\\\"\"");
                else
                    Append(string.Format("\"{0}\"", c));
            }
            else if (value.Type.IsEnum)
                Append(((int)Convert.ChangeType(value.Value, typeof(int))).ToString()!);
            else
                Append(value.Value.ToString()!.Replace(',', '.'));
        }

        private void AppendNextNode(DataNode node)
        {
            if (node is DataObject obj)
                AppendObject(obj);
            else if (node is DataArray arr)
                AppendArray(arr);
            else if (node is DataValue value)
                AppendValue(value);
        }

        public override void AppendNode(DataNode node)
        {
            if (m_Format.OpenScopeFirst)
                OpenScope('{');
            AppendNextNode(node);
            if (m_Format.OpenScopeFirst)
                CloseScope('}');
        }
    }
}
