using SocketTry.Http;
using SocketTry.Interfaces;
using SocketTry.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SocketTry.Implementations
{
    public class HttpRequest : IRequest
    {
        public bool IsValid => !string.IsNullOrEmpty(Url.RawUrl) && Method != null;

        public HttpMethod? Method { get; private set; }

        public IUrl Url { get; private set; } = new Url(string.Empty);

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public string UserAgent => GetHeaderValue(HttpMeta.Headers.USER_AGENT);

        public int HeaderCount => Headers.Count; 

        public int? ContentLength => Util.ToNullableInt(GetHeaderValue(HttpMeta.Headers.CONTENT_LENGTH));

        public string ContentType => GetHeaderValue(HttpMeta.Headers.CONTENT_TYPE);

        public Stream ContentStream => new MemoryStream(ContentBytes);

        public string ContentString { get; private set; }

        public byte[] ContentBytes { get; private set; }

        public float HttpVersion { get; private set; }

        private HttpParsingState _parsingState = HttpParsingState.MetaData;

        /// <exception cref="Exception"></exception>
        public bool ParseChunk(string[] chunk, string nextChunk)
        {
            if (chunk == null) return false;

            if(_parsingState == HttpParsingState.MetaData)
            {
                ProcessMetaData(chunk[0]);
                chunk = chunk.Where((source, index) => index != 0).ToArray();
            }
            if(_parsingState == HttpParsingState.Header)
            {
                ProcessHeaders(chunk, out var lastIndex);
                chunk = chunk.Where((source, index) => index > lastIndex).ToArray();
            }
            if(_parsingState == HttpParsingState.Data)
            {
                if (!Headers.ContainsKey(HttpMeta.Headers.CONTENT_LENGTH))
                {
                    return true;
                }
                ContentString += string.Join("\n", chunk);
                ContentString += nextChunk;
                if (ContentString.Length == ContentLength)
                {
                    return true;
                }
            }
            return false;
        }

        private void ProcessHeaders(string[] lines, out int i)
        {
            for (i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "")
                {
                    _parsingState = HttpParsingState.Data;
                    return;
                }
                ProcessHeader(lines[i]);
            }
        }

        private string GetHeaderValue(string header)
        {
            Headers.TryGetValue(header, out var value);
            return value;
        }

        /// <exception cref="Exception"></exception>
        private void ProcessMetaData(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            var fields = line.Split(" ");
            Method = Util.ProcessMethod(fields[0]);
            Url = new Url(fields[1]);
            try
            {
                HttpVersion = float.Parse(fields[2].Split("/")[1]);
            }
            catch (Exception e)
            {
                throw new Exception("Invalid HTTP version!", e);
            }
            _parsingState = HttpParsingState.Header;
        }

        /// <exception cref="Exception"></exception>
        private void ProcessHeader(string headerData)
        {
            var fields = headerData.Split(": ");
            var header = fields[0];
            var headerValue = fields[1];
            try
            {
                Headers.Add(header, headerValue);
            }
            catch (Exception e)
            {
                throw new Exception("Duplicate Header!", e);
            }
        }
    }
}
