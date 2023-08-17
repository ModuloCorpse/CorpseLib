using CorpseLib.Placeholder;
using System.Globalization;
using System.Text;

namespace CorpseLib.Translation
{
    public class Translator
    {
        private static readonly Dictionary<CultureInfo, Translation> ms_Translations = new();
        private static readonly HashSet<CultureInfo> ms_AvailablesLanguages = new();
        private static readonly Translation ms_DefaultTranslation = new();
        private static Translation? ms_CurrentTranslation = null;
        public static CultureInfo ms_CurrentLanguage = CultureInfo.CurrentUICulture;

        public static event Action? CurrentLanguageChanged;

        public static CultureInfo[] AvailablesLanguages => ms_AvailablesLanguages.ToArray();
        public static CultureInfo CurrentLanguage => ms_CurrentLanguage;

        public static void Clear()
        {
            ms_Translations.Clear();
            ms_DefaultTranslation.Clear();
            ms_CurrentTranslation = null;
        }

        public static bool SetLanguage(CultureInfo culture)
        {
            ms_CurrentLanguage = culture;
            CurrentLanguageChanged?.Invoke();
            return ms_Translations.TryGetValue(culture, out ms_CurrentTranslation);
        }

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
                                AddTranslation(loadingTranslation);
                            string[] locals = lines[i][1..^1].Split(',');
                            List<CultureInfo> cultureInfos = new();
                            bool isDefault = false;
                            foreach (string local in locals)
                            {
                                string trimmedLocal = local.Trim();
                                if (trimmedLocal == "*")
                                    isDefault = true;
                                else
                                    cultureInfos.Add(CultureInfo.GetCultureInfo(trimmedLocal));
                            }
                            loadingTranslation = new(cultureInfos.ToArray(), isDefault);
                        }
                        else
                        {
                            int keyPos = lines[i].IndexOf(':');
                            if (keyPos != -1)
                                loadingTranslation?.Add(lines[i][..keyPos].Trim(), lines[i][(keyPos + 1)..].Trim());
                        }
                    }
                }
                if (loadingTranslation != null)
                    AddTranslation(loadingTranslation);
            }
        }

        public static void AddDefaultTranslation(TranslationKey key, string translation) => ms_DefaultTranslation.Add(key, translation);

        public static void AddTranslation(CultureInfo cultureInfo, TranslationKey key, string translation)
        {
            if (ms_Translations.ContainsKey(cultureInfo))
                ms_Translations[cultureInfo].Add(key, translation);
            else
            {
                ms_Translations[cultureInfo] = new(cultureInfo) { { key, translation } };
                ms_AvailablesLanguages.Add(cultureInfo);
            }
        }

        public static void AddTranslation(Translation translation)
        {
            foreach (CultureInfo cultureInfo in translation.CultureInfos)
            {
                if (ms_Translations.ContainsKey(cultureInfo))
                    ms_Translations[cultureInfo].Merge(translation);
                else
                {
                    ms_Translations[cultureInfo] = translation;
                    ms_AvailablesLanguages.Add(cultureInfo);
                }

                if (ms_CurrentTranslation == null && ms_CurrentLanguage == cultureInfo)
                    ms_CurrentLanguage = cultureInfo;
            }
            if (translation.IsDefault)
                ms_DefaultTranslation.Merge(translation);
        }

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
                {
                    string fileName;
                    if (translation.CultureInfos.Length > 0)
                        fileName = translation.CultureInfos[0].Name;
                    else if (translation.IsDefault)
                        fileName = "all";
                    else
                        fileName = "unknown";
                    File.WriteAllText(Path.Join(path, fileName), translation.ToString().Trim());
                }
            }
        }

        public static bool HaveKey(TranslationKey key) => ms_DefaultTranslation.HaveKey(key) || (ms_CurrentTranslation?.HaveKey(key) ?? false);
        public static bool HaveKey(string key) => HaveKey(new TranslationKey(key));

        public static string Translate(string key, params object[] args)
        {
            if (ms_CurrentTranslation != null)
                return Converter.Convert(key, new TranslationContext(ms_DefaultTranslation, ms_CurrentTranslation, args));
            else if (ms_DefaultTranslation != null)
                return Converter.Convert(key, new TranslationContext(null, ms_DefaultTranslation, args));
            return key;
        }
    }
}
