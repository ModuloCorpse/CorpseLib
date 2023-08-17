using CorpseLib.Placeholder;

namespace CorpseLib.Translation
{
    internal class TranslationContext : AFunctionalContext
    {
        private readonly Translation? m_DefaultTranslation;
        private readonly Translation m_Translation;
        private readonly object[] m_Args;

        public TranslationContext(Translation? defaultTranslation, Translation translation, params object[] args)
        {
            m_DefaultTranslation = defaultTranslation;
            m_Translation = translation;
            m_Args = args;
            AddFunction("Translate", (variables) => variables.Length switch
            {
                0 => string.Empty,
                1 => Converter.Convert(variables[0], new TranslationContext(defaultTranslation, translation)),
                _ => Converter.Convert(variables[0], new TranslationContext(defaultTranslation, translation, variables[1..]))
            });
        }

        public override string GetVariable(string name)
        {
            if (int.TryParse(name, out var i) && i >= 0 && i < m_Args.Length)
                return m_Args[i].ToString() ?? name;
            else if (m_Translation.TryGetTranslation(new(name), out string? translation))
                return Converter.Convert(translation!, this);
            else if (m_DefaultTranslation?.TryGetTranslation(new(name), out string? defaultTranslation) ?? false)
                return Converter.Convert(defaultTranslation!, this);
            return name;
        }
    }
}
