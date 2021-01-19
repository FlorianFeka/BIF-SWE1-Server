namespace SocketTry.Http
{
    internal static class HttpMeta
    {
        // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
        internal const string EOL = "\r\n";
        internal const string VERSION = "HTTP/1.1";
        internal static class Headers
        {
            internal const string USER_AGENT = "User-Agent";
            internal const string CONTENT_LENGTH = "Content-Length";
            internal const string CONTENT_TYPE = "Content-Type";
        }
    }
}
