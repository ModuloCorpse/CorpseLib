using System.Text;
using System.Text.RegularExpressions;

namespace CorpseLib
{
    public class Version
    {
        private readonly int m_Major;
        private readonly int m_Minor;
        private readonly int m_Patch;
        private readonly string m_Prerelease;
        private readonly string m_Build;

        public Version(int major, int minor, int patch, string prerelease = "", string build = "")
        {
            m_Major = major;
            m_Minor = minor;
            m_Patch = patch;
            m_Prerelease = prerelease;
            m_Build = build;
        }

        public Version(string version)
        {
            string pattern = @"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
            Match match = Regex.Match(version, pattern);
            if (match.Success)
            {
                m_Major = int.Parse(match.Groups["major"].Value);
                m_Minor = int.Parse(match.Groups["minor"].Value);
                m_Patch = int.Parse(match.Groups["patch"].Value);
                m_Prerelease = match.Groups["prerelease"].Value;
                m_Build = match.Groups["buildmetadata"].Value;
            }
            else
            {
                m_Major = 0;
                m_Minor = 0;
                m_Patch = 0;
                m_Prerelease = string.Empty;
                m_Build = string.Empty;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append(m_Major);
            builder.Append('.');
            builder.Append(m_Minor);
            builder.Append('.');
            builder.Append(m_Patch);
            if (!string.IsNullOrEmpty(m_Prerelease))
            {
                builder.Append('-');
                builder.Append(m_Prerelease);
            }
            if (!string.IsNullOrEmpty(m_Build))
            {
                builder.Append('+');
                builder.Append(m_Build);
            }
            return builder.ToString();
        }

        public override bool Equals(object? obj) => obj is Version version &&
            m_Major == version.m_Major &&
            m_Minor == version.m_Minor &&
            m_Patch == version.m_Patch &&
            m_Prerelease == version.m_Prerelease &&
            m_Build == version.m_Build;

        public override int GetHashCode() => HashCode.Combine(m_Major, m_Minor, m_Patch, m_Prerelease, m_Build);

        public static bool operator ==(Version a, Version b) => a.Equals(b);

        public static bool operator !=(Version a, Version b) => !a.Equals(b);

        public static bool operator >(Version a, Version b)
        {
            if (a.m_Major > b.m_Major)
                return true;
            if (a.m_Minor > b.m_Minor)
                return true;
            if (a.m_Patch > b.m_Patch)
                return true;
            return false;
        }

        public static bool operator <(Version a, Version b)
        {
            if (a.m_Major < b.m_Major)
                return true;
            if (a.m_Minor < b.m_Minor)
                return true;
            if (a.m_Patch < b.m_Patch)
                return true;
            return false;
        }

        public static bool operator >=(Version a, Version b) => a == b || a > b;
        public static bool operator <=(Version a, Version b) => a == b || a < b;
    }
}
