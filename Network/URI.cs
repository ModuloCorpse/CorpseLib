using System.Text;

namespace CorpseLib.Network
{
    /// <summary>
    /// Class representing a Uniform Resource Identifier
    /// </summary>
    public class URI
    {
        public class Builder
        {
            private readonly string m_Scheme;
            private string m_UserInfo = string.Empty;
            private string m_Host = string.Empty;
            private int m_Port = -1;
            private string m_Path = string.Empty;
            private string m_Query = string.Empty;
            private string m_Fragment = string.Empty;

            internal Builder(string scheme) => m_Scheme = scheme;

            public Builder UserInfo(string userInfo)
            {
                m_UserInfo = userInfo;
                return this;
            }

            public Builder Port(int port)
            {
                m_Port = port;
                return this;
            }

            public Builder Host(string host)
            {
                m_Host = host;
                return this;
            }

            public Builder Path(string path)
            {
                if (!string.IsNullOrEmpty(path) && path[^1] == '/')
                    m_Path = path[0..^1];
                else
                    m_Path = path;
                return this;
            }

            public Builder Query(URIQuery query)
            {
                m_Query = query.ToString();
                return this;
            }

            public Builder Query(string query)
            {
                m_Query = query;
                return this;
            }

            public Builder Fragment(string fragment)
            {
                m_Fragment = fragment;
                return this;
            }

            public URI Build()
            {
                Authority? authority = null;
                if (!string.IsNullOrEmpty(m_UserInfo) || !string.IsNullOrEmpty(m_Host) || m_Port != -1)
                    authority = new(m_UserInfo, m_Host, m_Port);
                return new(m_Scheme, authority, m_Path, m_Query, m_Fragment);
            }
        }

        /// <summary>
        /// Class representing an URL exception
        /// </summary>
        /// <remarks>
        /// Constructor
        /// </remarks>
        /// <param name="message">Exception message</param>
        public class URLException(string message) : Exception(message) { }

        internal class Authority(string userInfo, string host, int port)
        {
            private readonly string m_UserInfo = userInfo;
            private readonly string m_Host = host;
            private readonly int m_Port = port;
            public string UserInfo => m_UserInfo;
            public string Host => m_Host;
            public int Port => m_Port;

            public string ToDebugString()
            {
                StringBuilder builder = new();
                builder.Append("{ user info: ");
                builder.Append(m_UserInfo);
                builder.Append(", host: ");
                builder.Append(m_Host);
                builder.Append(", port: ");
                builder.Append(m_Port);
                builder.Append('}');
                return builder.ToString();
            }

            public override string ToString()
            {
                StringBuilder builder = new();
                builder.Append("//");
                if (!string.IsNullOrWhiteSpace(m_UserInfo))
                {
                    builder.Append(m_UserInfo);
                    builder.Append('@');
                }
                builder.Append(m_Host);
                if (m_Port != -1)
                {
                    builder.Append(':');
                    builder.Append(m_Port);
                }
                return builder.ToString();
            }
        }

        private readonly static Dictionary<string, int> ms_DefaultPorts = new()
        {
            { "aaa", -1 },
            { "aaas", -1 },
            { "about", -1 },
            { "acap", -1 },
            { "acct", -1 },
            { "cap", -1 },
            { "cid", -1 },
            { "coap", -1 },
            { "coaps", -1 },
            { "crid", -1 },
            { "data", -1 },
            { "dav", -1 },
            { "dict", -1 },
            { "dns", -1 },
            { "example", -1 },
            { "fax", -1 },
            { "file", -1 },
            { "filesystem", -1 },
            { "ftp", -1 },
            { "geo", -1 },
            { "go", -1 },
            { "gopher", -1 },
            { "h323", -1 },
            { "http", 80 },
            { "https", 443 },
            { "iax", -1 },
            { "icap", -1 },
            { "im", -1 },
            { "imap", -1 },
            { "info", -1 },
            { "ipp", -1 },
            { "ipps", -1 },
            { "iris", -1 },
            { "iris.beep", -1 },
            { "iris.xpc", -1 },
            { "iris.xpcs", -1 },
            { "iris.lws", -1 },
            { "jabber", -1 },
            { "ldap", -1 },
            { "mailto", -1 },
            { "mailserver", -1 },
            { "modem", -1 },
            { "msrp", -1 },
            { "msrps", -1 },
            { "mtqp", -1 },
            { "mupdate", -1 },
            { "news", -1 },
            { "nfs", -1 },
            { "ni", -1 },
            { "nih", -1 },
            { "nntp", -1 },
            { "opaquelocktoken", -1 },
            { "pack", -1 },
            { "pkcs11", -1 },
            { "pop", -1 },
            { "pres", -1 },
            { "prospero", -1 },
            { "reload", -1 },
            { "rtsp", -1 },
            { "service", -1 },
            { "session", -1 },
            { "shttp", -1 },
            { "sieve", -1 },
            { "sip", -1 },
            { "sips", -1 },
            { "sms", -1 },
            { "snews", -1 },
            { "snmp", -1 },
            { "soap.beep", -1 },
            { "soap.beeps", -1 },
            { "stun", -1 },
            { "stuns", -1 },
            { "tag", -1 },
            { "tel", -1 },
            { "telnet", -1 },
            { "tftp", -1 },
            { "thismessage", -1 },
            { "tn3270", -1 },
            { "tip", -1 },
            { "turn", -1 },
            { "turns", -1 },
            { "tv", -1 },
            { "urn", -1 },
            { "vemmi", -1 },
            { "videotex", -1 },
            { "vnc", -1 },
            { "wais", -1 },
            { "ws", 80 },
            { "wss", 443 },
            { "xcon", -1 },
            { "xcon-userid", -1 },
            { "xmlrpc.beep", -1 },
            { "xmlrpc.beeps", -1 },
            { "xmpp", -1 },
            { "z39.50", -1 },
            { "z39.50r", -1 },
            { "z39.50s", -1 }
        };

        private readonly string m_Scheme;
        private readonly Authority? m_Authority = null;
        private readonly string m_Path;
        private readonly string m_Query;
        private readonly string m_Fragment;

        public static Builder Build() => new(string.Empty);
        public static Builder Build(string scheme) => new(scheme);

        public static URI Create(string host, int port) => new(string.Empty, new(string.Empty, host, port), string.Empty, string.Empty, string.Empty);

        public static URI? TryParse(string? url)
        {
            try
            {
                return NullParse(url);
            } catch { return null; }
        }

        public static URI? NullParse(string? url)
        {
            if (url == null)
                return null;
            return Parse(url);
        }

        public static URI Parse(string url)
        {
            string scheme;
            Authority? authority = null;
            string path;
            string query;
            string fragment;

            url = Decode(url);
            int tmp = url.IndexOf(':');
            scheme = url[..tmp];
            url = url[(tmp + 1)..];

            //Authority
            if (url.StartsWith("//"))
            {
                url = url[2..];
                tmp = url.IndexOf('/');
                string authorityStr;
                if (tmp > 0)
                {
                    authorityStr = url[..tmp];
                    url = url[tmp..];
                }
                else
                {
                    authorityStr = url;
                    url = string.Empty;
                }

                //Port
                int port;
                tmp = authorityStr.IndexOf(':');
                if (tmp > 0)
                {
                    if (int.TryParse(authorityStr[(tmp + 1)..], out port))
                        authorityStr = authorityStr[..tmp];
                    else if (!ms_DefaultPorts.TryGetValue(scheme, out port))
                        port = -1;
                }
                else if (!ms_DefaultPorts.TryGetValue(scheme, out port))
                    port = -1;

                //User Info
                string userInfo = string.Empty;
                tmp = authorityStr.IndexOf('@');
                if (tmp > 0)
                {
                    userInfo = authorityStr[..tmp];
                    authorityStr = authorityStr[(tmp + 1)..];
                }

                //Host
                authority = new(userInfo, authorityStr, port);
            }

            //Fragment
            tmp = url.IndexOf('#');
            if (tmp < 0)
                fragment = string.Empty;
            else
            {
                fragment = url[(tmp + 1)..];
                url = url[..tmp];
            }

            //Query
            tmp = url.IndexOf('?');
            if (tmp < 0)
                query = string.Empty;
            else
            {
                query = url[(tmp + 1)..];
                url = url[..tmp];
            }

            //Path
            if (!string.IsNullOrEmpty(url) && url[^1] == '/')
                path = url[0..^1];
            else
                path = url;

            return new(scheme, authority, path, query, fragment);
        }

        internal URI(string scheme, Authority? authority, string path, string query, string fragment)
        {
            m_Scheme = scheme;
            m_Authority = authority;
            m_Path = path;
            m_Query = query;
            m_Fragment = fragment;
        }

        /// <summary>
        /// Scheme of the URL
        /// </summary>
        public string Scheme => m_Scheme;
        /// <summary>
        /// Userinfo of the URL
        /// </summary>
        public string UserInfo => (m_Authority != null) ? m_Authority.UserInfo : string.Empty;
        /// <summary>
        /// Host of the URL
        /// </summary>
        public string Host => (m_Authority != null) ? m_Authority.Host : string.Empty;
        /// <summary>
        /// Port of the URL
        /// </summary>
        public int Port => (m_Authority != null) ? m_Authority.Port : -1;
        /// <summary>
        /// Path of the URL
        /// </summary>
        public string Path => m_Path;
        /// <summary>
        /// Path and query of the URL
        /// </summary>
        public string FullPath
        {
            get {
                StringBuilder builder = new();
                builder.Append(m_Path);
                if (!string.IsNullOrEmpty(m_Query))
                {
                    builder.Append('?');
                    builder.Append(m_Query);
                }
                return builder.ToString();
            }
        }
        /// <summary>
        /// Fragment of the URL
        /// </summary>
        public string Query => m_Query;
        /// <summary>
        /// Fragment of the URL
        /// </summary>
        public string Fragment => m_Fragment;

        public Builder BuildFrom() => ((m_Authority != null) ?
            new Builder(m_Scheme).UserInfo(m_Authority.UserInfo).Host(m_Authority.Host).Port(m_Authority.Port) :
            new Builder(m_Scheme)).Path(m_Path).Query(m_Query).Fragment(m_Fragment);

        public string ToDebugString()
        {
            StringBuilder builder = new();
            builder.Append("{ scheme: ");
            builder.Append(m_Scheme);
            builder.Append(", authority: ");
            builder.Append(m_Authority?.ToDebugString() ?? "{}");
            builder.Append(", path: ");
            builder.Append(m_Path);
            builder.Append(", query: ");
            builder.Append(m_Query);
            builder.Append(", fragment: ");
            builder.Append(m_Fragment);
            builder.Append('}');
            return builder.ToString();
        }

        /// <returns>The URL string</returns>
        public override string ToString()
        {
            StringBuilder builder = new();
            if (!string.IsNullOrWhiteSpace(m_Scheme))
            {
                builder.Append(m_Scheme);
                builder.Append(':');
            }
            if (m_Authority != null)
                builder.Append(m_Authority.ToString());
            builder.Append(m_Path);
            if (!string.IsNullOrEmpty(m_Query))
            {
                builder.Append('?');
                builder.Append(m_Query);
            }
            if (!string.IsNullOrEmpty(m_Fragment))
            {
                builder.Append('#');
                builder.Append(m_Fragment);
            }
            //TODO URI-Encode string
            return builder.ToString();
        }

        private readonly static Dictionary<string, char> ms_ReservedCharacters = new()
        {
            { "%0A", '\n' },
            { "%0D", '\n' },
            { "%0A%0D", '\n' },
            { "%20", ' ' },
            { "%21", '!' },
            { "%22", '"' },
            { "%23", '#' },
            { "%24", '$' },
            { "%25", '%' },
            { "%26", '&' },
            { "%27", '\'' },
            { "%28", '(' },
            { "%29", ')' },
            { "%2A", '*' },
            { "%2B", '+' },
            { "%2C", ',' },
            { "%2D", '-' },
            { "%2E", '.' },
            { "%2F", '/' },
            { "%3A", ':' },
            { "%3B", ';' },
            { "%3C", '<' },
            { "%3D", '=' },
            { "%3E", '>' },
            { "%3F", '?' },
            { "%40", '@' },
            { "%5B", '[' },
            { "%5C", '\\' },
            { "%5D", ']' },
            { "%5E", '^' },
            { "%5F", '_' },
            { "%60", '`' },
            { "%7B", '{' },
            { "%7C", '|' },
            { "%7D", '}' },
            { "%7E", '~' },
            { "%C2%A3", '£' },
            { "%E5%86%86", '円' }
        };

        public static string Encode(string value)
        {
            foreach (KeyValuePair<string, char> reservedCharacters in ms_ReservedCharacters)
                value = value.Replace(reservedCharacters.Value.ToString(), reservedCharacters.Key);
            return value;
        }

        public static string Decode(string value)
        {
            foreach (KeyValuePair<string, char> reservedCharacters in ms_ReservedCharacters)
                value = value.Replace(reservedCharacters.Key, reservedCharacters.Value.ToString());
            return value;
        }
    }
}
