namespace SocketTry.Attributes
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
