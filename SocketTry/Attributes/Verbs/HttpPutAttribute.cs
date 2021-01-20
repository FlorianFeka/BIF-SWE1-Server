namespace SocketTry.Attributes.Verbs
{
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public HttpPutAttribute() { }
        public HttpPutAttribute(string sufixRoute)
        {
            SufixRoute = sufixRoute;
        }
    }
}
