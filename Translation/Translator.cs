using CorpseLib.Ini;
using CorpseLib.Placeholder;
using System.Globalization;

namespace CorpseLib.Translation
{
    public class Translator
    {
        private static readonly Dictionary<CultureInfo, Translation> ms_Translations = [];
        private static readonly HashSet<CultureInfo> ms_AvailablesLanguages = [];
        private static readonly Translation ms_DefaultTranslation = [];
        private static Translation? ms_CurrentTranslation = null;
        private static CultureInfo ms_CurrentLanguage = CultureInfo.CurrentUICulture;

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
            if (ms_Translations.TryGetValue(culture, out ms_CurrentTranslation))
            {
                ms_CurrentLanguage = culture;
                CurrentLanguageChanged?.Invoke();
                return true;
            }
            return false;
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

        //TODO Allow XLIFF and other file format
        public static void LoadFile(string path)
        {
            if (File.Exists(path))
            {
                IniFile ini = IniParser.LoadFromFile(path);
                if (!ini.HaveEmptySection)
                {
                    foreach (IniSection section in ini)
                        AddTranslation(new(section));
                }
            }
        }

        public static void AddDefaultTranslation(TranslationKey key, string translation) => ms_DefaultTranslation.Add(key, translation);

        public static void AddTranslation(CultureInfo cultureInfo, TranslationKey key, string translation)
        {
            if (ms_Translations.TryGetValue(cultureInfo, out Translation? value))
                value.Add(key, translation);
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
                if (ms_Translations.TryGetValue(cultureInfo, out Translation? value))
                    value.Merge(translation);
                else
                {
                    ms_Translations[cultureInfo] = translation;
                    ms_AvailablesLanguages.Add(cultureInfo);
                }

                if (ms_CurrentTranslation == null && ms_CurrentLanguage == cultureInfo)
                    ms_CurrentTranslation = translation;
            }
            if (translation.IsDefault)
                ms_DefaultTranslation.Merge(translation);
        }

        public static void SaveToFile(string path)
        {
            IniFile file = new();
            foreach (Translation translation in ms_Translations.Values)
                file.Add(translation.ToIniSection());
            File.WriteAllText(path, file.ToString().Trim());
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
                    File.WriteAllText(Path.Join(path, fileName), translation.ToIniSection().ToString().Trim());
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
