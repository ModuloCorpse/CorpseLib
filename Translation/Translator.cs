using CorpseLib.Placeholder;
using System.Globalization;
using System.Text;

namespace CorpseLib.Translation
{
    public class Translator
    {
        private static readonly Dictionary<CultureInfo, Translation> ms_Translations = new();
        private static Translation? ms_CurrentTranslation = null;

        public static void Clear()
        {
            ms_Translations.Clear();
            ms_CurrentTranslation = null;
        }

        public static bool SetLanguage(CultureInfo culture) => ms_Translations.TryGetValue(culture, out ms_CurrentTranslation);

        public static void Load(params string[] translationFiles)
        {
            foreach (var translationFile in translationFiles)
                LoadFile(translationFile);
        }

        public static void LoadDirectory(string path)
        {
            foreach (var translationFile in Directory.GetFiles(path))
                LoadFile(translationFile);
        }

        public static void LoadFile(string path)
        {
            if (File.Exists(path))
            {
                Translation? loadingTranslation = null;
                string content = File.ReadAllText(path);
                string[] lines = content.Split(Environment.NewLine);
                for (int i = 0; i != lines.Length; ++i)
                {
                    if (!string.IsNullOrEmpty(lines[i]))
                    {
                        if (lines[i][0] == '[' && lines[i][^1] == ']')
                        {
                            if (loadingTranslation != null)
                                ms_Translations[loadingTranslation.CultureInfo] = loadingTranslation;
                            loadingTranslation = new(CultureInfo.GetCultureInfo(lines[i][1..^1]));
                        }
                        else
                        {
                            int keyPos = lines[i].IndexOf(':');
                            if (keyPos != -1)
                                loadingTranslation?.Add(new(lines[i][..keyPos].Trim()), lines[i][(keyPos + 1)..].Trim());
                        }
                    }
                }
                if (loadingTranslation != null)
                    ms_Translations[loadingTranslation.CultureInfo] = loadingTranslation;
            }
        }

        public static void AddTranslation(Translation translation) => ms_Translations[translation.CultureInfo] = translation;

        public static void SaveToFile(string path)
        {
            StringBuilder builder = new();
            foreach (Translation translation in ms_Translations.Values)
                builder.AppendLine(translation.ToString());
            File.WriteAllText(path, builder.ToString().Trim());
        }

        public static void SaveToDir(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (Translation translation in ms_Translations.Values)
                    File.WriteAllText(Path.Join(path, translation.CultureInfo.Name), translation.ToString().Trim());
            }
        }

        public static string Translate(string key, params object[] args) => (ms_CurrentTranslation != null) ? Converter.Convert(key, new TranslationContext(ms_CurrentTranslation, args)) : string.Empty;
    }
}
