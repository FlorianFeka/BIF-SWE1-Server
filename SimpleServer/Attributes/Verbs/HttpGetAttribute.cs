namespace SimpleServer.Attributes.Verbs
{
    public class HttpGetAttribute : HttpVerbAttribute
    {
        public HttpGetAttribute() { }
        public HttpGetAttribute(string sufixRoute)
        {
            SufixRoute = sufixRoute;
        }
    }
}
