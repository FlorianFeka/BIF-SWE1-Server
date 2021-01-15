namespace SocketTry.Http
{
    public static class HttpMeta
    {
        // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
        public const string EOL = "\r\n";
        public const string VERSION = "HTTP/1.1";
        public static class Headers
        {
            public const string USER_AGENT = "User-Agent";
            public const string CONTENT_LENGTH = "Content-Length";
            public const string CONTENT_TYPE = "Content-Type";
        }
    }
}
