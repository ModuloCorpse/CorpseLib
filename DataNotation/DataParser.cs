﻿namespace CorpseLib.DataNotation
{
    public class DataParser<TReader, TWriter, TFormat> where TReader : DataReader, new() where TWriter : DataWriter<TFormat>, new() where TFormat : DataFormat, new()
    {
        public static DataObject LoadFromFile(string path) => File.Exists(path) ? Parse(File.ReadAllText(path)) : throw new FileNotFoundException();

        public static DataObject Parse(string content)
        {
            TReader reader = new();
            reader.SetContent(content);
            return reader.Read();
        }

        public static string Str(DataNode node)
        {
            TWriter builder = new();
            builder.AppendNode(node);
            return builder.ToString();
        }

        public static string Str(DataNode node, TFormat format)
        {
            TWriter builder = new();
            builder.SetFormat(format);
            builder.AppendNode(node);
            return builder.ToString();
        }

        public static void WriteToFile(string path, DataObject node) => File.WriteAllText(path, Str(node));

        public static T? LoadFromFile<T>(string filePath)
        {
            try
            {
                DataObject obj = LoadFromFile(filePath);
                DataHelper.Cast(obj, out T? ret);
                return ret;
            }
            catch
            {
                return default;
            }
        }

        public static void WriteToFile<T>(string filePath, object value)
        {
            if (DataHelper.Cast(value) is DataObject obj)
                WriteToFile(filePath, obj);
        }
    }
}
