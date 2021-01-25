using SimpleServer.Interfaces;
using System;
using System.Collections.Generic;

namespace SimpleServer.Implementations
{
    public class Url : IUrl
    {
        public Url(string rawUrl)
        {
            if (rawUrl == null) throw new ArgumentNullException(nameof(rawUrl));
            RawUrl = rawUrl;
            var startIndex = RawUrl.IndexOf("/");
            var parameterIndex = RawUrl.IndexOf("?");
            var fragmentIndex = RawUrl.IndexOf("#");
            setParameters(parameterIndex, fragmentIndex);
            var lastIndex = parameterIndex;
            if(lastIndex < 0)
            {
                if(fragmentIndex < 0)
                {
                    lastIndex = RawUrl.Length;
                }
                else
                {
                    lastIndex = fragmentIndex;
                }
            }
            Path = startIndex < 0 ? "" : RawUrl.Substring(startIndex, lastIndex - startIndex);
            Fragment = fragmentIndex < 0 ? null : RawUrl.Substring(fragmentIndex + 1);
            if(!string.IsNullOrEmpty(Path) && Path != "/")
            {
                Segments = Path.Substring(1).Split("/");
            }
            else
            {
                Segments = new string[0];
            }
        }

        private void setParameters(int parameterIndex, int? fragmentIndex = null)
        {
            if(parameterIndex > -1)
            {
                var lastIndex = RawUrl.Length;
                if (fragmentIndex != null && fragmentIndex > 0)
                {
                    lastIndex = fragmentIndex.Value;
                }
                parameterIndex++;

                var parameterString = RawUrl.Substring(parameterIndex, lastIndex - parameterIndex);
                var keyValues = parameterString.Split("&");
                foreach (var keyValueString in keyValues)
                {
                    var keyValue = keyValueString.Split("=");
                    Parameter.Add(keyValue[0], keyValue[1]);
                }
            }
        }

        public string RawUrl { get; }

        public string Path { get; }

        public IDictionary<string, string> Parameter { get; } = new Dictionary<string, string>();

        public int ParameterCount => Parameter.Count;

        public string[] Segments { get; }

        public string FileName { get; }

        public string Extension { get
            {
                var extensionIndex = FileName.LastIndexOf(".");
                return extensionIndex < 1 ? null : FileName.Substring(extensionIndex);
            } }

        public string Fragment { get; }
    }
}
